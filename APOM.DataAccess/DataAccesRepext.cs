using APOM.DataAccess;
using APOM.Entities;
using APOM.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Globalization;
using APOM.Log;
using NLog;
using System.Configuration;
namespace APOM.DataAccess
{
    public class DataAccesRepext
    {
        string strConexion_repext = DataAccess.Conexion_Repext();


        public string GetBancaCuenta(string Cuenta)
        {

            string respuesta = "";

            try
            {
                string NomSp = "apom.AP_SP_LEE_CARACTERES_CONTA";
                string resultado = "";
                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                storeProcedure.AgregarParametro("@cuenta", Funciones.FormatoCuentaRepExt(Cuenta), Direccion.Input);
                
                DataTable dato = storeProcedure.RealizarConsulta(strConexion_repext);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: "+ NomSp + ", Descripcion:" + storeProcedure.Error.Trim());
                }

                if (dato.Rows.Count > 0)
                {
                    respuesta = Convert.ToString(dato.Rows[0]["DATO"]).TrimEnd(' ');
                   
                }
            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;


        }

        public string GetBancaDatoCuenta(string DatoCuenta)
        {

            string respuesta = "";

            try
            {
                string NomSp = "apom.[AP_SP_LEE_BANCA_CONTA]";

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                storeProcedure.AgregarParametro("@codigo", DatoCuenta, Direccion.Input);
                DataTable dato = storeProcedure.RealizarConsulta(strConexion_repext);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: " + NomSp + ", Descripcion:" + storeProcedure.Error.Trim());
                }

                if (dato.Rows.Count > 0)
                {
                    respuesta = Convert.ToString(dato.Rows[0]["LX_CODIGO_BCP"]).TrimEnd(' ');

                }


            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;


        }


        public string GetProductoCuenta(string Cuenta)
        {

            string respuesta = "";

            try
            {
                string NomSp = "apom.[AP_SP_LEE_PRODUCTO_CONTA]";

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                storeProcedure.AgregarParametro("@cuenta", Funciones.FormatoCuentaRepExt(Cuenta), Direccion.Input);
                DataTable dato = storeProcedure.RealizarConsulta(strConexion_repext);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: " + NomSp + ", Descripcion:" + storeProcedure.Error.Trim());
                }

                if (dato.Rows.Count > 0)
                {
                    respuesta = Convert.ToString(dato.Rows[0]["CP_TIOAUX"]).TrimEnd(' ');

                }


            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;


        }



        public decimal GetTIPOCAMBIOREPEXTa()
        {

            decimal respuesta = 0;

            try
            {
                string NomSp = "apom.AP_SP_LEETIPOCAMBIOVIEWRECAMBIOS";

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                
                DataTable dato = storeProcedure.RealizarConsulta(strConexion_repext);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: " + NomSp + ", Descripcion:" + storeProcedure.Error.Trim());
                }

                if (dato.Rows.Count > 0)
                {
                    respuesta = Convert.ToDecimal(dato.Rows[0]["CAM_TIPCAMBIO"]);

                }


            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;


        }
              





    }
}
