namespace ReproductorMusical.Vista
{
    partial class FrmMensaje
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMensajeVacioTrackList = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMensajeVacioTrackList
            // 
            this.lblMensajeVacioTrackList.AutoSize = true;
            this.lblMensajeVacioTrackList.BackColor = System.Drawing.Color.Transparent;
            this.lblMensajeVacioTrackList.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensajeVacioTrackList.Location = new System.Drawing.Point(52, 22);
            this.lblMensajeVacioTrackList.Name = "lblMensajeVacioTrackList";
            this.lblMensajeVacioTrackList.Size = new System.Drawing.Size(402, 62);
            this.lblMensajeVacioTrackList.TabIndex = 0;
            this.lblMensajeVacioTrackList.Text = "No se encontraron canciones \r\nen la lista.";
            this.lblMensajeVacioTrackList.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblMensajeVacioTrackList.Click += new System.EventHandler(this.lblMensajeVacioTrackList_Click);
            // 
            // btnAceptar
            // 
            this.btnAceptar.FlatAppearance.BorderSize = 0;
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.ForeColor = System.Drawing.Color.Black;
            this.btnAceptar.Location = new System.Drawing.Point(193, 103);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(107, 29);
            this.btnAceptar.TabIndex = 1;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click_1);
            // 
            // FrmMensaje
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(494, 160);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.lblMensajeVacioTrackList);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmMensaje";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmMensaje";
            this.Load += new System.EventHandler(this.FrmMensaje_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMensajeVacioTrackList;
        private System.Windows.Forms.Button btnAceptar;
    }
}