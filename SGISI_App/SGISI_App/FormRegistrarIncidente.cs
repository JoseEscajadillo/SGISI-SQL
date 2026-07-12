using System.Data;
using Microsoft.Data.SqlClient;

namespace SGISI_App
{
    // Formulario de captura para registrar un incidente nuevo (sp_RegistrarIncidente).
    // Construido por código para mantenerlo autocontenido.
    public class FormRegistrarIncidente : Form
    {
        private readonly TextBox txtTipo = new();
        private readonly TextBox txtDescripcion = new();
        private readonly NumericUpDown numSeveridad = new();
        private readonly ComboBox cmbAnalista = new();

        public FormRegistrarIncidente()
        {
            Text = "Registrar Incidente";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(460, 320);

            Controls.Add(new Label { Text = "Tipo de ataque:", Location = new Point(20, 22), AutoSize = true });
            txtTipo.Location = new Point(170, 18);
            txtTipo.Size = new Size(260, 25);
            Controls.Add(txtTipo);

            Controls.Add(new Label { Text = "Descripción:", Location = new Point(20, 57), AutoSize = true });
            txtDescripcion.Location = new Point(170, 53);
            txtDescripcion.Size = new Size(260, 80);
            txtDescripcion.Multiline = true;
            txtDescripcion.ScrollBars = ScrollBars.Vertical;
            Controls.Add(txtDescripcion);

            Controls.Add(new Label { Text = "Severidad (0-10):", Location = new Point(20, 152), AutoSize = true });
            numSeveridad.Location = new Point(170, 148);
            numSeveridad.Size = new Size(80, 25);
            numSeveridad.DecimalPlaces = 1;
            numSeveridad.Minimum = 0;
            numSeveridad.Maximum = 10;
            numSeveridad.Increment = 0.1M;
            numSeveridad.Value = 5.0M;
            Controls.Add(numSeveridad);

            Controls.Add(new Label { Text = "Analista:", Location = new Point(20, 192), AutoSize = true });
            cmbAnalista.Location = new Point(170, 188);
            cmbAnalista.Size = new Size(260, 25);
            cmbAnalista.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(cmbAnalista);

            Button btnGuardar = new() { Text = "Registrar", Location = new Point(170, 245), Size = new Size(120, 32) };
            btnGuardar.Click += btnGuardar_Click;
            Controls.Add(btnGuardar);

            Button btnCancelar = new() { Text = "Cancelar", Location = new Point(310, 245), Size = new Size(120, 32) };
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancelar);

            Load += (s, e) => CargarAnalistas();
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

        private void btnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTipo.Text))
            {
                MessageBox.Show("Ingresa el tipo de ataque.", "Validación",
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
                using SqlCommand cmd = new SqlCommand("dbo.sp_RegistrarIncidente", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@tipo_ataque", txtTipo.Text.Trim());
                cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());
                cmd.Parameters.AddWithValue("@severidad", numSeveridad.Value);
                cmd.Parameters.AddWithValue("@id_analista", analista.Id);

                cn.Open();
                object? resultado = cmd.ExecuteScalar();   // sp devuelve el nuevo id
                int nuevoId = Convert.ToInt32(resultado);

                MessageBox.Show($"Incidente registrado con éxito. Nuevo ID: {nuevoId}",
                    "Registrar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el incidente:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
