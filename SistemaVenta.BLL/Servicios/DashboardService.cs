using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;

namespace SistemaVenta.BLL.Servicios
{
    public class DashboardService : IDashboardService
    {
        private readonly IVentaRepository _ventaRepositorio;
        private readonly IGenericRepository<Producto> _productoRepositorio;
        private readonly IMapper _mapper;

        public DashboardService(IVentaRepository ventaRepositorio
            , IGenericRepository<Producto> productoRepositorio
            , IMapper mapper)
        {
            _ventaRepositorio = ventaRepositorio;
            _productoRepositorio = productoRepositorio;
            _mapper = mapper;
        }

        private IQueryable<Ventum> RetornarVentas(IQueryable<Ventum> tablaVenta, int restadcantidadDias)
        {
            DateTime? ultimaFecha= tablaVenta.OrderByDescending(v => v.FechaRegistro).Select(v =>  v.FechaRegistro).First();
            ultimaFecha = ultimaFecha.Value.AddDays(restadcantidadDias);
            return tablaVenta.Where(v=>v.FechaRegistro.Value.Date >= ultimaFecha.Value.Date);
        }

        private async Task<int> TotalVentasUltimaSemana()
        {
            int total = 0;
            IQueryable<Ventum> _ventaquery = await _ventaRepositorio.Consultar();
            if (_ventaquery.Count()>0)
            {
                var tablaVenta = RetornarVentas(_ventaquery, -7);
                total = tablaVenta.Count();
            }
            return total;
        }

        private async Task<string> TotalIngresosUltimaSemana()
        {
            decimal resultado = 0;
            IQueryable<Ventum> _ventaQuery = await _ventaRepositorio.Consultar();
            if (_ventaQuery.Count() > 0) {
                var tablaventa = RetornarVentas(_ventaQuery, -7);
                resultado = tablaventa.Select(v => v.Total).Sum(v => v.Value);
            }
            return Convert.ToString(resultado, new CultureInfo("es-PE"));

        }

        private async Task<int> TotalProductos()
        {
            IQueryable<Producto> _productoQuery = await _productoRepositorio.Consultar();

            int total = _productoQuery.Count();
            return total;
        }

        private async Task<Dictionary<string,int>> VentasUltimaSemana()
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();
            IQueryable<Ventum> _ventaQuery = await _ventaRepositorio.Consultar();
            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = RetornarVentas(_ventaQuery, -7);
                resultado = tablaVenta
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderBy(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);
            }
            return resultado;
        }

        public async Task<DashBoardDTO> Resumen()
        {
            DashBoardDTO vmDasboard = new DashBoardDTO();

            try
            {
                vmDasboard.TotalVentas = await TotalVentasUltimaSemana();
                vmDasboard.TotalIngresos = await TotalIngresosUltimaSemana();
                vmDasboard.totalProductos = await TotalProductos();

                List<VentasSemanaDTO> listaVentaSemana = new List<VentasSemanaDTO>();

                foreach(KeyValuePair<string,int> item in await VentasUltimaSemana())
                {
                    listaVentaSemana.Add(new VentasSemanaDTO()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }
                vmDasboard.VentasUltimaSemana=listaVentaSemana;
            }catch { throw; }
            return vmDasboard;
        }
    }
}
