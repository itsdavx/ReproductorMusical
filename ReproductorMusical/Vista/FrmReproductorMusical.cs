using ReproductorMusical.Controlador;
using ReproductorMusical.Modelo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ReproductorMusical
{
    public partial class FrmReproductorMusical : Form
    {
        // ── Dependencia única con el Controlador ─────────────────────────
        private readonly ReproductorControlador _controlador;

        // ── Estado estricto de UI ────────────────────────────────────────
        private bool _temaOscuro = true;
        private bool _reproduciendo = false;

        // Bloquea SelectedIndexChanged cuando el cambio es programático
        private bool _sincronizandoLista = false;

        // ── Colores activos del tema (se actualizan en AplicarTema) ─────
        private Color _colorTexto = Color.FromArgb(234, 234, 234);
        private Color _colorTextoSecundario = Color.FromArgb(176, 176, 176);
        private Color _colorFondoElementos = Color.FromArgb(20, 20, 20);
        private Color _colorSeleccion = Color.FromArgb(60, 60, 80);

        // ── Constructor ──────────────────────────────────────────────────
        public FrmReproductorMusical()
        {
            InitializeComponent();
            cmbEfectosMusicales.Paint += (s, e) =>
            {
                using (SolidBrush brush = new SolidBrush(_colorFondoElementos))
                {
                    e.Graphics.FillRectangle(brush, cmbEfectosMusicales.ClientRectangle);
                }
                ControlPaint.DrawComboButton(e.Graphics,
                    new Rectangle(cmbEfectosMusicales.Width - 20, 0, 20, cmbEfectosMusicales.Height),
                    ButtonState.Normal);
            };


            ReproductorModelo modelo = new ReproductorModelo();
            _controlador = new ReproductorControlador(modelo);

            SuscribirEventos();
            ConfigurarListBox();
            CargarEfectosEnCombo();
            ConfigurarVentana();
            AplicarTema();
        
        }

        // ── Inicialización ───────────────────────────────────────────────

        private void SuscribirEventos()
        {
            _controlador.OnTiempoActualizado += ActualizarTiempoActual;
            _controlador.OnDuracionActualizada += ActualizarDuracionTotal;
            _controlador.OnProgresoActualizado += ActualizarProgreso;
            _controlador.OnRedibujarGrafico += () => pnl_grafico.Invalidate();
            _controlador.OnErrorReproduccion += MostrarError;
            _controlador.OnStopReproduccion += ResetearPlayPause;
            _controlador.OnSincronizarLista += SincronizarListBox;
            _controlador.OnAlbumCambiado += CambiarImagenAlbum;
        }

        /// <summary>
        /// Activa OwnerDrawFixed para poder dibujar las columnas
        /// Nombre | Artista | Álbum dentro de cada fila.
        /// </summary>
        private void ConfigurarListBox()
        {
            track_list.DrawMode = DrawMode.OwnerDrawFixed;
            track_list.ItemHeight = 36;               // altura suficiente para dos líneas
            track_list.DrawItem += TrackList_DrawItem;
        }

        private void CargarEfectosEnCombo()
        {
            foreach (string nombre in _controlador.ObtenerNombresEfectos())
                cmbEfectosMusicales.Items.Add(nombre);
            cmbEfectosMusicales.SelectedIndex = 0;
        }

        private void ConfigurarVentana()
        {
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint,
                true
            );
            this.Icon = Properties.Resources.IconoReproductor;
        }

        // ── Dibujado personalizado del ListBox ───────────────────────────

        private void TrackList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            IReadOnlyList<PistaMusical> pistas = _controlador.ObtenerTodasLasPistas();
            if (e.Index >= pistas.Count) return;

            PistaMusical pista = pistas[e.Index];

            // ── Fondo ────────────────────────────────────────────────────
            bool seleccionado = (e.State & DrawItemState.Selected) != 0;
            bool esPistaActiva = e.Index == _controlador.IndicePistaActual;

            Color colorFondo;
            if (_temaOscuro)
            {
                // En tema oscuro: seleccionado/activo = negro intenso
                if (seleccionado || esPistaActiva)
                    colorFondo = Color.Black;
                else
                    colorFondo = _colorFondoElementos;
            }
            else
            {
                // En tema claro: se mantiene la lógica actual
                if (seleccionado || esPistaActiva)
                    colorFondo = _colorSeleccion;
                else
                    colorFondo = (e.Index % 2 == 0)
                        ? _colorFondoElementos
                        : Color.FromArgb(
                            Math.Min(_colorFondoElementos.R + 8, 255),
                            Math.Min(_colorFondoElementos.G + 8, 255),
                            Math.Min(_colorFondoElementos.B + 8, 255));
            }

            e.Graphics.FillRectangle(new SolidBrush(colorFondo), e.Bounds);

            // ── Divisiones de columnas ────────────────────────────────────
            int ancho = e.Bounds.Width;
            int col1End = (int)(ancho * 0.40);
            int col2End = (int)(ancho * 0.70);

            int margenV = 4;
            int margenH = 8;

            Color colorSeparador = Color.FromArgb(50, _colorTexto);
            using (Pen penSep = new Pen(colorSeparador, 1))
            {
                e.Graphics.DrawLine(penSep,
                    e.Bounds.Left + col1End, e.Bounds.Top + margenV,
                    e.Bounds.Left + col1End, e.Bounds.Bottom - margenV);

                e.Graphics.DrawLine(penSep,
                    e.Bounds.Left + col2End, e.Bounds.Top + margenV,
                    e.Bounds.Left + col2End, e.Bounds.Bottom - margenV);
            }

            // ── Textos ────────────────────────────────────────────────────
            StringFormat sf = new StringFormat
            {
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center
            };

            // Fuente: normal por defecto, negrita si está seleccionada/activa
            FontStyle estilo = (seleccionado || esPistaActiva) ? FontStyle.Bold : FontStyle.Regular;

            // Nombre de archivo (columna 1)
            using (Font fNombre = new Font("Segoe UI", 10f, estilo))
            using (SolidBrush brNombre = new SolidBrush(
                esPistaActiva ? Color.FromArgb(120, 180, 255) : _colorTexto))
            {
                RectangleF rectNombre = new RectangleF(
                    e.Bounds.Left + margenH,
                    e.Bounds.Top,
                    col1End - margenH * 2,
                    e.Bounds.Height);

                e.Graphics.DrawString(pista.NombreArchivo, fNombre, brNombre, rectNombre, sf);
            }

            // Artista y Álbum (columna 2 y 3)
            using (Font fMeta = new Font("Segoe UI", 9.5f, estilo))
            using (SolidBrush brMeta = new SolidBrush(_colorTextoSecundario))
            {
                string textoArtista = string.IsNullOrEmpty(pista.Artista) ? "—" : pista.Artista;
                RectangleF rectArtista = new RectangleF(
                    e.Bounds.Left + col1End + margenH,
                    e.Bounds.Top,
                    col2End - col1End - margenH * 2,
                    e.Bounds.Height);

                e.Graphics.DrawString(textoArtista, fMeta, brMeta, rectArtista, sf);

                string textoAlbum = string.IsNullOrEmpty(pista.Album) ? "—" : pista.Album;
                RectangleF rectAlbum = new RectangleF(
                    e.Bounds.Left + col2End + margenH,
                    e.Bounds.Top,
                    ancho - col2End - margenH * 2,
                    e.Bounds.Height);

                e.Graphics.DrawString(textoAlbum, fMeta, brMeta, rectAlbum, sf);
            }

            // ── Línea inferior separadora de filas ────────────────────────
            using (Pen penFila = new Pen(Color.FromArgb(25, _colorTexto), 1))
            {
                e.Graphics.DrawLine(penFila,
                    e.Bounds.Left, e.Bounds.Bottom - 1,
                    e.Bounds.Right, e.Bounds.Bottom - 1);
            }
        }



        // ── Eventos de botones ───────────────────────────────────────────

        private void btn_open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de Audio|*.mp3;*.wav";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                _controlador.ReemplazarPistas(ofd.FileNames);

                // Limpiamos y repoblamos — el DrawItem leerá los datos frescos
                track_list.Items.Clear();
                foreach (string nombre in _controlador.ObtenerNombresPistas())
                    track_list.Items.Add(nombre);

                if (track_list.Items.Count > 0)
                    track_list.SelectedIndex = 0;
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (track_list.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, selecciona una canción de la lista.");
                return;
            }

            if (!_reproduciendo)
            {
                _controlador.Play(track_list.SelectedIndex);
                _reproduciendo = true;
            }
            else
            {
                _controlador.Pause();
                _reproduciendo = false;
            }

            ActualizarIconos();
            track_list.Invalidate(); // refresca el resaltado de pista activa
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            _controlador.Stop();
            track_list.Invalidate();
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            _controlador.Next();
        }

        private void btn_preview_Click(object sender, EventArgs e)
        {
            _controlador.Previous();
        }

        private void track_volume_Scroll(object sender, EventArgs e)
        {
            _controlador.CambiarVolumen(track_volume.Value, track_volume.Maximum);

            float volNormalizado = (float)track_volume.Value / track_volume.Maximum;
            if (lvl_volumen != null)
                lvl_volumen.Text = ((int)(volNormalizado * 100)) + "%";
        }

        private void p_bar_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;
            if (mouse == null) return;
            _controlador.CambiarPosicion(mouse.X, p_bar.Width);
        }

        private void cmbEfectosMusicales_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEfectosMusicales.SelectedItem == null) return;
            _controlador.CambiarEfecto(cmbEfectosMusicales.SelectedItem.ToString());
            pnl_grafico.Invalidate();
        }

        private void pnl_grafico_Paint(object sender, PaintEventArgs e)
        {
            _controlador.RenderizarEfecto(e.Graphics, pnl_grafico.Width, pnl_grafico.Height);
        }

        private void track_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_sincronizandoLista) return;

            int idx = track_list.SelectedIndex;
            if (idx == -1) return;

            if (_reproduciendo)
                _controlador.Play(idx);
        }

        // ── Modo de reproducción ─────────────────────────────────────────

        private void btnModo_Click(object sender, EventArgs e)
        {
            int total = Enum.GetValues(typeof(ModoReproduccion)).Length;
            int actual = (int)_controlador.ModoReproduccion;
            _controlador.ModoReproduccion = (ModoReproduccion)((actual + 1) % total);

            switch (_controlador.ModoReproduccion)
            {
                case ModoReproduccion.Aleatorio: btnModo.Text = "🔀"; break;
                case ModoReproduccion.Secuencial: btnModo.Text = "🔁"; break;
                case ModoReproduccion.RepetirUno: btnModo.Text = "🔂"; break;
            }
        }

        // ── Tema visual ──────────────────────────────────────────────────

        private void btnTema_Click(object sender, EventArgs e)
        {
            _temaOscuro = !_temaOscuro;
            AplicarTema();
        }

        private void AplicarTema()
        {
            Color colorFondo, colorTexto, colorSecundario, colorFondoElementos, colorSeleccion, colorbtn, colorCmb;

            if (_temaOscuro)
            {
                colorFondo = Color.FromArgb(18, 18, 18);
                colorFondoElementos = Color.FromArgb(26, 26, 26);
                colorTexto = Color.FromArgb(234, 234, 234);
                colorSecundario = Color.FromArgb(176, 176, 176);
                colorSeleccion = Color.FromArgb(50, 60, 90);
                colorbtn = Color.FromArgb(30, 30, 30);
                colorCmb = Color.FromArgb(20, 20, 20);
                btnTema.Text = "🌙";
            }
            else
            {
                colorFondo = Color.FromArgb(245, 245, 245);
                colorFondoElementos = Color.FromArgb(225, 225, 225);
                colorTexto = Color.FromArgb(30, 30, 30);
                colorSecundario = Color.FromArgb(85, 85, 85);
                colorSeleccion = Color.FromArgb(190, 210, 240);
                colorbtn = Color.FromArgb(210, 210, 210);
                colorCmb = Color.FromArgb(200, 200, 200);
                btnTema.Text = "☀️";
            }

            // Guardamos los colores activos para que DrawItem los use
            _colorTexto = colorTexto;
            _colorTextoSecundario = colorSecundario;
            _colorFondoElementos = colorFondoElementos;
            _colorSeleccion = colorSeleccion;

            // Formulario base
            this.BackColor = colorFondo;
            pnlCancionImagen.BackColor = colorFondoElementos;
            pnlBarraSuperior.BackColor = colorFondoElementos;

            // Labels
            lvl_volumen.BackColor = colorFondo;
            lvl_volumen.ForeColor = colorTexto;
            lbl_track_start.ForeColor = colorTexto;
            lbl_track_end.ForeColor = colorTexto;
            lblVolume.ForeColor = colorTexto;
            lblMuSync.ForeColor = colorTexto;
            lblMusicalEffects.ForeColor = colorSecundario;
            lblIcono.ForeColor = colorTexto;
            lblIcono.BackColor = colorFondoElementos;

            // Botones
            btnModo.ForeColor = colorTexto;
            btn_preview.ForeColor = colorTexto;
            btn_next.ForeColor = colorTexto;
            btnPlayPause.ForeColor = colorTexto;
            btn_stop.ForeColor = colorTexto;
            btn_open.ForeColor = colorTexto;
            btnTema.ForeColor = colorTexto;
            btnExit.ForeColor = colorTexto;
            btnMinimizar.ForeColor = colorTexto;
            btnTema.BackColor = colorbtn;
            btnExit.BackColor = colorbtn;
            btnMinimizar.BackColor = colorbtn;
            btnLimpiar.BackColor = colorFondoElementos;
            btnLimpiar.ForeColor = colorTexto;

            // Controles compuestos
            cmbEfectosMusicales.BackColor = colorCmb;
            cmbEfectosMusicales.ForeColor = colorTexto;
            track_list.BackColor = colorFondoElementos;
            track_list.ForeColor = colorTexto;
            track_volume.BackColor = colorFondo;


            _controlador.ColorFondoGrafico = colorFondoElementos;

            ActualizarIconos();
            pnl_grafico.Invalidate();
            track_list.Invalidate(); // redibuja las filas con los nuevos colores
        }

        // ── Callbacks desde el Controlador ───────────────────────────────

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

        private void SincronizarListBox()
        {
            _sincronizandoLista = true;

            int idx = _controlador.IndicePistaActual;
            if (idx >= 0 && idx < track_list.Items.Count)
                track_list.SelectedIndex = idx;

            _sincronizandoLista = false;

            _reproduciendo = true;
            ActualizarIconos();
            track_list.Invalidate(); // resalta la fila activa recién cambiada
        }

        private void ResetearPlayPause()
        {
            _reproduciendo = false;
            ActualizarIconos();
            track_list.Invalidate();
        }

        private void ActualizarIconos()
        {
            btnPlayPause.Image = _reproduciendo
                ? (_temaOscuro
                    ? Properties.Resources.PauseOscuro
                    : Properties.Resources.PauseClaro)
                : (_temaOscuro
                    ? Properties.Resources.PlayOscuro
                    : Properties.Resources.PlayClaro);
        }

        private void CambiarImagenAlbum(Image portada)
        {
            if (pnlCancionImagen.BackgroundImage != null)
            {
                Image anterior = pnlCancionImagen.BackgroundImage;
                pnlCancionImagen.BackgroundImage = null;
                anterior.Dispose();
            }

            if (portada != null)
            {
                pnlCancionImagen.BackgroundImage = portada;
                pnlCancionImagen.BackgroundImageLayout = ImageLayout.Zoom;
            }
            else
            {
                pnlCancionImagen.BackgroundImage = Properties.Resources.NoPortada;
            }
        }

        // ── Carga del formulario ─────────────────────────────────────────

        private void FrmReproductorMusical_Load(object sender, EventArgs e)
        {
            RedondearControl(pnlCancionImagen, 50);
            RedondearControl(pnl_grafico, 50);
            RedondearControl(track_list, 20);
            RedondearControl(p_bar, 5);
            RedondearControl(btnModo, 50);
            RedondearControl(btn_preview, 50);
            RedondearControl(btn_next, 50);
            RedondearControl(btnPlayPause, 90);
            RedondearControl(btn_stop, 50);
            RedondearControl(btn_open, 50);
            RedondearControl(pnlBarraSuperior, 50);
            RedondearControl(btnLimpiar, 20);

            RedondearControl(btnTema, 20);
            RedondearControl(btnExit, 20);
            RedondearControl(btnMinimizar, 20);

            RedondearFormulario(50); // puedes ajustar el radio a tu gusto

            btnTema.TextAlign = ContentAlignment.MiddleCenter;
            btnModo.TextAlign = ContentAlignment.MiddleCenter;
            btn_preview.TextAlign = ContentAlignment.MiddleCenter;
            btn_next.TextAlign = ContentAlignment.MiddleCenter;
            btn_stop.TextAlign = ContentAlignment.MiddleCenter;
            btn_open.TextAlign = ContentAlignment.MiddleCenter;

            ActualizarIconos();
        }

        // ── Helper visual ────────────────────────────────────────────────


        private void RedondearControl(Control control, int radio)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(control.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(control.Width - radio, control.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, control.Height - radio, radio, radio, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        private void RedondearFormulario(int radio)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(this.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(this.Width - radio, this.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, this.Height - radio, radio, radio, 90, 90);
            path.CloseFigure();
            this.Region = new Region(path);
        }


        // ── Métodos vacíos requeridos por el Designer ────────────────────
        private void lvl_volumen_Click(object sender, EventArgs e) { }
        private void timer1_Tick(object sender, EventArgs e) { }
        private void lbl_track_start_Click(object sender, EventArgs e) { }
        private void lbl_track_end_Click(object sender, EventArgs e) { }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Limpiar la lista de canciones
            track_list.Items.Clear();

            // Detener cualquier reproducción en curso
            _controlador.Stop();

            // Reiniciar labels de tiempo
            lbl_track_start.Text = "00:00";
            lbl_track_end.Text = "00:00";

            // Reiniciar barra de progreso
            p_bar.Value = 0;

            // Reiniciar estado de reproducción
            _reproduciendo = false;
            ActualizarIconos();

            // Opcional: limpiar la imagen del álbum
            pnlCancionImagen.BackgroundImage = Properties.Resources.NoPortada;
        }

    }
}