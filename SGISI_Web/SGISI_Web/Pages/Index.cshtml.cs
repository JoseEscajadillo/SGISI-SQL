using Microsoft.AspNetCore.Mvc;
using SGISI_Web.Services;

namespace SGISI_Web.Pages;

public class IndexModel : SgisiPageModel
{
    public IActionResult OnGet()
    {
        return RedirigirSiNoHayLogin() ?? Page();
    }
}
