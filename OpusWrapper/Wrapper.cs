using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OpusNet
{
    internal class Wrapper
    {
        #region Variables
        //----------------------------------------------------------------------------------------------------------------

        [DllImport("kernel32")]
        extern static IntPtr LoadLibrary(string librayName);

        [DllImport("kernel32")]
        static extern IntPtr GetProcAddress(IntPtr hModule, String procName);

        //----------------------------------------------------------------------------------------------------------------

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr del_opus_encoder_create(int Fs, int channels, int application, out IntPtr error);
        internal static del_opus_encoder_create opus_encoder_create;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void del_opus_encoder_destroy(IntPtr encoder);
        internal static del_opus_encoder_destroy opus_encoder_destroy;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int del_opus_encode(IntPtr st, byte[] pcm, int frame_size, IntPtr data, int max_data_bytes);
        internal static del_opus_encode opus_encode;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr del_opus_decoder_create(int Fs, int channels, out IntPtr error);
        internal static del_opus_decoder_create opus_decoder_create;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void del_opus_decoder_destroy(IntPtr decoder);
        internal static del_opus_decoder_destroy opus_decoder_destroy;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int del_opus_decode(IntPtr st, byte[] data, int len, IntPtr pcm, int frame_size, int decode_fec);
        internal static del_opus_decode opus_decode;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int del_opus_encoder_ctl1(IntPtr st, Ctl request, int value);
        internal static del_opus_encoder_ctl1 opus_encoder_ctl1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int del_opus_encoder_ctl2(IntPtr st, Ctl request, out int value);
        internal static del_opus_encoder_ctl2 opus_encoder_ctl2;

        //----------------------------------------------------------------------------------------------------------------

        static IntPtr m_libraryHandle;
        static bool Initialized = false;

        //----------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructor
        //----------------------------------------------------------------------------------------------------------------
        static Wrapper()
        {
        }
        //----------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //----------------------------------------------------------------------------------------------------------------
        public static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;
                _LoadLibrary(Library.opus_dll);

                opus_encoder_create = GetFunction<del_opus_encoder_create>("opus_encoder_create");
                opus_encoder_destroy = GetFunction<del_opus_encoder_destroy>("opus_encoder_destroy");
                opus_encode = GetFunction<del_opus_encode>("opus_encode");
                opus_decoder_create = GetFunction<del_opus_decoder_create>("opus_decoder_create");
                opus_decoder_destroy = GetFunction<del_opus_decoder_destroy>("opus_decoder_destroy");
                opus_decode = GetFunction<del_opus_decode>("opus_decode");
                opus_encoder_ctl1 = GetFunction<del_opus_encoder_ctl1>("opus_encoder_ctl");
                opus_encoder_ctl2 = GetFunction<del_opus_encoder_ctl2>("opus_encoder_ctl");
            }
        }
        //----------------------------------------------------------------------------------------------------------------

        private static void _LoadLibrary(String path)
        {
            m_libraryHandle = LoadLibrary(path);

            if (m_libraryHandle == IntPtr.Zero)
            {
                int hr = Marshal.GetHRForLastWin32Error();
                Exception innerException = Marshal.GetExceptionForHR(hr);
                if (innerException != null)
                    throw new Exception("Error loading unmanaged library from path: " + path + ", see inner exception for details.\n" + innerException.Message, innerException);
                else
                    throw new Exception("Error loading unmanaged library from path: " + path);
            }
        }

        //----------------------------------------------------------------------------------------------------------------

        internal static T GetFunction<T>(String functionName) where T : class
        {
            var procAddr = GetProcAddress(m_libraryHandle, functionName);

            if (procAddr == IntPtr.Zero)
                return null;

            var function = Marshal.GetDelegateForFunctionPointer(procAddr, typeof(T));

            return function as T;
        }

        //----------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
