using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SGISI_Web.Services;

// Clase base para las paginas que necesitan sesion iniciada.
// Cada pagina protegida hereda de esta y llama a RedirigirSiNoHayLogin()
// al inicio de su OnGet.
public abstract class SgisiPageModel : PageModel
{
    protected string CadenaConexion => HttpContext.Session.ObtenerCadenaConexion();
    protected string NombreUsuario => HttpContext.Session.ObtenerNombreUsuario();
    protected bool EsAdministrador => HttpContext.Session.ObtenerEsAdministrador();

    protected IActionResult? RedirigirSiNoHayLogin()
    {
        if (!HttpContext.Session.EstaLogueado())
            return RedirectToPage("/Login");
        return null;
    }
}
