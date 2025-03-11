using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APOM.Entities
{
    public class AbonoCtaResponse : GenericResponse
    {
        public string CodRspta { get; set; }
        public string MsjRetorno { get; set; }
        public string NroOperacionHost { get; set; }
        public string Teti { get; set; }
        public string User { get; set; }
        public string TraceBiztalk { get; set; }
        public string SaldoContable { get; set; }
        public string SaldoDisponible { get; set; }
      

    }
}
