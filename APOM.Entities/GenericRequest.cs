using System;

namespace APOM.Entities
{
    public class GenericRequest: IDisposable
    {
        public string channel { get; set; }

        void IDisposable.Dispose() { }
    }
}
