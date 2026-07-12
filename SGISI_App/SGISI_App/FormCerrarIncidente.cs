using System.Data;
using Microsoft.Data.SqlClient;

namespace SGISI_App
{
    // Formulario para cerrar un incidente (sp_CerrarIncidente): cambia el estado a
    // CERRADO y registra un reporte. Genera un UPDATE -> queda rastro en la BITACORA.
    public class FormCerrarIncidente : Form
    {
        private readonly ComboBox cmbIncidente = new();
        private readonly TextBox txtConclusiones = new();
        private readonly TextBox txtRecomendaciones = new();
        private readonly ComboBox cmbAnalista = new();

        public FormCerrarIncidente()
        {
            Text = "Cerrar Incidente";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(470, 360);

            Controls.Add(new Label { Text = "Incidente:", Location = new Point(20, 22), AutoSize = true });
            cmbIncidente.Location = new Point(170, 18);
            cmbIncidente.Size = new Size(280, 25);
            cmbIncidente.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(cmbIncidente);

            Controls.Add(new Label { Text = "Conclusiones:", Location = new Point(20, 57), AutoSize = true });
            txtConclusiones.Location = new Point(170, 53);
            txtConclusiones.Size = new Size(280, 80);
            txtConclusiones.Multiline = true;
            txtConclusiones.ScrollBars = ScrollBars.Vertical;
            Controls.Add(txtConclusiones);

            Controls.Add(new Label { Text = "Recomendaciones:", Location = new Point(20, 147), AutoSize = true });
            txtRecomendaciones.Location = new Point(170, 143);
            txtRecomendaciones.Size = new Size(280, 80);
            txtRecomendaciones.Multiline = true;
            txtRecomendaciones.ScrollBars = ScrollBars.Vertical;
            Controls.Add(txtRecomendaciones);

            Controls.Add(new Label { Text = "Analista:", Location = new Point(20, 242), AutoSize = true });
            cmbAnalista.Location = new Point(170, 238);
            cmbAnalista.Size = new Size(280, 25);
            cmbAnalista.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(cmbAnalista);

            Button btnCerrar = new() { Text = "Cerrar incidente", Location = new Point(170, 290), Size = new Size(140, 32) };
            btnCerrar.Click += btnCerrar_Click;
            Controls.Add(btnCerrar);

            Button btnCancelar = new() { Text = "Cancelar", Location = new Point(330, 290), Size = new Size(120, 32) };
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancelar);

            Load += (s, e) => { CargarIncidentesAbiertos(); CargarAnalistas(); };
        }

        private void CargarIncidentesAbiertos()
        {
            try
            {
                using SqlConnection cn = Conexion.Crear();
                using SqlCommand cmd = new SqlCommand(
                    "SELECT id_incidente, tipo_ataque, estado FROM dbo.INCIDENTE " +
                    "WHERE estado <> 'CERRADO' ORDER BY id_incidente", cn);
                cn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    int id = r.GetInt32(0);
                    cmbIncidente.Items.Add(new ItemCombo
                    {
                        Id = id,
                        Texto = $"{id} - {r.GetString(1)} ({r.GetString(2)})"
                    });
                }
                if (cmbIncidente.Items.Count > 0) cmbIncidente.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar incidentes:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarAnalistas()
        {
            try
            {
                using SqlConnection cn = Conexion.Crear();
                using SqlCommand cmd = new SqlCommand(
                    "SELECT id_analista, nombre, apellido FROM dbo.ANALISTA ORDER BY id_analista", cn);
                cn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    int id = r.GetInt32(0);
                    cmbAnalista.Items.Add(new ItemCombo
                    {
                        Id = id,
                        Texto = $"{id} - {r.GetString(1)} {r.GetString(2)}"
                    });
                }
                if (cmbAnalista.Items.Count > 0) cmbAnalista.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar analistas:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object? sender, EventArgs e)
        {
            if (cmbIncidente.SelectedItem is not ItemCombo incidente)
            {
                MessageBox.Show("Selecciona un incidente.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbAnalista.SelectedItem is not ItemCombo analista)
            {
                MessageBox.Show("Selecciona un analista.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using SqlConnection cn = Conexion.Crear();
                using SqlCommand cmd = new SqlCommand("dbo.sp_CerrarIncidente", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@id_incidente", incidente.Id);
                cmd.Parameters.AddWithValue("@conclusiones", txtConclusiones.Text.Trim());
                cmd.Parameters.AddWithValue("@recomendaciones", txtRecomendaciones.Text.Trim());
                cmd.Parameters.AddWithValue("@id_analista", analista.Id);

                cn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show($"Incidente {incidente.Id} cerrado y reporte generado.",
                    "Cerrar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cerrar el incidente:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
