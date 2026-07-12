using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGISI_Web.Services;

namespace SGISI_Web.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        HttpContext.Session.CerrarSesion();
        return RedirectToPage("/Login");
    }
}
