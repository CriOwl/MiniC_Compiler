namespace MiniC
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpcNuevo = new System.Windows.Forms.ToolStripMenuItem();
            this.OpcAbrir = new System.Windows.Forms.ToolStripMenuItem();
            this.OpcGuardar = new System.Windows.Forms.ToolStripMenuItem();
            this.OpcGuardarComo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.OpcSalir = new System.Windows.Forms.ToolStripMenuItem();
            this.OpcCompilar = new System.Windows.Forms.ToolStripMenuItem();
            this.rtbEditor = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivoToolStripMenuItem,
            this.OpcCompilar});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(600, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // archivoToolStripMenuItem
            // 
            this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpcNuevo,
            this.OpcAbrir,
            this.OpcGuardar,
            this.OpcGuardarComo,
            this.toolStripSeparator1,
            this.OpcSalir});
            this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            this.archivoToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.archivoToolStripMenuItem.Text = "Archivo";
            // 
            // OpcNuevo
            // 
            this.OpcNuevo.Name = "OpcNuevo";
            this.OpcNuevo.Size = new System.Drawing.Size(180, 22);
            this.OpcNuevo.Text = "Nuevo";
            this.OpcNuevo.Click += new System.EventHandler(this.OpcNuevo_Click);
            // 
            // OpcAbrir
            // 
            this.OpcAbrir.Name = "OpcAbrir";
            this.OpcAbrir.Size = new System.Drawing.Size(180, 22);
            this.OpcAbrir.Text = "Abrir";
            this.OpcAbrir.Click += new System.EventHandler(this.OpcAbrir_Click);
            // 
            // OpcGuardar
            // 
            this.OpcGuardar.Name = "OpcGuardar";
            this.OpcGuardar.Size = new System.Drawing.Size(180, 22);
            this.OpcGuardar.Text = "Guardar";
            this.OpcGuardar.Click += new System.EventHandler(this.OpcGuardar_Click);
            // 
            // OpcGuardarComo
            // 
            this.OpcGuardarComo.Name = "OpcGuardarComo";
            this.OpcGuardarComo.Size = new System.Drawing.Size(180, 22);
            this.OpcGuardarComo.Text = "Guardar como";
            this.OpcGuardarComo.Click += new System.EventHandler(this.OpcGuardarComo_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // OpcSalir
            // 
            this.OpcSalir.Name = "OpcSalir";
            this.OpcSalir.Size = new System.Drawing.Size(180, 22);
            this.OpcSalir.Text = "Salir";
            this.OpcSalir.Click += new System.EventHandler(this.OpcSalir_Click);
            // 
            // OpcCompilar
            // 
            this.OpcCompilar.Name = "OpcCompilar";
            this.OpcCompilar.Size = new System.Drawing.Size(68, 20);
            this.OpcCompilar.Text = "Compilar";
            this.OpcCompilar.Click += new System.EventHandler(this.OpcCompilar_Click);
            // 
            // rtbEditor
            // 
            this.rtbEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbEditor.Location = new System.Drawing.Point(0, 24);
            this.rtbEditor.Margin = new System.Windows.Forms.Padding(2);
            this.rtbEditor.Name = "rtbEditor";
            this.rtbEditor.Size = new System.Drawing.Size(600, 342);
            this.rtbEditor.TabIndex = 1;
            this.rtbEditor.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.rtbEditor);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "MiniC";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.RichTextBox rtbEditor;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpcNuevo;
        private System.Windows.Forms.ToolStripMenuItem OpcAbrir;
        private System.Windows.Forms.ToolStripMenuItem OpcGuardar;
        private System.Windows.Forms.ToolStripMenuItem OpcGuardarComo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem OpcSalir;
        private System.Windows.Forms.ToolStripMenuItem OpcCompilar;
    }
}

