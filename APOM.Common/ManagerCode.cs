using System;

namespace APOM.Common
{
    public static class ManagerCode
    {
        public static string GetCode(string typeIN)
        {
            string result = string.Empty;
            if (typeIN.Equals("OK"))
                result = "200";
            else if (typeIN.Equals("CREATED"))
                result = "201";
            else if (typeIN.Equals("NO_CONTENT"))
                result = "203";
            else if (typeIN.Equals("NO_MODIFIED"))
                result = "304";
            else if (typeIN.Equals("UNAUTHORIZED"))
                result = "401";
            else if (typeIN.Equals("UNPROCESABLE_ENTITY"))
                result = "422";
            else if (typeIN.Equals("ERROR_FATAL"))
                result = "500";
            else if (typeIN.Equals("SERVICE_UNAVARIABLE"))
                result = "503";
            else
                result = "500";
            return result;
        }

        public static string GenerateOperation()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmssff");
        }
    }
}
