using System;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using APOM.Common;
using APOM.Security;
using APOM.Entities;
using APOM.DataAccess.SrvSLK;


namespace APOM.DataAccess
{
    /// <summary>
    /// Clase para armar una cadena de conexion a partir de los datos que se requiere
    /// </summary>    
    public class DataAccess
    {
        /// <summary>
        /// Funcion para armar una cadena de conexion a SQL
        /// </summary>
        /// <param name="Server">Nombre del servidor de base de datos</param>
        /// <param name="BaseDatos">Nombre de la base de datos</param>
        /// <param name="Usuario">Usuario de la basde de datos</param>
        /// <param name="Password">Password del usuario de base de datos</param>
        public static string ConexionSQL(string Server, string BaseDatos, string Usuario, string Password)
        {
            SqlConnectionStringBuilder conguardar = new SqlConnectionStringBuilder("Server= " + Server + "; DataBase=" + BaseDatos + " ; User Id=" + Usuario + ";Password=" + Password + ";"); // [ª0005ª])
            //SqlConnectionStringBuilder conguardar = new SqlConnectionStringBuilder("Server= " + Server + "; DataBase=" + BaseDatos + "; Integrated Security=SSPI "); // [ª0005ª])
            return conguardar.ConnectionString;
        }
        /// <summary>
        /// Funcion para armar una cadena de conexion Sybase, utilizando un DNS
        /// </summary>
        /// <param name="DSN">Nombre del DSN a invocar para la conexion ODBC</param>        
        /// <param name="Usuario">Usuario de la basde de datos</param>
        /// <param name="Password">Password del usuario de base de datos</param>
        public static string ConexionSybase(string DSN, string Usuario, string Password)
        {
            return "DSN=" + DSN + ";UID=" + Usuario + ";PWD=" + Password + ";";
        }

        /// <summary>
        /// Funcion que retorna la cadena de conexion de la base de datos de BD_SAP
        /// </summary>
        /// <returns>Cadena de coneccion BD_SAP</returns>
   
        /// <summary>
        /// Funcion que retorna la cadena de conexion de la base de datos de BD_SAP
        /// </summary>
        /// <returns>Cadena de coneccion BD_SAP</returns>

        public static String Conexion_Repext()
        {
            string Server = ManagerConfig.GetKeyCongigString("REPEXT_SERVER");
            string DataBase = ManagerConfig.GetKeyCongigString("REPEXT_BD");
            string Usuario = ManagerConfig.GetKeyCongigString("REPEXT_USUARIO");
            string Password = ManagerConfig.GetKeyCongigString("REPEXT_PASSWORD");
            Encryptador.EncryptDecrypt(false, Password, ref Password);
            return ConexionSQL(Server, DataBase, Usuario, Password);
        }

        public static String Conexion_Apom()
        {
            string Server = ManagerConfig.GetKeyCongigString("APOM_SERVER");
            string DataBase = ManagerConfig.GetKeyCongigString("APOM_BD");
            string Usuario = ManagerConfig.GetKeyCongigString("APOM_USUARIO");
            string Password = ManagerConfig.GetKeyCongigString("APOM_PASSWORD");
            Encryptador.EncryptDecrypt(false, Password, ref Password);
            return ConexionSQL(Server, DataBase, Usuario, Password);
        }

        public static String Conexion_Smart()
        {
            string Server = ManagerConfig.GetKeyCongigString("SMART_SERVER");
            string DataBase = ManagerConfig.GetKeyCongigString("SMART_BD");
            string Usuario = ManagerConfig.GetKeyCongigString("SMART_USUARIO");
            string Password = ManagerConfig.GetKeyCongigString("SMART_PASSWORD");
            Encryptador.EncryptDecrypt(false, Password, ref Password);
            return ConexionSQL(Server, DataBase, Usuario, Password);
        }



    }

    /// <summary>
    /// Clase que permite ejecutar varios Store Procedures controlando la Transaccionalidad de la operacion
    /// Store Procedures de Insert, Update y Delete
    /// </summary>    
    public class Transaccion
    {
        //Variable Para Alamacenar los Errores
        private string MensajeError = String.Empty;
        //Coleccion de Store Procedures
        private System.Collections.Generic.List<StoreProcedure> Lote = new List<StoreProcedure>();
        /// <summary>
        /// Constructor de la clase
        /// </summary>       
        public Transaccion()
        {
        }
        /// <summary>
        /// Atributo lista de Store Procedures
        /// </summary>    
        public System.Collections.Generic.List<StoreProcedure> Items
        {
            get { return Lote; }
        }
        /// <summary>
        /// Atributo Error, contiene el error en caso de fallar el proceso con la BD
        /// </summary>    
        public string Error
        {
            get { return MensajeError; }
        }

        /// <summary>
        /// Funcion para ejecutar la Transaccion con la base de datos
        /// </summary>}
        /// <param name="CadenaConexion">Cadena de Conexion a la base de datos donde se ejecutara el proceso</param>
        public bool EjecutarTransaccion(string CadenaConexion)
        {
            // Se verifica que se tiene StoreProcedure a ejecutar en la coleccion
            if (Lote.Count > 0)
            {
                SqlConnection conexion = new SqlConnection(CadenaConexion);
                SqlTransaction Transaccion;
                conexion.Open();
                //Se inicia la transaccion
                Transaccion = conexion.BeginTransaction();
                try
                {
                    //Se recorre los StoreProcedure que se tienen
                    for (int cont = 0; cont < Lote.Count; cont++)
                    {
                        SqlDataAdapter Comando = new SqlDataAdapter(Lote[cont].nombreSP, conexion);
                        Comando.SelectCommand.CommandType = CommandType.StoredProcedure;
                        Comando.SelectCommand.CommandTimeout = 600000;
                        //Se añaden los parametros que tiene el Store Procedure
                        for (int cont2 = 0; cont2 < Lote[cont].Items.Count; cont2++)
                        {
                            if (Lote[cont].Items[cont2].ValorParametro == null)
                                Comando.SelectCommand.Parameters.AddWithValue(Lote[cont].Items[cont2].NombreParametro, DBNull.Value);
                            else
                                Comando.SelectCommand.Parameters.AddWithValue(Lote[cont].Items[cont2].NombreParametro, Lote[cont].Items[cont2].ValorParametro);
                        }
                        Comando.SelectCommand.Transaction = Transaccion;
                        Comando.SelectCommand.ExecuteNonQuery();
                    }
                    //Se Hace el commit
                    Transaccion.Commit();
                    //Si no ha habido errores se guarda en la variable MensajeError una cadena vacia
                    MensajeError = String.Empty;
                    //Se retorna True indicando q' se realizo el proceso
                    return true;
                }
                catch (SqlException Error)
                {
                    //En caso de un error se realiza un roll back
                    Transaccion.Rollback();
                    //Se guarda en la variable MensajesError el error q' se produjo
                    MensajeError = Error.Message;
                    //Se retorna falso indicado q' no se ejecuto el proceso
                    return false;
                }
                finally
                {
                    conexion.Close();
                    //Limpieza de los pool de conexion, para depurar cualquier conexion que no se cerro
                    SqlConnection.ClearAllPools();
                }
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Clase que permite ejecutar un Store Procedure tanto para Insert, 
    /// Update, Delete y hacer una consulta de datos
    /// </summary> 
    public class StoreProcedure
    {
        //Variable que contiene el nombre del Store Procedure
        private string NombreSP = String.Empty;

        //Coleccion de Parametros        
        private System.Collections.Generic.List<ParametrosSP> Lista = new List<ParametrosSP>();

        //Variable que contiene el error que se ocaciono
        private string MensajeError = String.Empty;

        /// <summary>
        /// Constructor de la clase
        /// </summary> 
        /// <param name="NombreStoreProcedure">Nombre del Store Procedure</param>
        public StoreProcedure(string NombreStoreProcedure)
        {
            this.NombreSP = NombreStoreProcedure;
        }

        /// <summary>
        /// Metodo para agregar los parametros a enviar al Store Procedure
        /// </summary> 
        /// <param name="NombreParametro">Nombre del Parametro</param>
        /// <param name="ValorParametro">Valor para el parametro</param>
        public void AgregarParametro(string NombreParametro, object ValorParametro, Direccion Valordireccion)
        {
            Lista.Add(new ParametrosSP(NombreParametro, ValorParametro, Valordireccion));
        }

        /// <summary>
        /// Atributo Lista de parametros
        /// </summary>         
        public System.Collections.Generic.List<ParametrosSP> Items
        {
            get { return Lista; }
            set { Lista = value; }
        }

        public ParametrosSP getItem(string NombreParametro)
        {
            foreach (ParametrosSP parametroSP in Lista)
            {
                if (parametroSP.NombreParametro == NombreParametro)
                    return parametroSP;
            }
            return null;
        }

        /// <summary>
        /// Atributo nombre del Store Procedure
        /// </summary>
        public string nombreSP
        {
            get { return NombreSP; }
            set { NombreSP = value; }
        }

        /// <summary>
        /// Atributo Error generado despues de un proceso
        /// </summary>
        public string Error
        {
            get { return MensajeError; }
        }

        /// <summary>
        /// Funcion para ejecutar el Store Procedure de Insert, Update, Delete
        /// </summary>
        /// <param name="CadenaConexion">Cadena de conexion a la base de datos donde se desea ejecutar el Store Procedure</param>
        /// <returns>Indicador booleano del resultado de la ejecucion</returns>
        public Boolean EjecutarStoreProcedure(string CadenaConexion)
        {
            SqlConnection conexion = new SqlConnection(CadenaConexion);
            SqlCommand comando = new SqlCommand(NombreSP, conexion);
            //Se indica que es del tipo StoreProcedure
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandTimeout = 600000;
            //Se declaran los parametros en el SQLCommand
            for (int cont = 0; cont < Lista.Count; cont++)
            {
                if (Lista[cont].ValorParametro == null)
                    comando.Parameters.AddWithValue(Lista[cont].NombreParametro, DBNull.Value);
                else
                {
                    SqlParameter parameter = new SqlParameter(Lista[cont].NombreParametro, Lista[cont].ValorParametro);
                    switch (Lista[cont].Direccion)
                    {
                        case Direccion.Input:
                            parameter.Direction = ParameterDirection.Input;
                            break;
                        case Direccion.Ouput:
                            parameter.Direction = ParameterDirection.Output;
                            break;
                        case Direccion.InputOuput:
                            parameter.Direction = ParameterDirection.InputOutput;
                            break;
                        default:

                            break;
                    }
                    comando.Parameters.Add(parameter);
                }

            }
            try
            {
                //Se habre la conexion
                conexion.Open();
                //se ejecuta el query
                comando.ExecuteNonQuery();
                //Se cierra la conexion
                conexion.Close();
                //Si no ha habido errores se guarda en la variable MensajeError una cadena vacia
                MensajeError = String.Empty;
                //Limpieza de los pool de conexion, para depurar cualquier conexion que no se cerro
                SqlConnection.ClearAllPools();

                for (int i = 0; i < Lista.Count; i++)
                {
                    if (Lista[i].Direccion == Direccion.Ouput)
                        Lista[i].ValorParametro = comando.Parameters[Lista[i].NombreParametro].Value;
                }

                //Se Retorna true indicando que se ejecuto el proceso                
                return true;
            }
            catch (SqlException Error)
            {
                //Se guarda en la variable MensajeError el error q' se produjo
                MensajeError = Error.Message;
                //Se cierra la conexion
                conexion.Close();
                SqlConnection.ClearAllPools();
                //Se Retorna falso en caso de que no se haya ejecutado el proceso
                return false;
            }
        }

        /// <summary>
        /// Funcion para ejecutar un Store Procedure que devuelva un resultado
        /// </summary>
        /// <param name="CadenaConexion">Cadena de conexion a la base de datos donde se desea ejecutar el Store Procedure</param>
        /// <returns>Datatable no tipado</returns>
        public DataTable RealizarConsulta(string CadenaConexion)
        {
            DataTable Consulta = new DataTable();
            if (CadenaConexion.Length > 0)
            {
                SqlConnection conexion = new SqlConnection(CadenaConexion);
                SqlDataAdapter Comando = new SqlDataAdapter(NombreSP, conexion);
                Comando.SelectCommand.CommandType = CommandType.StoredProcedure;
                Comando.SelectCommand.CommandTimeout = 600000;
                for (int cont = 0; cont < Lista.Count; cont++)
                {
                    if (Lista[cont].ValorParametro == null)
                        Comando.SelectCommand.Parameters.AddWithValue(Lista[cont].NombreParametro, DBNull.Value);
                    else
                        Comando.SelectCommand.Parameters.AddWithValue(Lista[cont].NombreParametro, Lista[cont].ValorParametro);
                }
                try
                {
                    conexion.Open();
                    Comando.Fill(Consulta);
                    MensajeError = String.Empty;
                }
                catch (SqlException Error)
                {
                    MensajeError = Error.Message;
                }
                finally
                {
                    conexion.Close();
                    //Limpieza de los pool de conexion, para depurar cualquier conexion que no se cerro
                    SqlConnection.ClearAllPools();
                }
            }
            else
                MensajeError = "No se recibio la cadena de conexion";
            return Consulta;
        }

        /// <summary>
        /// Funcion para ejecutar una consulta con conexion de tipo ODBC
        /// </summary>
        /// <param name="CadenaConexion">Cadena de conexion a la base de datos donde se desea ejecutar el Store Procedure</param>
        /// <returns>Datatable no tipado/returns>
        public DataTable RealizarConsultaODBC(string CadenaConexion)
        {
            DataTable Consulta = new DataTable();
            OdbcConnection conexion = new OdbcConnection(CadenaConexion);
            OdbcDataAdapter Comando = new OdbcDataAdapter(NombreSP, conexion);
            Comando.SelectCommand.CommandTimeout = 600000;
            for (int cont = 0; cont < Lista.Count; cont++)
                Comando.SelectCommand.Parameters.AddWithValue(Lista[cont].NombreParametro, Lista[cont].ValorParametro);
            try
            {
                conexion.Open();
                Comando.Fill(Consulta);
                MensajeError = String.Empty;
            }
            catch (SqlException Error)
            {
                MensajeError = Error.Message;
            }
            finally
            {
                conexion.Close();
            }
            return Consulta;
        }
    }

    /// <summary>
    /// Clase de tipo Parametros de un SP
    /// </summary>    
    public class ParametrosSP
    {
        //Variable con el nombre del parametro
        private string Nombre = String.Empty;

        //Variable con el valor del parametro        
        private object Valor = null;

        public Direccion Direccion { get; set; }

        /// <summary>
        /// Constructor de la clase
        /// </summary> 
        /// <param name="NombreParametro">Nombre del Parametro del SP</param>
        /// <param name="ValorParametro">Valor a enviarse en el parametro</param>
        public ParametrosSP(string NombreParametro, object ValorParametro, Direccion DireccionParametro)
        {
            this.Nombre = NombreParametro;
            this.Valor = ValorParametro;
            this.Direccion = DireccionParametro;
        }

        //Funcion para Accesar al Nombre
        /// <summary>
        /// Atributo NombreParametro
        /// </summary> 
        public string NombreParametro
        {
            get { return Nombre; }
            set { Nombre = value; }
        }

        //Funcion para Accesar al Valor
        /// <summary>
        /// Atributo ValorParametro
        /// </summary> 
        public object ValorParametro
        {
            get { return Valor; }
            set { Valor = value; }
        }
    }

    [Flags]
    public enum Direccion
    {
        Input,
        Ouput,
        InputOuput,
    }

}
