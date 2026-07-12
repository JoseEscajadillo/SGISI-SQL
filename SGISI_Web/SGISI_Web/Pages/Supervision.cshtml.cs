using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SGISI_Web.Services;

namespace SGISI_Web.Pages;

// Combina SUPERVISOR y SUPERVISADO (especializacion de ANALISTA) en una sola vista,
// igual que el boton "Supervision" de la app de escritorio.
public class SupervisionModel : SgisiPageModel
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
            string consulta =
                "SELECT a.id_analista, a.nombre, a.apellido, " +
                "CASE WHEN s.id_analista IS NOT NULL THEN 'Supervisor' " +
                "     WHEN sd.id_analista IS NOT NULL THEN 'Supervisado' " +
                "     ELSE 'Sin especializacion' END AS tipo, " +
                "s.num_analistas_cargo, sd.nivel_experiencia, sd.area_operacion " +
                "FROM dbo.ANALISTA a " +
                "LEFT JOIN dbo.SUPERVISOR s ON a.id_analista = s.id_analista " +
                "LEFT JOIN dbo.SUPERVISADO sd ON a.id_analista = sd.id_analista " +
                "ORDER BY a.id_analista";

            SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
            adaptador.Fill(Tabla);
        }
        catch (Exception ex)
        {
            Error = "Error al consultar la supervisión: " + ex.Message;
        }

        return Page();
    }
}
