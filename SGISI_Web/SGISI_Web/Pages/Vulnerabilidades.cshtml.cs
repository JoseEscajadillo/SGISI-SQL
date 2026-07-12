using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SGISI_Web.Services;

namespace SGISI_Web.Pages;

public class VulnerabilidadesModel : SgisiPageModel
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
            string consulta = "SELECT id_vulnerabilidad, cve_id, descripcion, severidad_cvss, " +
                              "fecha_descubrimiento, estado, id_activo FROM dbo.VULNERABILIDAD " +
                              "ORDER BY id_vulnerabilidad";

            SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
            adaptador.Fill(Tabla);
        }
        catch (Exception ex)
        {
            Error = "Error al consultar vulnerabilidades: " + ex.Message;
        }

        return Page();
    }
}
