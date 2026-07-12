using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SGISI_Web.Services;

namespace SGISI_Web.Pages;

public class AlertasModel : SgisiPageModel
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
            string consulta = "SELECT id_alerta, origen, timestamp_alerta, tipo, nivel_prioridad, " +
                              "id_activo, id_incidente FROM dbo.ALERTA ORDER BY id_alerta";

            SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
            adaptador.Fill(Tabla);
        }
        catch (Exception ex)
        {
            Error = "Error al consultar alertas: " + ex.Message;
        }

        return Page();
    }
}
