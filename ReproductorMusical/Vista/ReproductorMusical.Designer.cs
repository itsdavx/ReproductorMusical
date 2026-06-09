namespace ReproductorMusical
{
    partial class ReproductorMusical
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
            this.track_list = new System.Windows.Forms.ListBox();
            this.pic_art = new System.Windows.Forms.PictureBox();
            this.track_volume = new System.Windows.Forms.TrackBar();
            this.lbl_volume = new System.Windows.Forms.Label();
            this.lvl_volumen = new System.Windows.Forms.Label();
            this.lbl_track_start = new System.Windows.Forms.Label();
            this.lbl_track_end = new System.Windows.Forms.Label();
            this.pnl_grafico = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblVolume = new System.Windows.Forms.Label();
            this.lbl100 = new System.Windows.Forms.Label();
            this.cmbEfectosMusicales = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pic_art)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.track_volume)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_preview
            // 
            this.btn_preview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_preview.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Bold);
            this.btn_preview.ForeColor = System.Drawing.Color.White;
            this.btn_preview.Location = new System.Drawing.Point(343, 943);
            this.btn_preview.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_preview.Name = "btn_preview";
            this.btn_preview.Size = new System.Drawing.Size(112, 35);
            this.btn_preview.TabIndex = 0;
            this.btn_preview.Text = "⏪";
            this.btn_preview.UseVisualStyleBackColor = true;
            this.btn_preview.Click += new System.EventHandler(this.btn_preview_Click);
            // 
            // btn_next
            // 
            this.btn_next.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_next.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Bold);
            this.btn_next.ForeColor = System.Drawing.Color.White;
            this.btn_next.Location = new System.Drawing.Point(826, 941);
            this.btn_next.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new System.Drawing.Size(112, 35);
            this.btn_next.TabIndex = 1;
            this.btn_next.Text = "⏭";
            this.btn_next.UseVisualStyleBackColor = true;
            this.btn_next.Click += new System.EventHandler(this.btn_next_Click);
            // 
            // btn_play
            // 
            this.btn_play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_play.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Bold);
            this.btn_play.ForeColor = System.Drawing.Color.White;
            this.btn_play.Location = new System.Drawing.Point(464, 943);
            this.btn_play.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_play.Name = "btn_play";
            this.btn_play.Size = new System.Drawing.Size(112, 35);
            this.btn_play.TabIndex = 2;
            this.btn_play.Text = "▶";
            this.btn_play.UseVisualStyleBackColor = true;
            this.btn_play.Click += new System.EventHandler(this.btn_play_Click);
            // 
            // btn_pause
            // 
            this.btn_pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_pause.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_pause.ForeColor = System.Drawing.Color.White;
            this.btn_pause.Location = new System.Drawing.Point(584, 943);
            this.btn_pause.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_pause.Name = "btn_pause";
            this.btn_pause.Size = new System.Drawing.Size(112, 35);
            this.btn_pause.TabIndex = 3;
            this.btn_pause.Text = "||";
            this.btn_pause.UseVisualStyleBackColor = true;
            this.btn_pause.Click += new System.EventHandler(this.btn_pause_Click);
            // 
            // btn_stop
            // 
            this.btn_stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_stop.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Bold);
            this.btn_stop.ForeColor = System.Drawing.Color.White;
            this.btn_stop.Location = new System.Drawing.Point(704, 943);
            this.btn_stop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(112, 35);
            this.btn_stop.TabIndex = 4;
            this.btn_stop.Text = "⏹";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // btn_open
            // 
            this.btn_open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_open.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Bold);
            this.btn_open.ForeColor = System.Drawing.Color.White;
            this.btn_open.Location = new System.Drawing.Point(235, 945);
            this.btn_open.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(91, 35);
            this.btn_open.TabIndex = 5;
            this.btn_open.Text = "📂";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
            // 
            // p_bar
            // 
            this.p_bar.Location = new System.Drawing.Point(112, 905);
            this.p_bar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.p_bar.Name = "p_bar";
            this.p_bar.Size = new System.Drawing.Size(915, 11);
            this.p_bar.TabIndex = 6;
            this.p_bar.Click += new System.EventHandler(this.p_bar_Click);
            // 
            // track_list
            // 
            this.track_list.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.track_list.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.track_list.ForeColor = System.Drawing.Color.White;
            this.track_list.FormattingEnabled = true;
            this.track_list.ItemHeight = 25;
            this.track_list.Location = new System.Drawing.Point(260, 14);
            this.track_list.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.track_list.Name = "track_list";
            this.track_list.Size = new System.Drawing.Size(556, 204);
            this.track_list.TabIndex = 7;
            this.track_list.SelectedIndexChanged += new System.EventHandler(this.track_list_SelectedIndexChanged);
            // 
            // pic_art
            // 
            this.pic_art.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            // this.pic_art.Image = global::ReproductorMusical.Properties.Resources.logo2;
            this.pic_art.Location = new System.Drawing.Point(4, 14);
            this.pic_art.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pic_art.Name = "pic_art";
            this.pic_art.Size = new System.Drawing.Size(249, 208);
            this.pic_art.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_art.TabIndex = 8;
            this.pic_art.TabStop = false;
            // 
            // track_volume
            // 
            this.track_volume.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.track_volume.Location = new System.Drawing.Point(901, 49);
            this.track_volume.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.track_volume.Maximum = 100;
            this.track_volume.Name = "track_volume";
            this.track_volume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.track_volume.Size = new System.Drawing.Size(69, 146);
            this.track_volume.TabIndex = 9;
            this.track_volume.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.track_volume.Scroll += new System.EventHandler(this.track_volume_Scroll);
            // 
            // lbl_volume
            // 
            this.lbl_volume.AutoSize = true;
            this.lbl_volume.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lbl_volume.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_volume.ForeColor = System.Drawing.Color.White;
            this.lbl_volume.Location = new System.Drawing.Point(838, 255);
            this.lbl_volume.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_volume.Name = "lbl_volume";
            this.lbl_volume.Size = new System.Drawing.Size(71, 20);
            this.lbl_volume.TabIndex = 10;
            this.lbl_volume.Text = "Volume";
            // 
            // lvl_volumen
            // 
            this.lvl_volumen.AutoSize = true;
            this.lvl_volumen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lvl_volumen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvl_volumen.ForeColor = System.Drawing.Color.White;
            this.lvl_volumen.Location = new System.Drawing.Point(838, 108);
            this.lvl_volumen.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lvl_volumen.Name = "lvl_volumen";
            this.lvl_volumen.Size = new System.Drawing.Size(55, 20);
            this.lvl_volumen.TabIndex = 11;
            this.lvl_volumen.Text = "100%";
            this.lvl_volumen.Click += new System.EventHandler(this.lvl_volumen_Click);
            // 
            // lbl_track_start
            // 
            this.lbl_track_start.AutoSize = true;
            this.lbl_track_start.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_track_start.ForeColor = System.Drawing.Color.White;
            this.lbl_track_start.Location = new System.Drawing.Point(39, 896);
            this.lbl_track_start.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_track_start.Name = "lbl_track_start";
            this.lbl_track_start.Size = new System.Drawing.Size(65, 28);
            this.lbl_track_start.TabIndex = 12;
            this.lbl_track_start.Text = "00:00";
            this.lbl_track_start.Click += new System.EventHandler(this.lbl_track_start_Click);
            // 
            // lbl_track_end
            // 
            this.lbl_track_end.AutoSize = true;
            this.lbl_track_end.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_track_end.ForeColor = System.Drawing.Color.White;
            this.lbl_track_end.Location = new System.Drawing.Point(1035, 896);
            this.lbl_track_end.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_track_end.Name = "lbl_track_end";
            this.lbl_track_end.Size = new System.Drawing.Size(65, 28);
            this.lbl_track_end.TabIndex = 13;
            this.lbl_track_end.Text = "00:00";
            this.lbl_track_end.Click += new System.EventHandler(this.lbl_track_end_Click);
            // 
            // pnl_grafico
            // 
            this.pnl_grafico.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnl_grafico.Location = new System.Drawing.Point(44, 132);
            this.pnl_grafico.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnl_grafico.Name = "pnl_grafico";
            this.pnl_grafico.Size = new System.Drawing.Size(1056, 428);
            this.pnl_grafico.TabIndex = 14;
            this.pnl_grafico.Paint += new System.Windows.Forms.PaintEventHandler(this.pnl_grafico_Paint);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.panel1.Controls.Add(this.lblVolume);
            this.panel1.Controls.Add(this.lbl100);
            this.panel1.Controls.Add(this.lbl_volume);
            this.panel1.Controls.Add(this.lvl_volumen);
            this.panel1.Controls.Add(this.pic_art);
            this.panel1.Controls.Add(this.track_list);
            this.panel1.Controls.Add(this.track_volume);
            this.panel1.Location = new System.Drawing.Point(73, 641);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(982, 235);
            this.panel1.TabIndex = 15;
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolume.ForeColor = System.Drawing.Color.White;
            this.lblVolume.Location = new System.Drawing.Point(909, 203);
            this.lblVolume.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(39, 32);
            this.lblVolume.TabIndex = 13;
            this.lblVolume.Text = "🔊";
            // 
            // lbl100
            // 
            this.lbl100.AutoSize = true;
            this.lbl100.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl100.ForeColor = System.Drawing.Color.White;
            this.lbl100.Location = new System.Drawing.Point(896, 22);
            this.lbl100.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl100.Name = "lbl100";
            this.lbl100.Size = new System.Drawing.Size(60, 22);
            this.lbl100.TabIndex = 12;
            this.lbl100.Text = "100%";
            // 
            // cmbEfectosMusicales
            // 
            this.cmbEfectosMusicales.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cmbEfectosMusicales.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbEfectosMusicales.ForeColor = System.Drawing.Color.White;
            this.cmbEfectosMusicales.FormattingEnabled = true;
            this.cmbEfectosMusicales.Location = new System.Drawing.Point(251, 589);
            this.cmbEfectosMusicales.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbEfectosMusicales.Name = "cmbEfectosMusicales";
            this.cmbEfectosMusicales.Size = new System.Drawing.Size(325, 30);
            this.cmbEfectosMusicales.TabIndex = 16;
            this.cmbEfectosMusicales.SelectedIndexChanged += new System.EventHandler(this.cmbEfectosMusicales_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(72, 588);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 25);
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
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(480, 44);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(186, 52);
            this.label2.TabIndex = 18;
            this.label2.Text = "MuSync";
            // 
            // ReproductorMusical
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(1150, 994);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbEfectosMusicales);
            this.Controls.Add(this.pnl_grafico);
            this.Controls.Add(this.lbl_track_end);
            this.Controls.Add(this.lbl_track_start);
            this.Controls.Add(this.p_bar);
            this.Controls.Add(this.btn_open);
            this.Controls.Add(this.btn_stop);
            this.Controls.Add(this.btn_pause);
            this.Controls.Add(this.btn_play);
            this.Controls.Add(this.btn_next);
            this.Controls.Add(this.btn_preview);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.Coral;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ReproductorMusical";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MuSync";
            ((System.ComponentModel.ISupportInitialize)(this.pic_art)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.track_volume)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.ListBox track_list;
        private System.Windows.Forms.PictureBox pic_art;
        private System.Windows.Forms.TrackBar track_volume;
        private System.Windows.Forms.Label lbl_volume;
        private System.Windows.Forms.Label lvl_volumen;
        private System.Windows.Forms.Label lbl_track_start;
        private System.Windows.Forms.Label lbl_track_end;
        private System.Windows.Forms.Panel pnl_grafico;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cmbEfectosMusicales;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.Label lbl100;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label2;
    }
}

