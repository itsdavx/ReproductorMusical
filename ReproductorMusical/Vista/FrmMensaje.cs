using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;

namespace ReproductorMusical.Vista
{
    public partial class FrmMensaje : Form
    {
        public FrmMensaje(bool temaOscuro)
        {
            InitializeComponent();
            AplicarTema(temaOscuro);
            this.Load += (s, e) => RedondearControles();
        }

        private void AplicarTema(bool temaOscuro)
        {
            if (temaOscuro)
            {
                this.BackColor = Color.FromArgb(0, 0, 0);
                lblMensajeVacioTrackList.ForeColor = Color.FromArgb(176, 176, 176);
                btnAceptar.BackColor = Color.FromArgb(30, 30, 30);
                btnAceptar.ForeColor = Color.FromArgb(234, 234, 234);
                btnAceptar.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 80);
            }
            else
            {
                this.BackColor = Color.FromArgb(245, 245, 245);
                lblMensajeVacioTrackList.ForeColor = Color.FromArgb(85, 85, 85);
                btnAceptar.BackColor = Color.FromArgb(210, 210, 210);
                btnAceptar.ForeColor = Color.FromArgb(30, 30, 30);
                btnAceptar.FlatAppearance.BorderColor = Color.FromArgb(190, 190, 190);
            }
        }

        private void RedondearControles()
        {
            RedondearControl(btnAceptar, 50);
            RedondearFormulario(90);
        }

        private void RedondearControl(Control control, int radio)
        {
            if (control == null || control.Width <= 0 || control.Height <= 0) return;
            int r = Math.Min(radio, Math.Min(control.Width, control.Height) / 2);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(0, 0, r, r, 180, 90);
                path.AddArc(control.Width - r, 0, r, r, 270, 90);
                path.AddArc(control.Width - r, control.Height - r, r, r, 0, 90);
                path.AddArc(0, control.Height - r, r, r, 90, 90);
                path.CloseFigure();
                control.Region = new Region(path);
            }
        }

        private void RedondearFormulario(int radio)
        {
            if (this.Width <= 0 || this.Height <= 0) return;
            int r = Math.Min(radio, Math.Min(this.Width, this.Height) / 2);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(0, 0, r, r, 180, 90);
                path.AddArc(this.Width - r, 0, r, r, 270, 90);
                path.AddArc(this.Width - r, this.Height - r, r, r, 0, 90);
                path.AddArc(0, this.Height - r, r, r, 90, 90);
                path.CloseFigure();
                this.Region = new Region(path);
            }
        }

        private void lblMensajeVacioTrackList_Click(object sender, EventArgs e) { }
        private void btnAceptar_Click_1(object sender, EventArgs e) => this.Close();
        private void FrmMensaje_Load(object sender, EventArgs e) => SystemSounds.Exclamation.Play();
    }
}