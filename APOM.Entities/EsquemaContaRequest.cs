using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APOM.Entities
{



    public class EsquemaContaRequest
    {
        public List<ContabilidadRequest> esquemarows { get; set; }

    }

    public class ContabilidadRequest : IDisposable
    {
        public string NOMBRE_ESQUEMA { get; set; }
        public string SUCURSAL { get; set; }
        public string CUENTA { get; set; }
        public string BANCA { get; set; }
        public string PRODUCTO { get; set; }
        public string MONEDA { get; set; }
        public string DEB_CRED { get; set; }
        public string SUCURSAL_ORIGEN { get; set; }
        public int IMPORTE { get; set; }
        public int CUENTA_ESPECIAL { get; set; }
        public string GLOSA { get; set; }


        public ContabilidadRequest()
        {

            this.NOMBRE_ESQUEMA = string.Empty;
            this.SUCURSAL = string.Empty;
            this.CUENTA = string.Empty;
            this.BANCA = string.Empty;
            this.PRODUCTO = string.Empty;
            this.MONEDA = string.Empty;
            this.DEB_CRED = string.Empty;
            this.SUCURSAL_ORIGEN = string.Empty;
            this.IMPORTE = 0;
            this.CUENTA_ESPECIAL = 0;

        }
        void IDisposable.Dispose() { }

      
    }
}
