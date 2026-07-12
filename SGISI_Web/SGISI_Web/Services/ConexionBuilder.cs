using Microsoft.Data.SqlClient;

namespace SGISI_Web.Services;

// Construye la cadena de conexion a SGISI para un usuario/contrasena especifico.
// Es sin estado (stateless) a proposito: en una app web varias personas pueden
// usarla al mismo tiempo, asi que NO se puede guardar "la conexion activa" en
// una variable compartida como en la app de escritorio. Cada request arma la
// suya y la guarda en su propia sesion de navegador (ver SesionExtensions).
public static class ConexionBuilder
{
    private const string Servidor = @"localhost\SQLEXPRESS";
    private const string BaseDatos = "SGISI";

    public static string Construir(string usuario, string contrasena)
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
}
