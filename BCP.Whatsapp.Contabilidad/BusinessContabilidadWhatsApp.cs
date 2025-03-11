using APOM.DataAccess;
using APOM.Entities;
using APOM.Log;
using BCP.Whatsapp.DataAccessConta;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCP.Whatsapp.Contabilidad
{
    public class BusinessContabilidadWhatsApp
    {
        string entidad = "BCBOL";
        private string agencia = string.Empty;
        private string centro = "000";
        private string fld_user_1 = string.Empty;
        private string fld_user_2 = string.Empty;
        private string user_created = "SOLWA";
        private string user_last_process = "SOLWA";
        private string status = "M";
        private decimal cambioRepext;

        static DataAccesApom dbapom;
        static DataAccesRepext dbarepext;
        static DataAccesSmart dbasmart;
        static DataAccessWp dbwhastapp;

        private string correoerrores;

        public BusinessContabilidadWhatsApp()
        {
            dbapom = new DataAccesApom();
            dbarepext = new DataAccesRepext();
            dbasmart = new DataAccesSmart();
            correoerrores = dbapom.DTMonitoreoCorreos("SISTEMAS");
            cambioRepext = dbarepext.GetTIPOCAMBIOREPEXTa();
            dbwhastapp = new DataAccessWp();
        }


        #region PROCESO CONTABILIDAD WHATSAPP 

        public void ProcesoContabilidadWhastApp()
        {
            EsquemaContaRequest esquemasoliwhastapp = new EsquemaContaRequest();

            DateTime fecha = DateTime.Now;
            
            try
            {
                DataTable conta = dbwhastapp.GetContabilidadPendinteWhatsApp();

                foreach (DataRow item in conta.Rows)
                {
                    ContabilidadGiftRequest operacion = new ContabilidadGiftRequest();
                    Diarios_Interfaz.BCB_diarios_interfacesDataTable asientos = new Diarios_Interfaz.BCB_diarios_interfacesDataTable();
                    asientos.Clear();
                    operacion = DatosOperacion(item);
                    string banca = ObtenerBanca(operacion.NRO_CUENTA);
                    string producto = dbarepext.GetProductoCuenta(operacion.NRO_CUENTA);
                    string sucursalorigen = operacion.NRO_CUENTA.Substring(0, 3);
                    string strIdDiario = "";

                    esquemasoliwhastapp = EsquemaContable("SOLIWA", sucursalorigen, 0);
                    
                    strIdDiario = Generar_ID_DIARIO(sucursalorigen, "SOLWA");
                    string strIdDiario201 = Generar_ID_DIARIO("201", "SOLWA");
                    int linea = dbasmart.lineaDiario(strIdDiario, sucursalorigen);
                    int linea201 = dbasmart.lineaDiario(strIdDiario201, "201");

                    aientosTrans(ref asientos, ref linea, ref linea201, esquemasoliwhastapp, operacion, strIdDiario, sucursalorigen);
                    
                    if (dbasmart.InsertAsientosContables(asientos, correoerrores))
                    {
                        dbwhastapp.UpdateApobrarContaWhatsApp(operacion, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Bitacora.Enviarmails5("Error general en el proceso de Contabilidad WhatsApp Soli " + ex.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error general en el proceso de Contabilidad WhatsApp Soli ", ex);
            }
        }

        #endregion 


        public EsquemaContaRequest EsquemaContable(string nombreEsquema, string Sucorigen, int especial)
        {
            EsquemaContaRequest response = new EsquemaContaRequest();
            try
            {
                DataTable Esquema = dbapom.GetEsquemaContable(nombreEsquema, Sucorigen, especial);
                response.esquemarows = new List<ContabilidadRequest>();
                foreach (DataRow item in Esquema.Rows)
                {
                    ContabilidadRequest con = new ContabilidadRequest();

                    con.NOMBRE_ESQUEMA = item.Field<string>("NOMBRE_ESQUEMA");
                    con.SUCURSAL = item.Field<string>("SUCURSAL");
                    con.CUENTA = item.Field<string>("CUENTA");
                    con.BANCA = item.Field<string>("BANCA");
                    con.PRODUCTO = item.Field<string>("PRODUCTO");
                    con.MONEDA = item.Field<string>("MONEDA");
                    con.DEB_CRED = item.Field<string>("DEB_CRED");
                    con.SUCURSAL_ORIGEN = item.Field<string>("SUCURSAL_ORIGEN");
                    con.IMPORTE = item.Field<int>("IMPORTE");
                    con.CUENTA_ESPECIAL = item.Field<int>("CUENTA_ESPECIAL");
                    con.GLOSA = item.Field<string>("GLOSA");

                    response.esquemarows.Add(con);
                }
            }
            catch (Exception ex)
            {
                Bitacora.Enviarmails5("Error General al momento obtener el esquema contable: " + nombreEsquema + " " + ex.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error General al momento obtener el esuquema contable: " + nombreEsquema, ex);
            }

            return response;

        }


        public ContabilidadGiftRequest DatosOperacion(DataRow operacion)
        {
            ContabilidadGiftRequest response = new ContabilidadGiftRequest();
            try
            {

                DataRow item = operacion;

                response.SERVICIO = item.Field<string>("SERVICIO");
                response.PRECIO = item.Field<decimal>("PRECIO");
                response.DEBITO_BOL = item.Field<decimal>("DEBITO_BOL");
                response.CODIGO = item.Field<string>("CODIGO");
                response.ESTADO = item.Field<string>("ESTADO");
                response.CIC = item.Field<string>("CIC");
                response.ID_SERVICIO = item.Field<string>("ID_SERVICIO");
                response.CANAL = item.Field<string>("CANAL");
                response.CELULAR = item.Field<string>("CELULAR");
                response.CORREO_ELECTRONICO = item.Field<string>("CORREO_ELECTRONICO");
                response.NRO_CUENTA = item.Field<string>("NRO_CUENTA");
                response.ESTADO_CONTA = item.Field<string>("ESTADO_CONTA");
                                

            }
            catch (Exception ex)
            {
                Bitacora.Enviarmails5("Error General al momento obtener los datos de Operacion contable " + ex.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error General al momento obtener el esuquema contable ", ex);

            }
            return response;


        }

        public string ObtenerBanca(string cuenta)
        {
            string Banca = "";
            try
            {
                string codbanca = dbarepext.GetBancaCuenta(cuenta);
                Banca = dbarepext.GetBancaDatoCuenta(codbanca);

            }
            catch (Exception ex)
            {
                Bitacora.Enviarmails5("Error General al momento obtener los datos de la Banca " + cuenta + ex.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error General al momento obtener los datos de la Banca " + cuenta, ex);

            }
            return Banca;

        }

        public string Generar_ID_DIARIO(string sucursal, string DetalleID)
        {
            int MES = 0;
            string mes_texto = string.Empty;
            MES = DateTime.Now.Month;
            if (MES < 10)
                mes_texto = "0" + MES.ToString();
            else
                mes_texto = MES.ToString();
            return mes_texto + sucursal + DetalleID;
        }

        public Diarios_Interfaz.BCB_diarios_interfacesDataTable aientosTrans(ref Diarios_Interfaz.BCB_diarios_interfacesDataTable asientos, ref int linea, ref int linea201, EsquemaContaRequest esquema, ContabilidadGiftRequest oper, string strIdDiario, string sucursal)
        {

            string idduario201 = Generar_ID_DIARIO("201", "SOLWA");


            foreach (ContabilidadRequest item in esquema.esquemarows)
            {
                Diarios_Interfaz.BCB_diarios_interfacesRow Fila = null;
                string iddiariofinal = strIdDiario;
                Fila = asientos.NewBCB_diarios_interfacesRow();
                
                string glosa = String.IsNullOrEmpty(item.GLOSA) ? oper.SERVICIO + " " + oper.CODIGO.Trim():item.GLOSA;
                
                if (item.GLOSA == "GLOSA")
                {
                    glosa = "AHO Transferencia Deposito a Cuenta Pasiva ME 201";
                }
                
                    int lineafin = linea;
                if (item.SUCURSAL == "201")
                {
                    lineafin = linea201;
                    iddiariofinal = idduario201;


                }
                string monedadestinofinal = item.MONEDA;

                               



                Fila.entidad = entidad;
                Fila.fecha = DateTime.Now;
                Fila.id_diario = iddiariofinal;
                Fila.line = lineafin;
                Fila.sucursal = item.SUCURSAL == "XXX" ? sucursal : item.SUCURSAL;
                Fila.cuenta = item.CUENTA.Trim() == "XXX" ? oper.NRO_CUENTA.Trim() : item.CUENTA.Trim();
                Fila.agencia = agencia;
                Fila.centro = centro;
                Fila.banca = item.BANCA;
                Fila.producto = item.PRODUCTO;
                Fila.moneda = monedadestinofinal;
                Fila.deb_cred = item.DEB_CRED;
                Fila.importe = item.MONEDA == "USD" ?oper.PRECIO* item.IMPORTE :oper.DEBITO_BOL* item.IMPORTE;
                Fila.glosa = glosa;
                Fila.fld_user_1 = fld_user_1;
                Fila.fld_user_2 = fld_user_2;
                Fila.date_created = DateTime.Now;
                Fila.date_last_process = DateTime.Now;
                Fila.user_created = user_created;
                Fila.user_last_process = user_last_process;
                Fila.status = status;
                asientos.AddBCB_diarios_interfacesRow(Fila);

                if (item.SUCURSAL == "201")
                {
                    linea201++;

                }
                else
                {
                    linea++;

                }


            }



            return asientos;

        }

       
     
        private decimal Redondeo_dos_decimales(string p)
        {

            decimal resultado = 0;
            resultado = Convert.ToDecimal(p.ToString().Trim());
            resultado = Math.Round(resultado, 2, MidpointRounding.AwayFromZero);
            return resultado;

        }






    }


   


}
