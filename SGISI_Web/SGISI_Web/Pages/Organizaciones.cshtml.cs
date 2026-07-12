using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SGISI_Web.Services;

namespace SGISI_Web.Pages;

public class OrganizacionesModel : SgisiPageModel
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
            string consulta = "SELECT id_organizacion, nombre_departamento, responsable, num_activos " +
                              "FROM dbo.ORGANIZACION ORDER BY id_organizacion";

            SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
            adaptador.Fill(Tabla);
        }
        catch (Exception ex)
        {
            Error = "Error al consultar organizaciones: " + ex.Message;
        }

        return Page();
    }
}
