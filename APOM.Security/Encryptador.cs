
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace APOM.Security
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    public class Encryptador
    {
        /// <summary>
        /// Función wraper para encriptar y desencriptar valores - Segurinet
        /// </summary>
        /// <param name="EnDec">true - Encriptar, false - Desencriptar</param>
        /// <param name="InputString">String Valor</param>
        /// <param name="OutputRaw">Resultado</param>
        /// <param name="Length">Tamaño del string destino.</param>
        /// <returns></returns>
        /// 

        //[DllImport("SegCryptAPOM.dll")]
        [DllImport(@"D:\Windows\SysWOW64\SegCryptAPOM.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]

        unsafe public static extern bool EncryptDecrypt(bool EnDec, string InputString, byte* OutputRaw, int* Length);

        unsafe public static bool EncryptDecrypt(bool enDec, string strInput, ref string strOutput)
        {
            bool res = false;

            int chrSize = strInput.Length * 2;
            byte[] byteRes = new byte[chrSize];
            strOutput = "";

            fixed (byte* pByteRes = &byteRes[0])
            {
                res = EncryptDecrypt(enDec, strInput, pByteRes, &chrSize);
                for (int i = 0; i < chrSize; i++)
                    strOutput += ((char)*(pByteRes + i));
            }

            return res;
        }

        //public static extern bool EncryptDecrypt([MarshalAs(UnmanagedType.Bool)] bool fEncrypt, string lpszInBuffer, StringBuilder sOut, [MarshalAs(UnmanagedType.I4)] ref int dsize);

        //public static string EncryptDecrypt(bool fEncrypt, string text)
        //{
        //    string res = string.Empty;
        //    if (string.IsNullOrWhiteSpace(text))
        //        return res;
        //    try
        //    {
        //        int nSize = text.Length * 2 + 1;
        //        bool bRet;
        //        StringBuilder outString = new StringBuilder(nSize);
        //        bRet = EncryptDecrypt(fEncrypt, text, outString, ref nSize);
        //        res = outString.ToString();
        //    }
        //
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error en EncryptDecrypt, " + ex.Message);
        //    }
        //    return res;
        //}

        /// <summary>
        /// Función para encriptar y desencriptar valores - Segurinet
        /// </summary>
        /// <param name="EnDec">true - Encriptar, false - Desencriptar</param>
        /// <param name="InputString">String Valor</param>
        /// <param name="OutputRaw">Resultado</param>
        /// <param name="Length">Tamaño del string destino.</param>
        /// <returns></returns>
        //[DllImport("Segcrypt_OpVenta.dll")]

        //[DllImport(@"D:\Windows\SysWOW64\SegCryptCentinela64.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]

    }
}