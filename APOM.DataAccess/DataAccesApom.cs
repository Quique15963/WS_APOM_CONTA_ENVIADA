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
    public class DataAccesApom
    {
        //string strConexion_apom = DataAccess.Conexion_Apom();
        private readonly string strConexion_apom = DataAccess.Conexion_Apom();


        #region SP DB_APOM

        public string ObtenerCorreo(int persona, ref string asunto, ref string mensaje)
        {

            string respuesta = "";

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_BuscarCorreo");
                storeProcedure.AgregarParametro("@Correo", persona, Direccion.Input);
                DataTable Correo = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_BuscarCorreo], Descripcion:" + storeProcedure.Error.Trim());
                }

                if (Correo.Rows.Count > 0)
                {
                    asunto = Convert.ToString(Correo.Rows[0]["asunto"]).TrimEnd(' ');
                    mensaje = Convert.ToString(Correo.Rows[0]["mensaje"]).TrimEnd(' ');
                    return Convert.ToString(Correo.Rows[0]["correo"]).Trim(' ');
                }


            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;


        }

        public string DTMonitoreoCorreos(string servicio)
        {
            string respuesta = "";
            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("napom.AP_SP_LEECORREOS_ERRORES");
                storeProcedure.AgregarParametro("@area_operativa", servicio, Direccion.Input);
                DataTable RESULTADO =storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_LEECORREOS_ERRORES], Descripcion:" + storeProcedure.Error.Trim());
                }
                respuesta = RESULTADO.Rows[0][0].ToString();

            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }

        public DataTable GetEsquemaContable(string NombreEsquema,string SucursalOrigen,int cuentaespecial)
        {
            DataTable Datos = new DataTable();

            try
            {
                string NomSp = "[napom].[GET_ESQUEMAS_CONTA]";

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                storeProcedure.AgregarParametro("@NOMBRE_ESQUEMA", NombreEsquema, Direccion.Input);
                storeProcedure.AgregarParametro("@SUCURSAL_ORIGEN", SucursalOrigen, Direccion.Input);
                storeProcedure.AgregarParametro("@CUENTA_ESPECIAL", cuentaespecial, Direccion.Input);

                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: "+ NomSp + ", Descripcion:" + storeProcedure.Error.Trim());
                }


            }
            catch (Exception)
            {
                throw;
            }

            return Datos;

        }

        public DataTable GetContabilidadPendinte()
        {
            DataTable Datos = new DataTable();

            try
            {
                string NomSp = "[napom].[GET_CONTABILIDAD_PENDIENTE]";

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                 Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: " + NomSp + ", Descripcion:" + storeProcedure.Error.Trim());
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Datos;

        }

        public DataTable GetContabilidadPendinteVent()
        {
            DataTable Datos = new DataTable();

            try
            {
                string NomSp = "[napom].[GET_CONTABILIDAD_PENDIENTE_VENT]";

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: " + NomSp + ", Descripcion:" + storeProcedure.Error.Trim());
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Datos;

        }

        #region SECTION 01: PROCESO DATA BASE CRIPTOS

        public DataTable GetContabilidadPendinteCripto()
        {
            DataTable Datos = new DataTable();

            try
            {
                string NomSp = "[napom].[GET_CONTABILIDAD_PENDIENTE_CRIPTO]";

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: " + NomSp + ", Descripcion:" + storeProcedure.Error.Trim());
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Datos;
        }
               

        public DataTable GetEsquemaContableCriptos(string NombreEsquema, string SucursalOrigen, int cuentaespecial)
        {
            DataTable Datos = new DataTable();

            try
            {
                string NomSp = "[napom].[GET_ESQUEMAS_CONTA_CRIPTO]";

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                storeProcedure.AgregarParametro("@NOMBRE_ESQUEMA", NombreEsquema, Direccion.Input);
                storeProcedure.AgregarParametro("@SUCURSAL_ORIGEN", SucursalOrigen, Direccion.Input);
                storeProcedure.AgregarParametro("@CUENTA_ESPECIAL", cuentaespecial, Direccion.Input);

                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: " + NomSp + ", Descripcion:" + storeProcedure.Error.Trim());
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Datos;
        }


        #endregion

        public bool UpdateApobrarConta(string operacion, int estado)
        {
            bool respuesta = false;
            string NomSp = "[napom].[APOM_UPDATE_CONTABILIDAD]";
            try
            {
                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                storeProcedure.AgregarParametro("@OPERACION", operacion, Direccion.Input);
                storeProcedure.AgregarParametro("@ESTADO", estado, Direccion.Input);
                storeProcedure.AgregarParametro("@USUARIO", "USRAPOMCONTA", Direccion.Input);

                storeProcedure.EjecutarStoreProcedure(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: "+ NomSp +" , Descripcion:" + storeProcedure.Error.Trim());
                }
               respuesta = true;
            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }

        public bool UpdateApobrarContaVent(string operacion, string estado)
        {
            bool respuesta = false;
            string NomSp = "[napom].[APOM_MOD_CONTA_VENT]";
            try
            {

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                storeProcedure.AgregarParametro("@OPERACION", operacion, Direccion.Input);
                storeProcedure.AgregarParametro("@ESTADO", estado, Direccion.Input);
             
                storeProcedure.EjecutarStoreProcedure(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: " + NomSp + " , Descripcion:" + storeProcedure.Error.Trim());
                }
                respuesta = true;


            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }



        #endregion



    }
}
