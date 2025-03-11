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
namespace APOM.BD
{
    public class DataAccesApom
    {
        string strConexion_apom = DataAccess.DataAccess.Conexion_Apom();
        string strConexion_repxt = DataAccess.DataAccess.Conexion_Repext();
        string userservicio = ConfigurationManager.AppSettings["USER_SERVICIO"];

             
      
        #region SP REPEXT
      
        public bool EstadoCuenta(string Cuenta)
        {

            bool response = false;
            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("apom.AP_SP_BuscarCuentaActiva");
                storeProcedure.AgregarParametro("@cuenta", Funciones.FormatoCuentaRepExt(Cuenta), Direccion.Input);
                DataTable Existe = storeProcedure.RealizarConsulta(strConexion_repxt);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_BuscarCuentaActiva], Descripcion:" + storeProcedure.Error.Trim());
                }
                
                if (Existe.Rows.Count > 0) response=true;
                

            }

            catch (Exception)
            {
                throw;
            }


            return response;
            
           
        }
        public bool VerificarClienteCuenta(string Cuenta, string NombreSwift, ref decimal porcentaje)
        {

            bool response = false;
            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("apom.[AP_SP_BuscarCuenta]");
                storeProcedure.AgregarParametro("@cuenta",Funciones.FormatoCuentaRepExt(Cuenta), Direccion.Input);
                DataTable Existe = storeProcedure.RealizarConsulta(strConexion_repxt);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: apom.[AP_SP_BuscarCuenta], Descripcion:" + storeProcedure.Error.Trim());
                }

                if (Existe.Rows.Count > 0)
                {
                      return Funciones.ComparacionClientes(Existe.Rows[0]["Cliente"].ToString(), NombreSwift,true, ref porcentaje);
                }
                else{
                    return Funciones.ComparacionClientes(Existe.Rows[0]["Cliente"].ToString(), NombreSwift, false, ref porcentaje);
                }


            }

            catch (Exception)
            {
                throw;
            }


            return response;


        }
        public DataTable GetClienteRepext(string cuenta)
        {
            DataTable Datos = new DataTable();

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("apom.AP_SP_BuscarCIC");
                storeProcedure.AgregarParametro("@Cuenta",Funciones.FormatoCuentaRepExt( cuenta), Direccion.Input);
                Datos = storeProcedure.RealizarConsulta(strConexion_repxt);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_BuscarCIC], Descripcion:" + storeProcedure.Error.Trim());
                }


            }
            catch (Exception)
            {
                throw;
            }

            return Datos;

        }


        #endregion

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

        public bool InsertDatosMensajeOrig(MensajeOriginalRequest mo)
        {
            bool respuesta = false;

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_GuardarMensaje");

                storeProcedure.AgregarParametro("@idmensaje", mo.idmensaje, Direccion.Input);
                storeProcedure.AgregarParametro("@tipomensaje", mo.tipomensaje, Direccion.Input);
                storeProcedure.AgregarParametro("@secrecepcion", mo.secrecepcion, Direccion.Input);
                storeProcedure.AgregarParametro("@fechaemision", mo.fechaemision, Direccion.Input);
                storeProcedure.AgregarParametro("@horaemision", mo.horaemision, Direccion.Input);
                storeProcedure.AgregarParametro("@fecharecepcion", mo.fecharecepcion, Direccion.Input);
                storeProcedure.AgregarParametro("@horarecepcion", mo.horarecepcion, Direccion.Input);
                storeProcedure.AgregarParametro("@status", mo.status, Direccion.Input);
                storeProcedure.AgregarParametro("@remitente", mo.remitente, Direccion.Input);
                storeProcedure.AgregarParametro("@receptor", mo.receptor, Direccion.Input);
                storeProcedure.AgregarParametro("@referencia", mo.referencia, Direccion.Input);
                storeProcedure.AgregarParametro("@indicadorhora1", mo.indicadorhora1, Direccion.Input);
                storeProcedure.AgregarParametro("@indicadorhora2", mo.indicadorhora2, Direccion.Input);
                storeProcedure.AgregarParametro("@indicadorhora3", mo.indicadorhora3, Direccion.Input);
                storeProcedure.AgregarParametro("@indicadorhora4", mo.indicadorhora4, Direccion.Input);
                storeProcedure.AgregarParametro("@CodOperacionBancarira", mo.CodOperacionBancarira, Direccion.Input);
                storeProcedure.AgregarParametro("@CodInstruccion1", mo.CodInstruccion1, Direccion.Input);
                storeProcedure.AgregarParametro("@CodInstruccion2", mo.CodInstruccion2, Direccion.Input);
                storeProcedure.AgregarParametro("@CodInstruccion3", mo.CodInstruccion3, Direccion.Input);
                storeProcedure.AgregarParametro("@CodInstruccion4", mo.CodInstruccion4, Direccion.Input);
                storeProcedure.AgregarParametro("@CodTipoTransaccion", mo.CodTipoTransaccion, Direccion.Input);
                storeProcedure.AgregarParametro("@UsoFuturo", mo.UsoFuturo, Direccion.Input);
                storeProcedure.AgregarParametro("@FechaValor", mo.FechaValor, Direccion.Input);
                storeProcedure.AgregarParametro("@CodMoneda", mo.CodMoneda, Direccion.Input);
                storeProcedure.AgregarParametro("@Importe", mo.sImporte, Direccion.Input);
                storeProcedure.AgregarParametro("@MonInstruida", mo.MonInstruida, Direccion.Input);
                storeProcedure.AgregarParametro("@ImporteInstruido", mo.sImporteInstruido, Direccion.Input);
                storeProcedure.AgregarParametro("@TasaCambio", mo.sTasaCambio, Direccion.Input);
                storeProcedure.AgregarParametro("@FCClienteOrdenante", mo.FCClienteOrdenante, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteOrdenante1", mo.ClienteOrdenante1, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteOrdenante2", mo.ClienteOrdenante2, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteOrdenante3", mo.ClienteOrdenante3, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteOrdenante4", mo.ClienteOrdenante4, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteOrdenante5", mo.ClienteOrdenante5, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionRemitente", mo.InstitucionRemitente, Direccion.Input);
                storeProcedure.AgregarParametro("@FCInstitucionOrdenante", mo.FCInstitucionOrdenante, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionOrdenante1", mo.InstitucionOrdenante1, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionOrdenante2", mo.InstitucionOrdenante2, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionOrdenante3", mo.InstitucionOrdenante3, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionOrdenante4", mo.InstitucionOrdenante4, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionOrdenante5", mo.InstitucionOrdenante5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCCorrersponsalRemitente", mo.FCCorrersponsalRemitente, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalRemitente1", mo.CorresponsalRemitente1, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalRemitente2", mo.CorresponsalRemitente2, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalRemitente3", mo.CorresponsalRemitente3, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalRemitente4", mo.CorresponsalRemitente4, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalRemitente5", mo.CorresponsalRemitente5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCCorresponsalReceptor", mo.FCCorresponsalReceptor, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalReceptor1", mo.CorresponsalReceptor1, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalReceptor2", mo.CorresponsalReceptor2, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalReceptor3", mo.CorresponsalReceptor3, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalReceptor4", mo.CorresponsalReceptor4, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalReceptor5", mo.CorresponsalReceptor5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCTerceraInstitucionReembolsante", mo.FCTerceraInstitucionReembolsante, Direccion.Input);
                storeProcedure.AgregarParametro("@TerceraInstitucionReembolsante1", mo.TerceraInstitucionReembolsante1, Direccion.Input);
                storeProcedure.AgregarParametro("@TerceraInstitucionReembolsante2", mo.TerceraInstitucionReembolsante2, Direccion.Input);
                storeProcedure.AgregarParametro("@TerceraInstitucionReembolsante3", mo.TerceraInstitucionReembolsante3, Direccion.Input);
                storeProcedure.AgregarParametro("@TerceraInstitucionReembolsante4", mo.TerceraInstitucionReembolsante4, Direccion.Input);
                storeProcedure.AgregarParametro("@TerceraInstitucionReembolsante5", mo.TerceraInstitucionReembolsante5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCInstitucionIntermediaria", mo.FCInstitucionIntermediaria, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionIntermediaria1", mo.InstitucionIntermediaria1, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionIntermediaria2", mo.InstitucionIntermediaria2, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionIntermediaria3", mo.InstitucionIntermediaria3, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionIntermediaria4", mo.InstitucionIntermediaria4, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionIntermediaria5", mo.InstitucionIntermediaria5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCCuentaInstitucion", mo.FCCuentaInstitucion, Direccion.Input);
                storeProcedure.AgregarParametro("@CuentaInstitucion1", mo.CuentaInstitucion1, Direccion.Input);
                storeProcedure.AgregarParametro("@CuentaInstitucion2", mo.CuentaInstitucion2, Direccion.Input);
                storeProcedure.AgregarParametro("@CuentaInstitucion3", mo.CuentaInstitucion3, Direccion.Input);
                storeProcedure.AgregarParametro("@CuentaInstitucion4", mo.CuentaInstitucion4, Direccion.Input);
                storeProcedure.AgregarParametro("@CuentaInstitucion5", mo.CuentaInstitucion5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCClienteBeneficiario", mo.FCClienteBeneficiario, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteBeneficiario1", mo.ClienteBeneficiario1, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteBeneficiario2", mo.ClienteBeneficiario2, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteBeneficiario3", mo.ClienteBeneficiario3, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteBeneficiario4", mo.ClienteBeneficiario4, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteBeneficiario5", mo.ClienteBeneficiario5, Direccion.Input);
                storeProcedure.AgregarParametro("@InformacionRemitente1", mo.InformacionRemitente1, Direccion.Input);
                storeProcedure.AgregarParametro("@InformacionRemitente2", mo.InformacionRemitente2, Direccion.Input);
                storeProcedure.AgregarParametro("@InformacionRemitente3", mo.InformacionRemitente3, Direccion.Input);
                storeProcedure.AgregarParametro("@InformacionRemitente4", mo.InformacionRemitente4, Direccion.Input);
                storeProcedure.AgregarParametro("@DetallesCargo", mo.DetallesCargo, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosRemitente1", mo.CargosRemitente1, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosRemitente2", mo.CargosRemitente2, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosRemitente3", mo.CargosRemitente3, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosRemitente4", mo.CargosRemitente4, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosRemitente5", mo.CargosRemitente5, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosReceptor", mo.CargosReceptor, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor1", mo.InfRemitAReceptor1, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor2", mo.InfRemitAReceptor2, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor3", mo.InfRemitAReceptor3, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor4", mo.InfRemitAReceptor4, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor5", mo.InfRemitAReceptor5, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor6", mo.InfRemitAReceptor6, Direccion.Input);
                storeProcedure.AgregarParametro("@ReporteRegulatorio1", mo.ReporteRegulatorio1, Direccion.Input);
                storeProcedure.AgregarParametro("@ReporteRegulatorio2", mo.ReporteRegulatorio2, Direccion.Input);
                storeProcedure.AgregarParametro("@ReporteRegulatorio3", mo.ReporteRegulatorio3, Direccion.Input);
                storeProcedure.AgregarParametro("@EstadoDuplicidad", mo.EstadoDuplicidad, Direccion.Input);
                storeProcedure.AgregarParametro("@EspacioBlanco", mo.EspacioBlanco, Direccion.Input);
            
                storeProcedure.EjecutarStoreProcedure(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_GuardarMensaje], Descripcion:" + storeProcedure.Error.Trim());
                }

                respuesta = true;
            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }

        public bool InsertDatosMensajeMT103(MensajeOriginalRequest mo)
        {
            bool respuesta = false;

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_GuardarMensajeValidado");

                storeProcedure.AgregarParametro("@idmensaje", mo.idmensaje, Direccion.Input);
                storeProcedure.AgregarParametro("@tipomensaje", mo.tipomensaje, Direccion.Input);
                storeProcedure.AgregarParametro("@secrecepcion", mo.secrecepcion, Direccion.Input);
                storeProcedure.AgregarParametro("@fechaemision", mo.fechaemision, Direccion.Input);
                storeProcedure.AgregarParametro("@horaemision", mo.horaemision, Direccion.Input);
                storeProcedure.AgregarParametro("@fecharecepcion", mo.fecharecepcion, Direccion.Input);
                storeProcedure.AgregarParametro("@horarecepcion", mo.horarecepcion, Direccion.Input);
                storeProcedure.AgregarParametro("@status", mo.status, Direccion.Input);
                storeProcedure.AgregarParametro("@remitente", mo.remitente, Direccion.Input);
                storeProcedure.AgregarParametro("@referencia", mo.referencia, Direccion.Input);
                storeProcedure.AgregarParametro("@indicadorhora1", mo.indicadorhora1, Direccion.Input);
                storeProcedure.AgregarParametro("@indicadorhora2", mo.indicadorhora2, Direccion.Input);
                storeProcedure.AgregarParametro("@indicadorhora3", mo.indicadorhora3, Direccion.Input);
                storeProcedure.AgregarParametro("@indicadorhora4", mo.indicadorhora4, Direccion.Input);
                storeProcedure.AgregarParametro("@CodOperacionBancarira", mo.CodOperacionBancarira, Direccion.Input);
                storeProcedure.AgregarParametro("@CodInstruccion1", mo.CodInstruccion1, Direccion.Input);
                storeProcedure.AgregarParametro("@CodInstruccion2", mo.CodInstruccion2, Direccion.Input);
                storeProcedure.AgregarParametro("@CodInstruccion3", mo.CodInstruccion3, Direccion.Input);
                storeProcedure.AgregarParametro("@CodInstruccion4", mo.CodInstruccion4, Direccion.Input);
                storeProcedure.AgregarParametro("@CodTipoTransaccion", mo.CodTipoTransaccion, Direccion.Input);
                storeProcedure.AgregarParametro("@UsoFuturo", mo.UsoFuturo, Direccion.Input);
                storeProcedure.AgregarParametro("@FechaValor", mo.FechaValor, Direccion.Input);
                storeProcedure.AgregarParametro("@CodMoneda", mo.CodMoneda, Direccion.Input);
                storeProcedure.AgregarParametro("@Importe", mo.Importe, Direccion.Input);
                storeProcedure.AgregarParametro("@MonInstruida", mo.MonInstruida, Direccion.Input);
                storeProcedure.AgregarParametro("@ImporteInstruido", mo.ImporteInstruido, Direccion.Input);
                storeProcedure.AgregarParametro("@TasaCambio", mo.TasaCambio, Direccion.Input);
                storeProcedure.AgregarParametro("@FCClienteOrdenante", mo.FCClienteOrdenante, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteOrdenante1", mo.ClienteOrdenante1, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteOrdenante2", mo.ClienteOrdenante2, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteOrdenante3", mo.ClienteOrdenante3, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteOrdenante4", mo.ClienteOrdenante4, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteOrdenante5", mo.ClienteOrdenante5, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionRemitente", mo.InstitucionRemitente, Direccion.Input);
                storeProcedure.AgregarParametro("@FCInstitucionOrdenante", mo.FCInstitucionOrdenante, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionOrdenante1", mo.InstitucionOrdenante1, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionOrdenante2", mo.InstitucionOrdenante2, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionOrdenante3", mo.InstitucionOrdenante3, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionOrdenante4", mo.InstitucionOrdenante4, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionOrdenante5", mo.InstitucionOrdenante5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCCorrersponsalRemitente", mo.FCCorrersponsalRemitente, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalRemitente1", mo.CorresponsalRemitente1, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalRemitente2", mo.CorresponsalRemitente2, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalRemitente3", mo.CorresponsalRemitente3, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalRemitente4", mo.CorresponsalRemitente4, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalRemitente5", mo.CorresponsalRemitente5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCCorresponsalReceptor", mo.FCCorresponsalReceptor, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalReceptor1", mo.CorresponsalReceptor1, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalReceptor2", mo.CorresponsalReceptor2, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalReceptor3", mo.CorresponsalReceptor3, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalReceptor4", mo.CorresponsalReceptor4, Direccion.Input);
                storeProcedure.AgregarParametro("@CorresponsalReceptor5", mo.CorresponsalReceptor5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCTerceraInstitucionReembolsante", mo.FCTerceraInstitucionReembolsante, Direccion.Input);
                storeProcedure.AgregarParametro("@TerceraInstitucionReembolsante1", mo.TerceraInstitucionReembolsante1, Direccion.Input);
                storeProcedure.AgregarParametro("@TerceraInstitucionReembolsante2", mo.TerceraInstitucionReembolsante2, Direccion.Input);
                storeProcedure.AgregarParametro("@TerceraInstitucionReembolsante3", mo.TerceraInstitucionReembolsante3, Direccion.Input);
                storeProcedure.AgregarParametro("@TerceraInstitucionReembolsante4", mo.TerceraInstitucionReembolsante4, Direccion.Input);
                storeProcedure.AgregarParametro("@TerceraInstitucionReembolsante5", mo.TerceraInstitucionReembolsante5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCInstitucionIntermediaria", mo.FCInstitucionIntermediaria, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionIntermediaria1", mo.InstitucionIntermediaria1, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionIntermediaria2", mo.InstitucionIntermediaria2, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionIntermediaria3", mo.InstitucionIntermediaria3, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionIntermediaria4", mo.InstitucionIntermediaria4, Direccion.Input);
                storeProcedure.AgregarParametro("@InstitucionIntermediaria5", mo.InstitucionIntermediaria5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCCuentaInstitucion", mo.FCCuentaInstitucion, Direccion.Input);
                storeProcedure.AgregarParametro("@CuentaInstitucion1", mo.CuentaInstitucion1, Direccion.Input);
                storeProcedure.AgregarParametro("@CuentaInstitucion2", mo.CuentaInstitucion2, Direccion.Input);
                storeProcedure.AgregarParametro("@CuentaInstitucion3", mo.CuentaInstitucion3, Direccion.Input);
                storeProcedure.AgregarParametro("@CuentaInstitucion4", mo.CuentaInstitucion4, Direccion.Input);
                storeProcedure.AgregarParametro("@CuentaInstitucion5", mo.CuentaInstitucion5, Direccion.Input);
                storeProcedure.AgregarParametro("@FCClienteBeneficiario", mo.FCClienteBeneficiario, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteBeneficiario1", mo.ClienteBeneficiario1, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteBeneficiario2", mo.ClienteBeneficiario2, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteBeneficiario3", mo.ClienteBeneficiario3, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteBeneficiario4", mo.ClienteBeneficiario4, Direccion.Input);
                storeProcedure.AgregarParametro("@ClienteBeneficiario5", mo.ClienteBeneficiario5, Direccion.Input);
                storeProcedure.AgregarParametro("@InformacionRemitente1", mo.InformacionRemitente1, Direccion.Input);
                storeProcedure.AgregarParametro("@InformacionRemitente2", mo.InformacionRemitente2, Direccion.Input);
                storeProcedure.AgregarParametro("@InformacionRemitente3", mo.InformacionRemitente3, Direccion.Input);
                storeProcedure.AgregarParametro("@InformacionRemitente4", mo.InformacionRemitente4, Direccion.Input);
                storeProcedure.AgregarParametro("@DetallesCargo", mo.DetallesCargo, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosRemitente1", mo.CargosRemitente1, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosRemitente2", mo.CargosRemitente2, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosRemitente3", mo.CargosRemitente3, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosRemitente4", mo.CargosRemitente4, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosRemitente5", mo.CargosRemitente5, Direccion.Input);
                storeProcedure.AgregarParametro("@CargosReceptor", mo.CargosReceptor, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor1", mo.InfRemitAReceptor1, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor2", mo.InfRemitAReceptor2, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor3", mo.InfRemitAReceptor3, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor4", mo.InfRemitAReceptor4, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor5", mo.InfRemitAReceptor5, Direccion.Input);
                storeProcedure.AgregarParametro("@InfRemitAReceptor6", mo.InfRemitAReceptor6, Direccion.Input);
                storeProcedure.AgregarParametro("@ReporteRegulatorio1", mo.ReporteRegulatorio1, Direccion.Input);
                storeProcedure.AgregarParametro("@ReporteRegulatorio2", mo.ReporteRegulatorio2, Direccion.Input);
                storeProcedure.AgregarParametro("@ReporteRegulatorio3", mo.ReporteRegulatorio3, Direccion.Input);
                storeProcedure.AgregarParametro("@EstadoDuplicidad", mo.EstadoDuplicidad, Direccion.Input);
                storeProcedure.AgregarParametro("@EspacioBlanco", mo.EspacioBlanco, Direccion.Input);
                storeProcedure.AgregarParametro("@Tipo_Pago", mo.Tipo_Pago, Direccion.Input);
                storeProcedure.AgregarParametro("@Observacion", mo.Observacion, Direccion.Input);
                storeProcedure.AgregarParametro("@DObservacion", mo.DObservacion, Direccion.Input);
                storeProcedure.AgregarParametro("@Banquero", mo.Banquero, Direccion.Input);
                storeProcedure.AgregarParametro("@ApPaternoBen", mo.ApPaternoBen, Direccion.Input);
                storeProcedure.AgregarParametro("@ApMaternoBen", mo.ApMaternoBen, Direccion.Input);
                storeProcedure.AgregarParametro("@validado", mo.validado, Direccion.Input);
                
                storeProcedure.EjecutarStoreProcedure(strConexion_apom);
                
                                
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_GuardarMensajeValidado], Descripcion:" + storeProcedure.Error.Trim());
                }

                respuesta = true;
            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }

        public string Get_Banqueros(string codigo, string CodigoOperacionBancaria, string moneda)
        {
            string respuesta = "";
            bool CodOpBan;
            DataTable Datos = new DataTable();
            try
            {
                CodOpBan = CodigoOperacionBancaria == "CRED" ? false : true;

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_BuscarBanquero");
                storeProcedure.AgregarParametro("@Codigo", codigo, Direccion.Input);
                storeProcedure.AgregarParametro("@CodOpBan", CodOpBan, Direccion.Input);
                storeProcedure.AgregarParametro("@Moneda", moneda, Direccion.Input);
                Datos=storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_LEECORREOS], Descripcion:" + storeProcedure.Error.Trim());
                }
                if (Datos.Rows.Count > 0)
                respuesta = Datos.Rows[0]["Codigo"].ToString();
                
            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }

        public bool VerificarExistenciaRegistro(string referencia, string banquero)
        {
            bool respuesta =false;
      
            DataTable Datos = new DataTable();
            try
            {
              
                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_VerificaRegistro");
                storeProcedure.AgregarParametro("@Referencia", referencia, Direccion.Input);
                storeProcedure.AgregarParametro("@Banquero", banquero, Direccion.Input);
               
                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_VerificaRegistro], Descripcion:" + storeProcedure.Error.Trim());
                }
                if (Datos.Rows.Count > 0) respuesta = true;
                    

            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }

        public DataTable GetParametros()
        {
            DataTable Datos = new DataTable();

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_AbrirParametros");
                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_AbrirParametros], Descripcion:" + storeProcedure.Error.Trim());
                }


            }
            catch (Exception)
            {
                throw;
            }

            return Datos;

        }

        public DataTable GetCredAutomatico()
        {
            DataTable Datos = new DataTable();

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_CargarMensajesCREDAutomatico");
                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_CargarMensajesCREDAutomatico], Descripcion:" + storeProcedure.Error.Trim());
                }


            }
            catch (Exception)
            {
                throw;
            }

            return Datos;

        }

        public bool GetTarifa(ref decimal tarifap, ref decimal tarifam, ref decimal maximo, ref decimal minimo, ref string Error)
        {
            DataTable Datos = new DataTable();

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_BuscarTarifa");
                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_BuscarTarifa], Descripcion:" + storeProcedure.Error.Trim());
                }

                if (Datos.Rows.Count > 0)
                {
                    tarifap = Convert.ToDecimal(Datos.Rows[0][0]);
                    tarifam = Convert.ToDecimal(Datos.Rows[0][1]);
                    maximo = Convert.ToDecimal(Datos.Rows[0][2]);
                    minimo = Convert.ToDecimal(Datos.Rows[0][3]);
                    return true;
                }
                else
                {
                    Error = "Error en Carga de Tarifas";
                    return false;
                }


            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public DataTable GetCuentaPreferente(string cuenta)
        {
            DataTable Datos = new DataTable();

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_BuscarPreferente");
                storeProcedure.AgregarParametro("@cuenta", cuenta, Direccion.Input);
                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_BuscarPreferente], Descripcion:" + storeProcedure.Error.Trim());
                }


            }
            catch (Exception)
            {
                throw;
            }

            return Datos;

        }
        public DataTable GetClientePreferente(string cic)
        {
            DataTable Datos = new DataTable();

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_BuscarClientePreferente");
                storeProcedure.AgregarParametro("@CIC", cic, Direccion.Input);
                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_BuscarClientePreferente], Descripcion:" + storeProcedure.Error.Trim());
                }


            }
            catch (Exception)
            {
                throw;
            }

            return Datos;

        }

        public DataTable GetBanqueroPreferente()
        {
            DataTable Datos = new DataTable();

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_BuscarSigla");
                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_BuscarSigla], Descripcion:" + storeProcedure.Error.Trim());
                }


            }
            catch (Exception)
            {
                throw;
            }

            return Datos;

        }
        public bool GetClienteTCPreferenteBolivianos(string CIC)
        {
            DataTable Datos = new DataTable();
            bool response = false;
            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_BuscarClienteTCPreferente");
                storeProcedure.AgregarParametro("@CIC", CIC, Direccion.Input);
                Datos = storeProcedure.RealizarConsulta(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_BuscarSigla], Descripcion:" + storeProcedure.Error.Trim());
                }
               
                response=Datos.Rows.Count > 0?true:false;

            }
            catch (Exception)
            {
                throw;
            }

            return response;

        }

        public bool insertValidadoMensaje(ComisionesAutoRequest Com)
        {
            bool respuesta = false;

            DataTable Datos = new DataTable();
            try
            {
                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_ValidarMensaje");
                storeProcedure.AgregarParametro("@idmensaje", Com.idmensaje, Direccion.Input);
                storeProcedure.AgregarParametro("@validado", Com.validado, Direccion.Input);
                storeProcedure.AgregarParametro("@comision", Com.comision, Direccion.Input);
                storeProcedure.AgregarParametro("@usuario", Com.usuario, Direccion.Input);
                storeProcedure.AgregarParametro("@ImporteC", Com.ImporteC, Direccion.Input);
                storeProcedure.AgregarParametro("@Porcentaje", Com.Porcentaje, Direccion.Input);

                storeProcedure.EjecutarStoreProcedure(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_ValidarMensaje], Descripcion:" + storeProcedure.Error.Trim());
                }
                respuesta = true;

            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }

        public bool UpdateApobrarMensaje(ComisionesAutoRequest Com)
        {
            bool respuesta = false;
                       
            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("[AP_SP_AprobarMensaje]");
                storeProcedure.AgregarParametro("@idmensaje", Com.idmensaje, Direccion.Input);
                storeProcedure.AgregarParametro("@validado", Com.validado, Direccion.Input);
                storeProcedure.AgregarParametro("@usuario", Com.usuario, Direccion.Input);

                storeProcedure.EjecutarStoreProcedure(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_AprobarMensaje], Descripcion:" + storeProcedure.Error.Trim());
                }
               respuesta = true;


            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }
        public bool UpdateobservacionMensaje(ComisionesAutoRequest Com)
        {
            bool respuesta = false;
                        
            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("AP_SP_ObservarOperacion");
                storeProcedure.AgregarParametro("@idmensaje", Com.idmensaje, Direccion.Input);
                storeProcedure.AgregarParametro("@observacion", Com.observacion, Direccion.Input);

                storeProcedure.EjecutarStoreProcedure(strConexion_apom);
                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AP_SP_ObservarOperacion], Descripcion:" + storeProcedure.Error.Trim());
                }
                respuesta = true;


            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }
              
        public bool InsertUpdate_TransRecibidas(MensajeOriginalRequest mo)
        {
            bool respuesta = false;

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("[napom].[AM_APOM_TRANS_RECIBIDA]");

                storeProcedure.AgregarParametro("@ID_MENSAJE", mo.idmensaje, Direccion.Input);
	            storeProcedure.AgregarParametro("@REFERENCIA", mo.referencia, Direccion.Input);
	            storeProcedure.AgregarParametro("@REMITENTE", mo.remitente, Direccion.Input);
	            storeProcedure.AgregarParametro("@COD_BANCARIO", mo.CodOperacionBancarira, Direccion.Input);
	            storeProcedure.AgregarParametro("@FECHA_VALOR", mo.FechaValor, Direccion.Input);
	            storeProcedure.AgregarParametro("@CUENTA_ABONO", mo.ClienteBeneficiario1, Direccion.Input);
	            storeProcedure.AgregarParametro("@IMPORTE", mo.Importe, Direccion.Input);
	            storeProcedure.AgregarParametro("@MONEDA", mo.CodMoneda, Direccion.Input);
	            storeProcedure.AgregarParametro("@IMPORTE_INSTRUIDO", mo.ImporteInstruido, Direccion.Input);
	            storeProcedure.AgregarParametro("@MONEDA_INSTRUIDO", mo.MonInstruida, Direccion.Input);
	            storeProcedure.AgregarParametro("@TASA_CAMBIO", mo.TasaCambio, Direccion.Input);
	            storeProcedure.AgregarParametro("@FORMATO_CLIENTE", mo.FCClienteOrdenante, Direccion.Input);
	            storeProcedure.AgregarParametro("@DETALLE_CARGO", mo.DetallesCargo, Direccion.Input);
	            storeProcedure.AgregarParametro("@BANQUERO", mo.Banquero, Direccion.Input);
	            storeProcedure.AgregarParametro("@COD_GPI", mo.CodGPI, Direccion.Input);
	            storeProcedure.AgregarParametro("@COD_UETR", mo.CodUETR, Direccion.Input);
	            storeProcedure.AgregarParametro("@FORM_CORRESPONSAL_REMITENTE", mo.FCCorrersponsalRemitente, Direccion.Input);
                storeProcedure.AgregarParametro("@CORRESPONSAL_REMITENTE", mo.CorresponsalRemitente1.Trim() + " " + mo.CorresponsalRemitente2.Trim() + " " + mo.CorresponsalRemitente3.Trim() + " " + mo.CorresponsalRemitente4.Trim() + " " + mo.CorresponsalRemitente5.Trim(), Direccion.Input);
	            storeProcedure.AgregarParametro("@FORM_CORRESPONSAL_RECEPTOR", mo.FCCorresponsalReceptor, Direccion.Input);
                storeProcedure.AgregarParametro("@CORRESPONSAL_RECEPTOR", mo.CorresponsalReceptor1.Trim() + " " + mo.CorresponsalReceptor2.Trim() + " " + mo.CorresponsalReceptor3.Trim() + " " + mo.CorresponsalReceptor4.Trim() + " " + mo.CorresponsalReceptor5.Trim(), Direccion.Input);
	            storeProcedure.AgregarParametro("@FORM_INSTITUCION_REEMBOLSANTE", mo.FCTerceraInstitucionReembolsante, Direccion.Input);
                storeProcedure.AgregarParametro("@INSTITUCION_REEMBOLSANTE", mo.TerceraInstitucionReembolsante1.Trim() + " " + mo.TerceraInstitucionReembolsante2.Trim() + " " + mo.TerceraInstitucionReembolsante3.Trim() + " " + mo.TerceraInstitucionReembolsante4.Trim() + " " + mo.TerceraInstitucionReembolsante5.Trim(), Direccion.Input);
	            storeProcedure.AgregarParametro("@FORM_INSTITUCION_INTERBANCARIA", mo.FCInstitucionIntermediaria, Direccion.Input);
                storeProcedure.AgregarParametro("@INSTITUCION_INTERBANCARIA", mo.InstitucionIntermediaria1.Trim() + " " + mo.InstitucionIntermediaria2.Trim() + " " + mo.InstitucionIntermediaria3.Trim() + " " + mo.InstitucionIntermediaria4.Trim() + " " + mo.InstitucionIntermediaria5.Trim(), Direccion.Input);
	            storeProcedure.AgregarParametro("@FORM_CUENTA_INSTITUCION", mo.FCCuentaInstitucion, Direccion.Input);
                storeProcedure.AgregarParametro("@CUENTA_INSTITUCION", mo.CuentaInstitucion1.Trim() + " " + mo.CuentaInstitucion2.Trim() + " " + mo.CuentaInstitucion3.Trim() + " " + mo.CuentaInstitucion4.Trim() + " " + mo.CuentaInstitucion5.Trim(), Direccion.Input);
	            storeProcedure.AgregarParametro("@INFO_REMITENTE", mo.InformacionRemitente1+ " " + mo.InformacionRemitente2.Trim() + " " + mo.InformacionRemitente3.Trim() + " " + mo.InformacionRemitente4.Trim() , Direccion.Input);
                storeProcedure.AgregarParametro("@INFO_REMITENTE_RECEPTOR", mo.InfRemitAReceptor1.Trim() + " " + mo.InfRemitAReceptor2.Trim() + " " + mo.InfRemitAReceptor3.Trim() + " " + mo.InfRemitAReceptor4.Trim() + " " + mo.InfRemitAReceptor5.Trim(), Direccion.Input);
	            storeProcedure.AgregarParametro("@ESTADO", mo.validado, Direccion.Input);
	            storeProcedure.AgregarParametro("@TIPO_PAGO", mo.Tipo_Pago, Direccion.Input);
                storeProcedure.AgregarParametro("@OBSERVACION", mo.DObservacion, Direccion.Input);
                storeProcedure.AgregarParametro("@STATUSTTPRODUCCION", mo.status, Direccion.Input);
	            storeProcedure.AgregarParametro("@FECHA_RECEPCION", mo.fecharecepcion, Direccion.Input);
	            storeProcedure.AgregarParametro("@HORA_RECEPCION", mo.horarecepcion, Direccion.Input);
	            storeProcedure.AgregarParametro("@FECHA_EMISION", mo.fechaemision, Direccion.Input);
	            storeProcedure.AgregarParametro("@HORA_EMISION", mo.horaemision, Direccion.Input);
	            storeProcedure.AgregarParametro("@TIPO_MENSAJE", mo.tipomensaje, Direccion.Input);
                storeProcedure.AgregarParametro("@USUARIO", userservicio, Direccion.Input);
                                
                storeProcedure.EjecutarStoreProcedure(strConexion_apom);


                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [AM_APOM_TRANS_RECIBIDA], Descripcion:" + storeProcedure.Error.Trim());
                }

                respuesta = true;
            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }
        public bool InsertUpdate_TransRecibidasOrd(MensajeOriginalRequest mo)
        {
            bool respuesta = false;

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("[napom].[AM_APOM_TRANS_RECIBIDA_ORD]");
               storeProcedure.AgregarParametro("@ID_MENSAJE", mo.idmensaje, Direccion.Input);
               storeProcedure.AgregarParametro("@NOMBRE_ORD", mo.ClienteOrdenante2, Direccion.Input);
               storeProcedure.AgregarParametro("@DIRECCION_ORD", mo.ClienteOrdenante3.Trim()+" "+mo.ClienteOrdenante4.Trim()+" "+mo.ClienteOrdenante5.Trim(), Direccion.Input);
               storeProcedure.AgregarParametro("@CIUDAD_ORD", string.Empty, Direccion.Input);
               storeProcedure.AgregarParametro("@CUENTA_ORD", mo.ClienteOrdenante1, Direccion.Input);
               storeProcedure.AgregarParametro("@INSTITUCION_ORD", mo.InstitucionOrdenante2 + " " + mo.InstitucionOrdenante3 + " " + mo.InstitucionOrdenante4 + " " + mo.InstitucionOrdenante5, Direccion.Input);
               storeProcedure.AgregarParametro("@INSTITUCION_FORMATO_ORD", mo.FCInstitucionOrdenante, Direccion.Input);
               storeProcedure.AgregarParametro("@INSTITUCION_CUENTA_ORD", mo.InstitucionOrdenante1, Direccion.Input);
               storeProcedure.AgregarParametro("@USUARIO", userservicio, Direccion.Input);

                storeProcedure.EjecutarStoreProcedure(strConexion_apom);


                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [napom].[APOM_TRANS_RECIBIDA_ORD], Descripcion:" + storeProcedure.Error.Trim());
                }

                respuesta = true;
            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }
        public bool InsertUpdate_TransRecibidasBen(MensajeOriginalRequest mo)
        {
            bool respuesta = false;

            try
            {

                StoreProcedure storeProcedure = new StoreProcedure("[napom].[AM_APOM_TRANS_RECIBIDA_BEN]");
                               
                 storeProcedure.AgregarParametro("@ID_MENSAJE", mo.idmensaje, Direccion.Input);
                 storeProcedure.AgregarParametro("@NOMBRE_BEN", mo.ClienteBeneficiario2.Trim(), Direccion.Input);
                 storeProcedure.AgregarParametro("@DIRECCION_BEN", mo.ClienteBeneficiario3.Trim() + " " + mo.ClienteBeneficiario4.Trim() + " " + mo.ClienteBeneficiario5.Trim(), Direccion.Input);
                 storeProcedure.AgregarParametro("@CIUDAD_BEN", string.Empty, Direccion.Input);
                 storeProcedure.AgregarParametro("@CUENTA_BEN", mo.ClienteBeneficiario1, Direccion.Input);
                 storeProcedure.AgregarParametro("@FORMATO_BEN", mo.FCClienteBeneficiario, Direccion.Input);
                 storeProcedure.AgregarParametro("@CIC_BEN", string.Empty, Direccion.Input);
                 storeProcedure.AgregarParametro("@IDC_BEN", string.Empty, Direccion.Input);
                 storeProcedure.AgregarParametro("@EXT_IDC_BEN", string.Empty, Direccion.Input);
                 storeProcedure.AgregarParametro("@TIPO_IDC_BEN", string.Empty, Direccion.Input);
                 storeProcedure.AgregarParametro("@USUARIO", userservicio, Direccion.Input);

                storeProcedure.EjecutarStoreProcedure(strConexion_apom);


                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [napom].[AM_APOM_TRANS_RECIBIDA_BEN], Descripcion:" + storeProcedure.Error.Trim());
                }

                respuesta = true;
            }
            catch (Exception)
            {
                throw;
            }

            return respuesta;

        }
        public bool InsertUpdate_TransRecibidasTCCOM(ComisionesAutoRequest mo)
        {
            bool respuesta = false;

            try
            {
                DateTime Hoy = DateTime.Today;
                string fecha_actual = Hoy.ToString("yyyyMMdd");

                StoreProcedure storeProcedure = new StoreProcedure("[napom].[AM_APOM_TRANS_RECIBIDA_TC_COM]");
                
               storeProcedure.AgregarParametro("@ID_MENSAJE", mo.idmensaje, Direccion.Input);
               storeProcedure.AgregarParametro("@TICKET_COMISION", "-", Direccion.Input);
               storeProcedure.AgregarParametro("@PORCENTAJE_COMISION", mo.Porcentaje, Direccion.Input);
               storeProcedure.AgregarParametro("@COMISION", mo.comision, Direccion.Input);
               storeProcedure.AgregarParametro("@TICKET_TIPO_CAMBIO", "-", Direccion.Input);
               storeProcedure.AgregarParametro("@TIPO_CAMBIO", 0, Direccion.Input);
               storeProcedure.AgregarParametro("@IMPORTE_CLIENTE", mo.ImporteC, Direccion.Input);
               storeProcedure.AgregarParametro("@ESTADO_TICKE_COM", 0, Direccion.Input);
               storeProcedure.AgregarParametro("@ESTADO_TICKE_TC", 0, Direccion.Input);
               storeProcedure.AgregarParametro("@FECHA_APROBACION", fecha_actual, Direccion.Input);
               storeProcedure.AgregarParametro("@USER_APROBACION", userservicio, Direccion.Input);
               storeProcedure.AgregarParametro("@USER_REG", userservicio, Direccion.Input);

                storeProcedure.EjecutarStoreProcedure(strConexion_apom);


                if (storeProcedure.Error != String.Empty)
                {
                    throw new Exception("Procedimiento Almacenado: [napom].[AM_APOM_TRANS_RECIBIDA_TC_COM], Descripcion:" + storeProcedure.Error.Trim());
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
