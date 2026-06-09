using System;
using System.Windows.Forms;
using ReproductorMusical.Controlador;
using ReproductorMusical.Modelo;

namespace ReproductorMusical
{
    public partial class FrmReproductorMusical : Form
    {
        // DEPENDENCIAS MVC
        private readonly ReproductorControlador controlador;

        public FrmReproductorMusical()
        {
            InitializeComponent();

            // Construye el modelo y el controlador
            ReproductorModelo modelo = new ReproductorModelo();
            controlador = new ReproductorControlador(modelo);

            // Suscribe los callbacks del controlador a los controles de la vista
            controlador.OnTiempoActualizado = ActualizarTiempoActual;
            controlador.OnDuracionActualizada = ActualizarDuracionTotal;
            controlador.OnProgresoActualizado = ActualizarProgreso;
            controlador.OnRedibujarGrafico = () => pnl_grafico.Invalidate();
            controlador.OnErrorReproduccion = MostrarError;

            // Llena el ComboBox con los nombres de efectos registrados
            foreach (string nombre in controlador.ObtenerNombresEfectos())
                cmbEfectosMusicales.Items.Add(nombre);

            cmbEfectosMusicales.SelectedIndex = 0;

            // Doble buffer para eliminar parpadeo
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint,
                true
            );
        }

        // EVENTOS DE BOTONES — solo delegan al controlador

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

                    if (track_list.SelectedIndex == -1 && track_list.Items.Count > 0)
                        track_list.SelectedIndex = 0;
                }
            }
        }

        private void btn_play_Click(object sender, EventArgs e)
        {
            if (track_list.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, selecciona una canción de la lista.");
                return;
            }
            controlador.Play(track_list.SelectedIndex);
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            controlador.Pause();
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            controlador.Stop();
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            controlador.Next();
            SincronizarListBox();
        }

        private void btn_preview_Click(object sender, EventArgs e)
        {
            controlador.Previous();
            SincronizarListBox();
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
            MouseEventArgs mouse = (MouseEventArgs)e;
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

        // ACTUALIZACIONES DE UI — llamadas desde los callbacks del controlador

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

        // Sincroniza el ListBox cuando Next/Previous cambian la pista activa
        private void SincronizarListBox()
        {
            int idx = controlador.IndicePistaActual;
            if (idx >= 0 && idx < track_list.Items.Count)
                track_list.SelectedIndex = idx;
        }

        // Métodos vacíos obligatorios para el Designer
        private void track_list_SelectedIndexChanged(object sender, EventArgs e) { }
        private void lvl_volumen_Click(object sender, EventArgs e) { }
        private void timer1_Tick(object sender, EventArgs e) { }
        private void lbl_track_start_Click(object sender, EventArgs e) { }
        private void lbl_track_end_Click(object sender, EventArgs e) { }
    }
}