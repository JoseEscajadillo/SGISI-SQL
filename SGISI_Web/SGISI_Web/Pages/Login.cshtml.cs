using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SGISI_Web.Services;

namespace SGISI_Web.Pages;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Usuario { get; set; } = "";

    [BindProperty]
    public string Contrasena { get; set; } = "";

    public string? Error { get; set; }

    public void OnGet()
    {
        // Si ya habia sesion, la limpiamos para permitir cambiar de usuario
        HttpContext.Session.CerrarSesion();
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Contrasena))
        {
            Error = "Ingresa usuario y contraseña.";
            return Page();
        }

        string cadena = ConexionBuilder.Construir(Usuario.Trim(), Contrasena);

        try
        {
            using SqlConnection cn = new SqlConnection(cadena);
            cn.Open();

            // Le preguntamos a SQL Server si este login es administrador
            // (db_owner o tiene el permiso UNMASK), igual que en la app de escritorio.
            using SqlCommand cmd = new SqlCommand(
                "SELECT CASE " +
                "  WHEN IS_ROLEMEMBER('db_owner') = 1 THEN 1 " +
                "  WHEN EXISTS (SELECT 1 FROM sys.database_permissions dp " +
                "               JOIN sys.database_principals pr ON dp.grantee_principal_id = pr.principal_id " +
                "               WHERE pr.name = SUSER_SNAME() AND dp.permission_name = 'UNMASK') THEN 1 " +
                "  ELSE 0 END", cn);

            bool esAdministrador = Convert.ToInt32(cmd.ExecuteScalar()) == 1;

            HttpContext.Session.IniciarSesion(cadena, Usuario.Trim(), esAdministrador);

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            Error = "No se pudo iniciar sesión: " + ex.Message;
            return Page();
        }
    }
}
