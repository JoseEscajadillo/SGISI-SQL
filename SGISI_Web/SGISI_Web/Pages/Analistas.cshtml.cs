using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SGISI_Web.Services;

namespace SGISI_Web.Pages;

// Muestra la tabla ANALISTA. El email, telefono y nivel_acceso se ven
// enmascarados o reales segun los permisos del login de la sesion actual
// (Enmascaramiento Dinamico de Datos) - lo decide SQL Server, no este codigo.
public class AnalistasModel : SgisiPageModel
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
            string consulta = "SELECT id_analista, nombre, apellido, cargo, especializacion, " +
                              "nivel_acceso, email, telefono, id_organizacion FROM dbo.ANALISTA " +
                              "ORDER BY id_analista";

            SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
            adaptador.Fill(Tabla);
        }
        catch (Exception ex)
        {
            Error = "Error al consultar analistas: " + ex.Message;
        }

        return Page();
    }
}
