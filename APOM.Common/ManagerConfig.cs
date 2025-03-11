using System;
using System.Configuration;

namespace APOM.Common
{
    public static class ManagerConfig
    {
        public static string GetKeyCongigString(string strKeyNameIN)
        {
            string resultado = string.Empty;
            try
            {
                resultado = ConfigurationManager.AppSettings.Get(strKeyNameIN);
            }
            catch (Exception)
            {
                throw;
            }
            return resultado;
        }

        public static int GetKeyCongigInt(string strKeyNameIN)
        {
            int resultado = 0;
            try
            {
                resultado = Convert.ToInt32(ConfigurationManager.AppSettings.Get(strKeyNameIN).ToString());
            }
            catch (Exception)
            {
                throw;
            }
            return resultado;
        }

    }
}
