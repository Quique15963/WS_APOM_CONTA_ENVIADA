using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APOM.Common
{
    public class DetalleNombre
    {
        //Variable con el nombre o apellido de la persona
        private string Nombre = String.Empty;
        //Variable con el resultado de la comparacion
        private bool Hallado = false;
        //Porcentaje macheo
        private decimal PorcentajeMacheo = 0;
        //Inicializacion de la Clase
        public DetalleNombre(string NombreParametro)
        {
            this.Nombre = NombreParametro;
            this.Hallado = false;
            this.PorcentajeMacheo = 0;
        }
        //Funcion para Accesar al Nombre o apellido
        public string NomApe
        {
            get { return Nombre; }
            set { Nombre = value; }
        }
        //Funcion para Accesar al Valor de comparacion
        public bool ResultadoComparacion
        {
            get { return Hallado; }
            set { Hallado = value; }
        }
        //Funcion para Accesar al Valor de Porcentaje en macheo
        public decimal Porcentaje
        {
            get { return PorcentajeMacheo; }
            set { PorcentajeMacheo = value; }
        }
    }

}
