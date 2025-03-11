using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APOM.Entities
{
    public class ContabilidadOperRequest : IDisposable
    {
        public string OPERACION { get; set; }
        public string NRO_REFER_TRANS { get; set; }
        public string MONEDAORIGEN { get; set; }
        public decimal MONTOORIGEN { get; set; }
        public string GLOSAORIGEN { get; set; }
        public string MONEDADESTINO { get; set; }
        public decimal MONTODESTINO { get; set; }
        public string BANCADESTINO { get; set; }
        public string CUENTADESTINO { get; set; }
        public decimal MONTOINTERMEDIO_USD { get; set; }
        public decimal COMISION_TRANS { get; set; }
        public string MONEDA_COMIS { get; set; }
        public decimal CARTA_ORDN_COMIS { get; set; }
        public decimal COMIS_PORTE { get; set; }
        public decimal COMIS_OUR { get; set; }
        public decimal COMIS_0 { get; set; }
        public decimal COMIS_1 { get; set; }
        public decimal IMPORTE_ITF { get; set; }
        public string MONEDA_ITF { get; set; }
        public string CUENTA_DEBITO { get; set; }
        public decimal ITF_COMISION { get; set; }
        public string DETALLE_CARGO { get; set; }

        public ContabilidadOperRequest()
        {

            this.OPERACION = string.Empty;
            this.NRO_REFER_TRANS = string.Empty;
            this.MONEDAORIGEN = string.Empty;
            this.MONTOORIGEN = 0.00M;
            this.GLOSAORIGEN = string.Empty;
            this.MONEDADESTINO = string.Empty;
            this.MONTODESTINO = 0.00M;
            this.BANCADESTINO = string.Empty;
            this.CUENTADESTINO = string.Empty;
            this.MONTOINTERMEDIO_USD = 0.00M;
            this.COMISION_TRANS = 0.00M;
            this.MONEDA_COMIS = string.Empty;
            this.CARTA_ORDN_COMIS = 0.00M;
            this.COMIS_PORTE = 0.00M;
            this.COMIS_OUR = 0.00M;
            this.COMIS_0 = 0.00M;
            this.COMIS_1 = 0.00M;
            this.IMPORTE_ITF = 0.00M;
            this.MONEDA_ITF = string.Empty;
            this.CUENTA_DEBITO = string.Empty;
            this.ITF_COMISION = 0.00M;

        }
        void IDisposable.Dispose() { }


    }
}
