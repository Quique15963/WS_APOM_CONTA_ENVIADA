using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APOM.Entities
{
    public class AbonoCtaRequest
    {

        public string NombreCanal { get; set; }
        public string Password { get; set; }
        public string NroCuenta { get; set; }
        public string TipoDeCuenta { get; set; }
        public string Moneda { get; set; }
        public string Monto { get; set; }
        public string Glosa { get; set; }
        public string TipoCambioCompra { get; set; }
        public string TipoCambioVenta { get; set; }
        public string FechaSolicitudCanal { get; set; }
        public string HoraSolicitudCanal { get; set; }
        public string NroReferencialCanal { get; set; }
        public string NroOperacionOriginal { get; set; }
        public string Teti { get; set; }
        public string User { get; set; }
        

    }
}
