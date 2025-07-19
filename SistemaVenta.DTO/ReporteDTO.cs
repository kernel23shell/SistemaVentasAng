using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DTO
{
    public class ReporteDTO
    {
        public string? NumeroDocumento { get; set; }
        public string? TipoPago { get; private set; }
        public string? FechaRegistro { get; private set; }
        public string? TotalVenta { get; private set; }
        public string? Producto { get; private set; }
        public int? Cantidad { get; private set; }
        public string? Precio { get; private set; }
        public string? Total { get; private set; }
    }
}
