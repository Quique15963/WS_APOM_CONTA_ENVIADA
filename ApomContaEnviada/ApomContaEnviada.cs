using APOM.Business;
using APOM.Log;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using BCP.Whatsapp.Contabilidad;

namespace ApomContaEnviada
{
    public partial class ApomContaEnviada : ServiceBase
    {
        private System.Timers.Timer Reloj;
        private DateTime HoraInicio;
        private DateTime HoraFin;
        private DateTime FechaEnvio = DateTime.Now;
        public static string CorreoErrores = string.Empty;
        public static string AsuntoErrores = string.Empty;
        public static string Asunto = string.Empty;
        public static string CorreoDe = string.Empty;
        public static string Cuerpo = string.Empty;
        public static string ServidorCorreo = string.Empty;
        public ApomContaEnviada()
        {
            InitializeComponent();
            try
            {
                CorreoErrores = ConfigurationSettings.AppSettings["CORREO_OPERACIONES"];
                CorreoDe = ConfigurationSettings.AppSettings["CORREO_APLICATIVO"];
                AsuntoErrores = ConfigurationSettings.AppSettings["CABECERA_ERROR"];
                ServidorCorreo = ConfigurationSettings.AppSettings["SERVIDOR_SMTP"];
                Asunto = ConfigurationSettings.AppSettings["ASUNTO"];
                Cuerpo = ConfigurationSettings.AppSettings["CUERPOCORREO"];
                HoraInicio = Convert.ToDateTime(ConfigurationSettings.AppSettings["HORA_INICIO"]);
                HoraFin = Convert.ToDateTime(ConfigurationSettings.AppSettings["HORA_FIN"]);
                //HoraInicioPago = Convert.ToDateTime(ConfigurationSettings.AppSettings["HORA_INICIO_PAGO_NOBATCH"]);
                //HoraFinPago = Convert.ToDateTime(ConfigurationSettings.AppSettings["HORA_FIN_PAGO_NOBATCH"]);

#if DEBUG
                Proceso();
#endif
                this.Reloj = new System.Timers.Timer();
                this.Reloj.Elapsed += new ElapsedEventHandler(this.RelojElapsed);
                this.Reloj.Enabled = true;
                this.Reloj.Interval = Convert.ToDouble(ConfigurationSettings.AppSettings["INTERVALO_EJECUCION"]) * 60 * 1000;
            }
            catch (Exception ex)
            {

                Bitacora.Enviarmails3(CorreoErrores,  AsuntoErrores, Cuerpo + ": Se produjo un error general" + ex.Message.ToString());
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Se produjo un error general: ", ex);
            }
        }

        protected override void OnStart(string[] args)
        {
            Bitacora.LogInformacion(LogManager.GetCurrentClassLogger(), "**********************SE INICIO EL SERVICIO APOM TRANSFERENCIAS ENVIADAS CONTABILIDAD**************************");
        }

        protected override void OnStop()
        {
            Bitacora.LogInformacion(LogManager.GetCurrentClassLogger(), "**********************SE APAGO EL SERVICIO APOM TRANSFERENCIAS ENVIADAS CONTABILIDAD**************************");
        }
        private void Proceso()
        {
            try
            {
                lock (this)
                {

                   
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);
                    if (DateTime.Now.TimeOfDay >= HoraInicio.TimeOfDay && DateTime.Now.TimeOfDay <= HoraFin.TimeOfDay)
                    {
                        BusinessContabilidad bc = new BusinessContabilidad();
                        bc.ProcesoContabilidad();
                        bc.ProcesoContabilidadVent();

                        bc.ProcesoContabilidadCripto();

                        BusinessContabilidadWhatsApp wp = new BusinessContabilidadWhatsApp();
                        wp.ProcesoContabilidadWhastApp();


                    }
                }
            }
            catch (Exception ex)
            {
                Bitacora.LogError(LogManager.GetCurrentClassLogger(), "Se produjo un error general en el PROCESO: ", ex);

            }
        }


        public void RelojElapsed(System.Object sender, System.Timers.ElapsedEventArgs e)
        {

            lock (this)
            {
                Proceso();

            }

        }

    }
}
