using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APOM.Entities
{
    public class DATOS_TRANSFERENCIA 
    {
        public List<ComisionRequest> tranrows { get; set; }
       
    }
    
    public class ComisionRequest : IDisposable
    {
        
        public decimal MONTO_ORIGINAL_MONEDA_EXTRANJERA { get; set; }
        public string MONEDA_ORIGINAL { get; set; }
        public decimal TIPO_CAMBIO { get; set; }
        public decimal MONTO_ORIGINAL_CONVERSION_USD_BOB { get; set; }
        public decimal TIPO_CAMBIO_USD_BOB { get; set; }
        public decimal MONTO_A_PAGAR { get; set; }
        public string MONEDA_A_PAGAR { get; set; }
        public decimal COMISION { get; set; }
        public string COMISION_MONEDA { get; set; }
        public decimal COMISION_CONVERSION_USD_BOB { get; set; }
        public string COMISION_MONEDA_CONVERSION { get; set; }
        public decimal COMISION_REMESADORA { get; set; }
        public string COMISION_MONEDA_REMESADORA { get; set; }
        public decimal ITF_COMISION { get; set; }
        public string ITF_COMISION_MONEDA { get; set; }
        public decimal APOM_ITF_COMISION_BS { get; set; }
        //-----------------------------------------------
        public string IdMensaje { get; set; }
        public string ClienteBeneficiarioUnoDeCinco { get; set; }
        public string ClienteBeneficiarioDosDeCinco { get; set; }
        public string Banquero { get; set; }
        public string CodigoDeMoneda { get; set; }
        public string Importe { get; set; }
        public string FechaEmision { get; set; }
        public string SecuenciaRecepcion { get; set; }
        public string Tipo_Pago { get; set; }
        public string Validado { get; set; }
        public string Referencia { get; set; }
        public string CodigoOperacionBancaria { get; set; }


        public ComisionRequest()
        {
            this.MONTO_ORIGINAL_MONEDA_EXTRANJERA = 0.00M;
            this.MONEDA_ORIGINAL ="-";
            this.TIPO_CAMBIO = 0.00M;
            this.MONTO_ORIGINAL_CONVERSION_USD_BOB = 0.00M;
            this.TIPO_CAMBIO_USD_BOB = 0.00M;
            this.MONTO_A_PAGAR = 0.00M;
            this.MONEDA_A_PAGAR  ="-";
            this.COMISION = 0.00M;
            this.COMISION_MONEDA  ="-";
            this.COMISION_CONVERSION_USD_BOB = 0.00M;
            this.COMISION_MONEDA_CONVERSION  ="-";
            this.COMISION_REMESADORA = 0.00M;
            this.COMISION_MONEDA_REMESADORA  ="-";
            this.ITF_COMISION = 0.00M;
            this.ITF_COMISION_MONEDA  ="-";
            this.APOM_ITF_COMISION_BS = 0.00M;
        }
        void IDisposable.Dispose() { }

   
    }

    

}


