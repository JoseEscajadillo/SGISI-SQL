namespace SGISI_App
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnConectar = new Button();
            btnMostrar = new Button();
            btnResumen = new Button();
            btnBitacora = new Button();
            btnRegistrar = new Button();
            btnCerrar = new Button();
            btnVerAnalistas = new Button();
            btnAuditoria = new Button();
            lblSesion = new Label();
            btnOrganizaciones = new Button();
            btnActivos = new Button();
            btnVulnerabilidades = new Button();
            btnAlertas = new Button();
            btnIncidenteActivo = new Button();
            btnSupervision = new Button();
            btnReportes = new Button();
            dgvDatos = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvDatos).BeginInit();
            SuspendLayout();
            //
            // btnConectar
            //
            btnConectar.Location = new Point(12, 12);
            btnConectar.Name = "btnConectar";
            btnConectar.Size = new Size(100, 32);
            btnConectar.TabIndex = 0;
            btnConectar.Text = "Conectar";
            btnConectar.UseVisualStyleBackColor = true;
            btnConectar.Click += btnConectar_Click;
            //
            // btnMostrar
            //
            btnMostrar.Location = new Point(118, 12);
            btnMostrar.Name = "btnMostrar";
            btnMostrar.Size = new Size(100, 32);
            btnMostrar.TabIndex = 1;
            btnMostrar.Text = "Mostrar";
            btnMostrar.UseVisualStyleBackColor = true;
            btnMostrar.Click += btnMostrar_Click;
            //
            // btnResumen
            //
            btnResumen.Location = new Point(224, 12);
            btnResumen.Name = "btnResumen";
            btnResumen.Size = new Size(140, 32);
            btnResumen.TabIndex = 2;
            btnResumen.Text = "Resumen (Cursor)";
            btnResumen.UseVisualStyleBackColor = true;
            btnResumen.Click += btnResumen_Click;
            //
            // btnBitacora
            //
            btnBitacora.Location = new Point(370, 12);
            btnBitacora.Name = "btnBitacora";
            btnBitacora.Size = new Size(100, 32);
            btnBitacora.TabIndex = 3;
            btnBitacora.Text = "Bitácora";
            btnBitacora.UseVisualStyleBackColor = true;
            btnBitacora.Click += btnBitacora_Click;
            //
            // btnRegistrar
            //
            btnRegistrar.Location = new Point(476, 12);
            btnRegistrar.Name = "btnRegistrar";
            btnRegistrar.Size = new Size(150, 32);
            btnRegistrar.TabIndex = 4;
            btnRegistrar.Text = "Registrar Incidente";
            btnRegistrar.UseVisualStyleBackColor = true;
            btnRegistrar.Click += btnRegistrar_Click;
            //
            // btnCerrar
            //
            btnCerrar.Location = new Point(632, 12);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(150, 32);
            btnCerrar.TabIndex = 5;
            btnCerrar.Text = "Cerrar Incidente";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Click += btnCerrar_Click;
            //
            // btnVerAnalistas
            //
            btnVerAnalistas.Location = new Point(12, 50);
            btnVerAnalistas.Name = "btnVerAnalistas";
            btnVerAnalistas.Size = new Size(150, 32);
            btnVerAnalistas.TabIndex = 6;
            btnVerAnalistas.Text = "Ver Analistas (DDM)";
            btnVerAnalistas.UseVisualStyleBackColor = true;
            btnVerAnalistas.Click += btnVerAnalistas_Click;
            //
            // btnAuditoria
            //
            btnAuditoria.Location = new Point(168, 50);
            btnAuditoria.Name = "btnAuditoria";
            btnAuditoria.Size = new Size(180, 32);
            btnAuditoria.TabIndex = 7;
            btnAuditoria.Text = "Auditoría de Accesos";
            btnAuditoria.UseVisualStyleBackColor = true;
            btnAuditoria.Click += btnAuditoria_Click;
            //
            // lblSesion
            //
            lblSesion.Location = new Point(370, 58);
            lblSesion.Name = "lblSesion";
            lblSesion.Size = new Size(600, 20);
            lblSesion.Font = new Font(Font, FontStyle.Bold);
            lblSesion.TabIndex = 9;
            lblSesion.Text = "Sesión: -";
            //
            // btnOrganizaciones
            //
            btnOrganizaciones.Location = new Point(12, 88);
            btnOrganizaciones.Name = "btnOrganizaciones";
            btnOrganizaciones.Size = new Size(130, 32);
            btnOrganizaciones.TabIndex = 10;
            btnOrganizaciones.Text = "Organizaciones";
            btnOrganizaciones.UseVisualStyleBackColor = true;
            btnOrganizaciones.Click += btnOrganizaciones_Click;
            //
            // btnActivos
            //
            btnActivos.Location = new Point(148, 88);
            btnActivos.Name = "btnActivos";
            btnActivos.Size = new Size(140, 32);
            btnActivos.TabIndex = 11;
            btnActivos.Text = "Activos (DDM)";
            btnActivos.UseVisualStyleBackColor = true;
            btnActivos.Click += btnActivos_Click;
            //
            // btnVulnerabilidades
            //
            btnVulnerabilidades.Location = new Point(294, 88);
            btnVulnerabilidades.Name = "btnVulnerabilidades";
            btnVulnerabilidades.Size = new Size(190, 32);
            btnVulnerabilidades.TabIndex = 12;
            btnVulnerabilidades.Text = "Vulnerabilidades (DDM)";
            btnVulnerabilidades.UseVisualStyleBackColor = true;
            btnVulnerabilidades.Click += btnVulnerabilidades_Click;
            //
            // btnAlertas
            //
            btnAlertas.Location = new Point(490, 88);
            btnAlertas.Name = "btnAlertas";
            btnAlertas.Size = new Size(90, 32);
            btnAlertas.TabIndex = 13;
            btnAlertas.Text = "Alertas";
            btnAlertas.UseVisualStyleBackColor = true;
            btnAlertas.Click += btnAlertas_Click;
            //
            // btnIncidenteActivo
            //
            btnIncidenteActivo.Location = new Point(12, 126);
            btnIncidenteActivo.Name = "btnIncidenteActivo";
            btnIncidenteActivo.Size = new Size(150, 32);
            btnIncidenteActivo.TabIndex = 14;
            btnIncidenteActivo.Text = "Incidente-Activo";
            btnIncidenteActivo.UseVisualStyleBackColor = true;
            btnIncidenteActivo.Click += btnIncidenteActivo_Click;
            //
            // btnSupervision
            //
            btnSupervision.Location = new Point(168, 126);
            btnSupervision.Name = "btnSupervision";
            btnSupervision.Size = new Size(150, 32);
            btnSupervision.TabIndex = 15;
            btnSupervision.Text = "Supervisión (DDM)";
            btnSupervision.UseVisualStyleBackColor = true;
            btnSupervision.Click += btnSupervision_Click;
            //
            // btnReportes
            //
            btnReportes.Location = new Point(324, 126);
            btnReportes.Name = "btnReportes";
            btnReportes.Size = new Size(100, 32);
            btnReportes.TabIndex = 16;
            btnReportes.Text = "Reportes";
            btnReportes.UseVisualStyleBackColor = true;
            btnReportes.Click += btnReportes_Click;
            //
            // dgvDatos
            //
            dgvDatos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDatos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDatos.AllowUserToAddRows = false;
            dgvDatos.Location = new Point(12, 168);
            dgvDatos.Name = "dgvDatos";
            dgvDatos.ReadOnly = true;
            dgvDatos.Size = new Size(976, 380);
            dgvDatos.TabIndex = 17;
            //
            // Form1
            //
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 560);
            Controls.Add(dgvDatos);
            Controls.Add(btnReportes);
            Controls.Add(btnSupervision);
            Controls.Add(btnIncidenteActivo);
            Controls.Add(btnAlertas);
            Controls.Add(btnVulnerabilidades);
            Controls.Add(btnActivos);
            Controls.Add(btnOrganizaciones);
            Controls.Add(lblSesion);
            Controls.Add(btnAuditoria);
            Controls.Add(btnVerAnalistas);
            Controls.Add(btnCerrar);
            Controls.Add(btnRegistrar);
            Controls.Add(btnBitacora);
            Controls.Add(btnResumen);
            Controls.Add(btnMostrar);
            Controls.Add(btnConectar);
            Name = "Form1";
            Text = "SGISI - Gestión de Incidentes";
            ((System.ComponentModel.ISupportInitialize)dgvDatos).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnConectar;
        private Button btnMostrar;
        private Button btnResumen;
        private Button btnBitacora;
        private Button btnRegistrar;
        private Button btnCerrar;
        private Button btnVerAnalistas;
        private Button btnAuditoria;
        private Label lblSesion;
        private Button btnOrganizaciones;
        private Button btnActivos;
        private Button btnVulnerabilidades;
        private Button btnAlertas;
        private Button btnIncidenteActivo;
        private Button btnSupervision;
        private Button btnReportes;
        private DataGridView dgvDatos;
    }
}
