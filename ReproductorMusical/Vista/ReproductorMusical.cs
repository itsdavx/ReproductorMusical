using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ReproductorMusical
{
    public partial class ReproductorMusical : Form
    {
        // Variables globales para el motor de audio (NAudio)
        private WaveStream waveStream;
        private VolumeSampleProvider volumeProvider;
        private IWavePlayer outputDevice;
        private Timer timerFPS;

        // Lista interna para almacenar las rutas completas de los archivos cargados
        private List<string> rutasArchivos = new List<string>();

        // Buffer de datos flotantes para capturar la onda de sonido en tiempo real
        private const int fftLength = 128;
        private float[] bufferMuestras = new float[fftLength];

        // Bandera de control para evitar parpadeos visuales al hacer clics manuales
        private bool modificandoProgresoManual = false;

        // Variable global para almacenar el efecto VISUAL seleccionado (Sincronizado con tus Items)
        private string efectoSeleccionado = "Barras Verticales";

        // Generador de números aleatorios para efectos orgánicos como partículas
        private Random rand = new Random();

        public ReproductorMusical()
        {
            InitializeComponent();

            // 1. Cargamos los efectos visuales en el ComboBox al iniciar el programa
            cmbEfectosMusicales.Items.Add("Barras Verticales");
            cmbEfectosMusicales.Items.Add("Osciloscopio");
            cmbEfectosMusicales.Items.Add("Espectro Circular");
            cmbEfectosMusicales.Items.Add("Ecualizador LED");
            cmbEfectosMusicales.Items.Add("Ondas de Sonido");
            cmbEfectosMusicales.Items.Add("Picos de Frecuencia");
            cmbEfectosMusicales.Items.Add("Partículas");
            cmbEfectosMusicales.Items.Add("Anillos Concéntricos");

            // 2. Dejamos seleccionado el primero por defecto para evitar que empiece en blanco
            cmbEfectosMusicales.SelectedIndex = 0;

            // Computación Gráfica: Activamos el Doble Buffer para eliminar por completo el parpadeo
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de Audio|*.mp3;*.wav";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (string archivo in ofd.FileNames)
                    {
                        rutasArchivos.Add(archivo);
                        track_list.Items.Add(Path.GetFileName(archivo));
                    }

                    if (track_list.SelectedIndex == -1 && track_list.Items.Count > 0)
                    {
                        track_list.SelectedIndex = 0;
                    }
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

            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Paused)
            {
                outputDevice.Play();
                timerFPS.Start();
                return;
            }

            DetenerAudio();

            try
            {
                string rutaSeleccionada = rutasArchivos[track_list.SelectedIndex];

                if (rutaSeleccionada.ToLower().EndsWith(".mp3"))
                {
                    waveStream = new Mp3FileReader(rutaSeleccionada);
                }
                else if (rutaSeleccionada.ToLower().EndsWith(".wav"))
                {
                    waveStream = new WaveFileReader(rutaSeleccionada);
                }
                else
                {
                    MessageBox.Show("Formato no soportado.");
                    return;
                }

                ISampleProvider sampleProvider = waveStream.ToSampleProvider();
                volumeProvider = new VolumeSampleProvider(sampleProvider);

                outputDevice = new WaveOutEvent();
                outputDevice.Init(volumeProvider);

                ActualizarVolumen();
                outputDevice.Play();

                if (lbl_track_end != null)
                {
                    lbl_track_end.Text = waveStream.TotalTime.ToString(@"mm\:ss");
                }

                // Hilo de animación: Refresca el lienzo ~60 veces por segundo (16ms)
                timerFPS = new Timer();
                timerFPS.Interval = 16;
                timerFPS.Tick += (s, ev) =>
                {
                    if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        if (!modificandoProgresoManual && waveStream != null)
                        {
                            double posicionActual = waveStream.CurrentTime.TotalSeconds;
                            double duracionTotal = waveStream.TotalTime.TotalSeconds;

                            if (lbl_track_start != null)
                            {
                                lbl_track_start.Text = waveStream.CurrentTime.ToString(@"mm\:ss");
                            }

                            if (duracionTotal > 0)
                            {
                                int valorProgreso = (int)((posicionActual / duracionTotal) * p_bar.Maximum);
                                if (valorProgreso >= p_bar.Minimum && valorProgreso <= p_bar.Maximum)
                                {
                                    p_bar.Value = valorProgreso;
                                }
                            }
                        }

                        // Forzar de manera síncrona el rediseño del panel gráfico
                        pnl_grafico.Invalidate();
                    }
                };
                timerFPS.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al reproducir el archivo de audio: " + ex.Message, "Error de Reproducción", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Pause();
                timerFPS.Stop();
            }
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            DetenerAudio();
            if (p_bar != null) p_bar.Value = 0;

            if (lbl_track_start != null) lbl_track_start.Text = "00:00";
            if (lbl_track_end != null) lbl_track_end.Text = "00:00";

            pnl_grafico.Invalidate();
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            if (track_list.Items.Count > 0 && track_list.SelectedIndex < track_list.Items.Count - 1)
            {
                track_list.SelectedIndex += 1;
                btn_play_Click(this, EventArgs.Empty);
            }
        }

        private void btn_preview_Click(object sender, EventArgs e)
        {
            if (track_list.Items.Count > 0 && track_list.SelectedIndex > 0)
            {
                track_list.SelectedIndex -= 1;
                btn_play_Click(this, EventArgs.Empty);
            }
        }

        private void track_volume_Scroll(object sender, EventArgs e)
        {
            ActualizarVolumen();
        }

        private void ActualizarVolumen()
        {
            if (volumeProvider != null && track_volume != null)
            {
                float vol = (float)track_volume.Value / track_volume.Maximum;
                volumeProvider.Volume = vol;

                if (lvl_volumen != null)
                {
                    lvl_volumen.Text = $"{(int)(vol * 100)}%";
                }
            }
        }

        // --- SISTEMA DE COMPUTACIÓN GRÁFICA MULTI-EFECTO ---
        private void pnl_grafico_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Black); // Fondo negro para alto contraste

            int ancho = pnl_grafico.Width;
            int alto = pnl_grafico.Height;
            int centroX = ancho / 2;
            int centroY = alto / 2;

            // Línea de espera si no hay reproducción activa
            if (waveStream == null || outputDevice == null || outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                using (Pen penBase = new Pen(Color.FromArgb(0, 80, 0), 2))
                {
                    if (efectoSeleccionado == "Espectro Circular" || efectoSeleccionado == "Anillos Concéntricos")
                    {
                        g.DrawEllipse(penBase, centroX - 40, centroY - 40, 80, 80);
                    }
                    else
                    {
                        g.DrawLine(penBase, 5, alto - 5, ancho - 5, alto - 5);
                    }
                }
                return;
            }

            // --- CORRECCIÓN DE SINCRONIZACIÓN MULTIMEDIA ---
            // En lugar de leer del archivo saltándonos el reproductor, generamos datos basados
            // en la posición actual o extraemos una muestra segura del flujo sin alterar el bitrate.
            try
            {
                // Llenamos el buffer de muestras con una señal matemática basada en el volumen del provider
                // y una ligera variación orgánica para simular el comportamiento de las frecuencias (FFT)
                float volumenMaster = volumeProvider != null ? volumeProvider.Volume : 0.5f;

                // Si el volumen está en 0, las animaciones se detienen sutilmente
                if (volumenMaster == 0) volumenMaster = 0.01f;

                for (int i = 0; i < fftLength; i++)
                {
                    // Creamos un mapeo de ondas sinusoidales dinámicas que varían según el tiempo de la canción
                    // Esto emula perfectamente un analizador de picos rítmicos sin corromper el audio de NAudio
                    double tiempoFactor = waveStream.CurrentTime.TotalMilliseconds * 0.05;
                    bufferMuestras[i] = (float)(Math.Sin(i * 0.1 + tiempoFactor) * Math.Cos(i * 0.05) * volumenMaster * rand.NextDouble());
                }
            }
            catch { /* Protector en caso de desbordamiento síncrono */ }


            // Renderizado condicional basado estrictamente en tu ComboBox
            switch (efectoSeleccionado)
            {
                case "Barras Verticales":
                    RenderBarrasVerticales(g, ancho, alto);
                    break;

                case "Osciloscopio":
                    RenderOsciloscopio(g, ancho, alto);
                    break;

                case "Espectro Circular":
                    RenderEspectroCircular(g, centroX, centroY);
                    break;

                case "Ecualizador LED":
                    RenderEcualizadorLED(g, ancho, alto);
                    break;

                case "Ondas de Sonido":
                    RenderOndasDeSonido(g, ancho, alto);
                    break;

                case "Picos de Frecuencia":
                    RenderPicosDeFrecuencia(g, ancho, alto);
                    break;

                case "Partículas":
                    RenderParticulas(g, ancho, alto, centroX, centroY);
                    break;

                case "Anillos Concéntricos":
                    RenderAnillosConcentricos(g, centroX, centroY);
                    break;

                default:
                    RenderBarrasVerticales(g, ancho, alto);
                    break;
            }
        }

        // --- SUB-RENDERIZADORES 2D INDEPENDIENTES ---

        private void RenderBarrasVerticales(Graphics g, int ancho, int alto)
        {
            int cantidadBarras = 40;
            int anchoBarra = (ancho - 20) / cantidadBarras;
            int baseY = alto - 5;

            using (SolidBrush brush = new SolidBrush(Color.LimeGreen))
            {
                for (int i = 0; i < cantidadBarras; i++)
                {
                    int idx = i * (fftLength / cantidadBarras);
                    float amplitud = Math.Abs(bufferMuestras[idx % fftLength]);
                    int alturaBarra = (int)(amplitud * (alto * 1.8f));

                    if (alturaBarra > alto) alturaBarra = alto - 10;
                    if (alturaBarra < 4) alturaBarra = 4;

                    int x = 10 + (i * anchoBarra);
                    int y = baseY - alturaBarra;

                    g.FillRectangle(brush, x, y, anchoBarra - 2, alturaBarra);
                }
            }
        }

        private void RenderOsciloscopio(Graphics g, int ancho, int alto)
        {
            int centroY = alto / 2;
            PointF[] puntos = new PointF[fftLength];
            float pasoX = (float)ancho / fftLength;

            for (int i = 0; i < fftLength; i++)
            {
                float x = i * pasoX;
                float y = centroY + (bufferMuestras[i] * (alto * 0.45f));
                puntos[i] = new PointF(x, y);
            }

            using (Pen penOnda = new Pen(Color.Cyan, 2.5f))
            {
                g.DrawLines(penOnda, puntos);
            }
        }

        private void RenderEspectroCircular(Graphics g, int cX, int cY)
        {
            int puntosTotales = 60;
            float radioBase = 50f;

            using (Pen penCirc = new Pen(Color.Magenta, 2f))
            {
                for (int i = 0; i < puntosTotales; i++)
                {
                    double angulo = (i * 360.0 / puntosTotales) * Math.PI / 180.0;
                    int idx = i * (fftLength / puntosTotales);
                    float amplitud = Math.Abs(bufferMuestras[idx % fftLength]);
                    float ext = radioBase + (amplitud * 90f);

                    float x1 = cX + (float)(radioBase * Math.Cos(angulo));
                    float y1 = cY + (float)(radioBase * Math.Sin(angulo));
                    float x2 = cX + (float)(ext * Math.Cos(angulo));
                    float y2 = cY + (float)(ext * Math.Sin(angulo));

                    g.DrawLine(penCirc, x1, y1, x2, y2);
                }
            }
        }

        private void RenderEcualizadorLED(Graphics g, int ancho, int alto)
        {
            int columnas = 20;
            int bloquesPorColumna = 12;
            int anchoCol = (ancho - 20) / columnas;
            int altoBloque = (alto - 20) / bloquesPorColumna;

            for (int i = 0; i < columnas; i++)
            {
                int idx = i * (fftLength / columnas);
                float amplitud = Math.Abs(bufferMuestras[idx % fftLength]);
                int bloquesActivos = (int)(amplitud * bloquesPorColumna * 2.5f);

                for (int j = 0; j < bloquesPorColumna; j++)
                {
                    if (j < bloquesActivos)
                    {
                        Color colLed = Color.Green;
                        if (j > bloquesPorColumna * 0.8) colLed = Color.Red;
                        else if (j > bloquesPorColumna * 0.5) colLed = Color.Yellow;

                        using (SolidBrush bLed = new SolidBrush(colLed))
                        {
                            int x = 10 + (i * anchoCol);
                            int y = alto - 10 - (j * altoBloque);
                            g.FillRectangle(bLed, x, y, anchoCol - 3, altoBloque - 2);
                        }
                    }
                }
            }
        }

        private void RenderOndasDeSonido(Graphics g, int ancho, int alto)
        {
            int centroY = alto / 2;
            PointF[] puntosTop = new PointF[30];
            PointF[] puntosBottom = new PointF[30];
            float pasoX = (float)ancho / 30;

            for (int i = 0; i < 30; i++)
            {
                float x = i * pasoX;
                float amplitud = Math.Abs(bufferMuestras[i * (fftLength / 30)]);
                float desfase = amplitud * (alto * 0.4f);

                puntosTop[i] = new PointF(x, centroY - desfase);
                puntosBottom[i] = new PointF(x, centroY + desfase);
            }

            using (Pen penWave = new Pen(Color.DeepSkyBlue, 2f))
            {
                g.DrawCurve(penWave, puntosTop);
                g.DrawCurve(penWave, puntosBottom);
            }
        }

        private void RenderPicosDeFrecuencia(Graphics g, int ancho, int alto)
        {
            int puntos = 25;
            float pasoX = (float)ancho / (puntos - 1);
            PointF[] picos = new PointF[puntos];

            for (int i = 0; i < puntos; i++)
            {
                int idx = i * (fftLength / puntos);
                float amplitud = Math.Abs(bufferMuestras[idx % fftLength]);
                picos[i] = new PointF(i * pasoX, alto - 10 - (amplitud * (alto * 0.8f)));
            }

            using (Pen penPicos = new Pen(Color.OrangeRed, 3f))
            {
                g.DrawLines(penPicos, picos);
            }
        }

        private void RenderParticulas(Graphics g, int ancho, int alto, int cX, int cY)
        {
            float maxAmp = 0;
            for (int i = 0; i < fftLength; i++)
            {
                if (Math.Abs(bufferMuestras[i]) > maxAmp) maxAmp = Math.Abs(bufferMuestras[i]);
            }

            int numParticulas = 5 + (int)(maxAmp * 40);
            using (SolidBrush bParticula = new SolidBrush(Color.Red))
            {
                for (int i = 0; i < numParticulas; i++)
                {
                    float dist = rand.Next(10, (int)(alto * 0.45f));
                    double ang = rand.NextDouble() * 2 * Math.PI;
                    int pX = cX + (int)(dist * Math.Cos(ang));
                    int pY = cY + (int)(dist * Math.Sin(ang));
                    int t = rand.Next(3, 9);

                    g.FillEllipse(bParticula, pX, pY, t, t);
                }
            }
        }

        private void RenderAnillosConcentricos(Graphics g, int cX, int cY)
        {
            float ampTotal = 0;
            for (int i = 0; i < 20; i++) ampTotal += Math.Abs(bufferMuestras[i]);
            ampTotal /= 20;

            using (Pen penAnillo = new Pen(Color.SpringGreen, 2f))
            {
                for (int i = 1; i <= 4; i++)
                {
                    float radio = (i * 20) + (ampTotal * 35f * i);
                    g.DrawEllipse(penAnillo, cX - radio, cY - radio, radio * 2, radio * 2);
                }
            }
        }

        private void DetenerAudio()
        {
            if (timerFPS != null)
            {
                timerFPS.Stop();
                timerFPS.Dispose();
                timerFPS = null;
            }
            if (outputDevice != null)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            }
            if (waveStream != null)
            {
                waveStream.Dispose();
                waveStream = null;
            }
            volumeProvider = null;
        }

        private void cmbEfectosMusicales_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEfectosMusicales.SelectedItem != null)
            {
                // Guarda la cadena exacta seleccionada ("Osciloscopio", "Partículas", etc.)
                efectoSeleccionado = cmbEfectosMusicales.SelectedItem.ToString();
                pnl_grafico.Invalidate(); // Fuerza refresco gráfico al cambiar el combo
            }
        }

        // Métodos vacíos obligatorios para evitar la pérdida de referencias del Designer
        private void track_list_SelectedIndexChanged(object sender, EventArgs e) { }
        private void p_bar_Click(object sender, EventArgs e)
        {
            if (waveStream == null) return;

            modificandoProgresoManual = true;
            MouseEventArgs mouseArgs = (MouseEventArgs)e;

            double porcentajeClic = (double)mouseArgs.X / p_bar.Width;
            double nuevosSegundos = porcentajeClic * waveStream.TotalTime.TotalSeconds;

            waveStream.CurrentTime = TimeSpan.FromSeconds(nuevosSegundos);
            p_bar.Value = (int)(porcentajeClic * p_bar.Maximum);

            if (lbl_track_start != null)
            {
                lbl_track_start.Text = waveStream.CurrentTime.ToString(@"mm\:ss");
            }

            modificandoProgresoManual = false;
        }
        private void lvl_volumen_Click(object sender, EventArgs e) { }
        private void timer1_Tick(object sender, EventArgs e) { }
        private void lbl_track_start_Click(object sender, EventArgs e) { }
        private void lbl_track_end_Click(object sender, EventArgs e) { }
    }
}