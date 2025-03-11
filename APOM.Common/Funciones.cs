using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace APOM.Common
{
    public class Funciones
    {
        /// <summary>
        /// Identificacion de Palabras que Machean al 100%
        /// </summary>
        /// <param name="Objetivo">Palabras Objetivo</param>
        /// <param name="comparacion">Palabras a Comparar</param>
        private static void CompararionPalabras100(ref System.Collections.Generic.List<DetalleNombre> Objetivo, ref System.Collections.Generic.List<DetalleNombre> comparacion)
        {
            for (int cont = 0; cont < Objetivo.Count; cont++)
            {
                if (!Objetivo[cont].ResultadoComparacion)
                {
                    for (int cont2 = 0; cont2 < comparacion.Count; cont2++)
                    {
                        if (!comparacion[cont2].ResultadoComparacion)
                        {
                            if (Objetivo[cont].NomApe == comparacion[cont2].NomApe)
                            {
                                Objetivo[cont].ResultadoComparacion = true;
                                Objetivo[cont].Porcentaje = 100;
                                comparacion[cont2].ResultadoComparacion = true;
                                comparacion[cont2].Porcentaje = 100;
                                break;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Identificacion de Palabras por porcentaje de comparacion
        /// </summary>
        /// <param name="Objetivo">Objeto con el listado de palabras a comparar</param>
        /// <param name="comparacion">Objeto con el listado de palabras para comparacion</param>
        private static void ComparacionPalabras(ref System.Collections.Generic.List<DetalleNombre> Objetivo, ref System.Collections.Generic.List<DetalleNombre> comparacion)
        {
            for (int cont = 0; cont < Objetivo.Count; cont++)
            {
                decimal mejorcaso = 0;
                decimal ValorAceptado = Convert.ToDecimal(ConfigurationSettings.AppSettings["PORCENTAJE_ACEPTADO"]);
                int ContadorComparacion = -1;
                if (!Objetivo[cont].ResultadoComparacion)
                {
                    for (int cont2 = 0; cont2 < comparacion.Count; cont2++)
                    {
                        if (!comparacion[cont2].ResultadoComparacion)
                        {
                            decimal res = ComparacionPalabras(Objetivo[cont].NomApe, comparacion[cont2].NomApe);
                            if (res >= mejorcaso)
                            {
                                ContadorComparacion = cont2;
                                mejorcaso = res;
                            }
                        }
                    }
                    if (mejorcaso >= ValorAceptado)
                    {
                        Objetivo[cont].Porcentaje = mejorcaso;
                        Objetivo[cont].ResultadoComparacion = true;
                        comparacion[ContadorComparacion].Porcentaje = mejorcaso;
                        comparacion[ContadorComparacion].ResultadoComparacion = true;
                    }
                }
            }
        }
     
        /// <summary>
        /// Metodo que permite mapear del arreglo del nombre del cliente al objeto que se va manejar
        /// </summary>
        /// <param name="Objetivo">Valore por referencia del objeto</param>
        /// <param name="Origen">Arreglo con el nombre del clinte</param>
        private static void MapearNombre(ref System.Collections.Generic.List<DetalleNombre> Objetivo, string[] Origen)
        {
            for (int cont = 0; cont < Origen.Length; cont++)
            {
                Objetivo.Add(new DetalleNombre(Origen[cont].ToString()));
            }
        }
        /// <summary>
        /// Metodo para el macheo de nombres
        /// </summary>
        /// <param name="Nombre1">Nombre 1</param>
        /// <param name="Nombre2">Nombre 2</param>
        /// <returns></returns>
        public static bool ComparacionClientes(string Nombre1, string Nombre2, bool Tipo, ref decimal porcent)
        {
            string[] nombre;
            string[] swift;
            if (Tipo)
            {
                nombre = EliminarCaracteresEspeciales(Nombre1).Trim().ToUpper().Replace(" DE ", "").Replace(" DEL ", "").Replace(".", "").Split(' ');
                swift = EliminarCaracteresEspeciales(Nombre2).Trim().ToUpper().Replace(" DE ", "").Replace(" DEL ", "").Replace(".", "").Split(' ');
            }
            else
            {
                nombre = EliminarCaracteresEspeciales(Nombre1).Trim().ToUpper().Replace(".", "").Split(' ');
                swift = EliminarCaracteresEspeciales(Nombre2).Trim().ToUpper().Replace(".", "").Split(' ');
            }
            //Seleccion nombre objetivo y comparativo
            List<DetalleNombre> objetivo = new List<DetalleNombre>();
            List<DetalleNombre> comparacion = new List<DetalleNombre>();
            if (nombre.Length > swift.Length)
            {
                MapearNombre(ref objetivo, nombre);
                MapearNombre(ref comparacion, swift);
            }
            else
            {
                MapearNombre(ref objetivo, swift);
                MapearNombre(ref comparacion, nombre);
            }

            //Identificacion palabra 100%
            CompararionPalabras100(ref objetivo, ref comparacion);
            //Identificacion por letras   
            ComparacionPalabras(ref objetivo, ref comparacion);
            decimal porcentaje = 0;
            decimal por = decimal.Round(100M / Convert.ToDecimal(objetivo.Count), 2, MidpointRounding.AwayFromZero);
            for (int cont = 0; cont < objetivo.Count; cont++)
            {
                if (objetivo[cont].ResultadoComparacion)
                    porcentaje += decimal.Round(objetivo[cont].Porcentaje * (por / 100), 2, MidpointRounding.AwayFromZero);
            }
            porcent = porcentaje;
            if (porcentaje >= Convert.ToDecimal(ConfigurationSettings.AppSettings["PORCENTAJE_ACEPTADO_FINAL"]))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Metodo de Comparacion para caso de error por omision en digitacion
        /// </summary>
        /// <param name="Origen">Palabra Origen, que debe ser mayor a la destino</param>
        /// <param name="Destino">Palabra destino, que debe tener menos caracteres que la origen</param>
        /// <returns>Resultado en porcentaje de la comparacion</returns>
        private static decimal ComparacionPalabrasDiferenteLongitud(string Origen, string Destino)
        {
            bool ReglaOmision = false;
            decimal PorcentajePalabra = decimal.Round(100M / Convert.ToDecimal(Origen.Length), 2, MidpointRounding.AwayFromZero);
            decimal avance = 0M;
            int contOK = 0;
            for (int cont = 0; cont < Origen.Length; cont++)
            {
                bool Ok = false;
                for (int cont2 = contOK; cont2 < Destino.Length; cont2++)
                {
                    if (Origen[cont].ToString() == Destino[cont2].ToString())
                    {
                        avance += PorcentajePalabra;
                        Ok = true;
                        contOK++;
                        break;
                    }
                    else
                        break;
                }
                if (!Ok)
                    if (!ReglaOmision)
                        ReglaOmision = true;
                    else
                        break;
            }
            return avance;
        }
        /// <summary>
        /// Metodo de Comparacion para caso de error por error en digitacion de una letra o fonetica
        /// </summary>
        /// <param name="Origen">Palabra Origen</param>
        /// <param name="Destino">Palabra destino</param>
        /// <returns>Resultado en porcentaje de la comparacion</returns>
        private static decimal ComparacionPalabrasMismaLongitud(string Origen, string Destino)
        {
            bool ReglaOmision = false;
            decimal PorcentajePalabra = decimal.Round(100M / Convert.ToDecimal(Origen.Length), 2, MidpointRounding.AwayFromZero);
            decimal avance = 0M;
            for (int cont = 0; cont < Origen.Length; cont++)
            {
                if (Origen[cont].ToString() == Destino[cont].ToString())
                    avance += PorcentajePalabra;
            }
            return avance;
        }
     
        /// <summary>
        /// Metodo que permite comparar palabras por porcenteja de aproximacion
        /// </summary>
        /// <param name="Origen">Objeto con la lista de palabras objetivo de comparacion</param>
        /// <param name="Destino">Objeto con la lista de palabras para comparar</param>
        /// <returns></returns>
        private static decimal ComparacionPalabras(string Origen, string Destino)
        {
            decimal resultado = 0;
            if (Origen.Length > Destino.Length)
                resultado = ComparacionPalabrasDiferenteLongitud(Origen, Destino);
            else if (Destino.Length > Origen.Length)
                resultado = ComparacionPalabrasDiferenteLongitud(Destino, Origen);
            else
                resultado = ComparacionPalabrasMismaLongitud(Origen, Destino);
            return resultado;
        }

        public static string AjustarDatosDecimales(string cadena, int longitud)
        {
            int long2 = cadena.Length - longitud;
            return (Left(cadena, long2) + "." + Right(cadena, longitud));
        }
        
        public static decimal Redondeo_dos_decimales(string p)
        {
            try
            {
                decimal resultado = 0;
                if (Convert.ToDecimal(p) >= 0)
                {
                    resultado = Convert.ToDecimal(p);
                    resultado = Math.Round(resultado, 2, MidpointRounding.AwayFromZero);
                    return resultado;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        #region "Manejo de Cadenas"
        public static double StringToDouble(string val, string mensaje)
        {
            double retval;
            if (val == "")
            {
                retval = 0;
            }
            else
            {
                try
                {
                    retval = Double.Parse(val);
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    retval = -1;
                }
                finally
                {
                }
            }
            return retval;
        }
        public static bool IsDecimal(string valor)
        {
            try
            {
                Convert.ToDecimal(valor);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static int StringToInt(string val, string mensaje)
        {
            int retval;
            if (val == "")
            {
                retval = 0;
            }
            else
            {
                try
                {
                    retval = int.Parse(val);
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    retval = 0;
                }
                finally
                {
                }
            }
            return retval;
        }
        public static char AsciiToChar(Byte codAscii)
        {
            Char[] chars;
            Char retval = ' ';
            Byte[] bytes = new Byte[] { codAscii };

            ASCIIEncoding ascii = new ASCIIEncoding();

            int charCount = ascii.GetCharCount(bytes, 1, 1);
            chars = new Char[charCount];
            int charsDecodedCount = ascii.GetChars(bytes, 1, 1, chars, 0);

            foreach (Char c in chars)
            {
                retval = c;
            }
            return retval;
        }

        public static string Left(string cadena, int numCarac)
        {
            string retVal = "";

            retVal = cadena.Substring(0, numCarac);

            return retVal;
        }
        public static string Right(string cadena, int longitud)
        {
            int tam = cadena.Length;
            int cont = tam - longitud;
            if (cont < 0)
                cont = tam - 1;
            string cadena2 = "";
            for (; cont < tam; cont++)
                cadena2 += cadena[cont];
            return cadena2;
        }
        public static string Mid(string cadena, int start, int length)
        {
            if (string.IsNullOrEmpty(cadena) || length < 1 || start > cadena.Length)
                return "";
            // Comprobar que no nos pasamos
            if (length > cadena.Length - start)
            {
                length = cadena.Length - start + 1;
            }
            return cadena.Substring(start - 1, length);
        }
        public static string DATETIME_TO_DATE(DateTime dt)
        {
            string retval = "";
            retval = dt.Year + String.Format("{0:00}", dt.Month) + String.Format("{0:00}", dt.Day);
            return retval;
        }

        public static string DATETIME_TO_TIME(DateTime dt)
        {
            string retval = "";
            retval = String.Format("{0:00}", dt.Hour) + String.Format("{0:00}", dt.Minute) + String.Format("{0:00}", dt.Second);
            return retval;
        }

        public static bool IsNumeric(object value)
        {
            bool result = false;

            try
            {
                int i = Convert.ToInt32(value);
                result = true;
            }
            catch
            {
                // Ignore errors 
            }
            return result;
        }

        public static string f_Nulo_NBSP(string v_valor)
        {
            //-------------------------------------------------------------
            // Devuelve vacio "" en caso de campos con valor "&nbsp;"
            //-------------------------------------------------------------
            if (v_valor == "&nbsp;" || v_valor == "&NBSP;")
            {
                return "";
            }
            else
            {
                return v_valor;
            }
        }
        public static string InvertirCadena(string cadena)
        {
            string cadenainvertida = "";
            for (int cont = cadena.Length - 1; cont >= 0; cont--)
            {
                cadenainvertida += cadena[cont];
            }
            return cadenainvertida;
        }
        public static string EliminarCaracteresEspeciales(string cadena)
        {
            string cadena2 = string.Empty;
            int cont;
            for (cont = 0; cont <= cadena.Length - 1; cont++)
            {
                if (Convert.ToInt32(cadena[cont]) == 209)
                    cadena2 += "N";
                else if (Convert.ToInt32(cadena[cont]) == 241)
                    cadena2 += "n";
                else if (Convert.ToInt32(cadena[cont]) == 193 || Convert.ToInt32(cadena[cont]) == 196)
                    cadena2 += "A";
                else if (Convert.ToInt32(cadena[cont]) == 201 || Convert.ToInt32(cadena[cont]) == 203)
                    cadena2 += "E";
                else if (Convert.ToInt32(cadena[cont]) == 205 || Convert.ToInt32(cadena[cont]) == 207)
                    cadena2 += "I";
                else if (Convert.ToInt32(cadena[cont]) == 211 || Convert.ToInt32(cadena[cont]) == 214)
                    cadena2 += "O";
                else if (Convert.ToInt32(cadena[cont]) == 218 || Convert.ToInt32(cadena[cont]) == 220)
                    cadena2 += "U";
                else if (Convert.ToInt32(cadena[cont]) == 225 || Convert.ToInt32(cadena[cont]) == 228)
                    cadena2 += "a";
                else if (Convert.ToInt32(cadena[cont]) == 233 || Convert.ToInt32(cadena[cont]) == 235)
                    cadena2 += "e";
                else if (Convert.ToInt32(cadena[cont]) == 237 || Convert.ToInt32(cadena[cont]) == 239)
                    cadena2 += "i";
                else if (Convert.ToInt32(cadena[cont]) == 243 || Convert.ToInt32(cadena[cont]) == 246)
                    cadena2 += "o";
                else if (Convert.ToInt32(cadena[cont]) == 250 || Convert.ToInt32(cadena[cont]) == 252)
                    cadena2 += "u";
                else
                    cadena2 += cadena[cont];
            }
            return cadena2;
        }
        public static string EliminarCaracteres(string Cadena, params char[] Caracteres)
        {
            string Cadena2 = String.Empty;
            for (int indice = 0; indice < Cadena.Length; indice++)
            {
                bool Existe = false;
                foreach (char caracter in Caracteres)
                {
                    if (Cadena[indice].CompareTo(caracter) == 0)
                    {
                        Existe = true;
                        break;
                    }
                }
                if (!Existe)
                    Cadena2 += Cadena[indice];
            }
            return Cadena2;
        }
        public static string FormatoMoneda(string cadena)
        {
            string cadena2 = Funciones.InvertirCadena(cadena);
            string cadenaformato = "";
            string punto = string.Empty;
            for (int cont3 = 0; cont3 < cadena2.Length; cont3++)
            {
                if (cadena2[cont3] == '.')
                {
                    cadenaformato = punto + ".";
                    cadena2 = Right(cadena2, cadena2.Length - punto.Length - 1);
                    break;
                }
                else
                    punto += cadena2[cont3];
            }
            for (int cont = 0, cont2 = 0; cont < cadena2.Length; cont++, cont2++)
            {
                if (cont2 == 3)
                {
                    if (cont < cadena2.Length)
                    {
                        if (cadena2[cont] != '-')
                        {
                            cadenaformato += ",";
                            cont2 = 0;
                        }
                    }
                    else
                    {
                        cadenaformato += ",";
                        cont2 = 0;
                    }
                }
                cadenaformato += cadena2[cont];
            }
            return Funciones.InvertirCadena(cadenaformato);
        }
        public static string EliminarCaracteresEspecialesASCII(string cadena, params char[] CaracteresAceptados)
        {
            string cadena2 = string.Empty;
            int cont;
            for (cont = 0; cont <= cadena.Length - 1; cont++)
            {
                if ((Convert.ToInt32(cadena[cont]) >= 48 && Convert.ToInt32(cadena[cont]) <= 57) || (Convert.ToInt32(cadena[cont]) >= 65 && Convert.ToInt32(cadena[cont]) <= 90) || (Convert.ToInt32(cadena[cont]) >= 97 && Convert.ToInt32(cadena[cont]) <= 122))
                    cadena2 += cadena[cont];
                else
                {
                    if (Convert.ToInt32(cadena[cont]) == 209)
                        cadena2 += "N";
                    else if (Convert.ToInt32(cadena[cont]) == 241)
                        cadena2 += "n";
                    else if (Convert.ToInt32(cadena[cont]) == 193 || Convert.ToInt32(cadena[cont]) == 196)
                        cadena2 += "A";
                    else if (Convert.ToInt32(cadena[cont]) == 201 || Convert.ToInt32(cadena[cont]) == 203)
                        cadena2 += "E";
                    else if (Convert.ToInt32(cadena[cont]) == 205 || Convert.ToInt32(cadena[cont]) == 207)
                        cadena2 += "I";
                    else if (Convert.ToInt32(cadena[cont]) == 211 || Convert.ToInt32(cadena[cont]) == 214)
                        cadena2 += "O";
                    else if (Convert.ToInt32(cadena[cont]) == 218 || Convert.ToInt32(cadena[cont]) == 220)
                        cadena2 += "U";
                    else if (Convert.ToInt32(cadena[cont]) == 225 || Convert.ToInt32(cadena[cont]) == 228)
                        cadena2 += "a";
                    else if (Convert.ToInt32(cadena[cont]) == 233 || Convert.ToInt32(cadena[cont]) == 235)
                        cadena2 += "e";
                    else if (Convert.ToInt32(cadena[cont]) == 237 || Convert.ToInt32(cadena[cont]) == 239)
                        cadena2 += "i";
                    else if (Convert.ToInt32(cadena[cont]) == 243 || Convert.ToInt32(cadena[cont]) == 246)
                        cadena2 += "o";
                    else if (Convert.ToInt32(cadena[cont]) == 250 || Convert.ToInt32(cadena[cont]) == 252)
                        cadena2 += "u";
                    else
                    {
                        bool Existe = false;
                        foreach (char caracter in CaracteresAceptados)
                        {
                            if (cadena[cont].CompareTo(caracter) == 0)
                            {
                                Existe = true;
                                break;
                            }
                        }
                        if (Existe)
                            cadena2 += cadena[cont];
                    }
                }
            }
            return cadena2;
        }
        public static bool EsNumeroEntero(string num)
        {
            try
            {
                Convert.ToUInt32(num);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string FiltrarSoloNumeros(string cadena)
        {
            int tam = cadena.Length;
            string cadena2 = "";
            int cont;
            for (cont = 0; cont < tam; cont++)
            {
                if (Convert.ToInt32(cadena[cont]) >= 48 & Convert.ToInt32(cadena[cont]) <= 57)
                {
                    cadena2 = cadena2 + cadena[cont];
                }
            }
            return cadena2;
        }

        #endregion
        #region FormatoCuenta

        public static string FormatoCuenta(string cuenta)
        {
            string DG = Digitocontrol(cuenta);
            string CuentaF = "";
            if (cuenta.Substring(11, 1) == "0")
                CuentaF += cuenta.Substring(5, 3) + Right(cuenta, 7) + cuenta.Substring(4, 1) + DG;
            else
                CuentaF += cuenta.Substring(5, 3) + Right(cuenta, 8) + cuenta.Substring(4, 1) + DG;
            return CuentaF;
        }
        /// <summary>
        /// Metodo que permite, obtener el digito de control de una cuenta
        /// </summary>
        /// <param name="cuenta">Numero de cuenta en formato Comercial</param>
        /// <returns></returns>
        public static string Digitocontrol(string cuenta)
        {
            string c2;
            //Basado en el SP de digto de control
            string suc = cuenta.Substring(5, 3);
            string valor = "8";
            if (suc == "101")
            {
                suc = "000";
                valor = "0";
            }
            //13 caracteres cta cte/ 14 caracteres ahorro
            if (cuenta.Substring(11, 1) == "0")
                c2 = suc + cuenta.Substring(4, 1) + "0" + Right(cuenta, 7) + "0";
            else
                c2 = suc + valor + "0" + Right(cuenta, 8);
            int acu = 0;
            string des;
            while (c2.Length > 0)
            {
                des = Right(c2, 2);
                c2 = c2.Substring(0, c2.Length - des.Length);
                acu = acu + Convert.ToInt32(des);
            }
            string dig = Right(Convert.ToString(acu), 2);
            return dig;
        }
        /// <summary>
        /// Funcion para convertir una cuenta en formato Comercial a Formato RepExt
        /// </summary>         
        /// <param name="cuenta">Cuenta en Formato Comercial</param>
        /// <returns> Cuenta en Formato RepExt</returns>
        public static string FormatoCuentaRepExt(string cuenta)
        {
            cuenta = cuenta.Trim().Replace("-", "");
            string CuentaF = "1030";
            if (cuenta.Length == 14 || cuenta.Length == 12)
                CuentaF += cuenta.Substring(11, 1) + cuenta.Substring(0, 3) + "0001000000" + cuenta.Substring(3, 8);
            else if (cuenta.Length == 13 || cuenta.Length == 11)
                CuentaF += cuenta.Substring(10, 1) + cuenta.Substring(0, 3) + "00000000000" + cuenta.Substring(3, 7);
            return CuentaF;
        }
        /// <summary>
        /// Funcion que convierte de formato RepExt a formato para envio a servicios canales
        /// </summary>
        /// <param name="credit">Credito ALS en formato RepExt</param>
        /// <returns></returns>
        public static string FormatoCreditoSercan(string credito)
        {
            credito = credito.Trim();
            if (credito.Length == 26)
                return credito.Substring(2, 3) + "-" + credito.Substring(5, 3) + "-" + credito.Substring(18, 8);
            else
                throw new Exception("El credito no tiene la longitud de 26 caracteres");
        }
        #endregion
    }
}
