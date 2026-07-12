using Microsoft.Data.SqlClient;

namespace SGISI_App
{
    // Mantiene la conexion ACTIVA de la sesion actual. Se define al iniciar sesion
    // (FormLogin) y de ahi en adelante todos los formularios la reutilizan.
    public static class Conexion
    {
        private const string Servidor = @"localhost\SQLEXPRESS";
        private const string BaseDatos = "SGISI";

        public static string CadenaConexion { get; private set; } = "";
        public static string NombreUsuario { get; private set; } = "";
        public static bool EsAdministrador { get; private set; }

        // Arma la cadena de conexion para un usuario/contrasena especifico
        // (se usa primero para probar el login, y luego queda como la activa).
        public static string ConstruirCadena(string usuario, string contrasena)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = Servidor,
                InitialCatalog = BaseDatos,
                UserID = usuario,
                Password = contrasena,
                TrustServerCertificate = true
            };
            return builder.ConnectionString;
        }

        // Se llama una sola vez, tras validar el login exitosamente en FormLogin.
        public static void IniciarSesion(string usuario, string contrasena, bool esAdministrador)
        {
            CadenaConexion = ConstruirCadena(usuario, contrasena);
            NombreUsuario = usuario;
            EsAdministrador = esAdministrador;
        }

        public static SqlConnection Crear() => new SqlConnection(CadenaConexion);
    }

    // Item simple para llenar ComboBox (guarda el Id y muestra un texto)
    public class ItemCombo
    {
        public int Id { get; set; }
        public string Texto { get; set; } = "";
        public override string ToString() => Texto;
    }
}
