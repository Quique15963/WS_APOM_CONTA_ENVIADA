using System;

namespace APOM.Common
{
    public static class ManagerChannel
    {
        static public bool IsValidChannel(string canalIN)
        {
            bool result = false;
            string canalesValidos = string.Empty;
            try
            {
                canalesValidos = ManagerConfig.GetKeyCongigString("VALID_CHANNEL");
                if (canalesValidos.Trim() != string.Empty)
                {
                    string[] canales = canalesValidos.Split('|');
                    foreach (string canal in canales)
                    {
                        if (canal.Trim().Equals(canalIN))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }
    }
}
