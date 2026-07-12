using System.Data;
using Microsoft.Data.SqlClient; 

namespace SGISI_App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string rol = Conexion.EsAdministrador
                ? "Administrador (ve datos reales)"
                : "Cliente (solo consulta, datos enmascarados)";
            lblSesion.Text = $"Sesión: {Conexion.NombreUsuario} — {rol}";
            lblSesion.ForeColor = Conexion.EsAdministrador ? Color.DarkGreen : Color.SaddleBrown;

            if (!Conexion.EsAdministrador)
            {
                btnRegistrar.Visible = false;
                btnCerrar.Visible = false;
            }
        }

        private void btnConectar_Click(object? sender, EventArgs e)
        {
            try
            {
                using SqlConnection conexion = Conexion.Crear();
                conexion.Open();
                MessageBox.Show("Conexion exitosa a SGISI.",
                    "Conexion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexion:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Metodo reutilizable: ejecuta una consulta y la vuelca en el DataGridView.
        // Lo usan los botones de las entidades que solo se listan (sin logica adicional).
        private void MostrarTabla(string consulta)
        {
            try
            {
                using SqlConnection conexion = Conexion.Crear();
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dgvDatos.DataSource = tabla;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMostrar_Click(object? sender, EventArgs e)
        {
            try
            {
                using SqlConnection conexion = Conexion.Crear();
                string consulta = "SELECT id_incidente, tipo_ataque, fecha_hora, " +
                                  "estado, severidad, id_analista FROM dbo.INCIDENTE";

                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);      

                dgvDatos.DataSource = tabla;    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnResumen_Click(object? sender, EventArgs e)
        {
            try
            {
                using SqlConnection conexion = Conexion.Crear();
                using SqlCommand comando = new SqlCommand("dbo.sp_ResumenSeguridadPorAnalista", conexion);
                comando.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                dgvDatos.DataSource = tabla;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar el cursor:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBitacora_Click(object? sender, EventArgs e)
        {
            try
            {
                using SqlConnection conexion = Conexion.Crear();
                string consulta = "SELECT id_bitacora, tabla_afectada, operacion, id_registro, " +
                                  "id_analista, usuario, fecha, valor_anterior, valor_nuevo " +
                                  "FROM dbo.BITACORA ORDER BY fecha DESC";

                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                dgvDatos.DataSource = tabla;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar la bitacora:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegistrar_Click(object? sender, EventArgs e)
        {
            using FormRegistrarIncidente dlg = new FormRegistrarIncidente();
            if (dlg.ShowDialog(this) == DialogResult.OK)
                btnBitacora_Click(sender, e);  
        }

        private void btnCerrar_Click(object? sender, EventArgs e)
        {
            using FormCerrarIncidente dlg = new FormCerrarIncidente();
            if (dlg.ShowDialog(this) == DialogResult.OK)
                btnBitacora_Click(sender, e);
        }

        private void btnVerAnalistas_Click(object? sender, EventArgs e)
        {
            try
            {
                using SqlConnection conexion = Conexion.Crear();
                string consulta = "SELECT id_analista, nombre, apellido, cargo, especializacion, " +
                                  "nivel_acceso, email, telefono, id_organizacion FROM dbo.ANALISTA " +
                                  "ORDER BY id_analista";

                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                dgvDatos.DataSource = tabla;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar analistas:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAuditoria_Click(object? sender, EventArgs e)
        {
            try
            {
                using SqlConnection conexion = Conexion.Crear();
                string consulta =
                    "SELECT event_time, action_id, succeeded, server_principal_name, host_name " +
                    "FROM sys.fn_get_audit_file('D:\\SQL_AUDILOG_SGISI\\*', DEFAULT, DEFAULT) " +
                    "ORDER BY event_time DESC";

                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                dgvDatos.DataSource = tabla;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar la auditoria de accesos:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Los siguientes botones completan la cobertura de las 10 entidades del modelo:
        // muestran las tablas que hasta ahora no se consultaban desde la app.
        private void btnOrganizaciones_Click(object? sender, EventArgs e)
        {
            MostrarTabla("SELECT id_organizacion, nombre_departamento, responsable, num_activos " +
                         "FROM dbo.ORGANIZACION ORDER BY id_organizacion");
        }

        // direccion_ip y sistema_operativo estan enmascaradas con default(): exponer la huella
        // tecnica exacta de un activo (IP y SO) a quien no la necesita facilita el reconocimiento
        // previo a un ataque.
        private void btnActivos_Click(object? sender, EventArgs e)
        {
            MostrarTabla("SELECT id_activo, nombre, direccion_ip, sistema_operativo, criticidad, id_organizacion " +
                         "FROM dbo.ACTIVO ORDER BY id_activo");
        }

        // cve_id y descripcion estan enmascaradas con default(): describen exactamente como es
        // vulnerable un activo, incluso mientras sigue sin parchear.
        private void btnVulnerabilidades_Click(object? sender, EventArgs e)
        {
            MostrarTabla("SELECT id_vulnerabilidad, cve_id, descripcion, severidad_cvss, " +
                         "fecha_descubrimiento, estado, id_activo FROM dbo.VULNERABILIDAD " +
                         "ORDER BY id_vulnerabilidad");
        }

        private void btnAlertas_Click(object? sender, EventArgs e)
        {
            MostrarTabla("SELECT id_alerta, origen, timestamp_alerta, tipo, nivel_prioridad, " +
                         "id_activo, id_incidente FROM dbo.ALERTA ORDER BY id_alerta");
        }

        private void btnIncidenteActivo_Click(object? sender, EventArgs e)
        {
            MostrarTabla("SELECT id_incidente, id_activo, impacto, fecha_deteccion " +
                         "FROM dbo.INCIDENTE_ACTIVO ORDER BY id_incidente, id_activo");
        }

        // Combina SUPERVISOR y SUPERVISADO (especializacion de ANALISTA) en una sola vista,
        // mostrando el tipo de cada analista y los atributos propios de su especializacion.
        // num_analistas_cargo, nivel_experiencia y area_operacion estan enmascaradas con
        // default() por el mismo criterio que nivel_acceso: jerarquia interna del personal.
        private void btnSupervision_Click(object? sender, EventArgs e)
        {
            MostrarTabla(
                "SELECT a.id_analista, a.nombre, a.apellido, " +
                "CASE WHEN s.id_analista IS NOT NULL THEN 'Supervisor' " +
                "     WHEN sd.id_analista IS NOT NULL THEN 'Supervisado' " +
                "     ELSE 'Sin especializacion' END AS tipo, " +
                "s.num_analistas_cargo, sd.nivel_experiencia, sd.area_operacion " +
                "FROM dbo.ANALISTA a " +
                "LEFT JOIN dbo.SUPERVISOR s ON a.id_analista = s.id_analista " +
                "LEFT JOIN dbo.SUPERVISADO sd ON a.id_analista = sd.id_analista " +
                "ORDER BY a.id_analista");
        }

        private void btnReportes_Click(object? sender, EventArgs e)
        {
            MostrarTabla("SELECT id_reporte, fecha_generacion, conclusiones, recomendaciones, " +
                         "id_analista, id_incidente FROM dbo.REPORTE ORDER BY id_reporte");
        }
    }
}
