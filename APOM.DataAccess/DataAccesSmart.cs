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
using APOM.DataAccess.SrvSLK;
using System.Net;

namespace APOM.DataAccess
{
    public class DataAccesSmart
    {
        string strConexion_smart = DataAccess.Conexion_Smart();
   
        public bool InsertAsientosContables(Diarios_Interfaz.BCB_diarios_interfacesDataTable Asientos ,string correoerrores)
        {
            bool respuesta = true;

            try
            {
                System.Data.SqlClient.SqlConnection ConSst = new SqlConnection(strConexion_smart);
                Diarios_InterfazTableAdapters.BCB_diarios_interfacesTableAdapter asientosAdapter = new Diarios_InterfazTableAdapters.BCB_diarios_interfacesTableAdapter();
                asientosAdapter.Connection = ConSst;
                asientosAdapter.Update(Asientos);
            }
            catch (Exception ex)
            {
                respuesta = false;
                Bitacora.Enviarmails5("Error al Insertar los datos de Contabilidad " + ex.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error al Insertar los datos de Contabilidad ", ex);
            }

            return respuesta;

        }


        public int lineaDiario(string IdDiario,string Sucursal)
        {
            int respuesta = 1;

            try
            {
                string NomSp = "dbo.[AP_SP_Lectura_Linea_Diario_Conta]";
               
                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                storeProcedure.AgregarParametro("@IdDiario", IdDiario, Direccion.Input);
                storeProcedure.AgregarParametro("@sucursal", Sucursal, Direccion.Input);
                storeProcedure.AgregarParametro("@fechaini", string.Format("{0:yyyyMMdd}", DateTime.Now), Direccion.Input);
                storeProcedure.AgregarParametro("@fechafin", string.Format("{0:MM/dd/yyyy}", DateTime.Now), Direccion.Input);
                
                DataTable dato = storeProcedure.RealizarConsulta(strConexion_smart);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: " + NomSp + ", Descripcion:" + storeProcedure.Error.Trim());
                }

                if (dato.Rows.Count > 0)
                {
                    if (dato.Rows[0]["linea"].ToString().Trim() != "")
                    {
                        respuesta = Convert.ToInt32(dato.Rows[0]["linea"]);
                        respuesta++;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }

        public InformacionTipoCambio TraerTipoCambio()
        {
            //--- FLAG SELECCIONA TLS 
            string FlagTLS = ConfigurationSettings.AppSettings["FlagTLS"];
            if (FlagTLS == "0") ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls | SecurityProtocolType.Tls;
            if (FlagTLS == "1") ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
            if (FlagTLS == "2") ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            (se, cert, chain, sslerror) =>
            {
                return true;
            };
            //--- FIN DE LA ASIGNACION DEL TLS
            InformacionTipoCambio tipoCambio;
            SrvSLKClient client = new SrvSLKClient();
            TipoCambio_PARAMETROS parametros;
            try
            {
                parametros = new TipoCambio_PARAMETROS();
                parametros.CodigoIso = "840";
                parametros.Fecha = DateTime.Now.ToString("yyyMMdd");
                SrvSLK.TipoCambio response = client.ConsultaTC(parametros);
                if (response.Error == string.Empty)
                {
                    tipoCambio = new InformacionTipoCambio();
                    tipoCambio.Fecha = response.Fecha;
                    tipoCambio.TasaCompra = response.TasaCompra;
                    tipoCambio.TasaContable = response.TasaContable;
                    tipoCambio.TasaVenta = response.TasaVenta;

                }
                else
                {
                    throw new Exception("Error al traer los tipos de cambio" + response.Error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (client.State == System.ServiceModel.CommunicationState.Opened)
                    client.Close();
            }
            return tipoCambio;
        }





    }
}
