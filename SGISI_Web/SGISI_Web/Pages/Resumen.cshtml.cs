using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SGISI_Web.Services;

namespace SGISI_Web.Pages;

// Ejecuta sp_ResumenSeguridadPorAnalista, el procedimiento que usa un CURSOR
// T-SQL para recorrer analista por analista.
public class ResumenModel : SgisiPageModel
{
    public DataTable Tabla { get; set; } = new();
    public string? Error { get; set; }

    public IActionResult OnGet()
    {
        IActionResult? redir = RedirigirSiNoHayLogin();
        if (redir != null) return redir;

        try
        {
            using SqlConnection conexion = new SqlConnection(CadenaConexion);
            using SqlCommand comando = new SqlCommand("dbo.sp_ResumenSeguridadPorAnalista", conexion)
            {
                CommandType = CommandType.StoredProcedure
            };

            SqlDataAdapter adaptador = new SqlDataAdapter(comando);
            adaptador.Fill(Tabla);
        }
        catch (Exception ex)
        {
            Error = "Error al ejecutar el cursor: " + ex.Message;
        }

        return Page();
    }
}
