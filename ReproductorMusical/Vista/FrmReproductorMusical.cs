using ReproductorMusical.Controlador;
using ReproductorMusical.Modelo;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReproductorMusical
{
    public partial class FrmReproductorMusical : Form
    {
        // DEPENDENCIAS MVC
        private readonly ReproductorControlador controlador;
        private bool temaOscuro = true;
        private bool reproduciendo = false;
        private int modoReproduccion = 0;

        // Bloquea SelectedIndexChanged cuando el cambio es programático
        private bool sincronizandoLista = false;

        public FrmReproductorMusical()
        {
            InitializeComponent();

            ReproductorModelo modelo = new ReproductorModelo();
            controlador = new ReproductorControlador(modelo);

            // CALLBACKS
            controlador.OnTiempoActualizado = ActualizarTiempoActual;
            controlador.OnDuracionActualizada = ActualizarDuracionTotal;
            controlador.OnProgresoActualizado = ActualizarProgreso;
            controlador.OnRedibujarGrafico = () => pnl_grafico.Invalidate();
            controlador.OnErrorReproduccion = MostrarError;
            controlador.OnStopReproduccion = ResetearPlayPause;

            // El controlador avisa cuando cambió de pista para sincronizar el ListBox
            // Esto reemplaza las llamadas a SincronizarListBox() en Next/Previous
            controlador.OnSincronizarLista = SincronizarListBox;

            // EFECTOS
            foreach (string nombre in controlador.ObtenerNombresEfectos())
                cmbEfectosMusicales.Items.Add(nombre);
            cmbEfectosMusicales.SelectedIndex = 0;

            // CONFIGURACIÓN VENTANA
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint,
                true
            );
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        // EVENTOS DE BOTONES

        private void btn_open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de Audio|*.mp3;*.wav";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    controlador.AgregarPistas(ofd.FileNames);

                    track_list.Items.Clear();
                    foreach (string nombre in controlador.ObtenerNombresPistas())
                        track_list.Items.Add(nombre);

                    if (track_list.Items.Count > 0)
                        track_list.SelectedIndex = 0;
                }
            }
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            controlador.Stop();
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            controlador.Next();
            // Ya NO llamamos SincronizarListBox() aquí;
            // el controlador lo hace via OnSincronizarLista
        }

        private void btn_preview_Click(object sender, EventArgs e)
        {
            controlador.Previous();
            // Ídem
        }

        private void track_volume_Scroll(object sender, EventArgs e)
        {
            controlador.CambiarVolumen(track_volume.Value, track_volume.Maximum);

            float volNormalizado = (float)track_volume.Value / track_volume.Maximum;
            if (lvl_volumen != null)
                lvl_volumen.Text = ((int)(volNormalizado * 100)).ToString() + "%";
        }

        private void p_bar_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;
            if (mouse == null) return;
            controlador.CambiarPosicion(mouse.X, p_bar.Width);
        }

        private void cmbEfectosMusicales_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEfectosMusicales.SelectedItem != null)
            {
                controlador.CambiarEfecto(cmbEfectosMusicales.SelectedItem.ToString());
                pnl_grafico.Invalidate();
            }
        }

        private void pnl_grafico_Paint(object sender, PaintEventArgs e)
        {
            controlador.RenderizarEfecto(e.Graphics, pnl_grafico.Width, pnl_grafico.Height);
        }

        // ACTUALIZACIONES DE UI

        private void ActualizarTiempoActual(string texto)
        {
            if (lbl_track_start != null) lbl_track_start.Text = texto;
        }

        private void ActualizarDuracionTotal(string texto)
        {
            if (lbl_track_end != null) lbl_track_end.Text = texto;
        }

        private void ActualizarProgreso(int valor)
        {
            if (p_bar == null) return;
            if (valor >= p_bar.Minimum && valor <= p_bar.Maximum)
                p_bar.Value = valor;
        }

        private void MostrarError(string mensaje)
        {
            MessageBox.Show(
                "Error al reproducir: " + mensaje,
                "Error de Reproducción",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        // Sincroniza el ListBox con la pista activa del controlador
        private void SincronizarListBox()
        {
            sincronizandoLista = true;
            int idx = controlador.IndicePistaActual;
            if (idx >= 0 && idx < track_list.Items.Count)
                track_list.SelectedIndex = idx;
            sincronizandoLista = false;

            // Sincroniza también el estado del botón Play/Pause
            reproduciendo = true;
            btnPlayPause.Text = "||";
        }

        // Solo actúa cuando el cambio viene del usuario (clic en lista)
        private void track_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sincronizandoLista) return;

            int idx = track_list.SelectedIndex;
            if (idx == -1) return;

            if (reproduciendo)
            {
                controlador.Play(idx);
                // No llamar SincronizarListBox aquí; Play() lo hará via OnSincronizarLista
            }
        }

        // MÉTODOS VACÍOS REQUERIDOS POR EL DESIGNER
        private void lvl_volumen_Click(object sender, EventArgs e) { }
        private void timer1_Tick(object sender, EventArgs e) { }
        private void lbl_track_start_Click(object sender, EventArgs e) { }
        private void lbl_track_end_Click(object sender, EventArgs e) { }

        // TEMA VISUAL

        private void btnTema_Click(object sender, EventArgs e)
        {
            temaOscuro = !temaOscuro;

            Color colorFondo, colorTexto, colorSecundario, colorFondoElementos;

            if (temaOscuro)
            {
                colorFondo = Color.FromArgb(18, 18, 18);
                colorFondoElementos = Color.FromArgb(20, 20, 20);
                colorTexto = Color.FromArgb(234, 234, 234);
                colorSecundario = Color.FromArgb(176, 176, 176);
                btnTema.Text = "🌙";
            }
            else
            {
                colorFondo = Color.FromArgb(245, 245, 245);
                colorFondoElementos = Color.FromArgb(230, 230, 230);
                colorTexto = Color.FromArgb(30, 30, 30);
                colorSecundario = Color.FromArgb(85, 85, 85);
                btnTema.Text = "☀️";
            }

            this.BackColor = colorFondo;
            pnl_grafico.BackColor = colorFondo;
            pnlCancionImagen.BackColor = colorFondoElementos;
            lvl_volumen.BackColor = colorFondo;
            lvl_volumen.ForeColor = colorTexto;
            lbl_track_start.ForeColor = colorTexto;
            lbl_track_end.ForeColor = colorTexto;
            lblVolume.ForeColor = colorTexto;
            lblMusicalEffects.ForeColor = colorSecundario;
            lblMuSync.ForeColor = colorTexto;
            btnModo.ForeColor = colorTexto;
            btn_preview.ForeColor = colorTexto;
            btn_next.ForeColor = colorTexto;
            btnPlayPause.ForeColor = colorTexto;
            btn_stop.ForeColor = colorTexto;
            btn_open.ForeColor = colorTexto;
            btnTema.ForeColor = colorTexto;
            cmbEfectosMusicales.BackColor = colorFondo;
            cmbEfectosMusicales.ForeColor = colorTexto;
            track_list.BackColor = colorFondoElementos;
            track_list.ForeColor = colorTexto;
            track_volume.BackColor = colorFondo;

            controlador.ColorFondoGrafico = colorFondo;
            pnl_grafico.Invalidate();
        }

        // PLAY / PAUSE

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (track_list.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, selecciona una canción de la lista.");
                return;
            }

            if (!reproduciendo)
            {
                controlador.Play(track_list.SelectedIndex);
                btnPlayPause.Text = "||";
                reproduciendo = true;
            }
            else
            {
                controlador.Pause();
                btnPlayPause.Text = "▶";
                reproduciendo = false;
            }
        }

        private void ResetearPlayPause()
        {
            reproduciendo = false;
            btnPlayPause.Text = "▶";
        }

        private void btnPlayPause_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(1, 1, btnPlayPause.Width - 2, btnPlayPause.Height - 2);

            using (System.Drawing.Drawing2D.LinearGradientBrush brush =
                new System.Drawing.Drawing2D.LinearGradientBrush(
                    rect,
                    Color.FromArgb(186, 85, 211),
                    Color.FromArgb(0, 191, 255),
                    45f))
            {
                e.Graphics.FillEllipse(brush, rect);
            }

            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(
                    btnPlayPause.Text,
                    btnPlayPause.Font,
                    Brushes.White,
                    rect,
                    sf
                );
            }
        }

        // MODO DE REPRODUCCIÓN

        private void btnModo_Click(object sender, EventArgs e)
        {
            modoReproduccion = (modoReproduccion + 1) % 3;

            switch (modoReproduccion)
            {
                case 0: btnModo.Text = "🔀"; break;
                case 1: btnModo.Text = "🔁"; break;
                case 2: btnModo.Text = "🔂"; break;
            }

            controlador.ModoReproduccion = modoReproduccion;
        }
    }
}