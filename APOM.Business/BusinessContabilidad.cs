using APOM.DataAccess;
using APOM.Entities;
using APOM.Log;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace APOM.Business
{
    public class BusinessContabilidad
    {

        string entidad = "BCBOL";
        private string agencia = string.Empty;
        private string centro = "000";
        private string fld_user_1 = string.Empty;
        private string fld_user_2 = string.Empty;
        private string user_created = "MT103";
        private string user_last_process = "dbo";
        private string status = "M";
        private decimal cambioRepext;

        static DataAccesApom dbapom;
        static DataAccesRepext dbarepext;
        static DataAccesSmart dbasmart;
        private string correoerrores;
        string esquemacom = ConfigurationSettings.AppSettings["ESQUEMA_COMISION"];
        string esquemacomtran = ConfigurationSettings.AppSettings["ESQUEMA_COM_TRANS"];
        string esquemacomcarta = ConfigurationSettings.AppSettings["ESQUEMA_COM_CART_ORD"];
        string esquematrans = ConfigurationSettings.AppSettings["ESQUEMA_TRANSFERENCIA"];

        string esquemacomperu = ConfigurationSettings.AppSettings["ESQUEMA_COMISION_PERU"];

        string cuentacom = ConfigurationSettings.AppSettings["CUENTA_COMISION"];
        string cuentacomcarta = ConfigurationSettings.AppSettings["CUENTA_COM_CART_ORD"];
        string cuentacomtrans = ConfigurationSettings.AppSettings["CUENTA_COM_TRANS"];
        string cuentacomtransCripto = ConfigurationSettings.AppSettings["CUENTA_COM_CRIPTO"];

        public BusinessContabilidad()
        {
            dbapom = new DataAccesApom();
            dbarepext = new DataAccesRepext();
            dbasmart = new DataAccesSmart();
            correoerrores = dbapom.DTMonitoreoCorreos("SISTEMAS");
            cambioRepext = dbarepext.GetTIPOCAMBIOREPEXTa();
        }

        public void ProcesoContabilidad()
        {
            EsquemaContaRequest esqcomour = new EsquemaContaRequest();
            EsquemaContaRequest esqcomporte = new EsquemaContaRequest();
            EsquemaContaRequest esqcom1 = new EsquemaContaRequest();
            EsquemaContaRequest esqcom0 = new EsquemaContaRequest();
            EsquemaContaRequest esqcomourperu = new EsquemaContaRequest();
            EsquemaContaRequest esqcomtra= new EsquemaContaRequest();
            EsquemaContaRequest esqcomcarta = new EsquemaContaRequest();
            EsquemaContaRequest esqtran = new EsquemaContaRequest();
            DateTime fecha = DateTime.Now;           

            try {
                DataTable conta = dbapom.GetContabilidadPendinte(); //trae datos solo de cuentas criptos

                foreach (DataRow item in conta.Rows)
                {
                    ContabilidadOperRequest operacion = new ContabilidadOperRequest();
                    Diarios_Interfaz.BCB_diarios_interfacesDataTable asientos = new Diarios_Interfaz.BCB_diarios_interfacesDataTable();
                    asientos.Clear();
                    operacion = DatosOperacion(item);
                    string banca = ObtenerBanca(operacion.CUENTA_DEBITO);
                    string producto = dbarepext.GetProductoCuenta(operacion.CUENTA_DEBITO);
                    string sucursalorigen = operacion.CUENTA_DEBITO.Substring(0, 3);
                    string strIdDiario = "";
                    if (operacion.MONEDADESTINO == "USD")
                    {
                        esqtran = EsquemaContable(esquematrans, sucursalorigen, 0);
                        // PARTE CLAVE A MODIFICA
                        // 
                    }
                    else
                    {
                        esqtran = EsquemaContable(esquematrans, sucursalorigen, 1);
                    }

                    esqcomour = operacion.COMIS_OUR>0 ? EsquemaContable(esquemacom, sucursalorigen, 0):null; 
                    esqcomporte = operacion.COMIS_PORTE > 0 ? EsquemaContable(esquemacom, sucursalorigen, 0) : null;
                    esqcom1 = operacion.COMIS_1 > 0 ? EsquemaContable(esquemacom, sucursalorigen, 0) : null;
                    esqcom0 = operacion.COMIS_0 > 0 ? EsquemaContable(esquemacom, sucursalorigen, 0) : null;
                    esqcomourperu = operacion.DETALLE_CARGO == "OUR"? EsquemaContable(esquemacomperu, sucursalorigen, 0) : null;
                    //esqcomourperu = operacion.COMIS_OUR > 0 ? EsquemaContable(esquemacomperu, sucursalorigen, 0) : null;
                    esqcomtra = operacion.COMISION_TRANS > 0 ? EsquemaContable(esquemacomtran, sucursalorigen, 0) : null;
                    esqcomcarta = operacion.CARTA_ORDN_COMIS > 0 ? EsquemaContable(esquemacomcarta, sucursalorigen, 0) : null;

                    strIdDiario = Generar_ID_DIARIO(sucursalorigen, "SWIFT");
                    string strIdDiario201 = Generar_ID_DIARIO("201", "SWIFT");
                    int linea = dbasmart.lineaDiario(strIdDiario, sucursalorigen);
                    int linea201 = dbasmart.lineaDiario(strIdDiario201, "201");
                    if (esqtran != null)
                    {
                        aientosTrans(ref asientos,ref linea, ref linea201, esqtran, operacion, strIdDiario, sucursalorigen);
                    }

                    if (esqcomcarta != null)
                    {
                        aientosComisionCartaOrden(ref asientos, ref linea, ref linea201, esqcomcarta, operacion, strIdDiario, sucursalorigen, banca, producto);
                    }
                    if (esqcomtra != null)
                    {
                        aientosComision(ref asientos, ref linea, ref linea201, esqcomtra, operacion.COMISION_TRANS, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                    }

                    if (esqcomour != null)
                    {
                        aientosComision(ref asientos, ref linea, ref linea201, esqcomour, operacion.COMIS_OUR, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                    }
                    if (esqcomourperu != null)
                    {
                        //Requerimiento Comision Peru 
                        if (operacion.BANCADESTINO.ToString().Trim() == "BCPLPEPLXXX" || operacion.BANCADESTINO.ToString().Trim() == "BCPLPEPL")
                        {
                            if (operacion.MONEDADESTINO == "USD")
                            {
                                decimal importe_comis_our_peru = Convert.ToDecimal(ConfigurationSettings.AppSettings["COMIS_OUR_PERU"]);
                                aientosComision(ref asientos, ref linea, ref linea201, esqcomourperu, importe_comis_our_peru, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                            }
                        }
                    }
                    if (esqcomporte != null)
                    {
                        aientosComision(ref asientos, ref linea, ref linea201, esqcomporte, operacion.COMIS_PORTE, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                    }
                    if (esqcom1 != null)
                    {
                        aientosComision(ref asientos, ref linea, ref linea201, esqcom1, operacion.COMIS_1, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                    }
                    if (esqcom0 != null)
                    {
                        aientosComision(ref asientos, ref linea, ref linea201, esqcom0, operacion.COMIS_0, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                    }
                    if (dbasmart.InsertAsientosContables(asientos, correoerrores))
                    {
                        dbapom.UpdateApobrarConta(operacion.OPERACION, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Bitacora.Enviarmails5("Error general en el proceso de Contabilidad APOM ENVIADAS " + ex.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error general en el proceso de Contabilidad APOM ENVIADAS ", ex);
            }
        }


        public void ProcesoContabilidadVent()
        {
            EsquemaContaRequest esqcomourperu = new EsquemaContaRequest();

            DateTime fecha = DateTime.Now;

            try
            {
                DataTable conta = dbapom.GetContabilidadPendinteVent();

                foreach (DataRow item in conta.Rows)
                {
                    ContabilidadOperRequest operacion = new ContabilidadOperRequest();
                    Diarios_Interfaz.BCB_diarios_interfacesDataTable asientos = new Diarios_Interfaz.BCB_diarios_interfacesDataTable();
                    asientos.Clear();
                    operacion = DatosOperacion(item);
                    string banca = ""; 
                    string producto = "";
                    string sucursalorigen = "201";
                    string strIdDiario = "";

                    esqcomourperu = EsquemaContable(esquemacomperu, sucursalorigen, 0);

                    strIdDiario = Generar_ID_DIARIO(sucursalorigen, "SWIFT");
                    string strIdDiario201 = Generar_ID_DIARIO("201", "SWIFT");
                    int linea = dbasmart.lineaDiario(strIdDiario, sucursalorigen);
                    int linea201 = dbasmart.lineaDiario(strIdDiario201, "201");

                    if (esqcomourperu != null)
                    {
                        //Requerimiento Comision Peru 
                        if (operacion.BANCADESTINO.ToString().Trim() == "BCPLPEPLXXX" || operacion.BANCADESTINO.ToString().Trim() == "BCPLPEPL")
                        {
                            if (operacion.MONEDADESTINO == "USD")
                            {
                                decimal importe_comis_our_peru = Convert.ToDecimal(ConfigurationSettings.AppSettings["COMIS_OUR_PERU"]);
                                aientosComision(ref asientos, ref linea, ref linea201, esqcomourperu, importe_comis_our_peru, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                            }
                        }
                    }

                    if (dbasmart.InsertAsientosContables(asientos, correoerrores))
                    {
                        dbapom.UpdateApobrarContaVent(operacion.OPERACION, "CC");
                    }
                }
            }
            catch (Exception ex)
            {
                Bitacora.Enviarmails5("Error general en el proceso de Contabilidad WhatsApp Soli " + ex.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error general en el proceso de Contabilidad WhatsApp Soli ", ex);
            }
        }


        public EsquemaContaRequest EsquemaContable(string nombreEsquema, string Sucorigen, int especial)
        {
            EsquemaContaRequest response = new EsquemaContaRequest();
            try
            {
                DataTable Esquema = dbapom.GetEsquemaContable( nombreEsquema, Sucorigen, especial);
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
                Bitacora.Enviarmails5("Error General al momento obtener el esquema contable: "+ nombreEsquema+" " + ex.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error General al momento obtener el esuquema contable: " + nombreEsquema, ex);

            }

            return response;

        }


        public ContabilidadOperRequest DatosOperacion(DataRow operacion)
        {
            ContabilidadOperRequest response = new ContabilidadOperRequest();
            try
            {
                DataRow item = operacion;
                
                    response.OPERACION = item.Field<string>("OPERACION");
                    response.NRO_REFER_TRANS = item.Field<string>("NRO_REFER_TRANS");
                    response.MONEDAORIGEN = item.Field<string>("MONEDAORIGEN");
                    response.MONTOORIGEN = item.Field<decimal>("MONTOORIGEN");
                    response.GLOSAORIGEN = item.Field<string>("GLOSAORIGEN");
                    response.MONEDADESTINO = item.Field<string>("MONEDADESTINO");
                    response.MONTODESTINO = item.Field<decimal>("MONTODESTINO");
                    response.BANCADESTINO = item.Field<string>("BANCADESTINO");
                    response.CUENTADESTINO = item.Field<string>("CUENTADESTINO");
                    response.MONTOINTERMEDIO_USD = item.Field<decimal>("MONTOINTERMEDIO_USD");
                    response.COMISION_TRANS = item.Field<decimal>("COMISION_TRANS");
                    response.MONEDA_COMIS = item.Field<string>("MONEDA_COMIS");
                    response.CARTA_ORDN_COMIS = item.Field<decimal>("CARTA_ORDN_COMIS");
                    response.COMIS_PORTE = item.Field<decimal>("COMIS_PORTE");
                    response.COMIS_OUR = item.Field<decimal>("COMIS_OUR");
                    response.COMIS_0 = item.Field<decimal>("COMIS_0");
                    response.COMIS_1 = item.Field<decimal>("COMIS_1");
                    response.IMPORTE_ITF = item.Field<decimal>("IMPORTE_ITF");
                    response.MONEDA_ITF = item.Field<string>("MONEDA_ITF");
                    response.CUENTA_DEBITO = item.Field<string>("CUENTA_DEBITO");
                    response.ITF_COMISION = item.Field<decimal>("ITF_COMISION");
                    response.DETALLE_CARGO = item.Field<string>("DETALLE_CARGO");
            }
            catch (Exception ex)
            {
                Bitacora.Enviarmails5("Error General al momento obtener los datos de Operacion contable " + ex.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error General al momento obtener el esuquema contable " , ex);
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
                Bitacora.Enviarmails5("Error General al momento obtener los datos de la Banca "+ cuenta + ex.Message.ToString(), correoerrores);
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

        public Diarios_Interfaz.BCB_diarios_interfacesDataTable aientosTrans(ref Diarios_Interfaz.BCB_diarios_interfacesDataTable asientos,ref int linea, ref int linea201, EsquemaContaRequest esquema, ContabilidadOperRequest oper, string strIdDiario, string sucursal)
        {
            string idduario201=  Generar_ID_DIARIO("201", "SWIFT");          

            foreach (ContabilidadRequest item in esquema.esquemarows)
            {
                Diarios_Interfaz.BCB_diarios_interfacesRow Fila=null;
                string iddiariofinal = strIdDiario;
                Fila = asientos.NewBCB_diarios_interfacesRow();
                string glosa = oper.GLOSAORIGEN.Trim();
                
                if (item.GLOSA.Trim() != "")
                {
                    glosa= item.GLOSA.Trim()+" " + oper.GLOSAORIGEN.Trim();
                }

                int lineafin = linea;
                if (item.SUCURSAL =="201")
                {
                    lineafin = linea201;
                    iddiariofinal = idduario201;
                }
                string monedadestinofinal = item.MONEDA;

                if (item.MONEDA == "XEU")
                {
                    if (oper.MONEDADESTINO == "USD" || oper.MONEDADESTINO == "EUR")
                    {
                        monedadestinofinal = item.MONEDA;
                    }
                    else
                    {
                        monedadestinofinal = oper.MONEDADESTINO;
                    }
                }

                Fila.entidad = entidad;
                Fila.fecha = DateTime.Now;
                Fila.id_diario = iddiariofinal;
                Fila.line = lineafin;
                Fila.sucursal = item.SUCURSAL == "XXX" ? sucursal: item.SUCURSAL;
                Fila.cuenta = item.CUENTA.Trim() == "XXX" ? oper.CUENTADESTINO.Trim() : item.CUENTA.Trim();
                Fila.agencia = agencia;
                Fila.centro = centro;
                Fila.banca = item.BANCA;
                Fila.producto = item.PRODUCTO;
                Fila.moneda = monedadestinofinal;
                Fila.deb_cred = item.DEB_CRED;
                Fila.importe = item.MONEDA == "USD"? oper.MONTOORIGEN*item.IMPORTE:oper.MONTODESTINO * item.IMPORTE;
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
        
        public Diarios_Interfaz.BCB_diarios_interfacesDataTable aientosComision(ref Diarios_Interfaz.BCB_diarios_interfacesDataTable asientos, ref int linea, ref int linea201, EsquemaContaRequest esquema, decimal importe,string glosa, string strIdDiario, string sucursal,string sbanca,string sproducto)
        {
            string idduario201 =  Generar_ID_DIARIO("201", "SWIFT");

            foreach (ContabilidadRequest item in esquema.esquemarows)
            {
                Diarios_Interfaz.BCB_diarios_interfacesRow Fila = null;
                string iddiariofinal = strIdDiario;
                Fila = asientos.NewBCB_diarios_interfacesRow();

                string banca = item.BANCA;
                string producto = item.PRODUCTO;
                if (item.CUENTA == cuentacom || item.CUENTA == cuentacomtrans)
                {
                    banca = sbanca;
                    producto = sproducto;
                }
                int lineafin = linea;
                if (item.SUCURSAL == "201")
                {
                    lineafin = linea201;
                    iddiariofinal = idduario201;
                }

                Fila.entidad = entidad;
                Fila.fecha = DateTime.Now;
                Fila.id_diario = iddiariofinal;
                Fila.line = lineafin;
                Fila.sucursal = item.SUCURSAL == "XXX" ? sucursal : item.SUCURSAL;
                Fila.cuenta = item.CUENTA;
                Fila.agencia = agencia;
                Fila.centro = centro;
                Fila.banca = banca;
                Fila.producto = producto;
                Fila.moneda = item.MONEDA;
                Fila.deb_cred = item.DEB_CRED;
                Fila.importe = importe * item.IMPORTE;
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

        public Diarios_Interfaz.BCB_diarios_interfacesDataTable aientosComisionCartaOrden(ref Diarios_Interfaz.BCB_diarios_interfacesDataTable asientos, ref int linea, ref int linea201, EsquemaContaRequest esquema, ContabilidadOperRequest oper, string strIdDiario, string sucursal, string sbanca, string sproducto)
        {

            string idduario201 =  Generar_ID_DIARIO("201", "SWIFT");
            
            InformacionTipoCambio tc = new InformacionTipoCambio();
            tc = dbasmart.TraerTipoCambio();
            decimal importebob = decimal.Round(Math.Ceiling((oper.CARTA_ORDN_COMIS * tc.TasaVenta) * 100) / 100 );

            decimal importeajuste = importebob / cambioRepext;
            importeajuste = Redondeo_dos_decimales(importeajuste.ToString());
            decimal Monto_Calculado_Ajuste = importeajuste- oper.CARTA_ORDN_COMIS;
            Monto_Calculado_Ajuste = Redondeo_dos_decimales(Monto_Calculado_Ajuste.ToString());
            
            foreach (ContabilidadRequest item in esquema.esquemarows)
            {
                Diarios_Interfaz.BCB_diarios_interfacesRow Fila = null;
                string iddiariofinal = strIdDiario;
                Fila = asientos.NewBCB_diarios_interfacesRow();

                string glosa = oper.GLOSAORIGEN.Trim();
                string banca = item.BANCA;
                string producto = item.PRODUCTO;
                decimal importefin = oper.CARTA_ORDN_COMIS;
                
                if (item.GLOSA.Trim() != "")
                {
                    glosa = item.GLOSA.Trim() + " " + oper.GLOSAORIGEN.Trim();
                }
                if (item.CUENTA == cuentacomcarta)
                {
                    banca = sbanca;
                    producto = sproducto;
                }

                if (item.GLOSA.Trim() == "AJUSTE")
                {
                    importefin = Monto_Calculado_Ajuste;
                }

                if (item.MONEDA.Trim() == "BOB")
                {
                    importefin = importebob;
                }

                int lineafin = linea;
                if (item.SUCURSAL == "201")
                {
                    lineafin = linea201;
                    iddiariofinal = idduario201;
                }

                Fila.entidad = entidad;
                Fila.fecha = DateTime.Now;
                Fila.id_diario = iddiariofinal;
                Fila.line = lineafin;
                Fila.sucursal = item.SUCURSAL == "XXX" ? sucursal : item.SUCURSAL;
                Fila.cuenta = item.CUENTA;
                Fila.agencia = agencia;
                Fila.centro = centro;
                Fila.banca = banca;
                Fila.producto = producto;
                Fila.moneda = item.MONEDA;
                Fila.deb_cred = item.DEB_CRED;
                Fila.importe = importefin * item.IMPORTE;
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




        #region SECTION.00: PROCESO PRINCIPAL CONTABILIDAD APOM CRIPTOS

        public void ProcesoContabilidadCripto()
        {
            EsquemaContaRequest esqcomour = new EsquemaContaRequest();
            EsquemaContaRequest esqcomporte = new EsquemaContaRequest();
            EsquemaContaRequest esqcom1 = new EsquemaContaRequest();
            EsquemaContaRequest esqcom0 = new EsquemaContaRequest();
            EsquemaContaRequest esqcomourperu = new EsquemaContaRequest();
            EsquemaContaRequest esqcomtra = new EsquemaContaRequest();
            EsquemaContaRequest esqcomcarta = new EsquemaContaRequest();
            EsquemaContaRequest esqtran = new EsquemaContaRequest();
            DateTime fecha = DateTime.Now;

            try
            {
                DataTable conta = dbapom.GetContabilidadPendinteCripto();

                foreach (DataRow item in conta.Rows)
                {
                    ContabilidadOperRequest operacion = new ContabilidadOperRequest();
                    Diarios_Interfaz.BCB_diarios_interfacesDataTable asientos = new Diarios_Interfaz.BCB_diarios_interfacesDataTable();
                    asientos.Clear();
                    operacion = DatosOperacion(item); // Agrupra 
                    string banca = ObtenerBanca(operacion.CUENTA_DEBITO); // obtiene bank RepExt
                    string producto = dbarepext.GetProductoCuenta(operacion.CUENTA_DEBITO); // obtiene producto RepExt
                    string sucursalorigen = operacion.CUENTA_DEBITO.Substring(0, 3); // Sucursal origen para criptos es -> 901
                    string strIdDiario = "";

                    if (operacion.MONEDADESTINO == "USD")
                    {
                        esqtran = EsquemaContableCriptos(esquematrans, sucursalorigen, 0);// string nombreEsquema, string Sucorigen, int especial
                    }
                    else
                    {
                        esqtran = EsquemaContableCriptos(esquematrans, sucursalorigen, 1);
                    }

                    esqcomour = operacion.COMIS_OUR > 0 ? EsquemaContableCriptos(esquemacom, sucursalorigen, 0) : null;
                    esqcomporte = operacion.COMIS_PORTE > 0 ? EsquemaContableCriptos(esquemacom, sucursalorigen, 0) : null;
                    esqcom1 = operacion.COMIS_1 > 0 ? EsquemaContableCriptos(esquemacom, sucursalorigen, 0) : null;
                    esqcom0 = operacion.COMIS_0 > 0 ? EsquemaContableCriptos(esquemacom, sucursalorigen, 0) : null;
                    esqcomourperu = operacion.DETALLE_CARGO == "OUR" ? EsquemaContableCriptos(esquemacomperu, sucursalorigen, 0) : null;
                    //esqcomourperu = operacion.COMIS_OUR > 0 ? EsquemaContableCriptos(esquemacomperu, sucursalorigen, 0) : null;
                    esqcomtra = operacion.COMISION_TRANS > 0 ? EsquemaContableCriptos(esquemacomtran, sucursalorigen, 0) : null;
                    
                    //AJUSTE ESQUEMA CONTABLE
                    esqcomtra = operacion.COMISION_TRANS > 0 ? EsquemaContableCriptos(esquemacomtran, sucursalorigen, 0) : null;

                    esqcomcarta = operacion.CARTA_ORDN_COMIS > 0 ? EsquemaContableCriptos(esquemacomcarta, sucursalorigen, 0) : null;

                    strIdDiario = Generar_ID_DIARIO(sucursalorigen, "SWIFT");
                    string strIdDiario201 = Generar_ID_DIARIO("201", "SWIFT"); // cambiar a 901
                    int linea = dbasmart.lineaDiario(strIdDiario, sucursalorigen); //detalle cuenta
                    int linea201 = dbasmart.lineaDiario(strIdDiario201, "201"); // detalle cuenta sucursal 201
                    if (esqtran != null)
                    {
                        aientosTrans(ref asientos, ref linea, ref linea201, esqtran, operacion, strIdDiario, sucursalorigen);
                    }

                    if (esqcomcarta != null)
                    {
                        aientosComisionCartaOrden(ref asientos, ref linea, ref linea201, esqcomcarta, operacion, strIdDiario, sucursalorigen, banca, producto);
                    }
                    if (esqcomtra != null)
                    {
                        aientosComision(ref asientos, ref linea, ref linea201, esqcomtra, operacion.COMISION_TRANS, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                    }

                    if (esqcomour != null)
                    {
                        aientosComision(ref asientos, ref linea, ref linea201, esqcomour, operacion.COMIS_OUR, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                    }
                    if (esqcomourperu != null)
                    {
                        //Requerimiento Comision Peru 
                        if (operacion.BANCADESTINO.ToString().Trim() == "BCPLPEPLXXX" || operacion.BANCADESTINO.ToString().Trim() == "BCPLPEPL")
                        {
                            if (operacion.MONEDADESTINO == "USD")
                            {
                                decimal importe_comis_our_peru = Convert.ToDecimal(ConfigurationSettings.AppSettings["COMIS_OUR_PERU"]);
                                aientosComision(ref asientos, ref linea, ref linea201, esqcomourperu, importe_comis_our_peru, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                            }
                        }
                    }
                    if (esqcomporte != null)
                    {
                        aientosComision(ref asientos, ref linea, ref linea201, esqcomporte, operacion.COMIS_PORTE, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                    }
                    if (esqcom1 != null)
                    {
                        aientosComision(ref asientos, ref linea, ref linea201, esqcom1, operacion.COMIS_1, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                    }
                    if (esqcom0 != null)
                    {
                        aientosComision(ref asientos, ref linea, ref linea201, esqcom0, operacion.COMIS_0, operacion.GLOSAORIGEN, strIdDiario, sucursalorigen, banca, producto);
                    }
                    if (dbasmart.InsertAsientosContables(asientos, correoerrores))
                    {
                        dbapom.UpdateApobrarConta(operacion.OPERACION, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Bitacora.Enviarmails5("Error general en el proceso de Contabilidad APOM ENVIADAS CRIPTOMONEDAS" + ex.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error general en el proceso de Contabilidad APOM ENVIADAS ", ex);
            }
        }

        public Diarios_Interfaz.BCB_diarios_interfacesDataTable aientosComisionCripto(ref Diarios_Interfaz.BCB_diarios_interfacesDataTable asientos, ref int linea, ref int linea201, EsquemaContaRequest esquema, decimal importe, string glosa, string strIdDiario, string sucursal, string sbanca, string sproducto)
        {
            string idduario201 = Generar_ID_DIARIO("201", "SWIFT");

            foreach (ContabilidadRequest item in esquema.esquemarows)
            {
                Diarios_Interfaz.BCB_diarios_interfacesRow Fila = null;
                string iddiariofinal = strIdDiario;
                Fila = asientos.NewBCB_diarios_interfacesRow();

                string banca = item.BANCA;
                string producto = item.PRODUCTO;
                if (item.CUENTA == cuentacom || item.CUENTA == cuentacomtransCripto)
                {
                    banca = sbanca;
                    producto = sproducto;
                }
                int lineafin = linea;
                if (item.SUCURSAL == "201")
                {
                    lineafin = linea201;
                    iddiariofinal = idduario201;
                }

                Fila.entidad = entidad;
                Fila.fecha = DateTime.Now;
                Fila.id_diario = iddiariofinal;
                Fila.line = lineafin;
                Fila.sucursal = item.SUCURSAL == "XXX" ? sucursal : item.SUCURSAL;
                Fila.cuenta = item.CUENTA;
                Fila.agencia = agencia;
                Fila.centro = centro;
                Fila.banca = banca;
                Fila.producto = producto;
                Fila.moneda = item.MONEDA;
                Fila.deb_cred = item.DEB_CRED;
                Fila.importe = importe * item.IMPORTE;
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

        public EsquemaContaRequest EsquemaContableCriptos(string nombreEsquema, string Sucorigen, int especial)
        {
            EsquemaContaRequest response = new EsquemaContaRequest();
            try
            {
                DataTable Esquema = dbapom.GetEsquemaContableCriptos(nombreEsquema, Sucorigen, especial);
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

        #endregion



    }
}
