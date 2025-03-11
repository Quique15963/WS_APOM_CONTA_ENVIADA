using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APOM.Common;
using APOM.DataAccess;
using APOM.Security;
using System.Data;

namespace BCP.Whatsapp.DataAccessConta
{
    public class DataAccessWp
    {

        private string usuario ;
        private string server ;
        private string basedatos;
        private string pass;
        private string strconeccion;
      

        public DataAccessWp()
        {
            usuario = ManagerConfig.GetKeyCongigString("WPP_USUARIO");
            pass = ManagerConfig.GetKeyCongigString("WPP_PASSWORD");
            basedatos = ManagerConfig.GetKeyCongigString("WPP_BD");
            server = ManagerConfig.GetKeyCongigString("WPP_SERVER");
            Encryptador.EncryptDecrypt(false, pass, ref pass);

            strconeccion = DataAccess.ConexionSQL(server, basedatos, usuario, pass);
            
        }

        public DataTable GetContabilidadPendinteWhatsApp()
        {
            DataTable Datos = new DataTable();

            try
            {
                string NomSp = "[wpp].[WPP_GET_SERVICIOS_GIFT_CARD_CONTA]";

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                Datos = storeProcedure.RealizarConsulta(strconeccion);
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
        
        public bool UpdateApobrarContaWhatsApp(ContabilidadGiftRequest operacion, int estado)
        {
            bool respuesta = false;
            string NomSp = "[wpp].[WPP_UPDATE_SERVICES_GIFT_CONTA]";
            try
            {

                StoreProcedure storeProcedure = new StoreProcedure(NomSp);
                storeProcedure.AgregarParametro("@SERVICIO", operacion.SERVICIO, Direccion.Input);
                storeProcedure.AgregarParametro("@PRECIO", operacion.PRECIO, Direccion.Input);
                storeProcedure.AgregarParametro("@ESTADO", "C", Direccion.Input);
                storeProcedure.AgregarParametro("@CODIGO", operacion.CODIGO, Direccion.Input);
               
                storeProcedure.EjecutarStoreProcedure(strconeccion);
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





    }

    public class ContabilidadGiftRequest
    {


        public string SERVICIO { get; set; }
        public decimal PRECIO { get; set; }
        public string CODIGO { get; set; }
        public string ESTADO { get; set; }
        public string CIC { get; set; }
        public string ID_SERVICIO { get; set; }
        public string CANAL { get; set; }
        public string CELULAR { get; set; }
        public string CORREO_ELECTRONICO { get; set; }
        public string NRO_CUENTA { get; set; }
        public string ESTADO_CONTA { get; set; }
        public decimal DEBITO_BOL { get; set; }

        public string GLOSA { get; set; }
        public string MONEDA { get; set; }
        public string MONEDADESTINO { get; set; }



    }

}
