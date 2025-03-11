using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APOM.Entities
{
    public class InformacionTipoCambio
    {
        private string fecha;
        public string Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }
        private decimal tasaContable;
        public decimal TasaContable
        {
            get { return tasaContable; }
            set { tasaContable = value; }
        }
        private decimal tasaCompra;
        public decimal TasaCompra
        {
            get { return tasaCompra; }
            set { tasaCompra = value; }
        }
        private decimal tasaVenta;
        public decimal TasaVenta
        {
            get { return tasaVenta; }
            set { tasaVenta = value; }
        }
    }
}
