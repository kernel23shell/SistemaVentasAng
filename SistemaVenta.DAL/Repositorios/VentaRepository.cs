using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.Model;

namespace SistemaVenta.DAL.Repositorios
{
    public class VentaRepository : GenericRepository<Ventum>, IVentaRepository
    {
        private readonly DbventaContext _dbcontext;

        public VentaRepository(DbventaContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Ventum> Registrar(Ventum modelo) { 
            Ventum Ventagenerada = new Ventum();

            using(var transaction=_dbcontext.Database.BeginTransaction())

                try
                {
                    foreach (DetalleVentum dv in modelo.DetalleVenta) {
                        Producto producto_encontrado = _dbcontext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();
                        producto_encontrado.Stock=producto_encontrado.Stock - dv.Cantidad;
                        _dbcontext.Productos.Update(producto_encontrado);
                        await _dbcontext.SaveChangesAsync();
                    }

                    NumeroDocumento correlativo = _dbcontext.NumeroDocumentos.First();
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaRegistro = DateTime.Now;

                    _dbcontext.NumeroDocumentos.Update(correlativo);
                    await _dbcontext.SaveChangesAsync();

                    int cantidadDigitos = 4;
                    string ceros = string.Concat(Enumerable.Repeat("0", cantidadDigitos));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    //00001
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - cantidadDigitos, cantidadDigitos);

                    modelo.NumeroDocumento = numeroVenta;

                    await _dbcontext.Venta.AddAsync(modelo);
                    await _dbcontext.SaveChangesAsync();

                    Ventagenerada = modelo;

                    transaction.Commit();

                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            return Ventagenerada;
        }
        
    }
}

