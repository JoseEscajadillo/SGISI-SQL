namespace SGISI_Web.Services;

// Guarda los datos de la sesion (cadena de conexion, usuario, rol) en la
// SESION DE NAVEGADOR de cada visitante (ASP.NET Core Session), no en una
// variable compartida del servidor. Asi, si dos personas entran a la vez
// con usuarios distintos, cada una ve solo lo suyo.
public static class SesionExtensions
{
    private const string ClaveCadena = "CadenaConexion";
    private const string ClaveUsuario = "NombreUsuario";
    private const string ClaveAdmin = "EsAdministrador";

    public static void IniciarSesion(this ISession session, string cadena, string usuario, bool esAdmin)
    {
        session.SetString(ClaveCadena, cadena);
        session.SetString(ClaveUsuario, usuario);
        session.SetString(ClaveAdmin, esAdmin ? "1" : "0");
    }

    public static void CerrarSesion(this ISession session) => session.Clear();

    public static string ObtenerCadenaConexion(this ISession session) => session.GetString(ClaveCadena) ?? "";
    public static string ObtenerNombreUsuario(this ISession session) => session.GetString(ClaveUsuario) ?? "";
    public static bool ObtenerEsAdministrador(this ISession session) => session.GetString(ClaveAdmin) == "1";
    public static bool EstaLogueado(this ISession session) => !string.IsNullOrEmpty(session.GetString(ClaveCadena));
}
