using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OpusNet
{
    public static class Library
    {
        public static string opus_x32 = "opus32.dll";
        public static string opus_x64 = "opus64.dll";

        public static string opus_dll = IntPtr.Size == 4 ? opus_x32 : opus_x64;
        
    }
}
