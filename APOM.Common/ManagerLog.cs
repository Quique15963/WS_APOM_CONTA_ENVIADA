using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace APOM.Common
{
    public static class ManagerLog
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethodLogger()
        {
            var st = new StackTrace(new StackFrame(1));
            return st.GetFrame(0).GetMethod().Name;
        }
    }
}
