using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SGISI_Web.Services;

namespace SGISI_Web.Pages;

public class BitacoraModel : SgisiPageModel
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
            string consulta = "SELECT id_bitacora, tabla_afectada, operacion, id_registro, " +
                              "id_analista, usuario, fecha, valor_anterior, valor_nuevo " +
                              "FROM dbo.BITACORA ORDER BY fecha DESC";

            SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
            adaptador.Fill(Tabla);
        }
        catch (Exception ex)
        {
            Error = "Error al consultar la bitácora: " + ex.Message;
        }

        return Page();
    }
}
