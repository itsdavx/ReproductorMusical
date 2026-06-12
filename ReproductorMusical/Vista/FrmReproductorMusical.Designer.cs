namespace ReproductorMusical
{
    partial class FrmReproductorMusical
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
            this.components = new System.ComponentModel.Container();
            this.btn_preview = new System.Windows.Forms.Button();
            this.btn_next = new System.Windows.Forms.Button();
            this.btn_play = new System.Windows.Forms.Button();
            this.btn_pause = new System.Windows.Forms.Button();
            this.btn_stop = new System.Windows.Forms.Button();
            this.btn_open = new System.Windows.Forms.Button();
            this.p_bar = new System.Windows.Forms.ProgressBar();
            this.track_volume = new System.Windows.Forms.TrackBar();
            this.lvl_volumen = new System.Windows.Forms.Label();
            this.lbl_track_start = new System.Windows.Forms.Label();
            this.lbl_track_end = new System.Windows.Forms.Label();
            this.pnl_grafico = new System.Windows.Forms.Panel();
            this.lblVolume = new System.Windows.Forms.Label();
            this.cmbEfectosMusicales = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.track_list = new System.Windows.Forms.ListBox();
            this.btnTema = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.track_volume)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_preview
            // 
            this.btn_preview.FlatAppearance.BorderSize = 0;
            this.btn_preview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_preview.Font = new System.Drawing.Font("Segoe UI Symbol", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_preview.ForeColor = System.Drawing.Color.White;
            this.btn_preview.Location = new System.Drawing.Point(269, 735);
            this.btn_preview.Name = "btn_preview";
            this.btn_preview.Size = new System.Drawing.Size(75, 50);
            this.btn_preview.TabIndex = 0;
            this.btn_preview.Text = "⏮";
            this.btn_preview.UseVisualStyleBackColor = true;
            this.btn_preview.Click += new System.EventHandler(this.btn_preview_Click);
            // 
            // btn_next
            // 
            this.btn_next.FlatAppearance.BorderSize = 0;
            this.btn_next.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_next.Font = new System.Drawing.Font("Segoe UI Symbol", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_next.ForeColor = System.Drawing.Color.White;
            this.btn_next.Location = new System.Drawing.Point(650, 735);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new System.Drawing.Size(75, 50);
            this.btn_next.TabIndex = 1;
            this.btn_next.Text = "⏭";
            this.btn_next.UseVisualStyleBackColor = true;
            this.btn_next.Click += new System.EventHandler(this.btn_next_Click);
            // 
            // btn_play
            // 
            this.btn_play.FlatAppearance.BorderSize = 0;
            this.btn_play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_play.Font = new System.Drawing.Font("Segoe UI Symbol", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_play.ForeColor = System.Drawing.Color.White;
            this.btn_play.Location = new System.Drawing.Point(460, 735);
            this.btn_play.Name = "btn_play";
            this.btn_play.Size = new System.Drawing.Size(75, 50);
            this.btn_play.TabIndex = 2;
            this.btn_play.Text = "▶";
            this.btn_play.UseVisualStyleBackColor = true;
            this.btn_play.Click += new System.EventHandler(this.btn_play_Click);
            // 
            // btn_pause
            // 
            this.btn_pause.FlatAppearance.BorderSize = 0;
            this.btn_pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_pause.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_pause.ForeColor = System.Drawing.Color.White;
            this.btn_pause.Location = new System.Drawing.Point(554, 735);
            this.btn_pause.Name = "btn_pause";
            this.btn_pause.Size = new System.Drawing.Size(75, 50);
            this.btn_pause.TabIndex = 3;
            this.btn_pause.Text = "||";
            this.btn_pause.UseVisualStyleBackColor = true;
            this.btn_pause.Click += new System.EventHandler(this.btn_pause_Click);
            // 
            // btn_stop
            // 
            this.btn_stop.FlatAppearance.BorderSize = 0;
            this.btn_stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_stop.Font = new System.Drawing.Font("Segoe UI Symbol", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_stop.ForeColor = System.Drawing.Color.White;
            this.btn_stop.Location = new System.Drawing.Point(365, 735);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(75, 50);
            this.btn_stop.TabIndex = 4;
            this.btn_stop.Text = "⏹";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // btn_open
            // 
            this.btn_open.FlatAppearance.BorderSize = 0;
            this.btn_open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_open.Font = new System.Drawing.Font("Segoe UI Symbol", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_open.ForeColor = System.Drawing.Color.White;
            this.btn_open.Location = new System.Drawing.Point(72, 735);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(75, 50);
            this.btn_open.TabIndex = 5;
            this.btn_open.Text = "📂";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
            // 
            // p_bar
            // 
            this.p_bar.Location = new System.Drawing.Point(100, 807);
            this.p_bar.Name = "p_bar";
            this.p_bar.Size = new System.Drawing.Size(846, 10);
            this.p_bar.TabIndex = 6;
            this.p_bar.Click += new System.EventHandler(this.p_bar_Click);
            // 
            // track_volume
            // 
            this.track_volume.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.track_volume.Cursor = System.Windows.Forms.Cursors.Hand;
            this.track_volume.LargeChange = 1;
            this.track_volume.Location = new System.Drawing.Point(851, 756);
            this.track_volume.Maximum = 100;
            this.track_volume.Name = "track_volume";
            this.track_volume.Size = new System.Drawing.Size(95, 45);
            this.track_volume.TabIndex = 9;
            this.track_volume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.track_volume.Value = 100;
            this.track_volume.Scroll += new System.EventHandler(this.track_volume_Scroll);
            // 
            // lvl_volumen
            // 
            this.lvl_volumen.AutoSize = true;
            this.lvl_volumen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lvl_volumen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvl_volumen.ForeColor = System.Drawing.Color.White;
            this.lvl_volumen.Location = new System.Drawing.Point(941, 759);
            this.lvl_volumen.Name = "lvl_volumen";
            this.lvl_volumen.Size = new System.Drawing.Size(37, 13);
            this.lvl_volumen.TabIndex = 11;
            this.lvl_volumen.Text = "100%";
            this.lvl_volumen.Click += new System.EventHandler(this.lvl_volumen_Click);
            // 
            // lbl_track_start
            // 
            this.lbl_track_start.AutoSize = true;
            this.lbl_track_start.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_track_start.ForeColor = System.Drawing.Color.White;
            this.lbl_track_start.Location = new System.Drawing.Point(51, 801);
            this.lbl_track_start.Name = "lbl_track_start";
            this.lbl_track_start.Size = new System.Drawing.Size(45, 19);
            this.lbl_track_start.TabIndex = 12;
            this.lbl_track_start.Text = "00:00";
            this.lbl_track_start.Click += new System.EventHandler(this.lbl_track_start_Click);
            // 
            // lbl_track_end
            // 
            this.lbl_track_end.AutoSize = true;
            this.lbl_track_end.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_track_end.ForeColor = System.Drawing.Color.White;
            this.lbl_track_end.Location = new System.Drawing.Point(952, 801);
            this.lbl_track_end.Name = "lbl_track_end";
            this.lbl_track_end.Size = new System.Drawing.Size(45, 19);
            this.lbl_track_end.TabIndex = 13;
            this.lbl_track_end.Text = "00:00";
            this.lbl_track_end.Click += new System.EventHandler(this.lbl_track_end_Click);
            // 
            // pnl_grafico
            // 
            this.pnl_grafico.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnl_grafico.Location = new System.Drawing.Point(30, 86);
            this.pnl_grafico.Name = "pnl_grafico";
            this.pnl_grafico.Size = new System.Drawing.Size(983, 349);
            this.pnl_grafico.TabIndex = 14;
            this.pnl_grafico.Paint += new System.Windows.Forms.PaintEventHandler(this.pnl_grafico_Paint);
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolume.ForeColor = System.Drawing.Color.White;
            this.lblVolume.Location = new System.Drawing.Point(828, 756);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(27, 21);
            this.lblVolume.TabIndex = 13;
            this.lblVolume.Text = "🔊";
            // 
            // cmbEfectosMusicales
            // 
            this.cmbEfectosMusicales.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cmbEfectosMusicales.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEfectosMusicales.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbEfectosMusicales.ForeColor = System.Drawing.Color.White;
            this.cmbEfectosMusicales.FormattingEnabled = true;
            this.cmbEfectosMusicales.Location = new System.Drawing.Point(175, 458);
            this.cmbEfectosMusicales.Name = "cmbEfectosMusicales";
            this.cmbEfectosMusicales.Size = new System.Drawing.Size(298, 28);
            this.cmbEfectosMusicales.TabIndex = 16;
            this.cmbEfectosMusicales.SelectedIndexChanged += new System.EventHandler(this.cmbEfectosMusicales_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(27, 462);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 20);
            this.label1.TabIndex = 17;
            this.label1.Text = "Musical Effects :";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(441, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 42);
            this.label2.TabIndex = 18;
            this.label2.Text = "MuSync";
            // 
            // track_list
            // 
            this.track_list.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.track_list.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.track_list.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.track_list.ForeColor = System.Drawing.Color.White;
            this.track_list.FormattingEnabled = true;
            this.track_list.ItemHeight = 22;
            this.track_list.Location = new System.Drawing.Point(30, 519);
            this.track_list.Name = "track_list";
            this.track_list.Size = new System.Drawing.Size(983, 176);
            this.track_list.TabIndex = 7;
            this.track_list.SelectedIndexChanged += new System.EventHandler(this.track_list_SelectedIndexChanged);
            // 
            // btnTema
            // 
            this.btnTema.FlatAppearance.BorderSize = 0;
            this.btnTema.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTema.Font = new System.Drawing.Font("Segoe UI Symbol", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTema.ForeColor = System.Drawing.Color.White;
            this.btnTema.Location = new System.Drawing.Point(959, 16);
            this.btnTema.Name = "btnTema";
            this.btnTema.Size = new System.Drawing.Size(75, 50);
            this.btnTema.TabIndex = 19;
            this.btnTema.Text = "☀️";
            this.btnTema.UseVisualStyleBackColor = true;
            // 
            // FrmReproductorMusical
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(1046, 859);
            this.Controls.Add(this.btnTema);
            this.Controls.Add(this.lbl_track_end);
            this.Controls.Add(this.track_list);
            this.Controls.Add(this.btn_open);
            this.Controls.Add(this.lbl_track_start);
            this.Controls.Add(this.p_bar);
            this.Controls.Add(this.lvl_volumen);
            this.Controls.Add(this.btn_stop);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.btn_pause);
            this.Controls.Add(this.track_volume);
            this.Controls.Add(this.btn_play);
            this.Controls.Add(this.btn_next);
            this.Controls.Add(this.btn_preview);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbEfectosMusicales);
            this.Controls.Add(this.pnl_grafico);
            this.ForeColor = System.Drawing.Color.Coral;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmReproductorMusical";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MuSync";
            ((System.ComponentModel.ISupportInitialize)(this.track_volume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_preview;
        private System.Windows.Forms.Button btn_next;
        private System.Windows.Forms.Button btn_play;
        private System.Windows.Forms.Button btn_pause;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.ProgressBar p_bar;
        private System.Windows.Forms.TrackBar track_volume;
        private System.Windows.Forms.Label lvl_volumen;
        private System.Windows.Forms.Label lbl_track_start;
        private System.Windows.Forms.Label lbl_track_end;
        private System.Windows.Forms.Panel pnl_grafico;
        private System.Windows.Forms.ComboBox cmbEfectosMusicales;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox track_list;
        private System.Windows.Forms.Button btnTema;
    }
}

