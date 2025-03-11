
namespace APOM.Common
{
    public static  class ManagerMessage
    {
        public static string GetMessage(string typeIN) {
            string result = string.Empty;
            if (typeIN.Equals("OK"))
                result = "La solicitud se ejecutó correctamente.";
            else if (typeIN.Equals("CREATED"))
                result = "El registro  se creó correctamente.";
            else if (typeIN.Equals("NO_CONTENT"))
                result = "La solicitud se ejecutó correctamente pero no afecto ningún registro.";
            else if (typeIN.Equals("NO_MODIFIED"))
                result = "No se pudo actualizar el registro solicitado.";
            else if (typeIN.Equals("UNAUTHORIZED"))
                result = "Canal no autorizado.";
            else if (typeIN.Equals("UNPROCESABLE_ENTITY"))
                result = "La información enviada no puede ser procesada.";
            else if (typeIN.Equals("ERROR_FATAL"))
                result = "Ocurrió un error comuníquese con el administrador del sistema.";
            else if (typeIN.Equals("SERVICE_UNAVARIABLE"))
                result = "El servicio se encuentra temporalmente fuera de servicio.";
            else
                result = "Ocurrió un error comuníquese con el administrador del sistema.";
            return result;
        }
    }
}
