using Microsoft.Data.SqlClient;

namespace SGISI_App
{
    // Pantalla de inicio de sesion. Se conecta a SGISI con las credenciales SQL
    // REALES que la persona escriba (no hay usuarios "de mentira" en el codigo):
    // el enmascaramiento (DDM) lo resuelve SQL Server segun el login que entro,
    // no la aplicacion.
    public class FormLogin : Form
    {
        private readonly TextBox txtUsuario = new();
        private readonly TextBox txtContrasena = new() { PasswordChar = '*' };
        private readonly Label lblEstado = new()
        {
            ForeColor = Color.Firebrick,
            AutoSize = false,
            MaximumSize = new Size(330, 0)
        };

        public FormLogin()
        {
            Text = "SGISI - Iniciar sesión";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(380, 230);

            Controls.Add(new Label { Text = "Usuario:", Location = new Point(20, 25), AutoSize = true });
            txtUsuario.Location = new Point(130, 21);
            txtUsuario.Size = new Size(230, 25);
            Controls.Add(txtUsuario);

            Controls.Add(new Label { Text = "Contraseña:", Location = new Point(20, 60), AutoSize = true });
            txtContrasena.Location = new Point(130, 56);
            txtContrasena.Size = new Size(230, 25);
            Controls.Add(txtContrasena);

            lblEstado.Location = new Point(20, 92);
            lblEstado.Size = new Size(330, 60);
            Controls.Add(lblEstado);

            Button btnIngresar = new() { Text = "Ingresar", Location = new Point(130, 160), Size = new Size(110, 32) };
            btnIngresar.Click += btnIngresar_Click;
            Controls.Add(btnIngresar);

            Button btnSalir = new() { Text = "Salir", Location = new Point(250, 160), Size = new Size(110, 32) };
            btnSalir.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnSalir);

            AcceptButton = btnIngresar;
        }

        private void btnIngresar_Click(object? sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;

            if (usuario.Length == 0 || contrasena.Length == 0)
            {
                lblEstado.Text = "Ingresa usuario y contraseña.";
                return;
            }

            string cadena = Conexion.ConstruirCadena(usuario, contrasena);

            try
            {
                using SqlConnection cn = new SqlConnection(cadena);
                cn.Open();

                // Le preguntamos a SQL Server si este login es administrador
                // (db_owner o tiene el permiso UNMASK), en vez de adivinarlo en C#.
                using SqlCommand cmd = new SqlCommand(
                    "SELECT CASE " +
                    "  WHEN IS_ROLEMEMBER('db_owner') = 1 THEN 1 " +
                    "  WHEN EXISTS (SELECT 1 FROM sys.database_permissions dp " +
                    "               JOIN sys.database_principals pr ON dp.grantee_principal_id = pr.principal_id " +
                    "               WHERE pr.name = SUSER_SNAME() AND dp.permission_name = 'UNMASK') THEN 1 " +
                    "  ELSE 0 END", cn);

                bool esAdministrador = Convert.ToInt32(cmd.ExecuteScalar()) == 1;

                Conexion.IniciarSesion(usuario, contrasena, esAdministrador);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                lblEstado.Text = "No se pudo iniciar sesión:\n" + ex.Message;
            }
        }
    }
}
