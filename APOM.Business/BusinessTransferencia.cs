using APOM.BD;
using APOM.Common;
using APOM.DataAccess;
using APOM.Entities;
using APOM.Log;
using APOM.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Globalization;
namespace APOM.Business
{
    public class BusinessTransferencia
    {

        static DataAccesApom dbapom = new DataAccesApom();
        private static string correoerrores = dbapom.DTMonitoreoCorreos("SISTEMAS");
        private static string SmarttcCompra;
        private static string SmarttcVenta;
        public BusinessTransferencia(string tcCompra, string tcVenta)
        {
            SmarttcCompra = tcCompra;
            SmarttcVenta = tcVenta;
        }
               
       


        public DATOS_TRANSFERENCIA Leer_Datos_Trans(int TipoCuenta)
        {
            string area_operativa = string.Empty;
            decimal Monto_Original_Moneda_Extrajera = 0;
            decimal Comision_cobrada_bob = 0;
            decimal Tipo_Cambio_XEU_USD = 0;
            decimal comision_cobrada = 0;
            decimal Tipo_Cambio_USD_BOB = 0;
            decimal MONTO_A_PAGAR = 0;
            decimal COMISION_REMESADORA = 0;
            decimal IMPORTE_ORIGINAL_USD = 0;
            DATOS_TRANSFERENCIA DatosTranFin = new DATOS_TRANSFERENCIA();

            try
            {
                DATOS_TRANSFERENCIA DatosTran = new DATOS_TRANSFERENCIA();

                DatosTran = dbapom.Datos_Tran(TipoCuenta);
                BusinessITF CalculoITF = new BusinessITF();
                ComisionRequest com = new ComisionRequest();

                bool validado = true;
                //Calculo de comision y tipos de cambio
                DatosTranFin.tranrows = new List<ComisionRequest>();
                foreach (ComisionRequest item in DatosTran.tranrows)
                {
                    validado = true;
                    item.MONEDA_ORIGINAL = item.CodigoDeMoneda;
                    if (item.CodigoDeMoneda.ToString() == "USD")//Si la transferencia llego en dolares
                    {
                        MONTO_A_PAGAR = Conseguir_Importe_A_Pagar_USD(item, ref comision_cobrada, ref COMISION_REMESADORA);
                        if (MONTO_A_PAGAR >= 0)
                        {
                            //El monto menos el cobro de ITF por la comision
                            item.MONTO_A_PAGAR = MONTO_A_PAGAR - CalculoITF.Calcular_ITF(comision_cobrada);
                            item.MONEDA_A_PAGAR = "USD";
                            item.COMISION = comision_cobrada;
                            item.COMISION_MONEDA = "USD";
                            //Se guarda cuanto se cobro por la comision
                            item.ITF_COMISION = CalculoITF.Calcular_ITF(comision_cobrada);
                            item.ITF_COMISION_MONEDA = "USD";
                            item.COMISION_REMESADORA = COMISION_REMESADORA;
                            item.COMISION_MONEDA_REMESADORA = "USD";
                            //Si la cuenta de abono fuera en bolivianos
                            if (Convert.ToInt32(item.ClienteBeneficiarioUnoDeCinco.ToString().Substring((item.ClienteBeneficiarioUnoDeCinco.ToString().Trim().Length - 3), 1)) == 3)
                            {
                                MONTO_A_PAGAR = Conseguir_Importe_A_Pagar_BOB(item, comision_cobrada, ref Comision_cobrada_bob, ref Tipo_Cambio_USD_BOB, MONTO_A_PAGAR, COMISION_REMESADORA, ref IMPORTE_ORIGINAL_USD);
                                if (MONTO_A_PAGAR >= 0)
                                {
                                    //El monto menos el cobro de ITF por la comision
                                    item.MONTO_ORIGINAL_CONVERSION_USD_BOB = IMPORTE_ORIGINAL_USD;
                                    item.MONTO_A_PAGAR = MONTO_A_PAGAR - CalculoITF.Calcular_ITF(Comision_cobrada_bob);
                                    item.MONEDA_A_PAGAR = "BOB";
                                    item.TIPO_CAMBIO_USD_BOB = Tipo_Cambio_USD_BOB;
                                    item.CodigoDeMoneda = "BOB";
                                    item.COMISION_CONVERSION_USD_BOB = Comision_cobrada_bob;
                                    item.COMISION_MONEDA_CONVERSION = "BOB";
                                    item.APOM_ITF_COMISION_BS = CalculoITF.Calcular_ITF(Comision_cobrada_bob);
                                }
                                else
                                {
                                    validado = false;
                                }
                            }
                        }
                        else
                        {
                            validado = false;
                        }

                    }
                    else
                    {
                        //Si la transferencia llego en otras monedas
                        MONTO_A_PAGAR = Conseguir_Importe_A_Pagar_EUR(item, ref Monto_Original_Moneda_Extrajera, ref Tipo_Cambio_XEU_USD, ref comision_cobrada);
                        if (MONTO_A_PAGAR >= 0)
                        {
                            //El monto menos el cobro de ITF por la comision
                            item.MONTO_A_PAGAR = MONTO_A_PAGAR - CalculoITF.Calcular_ITF(comision_cobrada);
                            item.MONTO_ORIGINAL_MONEDA_EXTRANJERA = Convert.ToDecimal(item.Importe);
                            item.MONTO_ORIGINAL_CONVERSION_USD_BOB = Monto_Original_Moneda_Extrajera;
                            item.TIPO_CAMBIO = Tipo_Cambio_XEU_USD;
                            item.MONEDA_A_PAGAR = "USD";
                            item.COMISION = comision_cobrada;
                            item.COMISION_MONEDA = "USD";
                            item.CodigoDeMoneda = "USD".Trim();
                            //Se guarda cuanto se cobro por la comision
                            item.ITF_COMISION = CalculoITF.Calcular_ITF(comision_cobrada);
                            item.ITF_COMISION_MONEDA = "USD";
                            //Si la cuenta de abono es en bolivianos
                            if (Convert.ToInt32(item.ClienteBeneficiarioUnoDeCinco.ToString().Substring((item.ClienteBeneficiarioUnoDeCinco.ToString().Trim().Length - 3), 1)) == 3)
                            {
                                MONTO_A_PAGAR = Conseguir_Importe_A_Pagar_BOB_EUR(item, comision_cobrada, ref Comision_cobrada_bob, ref Tipo_Cambio_USD_BOB, Tipo_Cambio_XEU_USD);
                                if (MONTO_A_PAGAR >= 0)
                                {
                                    //El monto menos el cobro de ITF por la comision
                                    item.MONTO_A_PAGAR = MONTO_A_PAGAR - CalculoITF.Calcular_ITF(Comision_cobrada_bob);
                                    item.MONEDA_A_PAGAR = "BOB";
                                    item.TIPO_CAMBIO_USD_BOB = Tipo_Cambio_USD_BOB;
                                    item.CodigoDeMoneda = "BOB";
                                    item.COMISION_CONVERSION_USD_BOB = Comision_cobrada_bob;
                                    item.COMISION_MONEDA_CONVERSION = "BOB";
                                    //Se guarda cuanto se cobro por la comision
                                    item.APOM_ITF_COMISION_BS = CalculoITF.Calcular_ITF(Comision_cobrada_bob);
                                }
                                else
                                {
                                    validado = false;
                                }
                            }
                        }
                        else
                        {
                            validado = false;
                        }
                    }
                    if (validado == false)
                    {
                        if (item.CodigoOperacionBancaria.ToString().Trim() == "CRED")
                        {
                            area_operativa = dbapom.DTMonitoreoCorreos("PAGOS");

                        }
                        else
                        {
                            area_operativa = dbapom.DTMonitoreoCorreos("SERVICIOS");

                        }

                        dbapom.Registro_erroneo(item.IdMensaje, "ERROR EN EL MOTOR ACH AL CALCULAR EL MONTO DE DINERO EN  LA LECTURA DE DATOS DEL REGISTRO" + item.IdMensaje.ToString().Trim() + " CON NUMERO DE REFERENCIA " + item.Referencia.ToString().Trim() + " SE LE RUEGA PROCEDER CON EL PROCESO MANUAL DEL MISMO ");
                        Bitacora.Enviarmails5("ERROR EN EL MOTOR ACH AL CALCULAR EL MONTO DE DINERO EN  LA LECTURA DE DATOS DEL REGISTRO" + item.IdMensaje.ToString().Trim() + " CON NUMERO DE REFERENCIA " + item.Referencia.ToString().Trim() + " SE LE RUEGA PROCEDER CON EL PROCESO MANUAL DEL MISMO ", area_operativa);
                        Bitacora.LogAdvertencia(LogManager.GetCurrentClassLogger(), "ERROR EN EL MOTOR ACH AL CALCULAR EL MONTO DE DINERO EN  LA LECTURA DE DATOS DEL REGISTRO" + item.IdMensaje.ToString().Trim() + " CON NUMERO DE REFERENCIA " + item.Referencia.ToString().Trim() + " SE LE RUEGA PROCEDER CON EL PROCESO MANUAL DEL MISMO ");

                        item.Validado = "10";

                    }

                    DatosTranFin.tranrows.Add(item);
                }


            }

            catch (Exception e)
            {
                Bitacora.Enviarmails5("Excepcion no idetificada al cargar los datos en ACH para operaciones en CAJAS DE AHORRO Error no identificado: " + e.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Excepcion no idetificada al cargar los datos en ACH para operaciones en CAJAS DE AHORRO Error no identificado:  ", e);

            }
            return DatosTranFin;
        }

        public decimal Conseguir_Importe_A_Pagar_USD(ComisionRequest com, ref decimal importe_de_comision, ref decimal comision_remesadora_res)
        {
            decimal Importe_a_pagar = 0;
            decimal comision_a_pagar = 0;
            decimal monto_maximo = 0;
            decimal Importe_total = 0;
            decimal comision_remesadora = 0;

            DataTable DatosachImportes = new DataTable();

            DatosachImportes = dbapom.DatosTranImportes(com.IdMensaje.Trim());

            bool validado = true;
            try
            {

                if (Convert.ToDecimal(com.Importe) >= 0)
                {
                    Importe_total = Convert.ToDecimal(com.Importe);
                    if (DatosachImportes.Rows.Count > 0)
                    {
                        if (DatosachImportes.Rows[0]["prm_TicketComision"].ToString().Trim() == "-")
                        {
                            if (Funciones.Redondeo_dos_decimales(DatosachImportes.Rows[0]["prm_Comision"].ToString().Trim()) >= 0)
                            {
                                importe_de_comision = Funciones.Redondeo_dos_decimales(DatosachImportes.Rows[0]["prm_Comision"].ToString().Trim());
                                Importe_a_pagar = Importe_total - importe_de_comision;
                            }
                            else
                            {
                                validado = false;
                            }

                        }
                        else
                        {
                            DataTable ticketcom = new DataTable();

                            ticketcom = dbapom.GmesaTicketComision(DatosachImportes.Rows[0]["prm_TicketComision"].ToString().Trim());

                            if (Funciones.Redondeo_dos_decimales(ticketcom.Rows[0]["Comision"].ToString().Trim()) >= 0)
                            {
                                importe_de_comision = Funciones.Redondeo_dos_decimales(ticketcom.Rows[0]["Comision"].ToString().Trim());
                                Importe_a_pagar = Importe_total - importe_de_comision;
                            }
                            else
                            {
                                validado = false;
                            }
                        }

                    }
                    else
                    {
                        //Es un abono en cuenta que viene de remesadora es SPRI
                        if (com.CodigoOperacionBancaria == "SPRI")
                        {
                            comision_a_pagar = conseguir_comision_remesa(com.Banquero, ref monto_maximo, ref comision_remesadora);
                            if (comision_a_pagar >= 0 && monto_maximo >= 0)
                            {
                                Importe_a_pagar = Importe_total - comision_remesadora;
                                comision_remesadora_res = comision_remesadora;
                                if (Importe_total > monto_maximo)
                                {
                                    comision_a_pagar = (Importe_a_pagar * comision_a_pagar) / 100;
                                    if (Funciones.Redondeo_dos_decimales(comision_a_pagar.ToString().Trim()) >= 0)
                                    {
                                        comision_a_pagar = Funciones.Redondeo_dos_decimales(comision_a_pagar.ToString().Trim());
                                        Importe_a_pagar = Importe_a_pagar - comision_a_pagar;
                                        importe_de_comision = comision_a_pagar;
                                    }
                                    else
                                    {
                                        validado = false;
                                    }
                                }
                                else
                                {
                                    importe_de_comision = 0.00M;
                                }

                            }
                            else
                            {
                                validado = false;
                            }
                        }
                        else
                        {
                            validado = false;
                        }
                    }
                }
                else
                {
                    validado = false;
                }
                if (validado == false)
                {

                    Bitacora.Enviarmails5("Se produjo un error al calcular el monto de dinero para el pago del archivo en CONSEGUIR IMPORTE USD  " + com.IdMensaje + " que hara el pago del siguiente banquero " + com.Banquero + "Error al calculo de importe", correoerrores);
                    Bitacora.LogAdvertencia(LogManager.GetCurrentClassLogger(), "Se produjo un error al calcular el monto de dinero para el pago del archivo en CONSEGUIR IMPORTE USD  " + com.IdMensaje + " que hara el pago del siguiente banquero " + com.Banquero + "Error al calculo de importe");
                    Importe_a_pagar = -1;
                    importe_de_comision = -1;
                }

                return Importe_a_pagar;

            }

            catch (Exception e)
            {

                Bitacora.Enviarmails5("Error no se procesaron los datos en CONSEGUIR IMPORTE USD generico de la Referencia: " + com.IdMensaje + "  " + e.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error no se procesaron los datos en CONSEGUIR IMPORTE USD generico de la Referencia:" + com.IdMensaje, e);

                return -1;
            }



        }
        public decimal Conseguir_Importe_A_Pagar_BOB(ComisionRequest com, decimal comision_cobrada, ref  decimal comision_cobrada_conversion_bob, ref decimal TC_Ex, decimal MONTO_A_PAGAR, decimal COMISION_REMESADORA, ref  decimal Importe_original_USD)
        {
            decimal Importe_a_pagar = 0;
            decimal Importe_original_BOB = 0;

            DataTable DatosachImportes = new DataTable();
            DatosachImportes = dbapom.DatosTranImportes(com.IdMensaje.Trim());


            bool validado = true;
            try
            {

                if (Convert.ToDecimal(com.Importe) >= 0)
                {
                    Importe_original_USD = Convert.ToDecimal(com.Importe);
                    if (DatosachImportes.Rows.Count > 0)
                    {
                        if (com.CodigoDeMoneda == "USD")
                        {

                            if (DatosachImportes.Rows[0]["prm_TicketBackCambios"].ToString().Trim() != "-")
                            {

                                if (Convert.ToDecimal(DatosachImportes.Rows[0]["prm_TipoCambio"].ToString().Trim()) >= 0)
                                {
                                    TC_Ex = Convert.ToDecimal(DatosachImportes.Rows[0]["prm_TipoCambio"].ToString().Trim());
                                    comision_cobrada_conversion_bob = comision_cobrada * TC_Ex;
                                    if (Funciones.Redondeo_dos_decimales(comision_cobrada_conversion_bob.ToString()) >= 0 && Funciones.Redondeo_dos_decimales(Importe_original_USD.ToString()) >= 0)
                                    {
                                        comision_cobrada_conversion_bob = Funciones.Redondeo_dos_decimales(comision_cobrada_conversion_bob.ToString());
                                        Importe_original_USD = Funciones.Redondeo_dos_decimales(Importe_original_USD.ToString());
                                        Importe_original_BOB = Importe_original_USD * TC_Ex;
                                        if (Funciones.Redondeo_dos_decimales(Importe_original_BOB.ToString()) >= 0)
                                        {
                                            Importe_original_BOB = Funciones.Redondeo_dos_decimales(Importe_original_BOB.ToString());
                                            Importe_a_pagar = Importe_original_BOB - comision_cobrada_conversion_bob;
                                        }
                                        else
                                        {
                                            validado = false;
                                        }
                                    }
                                    else
                                    {
                                        validado = false;
                                    }
                                }
                                else
                                {
                                    validado = false;
                                }


                            }
                            else
                            {
                                InformacionTipoCambio tc = new InformacionTipoCambio();


                                if (Convert.ToDecimal(SmarttcCompra) >= 0)
                                {
                                    TC_Ex = Convert.ToDecimal(SmarttcCompra);
                                    comision_cobrada_conversion_bob = comision_cobrada * TC_Ex;
                                    if (Funciones.Redondeo_dos_decimales(comision_cobrada_conversion_bob.ToString()) >= 0 && Funciones.Redondeo_dos_decimales(Importe_original_USD.ToString()) >= 0)
                                    {
                                        comision_cobrada_conversion_bob = Funciones.Redondeo_dos_decimales(comision_cobrada_conversion_bob.ToString());
                                        Importe_original_USD = Funciones.Redondeo_dos_decimales(Importe_original_USD.ToString());
                                        Importe_original_BOB = Importe_original_USD * TC_Ex;
                                        if (Funciones.Redondeo_dos_decimales(Importe_original_BOB.ToString()) >= 0)
                                        {
                                            Importe_original_BOB = Funciones.Redondeo_dos_decimales(Importe_original_BOB.ToString());
                                            Importe_a_pagar = Importe_original_BOB - comision_cobrada_conversion_bob;
                                        }
                                        else
                                        {
                                            validado = false;
                                        }
                                    }
                                    else
                                    {
                                        validado = false;
                                    }
                                }
                                else
                                {
                                    validado = false;
                                }

                            }

                        }
                        else
                        {
                            validado = false;
                        }

                    }
                    else
                    {
                        // aqui aumentar codigo para SPRI 
                        if (com.CodigoOperacionBancaria == "SPRI")
                        {
                            if (COMISION_REMESADORA > 0)
                            {
                                Importe_original_USD = MONTO_A_PAGAR;
                            }

                            if (Convert.ToDecimal(SmarttcCompra) >= 0)
                            {
                                TC_Ex = Convert.ToDecimal(SmarttcCompra);
                                comision_cobrada_conversion_bob = comision_cobrada * TC_Ex;
                                if (Funciones.Redondeo_dos_decimales(comision_cobrada_conversion_bob.ToString()) >= 0 && Funciones.Redondeo_dos_decimales(Importe_original_USD.ToString()) >= 0)
                                {
                                    comision_cobrada_conversion_bob = Funciones.Redondeo_dos_decimales(comision_cobrada_conversion_bob.ToString());
                                    Importe_original_USD = Funciones.Redondeo_dos_decimales(Importe_original_USD.ToString());
                                    Importe_original_BOB = Importe_original_USD * TC_Ex;
                                    if (Funciones.Redondeo_dos_decimales(Importe_original_BOB.ToString()) >= 0)
                                    {
                                        Importe_original_BOB = Funciones.Redondeo_dos_decimales(Importe_original_BOB.ToString());
                                        Importe_a_pagar = Importe_original_BOB - comision_cobrada_conversion_bob;
                                    }
                                    else
                                    {
                                        validado = false;
                                    }
                                }
                                else
                                {
                                    validado = false;
                                }
                            }
                            else
                            {
                                validado = false;
                            }

                        }
                        else
                        {
                            validado = false;
                        }
                    }
                }
                else
                {
                    validado = false;
                }
                if (validado == false)
                {

                    Bitacora.Enviarmails5("Se produjo un error al calcular el monto de dinero en moneda extranjera para el pago del archivo " + com.IdMensaje + " que hara el pago del siguiente banquero  Conseguir_Importe_A_Pagar_BOB " + com.Banquero + "Error al calculo de importe", correoerrores);
                    Bitacora.LogAdvertencia(LogManager.GetCurrentClassLogger(), "Se produjo un error al calcular el monto de dinero en moneda extranjera para el pago del archivo " + com.IdMensaje + " que hara el pago del siguiente banquero  Conseguir_Importe_A_Pagar_BOB " + com.Banquero + "Error al calculo de importe");
                    Importe_a_pagar = -1;
                    comision_cobrada_conversion_bob = -1;
                }
                return Importe_a_pagar;

            }

            catch (Exception e)
            {
                Bitacora.Enviarmails5("Error no se procesaron los datos Conseguir_Importe_A_Pagar_BOB generico de la Referencia: " + com.IdMensaje + "  " + e.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error no se procesaron los datos Conseguir_Importe_A_Pagar_BOB generico de la Referencia: " + com.IdMensaje, e);
                return -1;

            }


        }
        public decimal Conseguir_Importe_A_Pagar_EUR(ComisionRequest com, ref  decimal Monto_Ex, ref decimal TC_Ex, ref decimal importe_de_comision)
        {
            decimal Importe_a_pagar = 0;
            decimal Importe_total = 0;
            decimal Importe_total_calculado_XEU_SUS = 0;
            DataTable DatosachImportes = new DataTable();
            DatosachImportes = dbapom.DatosTranImportes(com.IdMensaje.Trim());

            bool validado = true;
            try
            {

                if (Convert.ToDecimal(com.Importe) >= 0)
                {
                    Importe_total = Convert.ToDecimal(com.Importe);

                    if (DatosachImportes.Rows.Count > 0)
                    {
                        if (DatosachImportes.Rows[0]["prm_TicketTC"].ToString().Trim() != "-")
                        {
                            DataTable ticketTC = new DataTable();
                            ticketTC = dbapom.GmesaTicketTC(DatosachImportes.Rows[0]["prm_TicketTC"].ToString().Trim());
                            if (DatosachImportes.Rows[0]["prm_TicketComision"].ToString().Trim() == "-")
                            {


                                if (Funciones.Redondeo_dos_decimales(DatosachImportes.Rows[0]["prm_Comision"].ToString().Trim()) >= 0 && Funciones.Redondeo_dos_decimales(ticketTC.Rows[0]["cmp_EQ_SUS"].ToString().Trim()) >= 0 && Convert.ToDecimal(ticketTC.Rows[0]["cmp_TC_Operacion"].ToString().Trim()) >= 0)
                                {
                                    Importe_total_calculado_XEU_SUS = Funciones.Redondeo_dos_decimales(ticketTC.Rows[0]["cmp_EQ_SUS"].ToString().Trim());
                                    importe_de_comision = Funciones.Redondeo_dos_decimales(DatosachImportes.Rows[0]["prm_Comision"].ToString().Trim());
                                    Importe_a_pagar = Importe_total_calculado_XEU_SUS - importe_de_comision;
                                    Monto_Ex = Importe_total_calculado_XEU_SUS;
                                    TC_Ex = Convert.ToDecimal(ticketTC.Rows[0]["cmp_TC_Operacion"].ToString().Trim());
                                }
                                else
                                {
                                    validado = false;
                                }

                            }
                            else
                            {
                                DataTable ticketComision = new DataTable();
                                ticketComision = dbapom.GmesaTicketComision(DatosachImportes.Rows[0]["prm_TicketComision"].ToString().Trim());

                                if (ticketComision.Rows.Count > 0)
                                {
                                    if (Funciones.Redondeo_dos_decimales(ticketTC.Rows[0]["cmp_EQ_SUS"].ToString().Trim()) >= 0 && Convert.ToDecimal(ticketTC.Rows[0]["cmp_TC_Operacion"].ToString().Trim()) >= 0 && Funciones.Redondeo_dos_decimales(ticketComision.Rows[0]["Comision"].ToString().Trim()) >= 0)
                                    {
                                        Importe_total_calculado_XEU_SUS = Funciones.Redondeo_dos_decimales(ticketTC.Rows[0]["cmp_EQ_SUS"].ToString().Trim());
                                        importe_de_comision = Funciones.Redondeo_dos_decimales(ticketComision.Rows[0]["Comision"].ToString().Trim());
                                        Importe_a_pagar = Importe_total_calculado_XEU_SUS - importe_de_comision;
                                        Monto_Ex = Importe_total_calculado_XEU_SUS;
                                        TC_Ex = Convert.ToDecimal(ticketTC.Rows[0]["cmp_TC_Operacion"].ToString().Trim());
                                    }
                                    else
                                    {
                                        validado = false;
                                    }
                                }
                                else
                                {
                                    validado = false;
                                }

                            }

                        }
                        else
                        {
                            validado = false;
                        }
                    }
                    else
                    {
                        validado = false;
                    }
                }
                else
                {
                    validado = false;
                }
                if (validado == false)
                {
                    Bitacora.Enviarmails5("Se produjo un error al calcular el monto de dinero en moneda extranjera para el pago del archivo " + com.IdMensaje + " que hara el pago del siguiente banquero Conseguir_Importe_A_Pagar_EUR " + com.Banquero + "Error al calculo de importe", correoerrores);
                    Bitacora.LogAdvertencia(LogManager.GetCurrentClassLogger(), "Se produjo un error al calcular el monto de dinero en moneda extranjera para el pago del archivo " + com.IdMensaje + " que hara el pago del siguiente banquero Conseguir_Importe_A_Pagar_EUR " + com.Banquero + "Error al calculo de importe");

                    Importe_a_pagar = -1;
                    importe_de_comision = -1;
                }
                return Importe_a_pagar;

            }

            catch (Exception e)
            {
                Bitacora.Enviarmails5("Error no se procesaron los datos Conseguir_Importe_A_Pagar_EUR generico de la Referencia: " + com.IdMensaje + "  " + e.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error no se procesaron los datos Conseguir_Importe_A_Pagar_EUR generico de la Referencia: " + com.IdMensaje, e);

                return -1;
            }

        }
        public decimal Conseguir_Importe_A_Pagar_BOB_EUR(ComisionRequest com, decimal comision_cobrada, ref  decimal comision_cobrada_conversion_bob, ref decimal TC_Ex, decimal TipoCambio_XEU)
        { //Importe MONTO_ORIGINAL_CONVERSION_USD_BOB

            decimal Importe_a_pagar = 0;
            decimal Importe_original_XEU = 0;
            decimal Importe_original_USD = 0;
            decimal Importe_original_BOB = 0;
            decimal Importe_USD = 0;
            DataTable DatosachImportes = new DataTable();
            DatosachImportes = dbapom.DatosTranImportes(com.IdMensaje.Trim());
            bool validado = true;
            try
            {

                if (Convert.ToDecimal(com.MONTO_ORIGINAL_CONVERSION_USD_BOB) > 0)
                {
                    Importe_USD = Convert.ToDecimal(com.MONTO_ORIGINAL_CONVERSION_USD_BOB);
                    if (DatosachImportes.Rows.Count > 0)
                    {
                        if (com.MONEDA_ORIGINAL != "USD")
                        {

                            if (DatosachImportes.Rows[0]["prm_TicketBackCambios"].ToString().Trim() != "-")
                            {

                                if (Convert.ToDecimal(DatosachImportes.Rows[0]["prm_TipoCambio"].ToString().Trim()) > 0)
                                {
                                    TC_Ex = Convert.ToDecimal(DatosachImportes.Rows[0]["prm_TipoCambio"].ToString().Trim());
                                    comision_cobrada_conversion_bob = comision_cobrada * TC_Ex;
                                    if (Funciones.Redondeo_dos_decimales(comision_cobrada_conversion_bob.ToString()) >= 0 && Funciones.Redondeo_dos_decimales(com.Importe) > 0)
                                    {
                                        comision_cobrada_conversion_bob = Funciones.Redondeo_dos_decimales(comision_cobrada_conversion_bob.ToString());
                                        Importe_original_XEU = Funciones.Redondeo_dos_decimales(com.Importe);
                                        Importe_original_USD = Importe_USD;
                                        Importe_original_BOB = Importe_original_USD * TC_Ex;
                                        if (Funciones.Redondeo_dos_decimales(Importe_original_BOB.ToString()) >= 0)
                                        {
                                            Importe_original_BOB = Funciones.Redondeo_dos_decimales(Importe_original_BOB.ToString());
                                            Importe_a_pagar = Importe_original_BOB - comision_cobrada_conversion_bob;
                                        }
                                        else
                                        {
                                            validado = false;
                                        }
                                    }
                                    else
                                    {
                                        validado = false;
                                    }
                                }
                                else
                                {
                                    validado = false;
                                }


                            }
                            else
                            {

                                if (Convert.ToDecimal(SmarttcCompra.ToString().Trim()) >= 0)
                                {
                                    TC_Ex = Convert.ToDecimal(SmarttcCompra.ToString().Trim());
                                    comision_cobrada_conversion_bob = comision_cobrada * TC_Ex;
                                    if (Funciones.Redondeo_dos_decimales(comision_cobrada_conversion_bob.ToString()) >= 0 && Funciones.Redondeo_dos_decimales(com.Importe) >= 0)
                                    {
                                        comision_cobrada_conversion_bob = Funciones.Redondeo_dos_decimales(comision_cobrada_conversion_bob.ToString());
                                        Importe_original_XEU = Funciones.Redondeo_dos_decimales(com.Importe);
                                        Importe_original_USD = Importe_USD;
                                        Importe_original_BOB = Importe_original_USD * TC_Ex;
                                        if (Funciones.Redondeo_dos_decimales(Importe_original_BOB.ToString()) >= 0)
                                        {
                                            Importe_original_BOB = Funciones.Redondeo_dos_decimales(Importe_original_BOB.ToString());
                                            Importe_a_pagar = Importe_original_BOB - comision_cobrada_conversion_bob;
                                        }
                                        else
                                        {
                                            validado = false;
                                        }
                                    }
                                    else
                                    {
                                        validado = false;
                                    }
                                }
                                else
                                {
                                    validado = false;
                                }

                            }

                        }
                        else
                        {
                            validado = false;
                        }

                    }
                    else
                    {
                        validado = false;
                    }
                }
                else
                {
                    validado = false;
                }
                if (validado == false)
                {
                    Bitacora.Enviarmails5("Se produjo un error al calcular el monto de dinero en moneda extranjera para el pago del archivo " + com.IdMensaje + " que hara el pago del siguiente banquero Conseguir_Importe_A_Pagar_BOB_EUR " + com.Banquero + "Error al calculo de importe", correoerrores);
                    Bitacora.LogAdvertencia(LogManager.GetCurrentClassLogger(), "Se produjo un error al calcular el monto de dinero en moneda extranjera para el pago del archivo " + com.IdMensaje + " que hara el pago del siguiente banquero Conseguir_Importe_A_Pagar_BOB_EUR " + com.Banquero + "Error al calculo de importe");
                    Importe_a_pagar = -1;
                    comision_cobrada_conversion_bob = -1;
                }
                return Importe_a_pagar;

            }

            catch (Exception e)
            {
                Bitacora.Enviarmails5("Error no se procesaron los datos Conseguir_Importe_A_Pagar_BOB_EUR generico de la Referencia: " + com.IdMensaje + "  " + e.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error no se procesaron los datos Conseguir_Importe_A_Pagar_BOB_EUR generico de la Referencia: " + com.IdMensaje + com.IdMensaje, e);
                return -1;
            }



        }
        public decimal conseguir_comision_remesa(string Banquero, ref decimal monto_maximo, ref decimal comision_remesadora)
        {

            comision_remesadora = 0;
            decimal comision_cobrada = 0;
            monto_maximo = 0;

            try
            {
                string RESULTADO = "";
                string RESULTADO_2 = "";
                string RESULTADO_3 = "";

                RESULTADO = dbapom.Comision_SPRI(Banquero.Trim(), ref RESULTADO_2, ref RESULTADO_3);

                if (RESULTADO.ToString().Trim() != "" && RESULTADO_2.ToString().Trim() != "" && RESULTADO_3.ToString().Trim() != "")
                {
                    comision_cobrada = Convert.ToDecimal(RESULTADO.ToString().Trim());
                    monto_maximo = Convert.ToDecimal(RESULTADO_2.ToString().Trim());
                    comision_remesadora = Convert.ToDecimal(RESULTADO_3.ToString().Trim());
                }
                else
                {
                    comision_cobrada = -1;
                    monto_maximo = -1;
                    comision_remesadora = -1;
                }

                return comision_cobrada;

            }

            catch (Exception e)
            {

                Bitacora.Enviarmails5("Error al conseguir la comision de la remesadora GENERICO" + e.Message.ToString(), correoerrores);
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Error al conseguir la comision de la remesadora GENERICO", e);
                return -1;

            }


        }



    }
}
