using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APOM.Entities
{
    public class ComisionesAutoRequest : IDisposable
    {
        public string idmensaje { get; set; }
        public int validado { get; set; }
        public decimal comision { get; set; }
        public string usuario { get; set; }
        public decimal ImporteC { get; set; }
        public string Porcentaje { get; set; }
        public string observacion { get; set; }
        public bool verificado { get; set; }

        public ComisionesAutoRequest()
        {

            this.idmensaje = "-";
            this.validado = 0;
            this.comision = 0.00M;
            this.usuario = "USERSWIFT";
            this.ImporteC = 0.00M;
            this.Porcentaje = "-";
            this.verificado = false;
            this.observacion = "";
           
        }
        void IDisposable.Dispose() { }

      
    }
}
