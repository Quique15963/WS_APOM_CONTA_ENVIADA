using System;

namespace APOM.Entities
{
    public class GenericResponse : IDisposable
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string code { get; set; }
        public string codextorno { get; set; }

        public GenericResponse()
        {
            this.code = APOM.Common.ManagerCode.GetCode("ERROR_FATAL");
            this.message = APOM.Common.ManagerMessage.GetMessage("ERROR_FATAL");
            this.success = false;
        }
        void IDisposable.Dispose() { }
    }
}
