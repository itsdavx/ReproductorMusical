using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ReproductorMusical.EfectosVisuales;
using ReproductorMusical.Modelo;

namespace ReproductorMusical.Controlador
{
    public class ReproductorControlador
    {
        // DEPENDENCIAS
        private readonly ReproductorModelo modelo;
        public Action OnStopReproduccion;
        public int ModoReproduccion { get; set; } = 1;
        public Action<string> OnAlbumCambiado;

        // ESTADO INTERNO
        private List<PistaMusical> listaPistas = new List<PistaMusical>();
        private int indicePistaActual = -1;
        private Dictionary<string, IEfectoVisual> efectos = new Dictionary<string, IEfectoVisual>();
        private IEfectoVisual efectoActual;
        private System.Windows.Forms.Timer timerFPS;
        private Random rand = new Random();

        // FLAG: true mientras Play() está ejecutándose, para que el timer
        // no detecte un "fin de pista" falso durante el cambio de canción
        private bool cambiandoPista = false;

        // BUFFER DE AUDIO SIMULADO
        private const int FFT_LENGTH = 128;
        private float[] bufferMuestras = new float[FFT_LENGTH];

        // CALLBACKS HACIA LA VISTA
        public Action<string> OnTiempoActualizado;
        public Action<string> OnDuracionActualizada;
        public Action<int> OnProgresoActualizado;
        public Action OnRedibujarGrafico;
        public Action<string> OnErrorReproduccion;
        public Action OnSincronizarLista;   // nuevo: avisa a la vista que sincronice el ListBox

        // COLOR DE FONDO DEL PANEL
        public Color ColorFondoGrafico = Color.FromArgb(20, 20, 20);

        public ReproductorControlador(ReproductorModelo modelo)
        {
            this.modelo = modelo;
            RegistrarEfectos();
            // Sin suscripción a OnTrackFinished — el timer lo maneja todo
        }

        // REGISTRO DE EFECTOS

        private void RegistrarEfectos()
        {
            AgregarEfecto(new EfectoBarrasVerticales());
            AgregarEfecto(new EfectoOsciloscopio());
            AgregarEfecto(new EfectoEspectroCircular());
            AgregarEfecto(new EfectoEcualizadorLED());
            AgregarEfecto(new EfectoOndasDeSonido());
            AgregarEfecto(new EfectoPicosDeFrecuencia());
            AgregarEfecto(new EfectoParticulas());
            AgregarEfecto(new EfectoAnillosConcentricos());

            efectoActual = new EfectoBarrasVerticales();
        }

        private void AgregarEfecto(IEfectoVisual efecto)
        {
            efectos[efecto.Nombre] = efecto;
        }

        // CONSULTAS PARA LA VISTA

        public List<string> ObtenerNombresEfectos()
        {
            List<string> nombres = new List<string>();
            foreach (string clave in efectos.Keys)
                nombres.Add(clave);
            return nombres;
        }

        public void CambiarEfecto(string nombre)
        {
            if (efectos.ContainsKey(nombre))
                efectoActual = efectos[nombre];
        }

        public void AgregarPistas(string[] rutas)
        {
            listaPistas.Clear();
            indicePistaActual = -1;

            foreach (string ruta in rutas)
                listaPistas.Add(new PistaMusical(ruta));
        }

        public List<string> ObtenerNombresPistas()
        {
            List<string> nombres = new List<string>();
            foreach (PistaMusical p in listaPistas)
                nombres.Add(p.NombreArchivo);
            return nombres;
        }

        // REPRODUCCIÓN

        // En Play(), después de IniciarTimer() agregar:
        public void Play(int indicePista)
        {
            if (indicePista < 0 || indicePista >= listaPistas.Count)
                return;

            if (modelo.EstaPausado && indicePista == indicePistaActual)
            {
                modelo.Reanudar();
                IniciarTimer();
                return;
            }

            cambiandoPista = true;

            try
            {
                indicePistaActual = indicePista;
                modelo.Reproducir(listaPistas[indicePista].RutaCompleta);

                listaPistas[indicePista].Duracion = modelo.DuracionTotal;

                if (OnDuracionActualizada != null)
                    OnDuracionActualizada(modelo.DuracionTotal.ToString(@"mm\:ss"));

                if (OnSincronizarLista != null)
                    OnSincronizarLista();

                // Notifica el nombre del álbum para que la vista cargue la imagen
                if (OnAlbumCambiado != null)
                    OnAlbumCambiado(listaPistas[indicePista].NombreAlbum);

                IniciarTimer();
            }
            catch (Exception ex)
            {
                if (OnErrorReproduccion != null)
                    OnErrorReproduccion(ex.Message);
            }
            finally
            {
                cambiandoPista = false;
            }
        }

        public void Pause()
        {
            modelo.Pausar();
            DetenerTimer();
        }

        public void Stop()
        {
            modelo.Detener();
            DetenerTimer();

            if (OnTiempoActualizado != null) OnTiempoActualizado("00:00");
            if (OnDuracionActualizada != null) OnDuracionActualizada("00:00");
            if (OnProgresoActualizado != null) OnProgresoActualizado(0);
            if (OnRedibujarGrafico != null) OnRedibujarGrafico();
            if (OnStopReproduccion != null) OnStopReproduccion();
        }

        public void Next()
        {
            if (listaPistas.Count == 0) return;

            switch (ModoReproduccion)
            {
                case 0: // Aleatorio
                    Play(rand.Next(listaPistas.Count));
                    break;
                case 1: // Secuencial
                    Play(indicePistaActual < listaPistas.Count - 1
                        ? indicePistaActual + 1 : 0);
                    break;
                case 2: // Repetir 1
                    Play(indicePistaActual);
                    break;
            }
        }

        public void Previous()
        {
            if (listaPistas.Count == 0) return;

            switch (ModoReproduccion)
            {
                case 0: // Aleatorio
                    Play(rand.Next(listaPistas.Count));
                    break;
                case 1: // Secuencial
                    Play(indicePistaActual > 0
                        ? indicePistaActual - 1 : listaPistas.Count - 1);
                    break;
                case 2: // Repetir 1
                    Play(indicePistaActual);
                    break;
            }
        }

        public void CambiarVolumen(int valorTrackBar, int maximoTrackBar)
        {
            float volNormalizado = (float)valorTrackBar / maximoTrackBar;
            modelo.CambiarVolumen(volNormalizado);
        }

        public void CambiarPosicion(int clickX, int anchoPBar)
        {
            if (!modelo.EstaReproduciendo && !modelo.EstaPausado) return;

            double porcentaje = (double)clickX / anchoPBar;
            double nuevoSegundo = porcentaje * modelo.DuracionTotal.TotalSeconds;
            modelo.CambiarPosicion(nuevoSegundo);

            if (OnProgresoActualizado != null)
                OnProgresoActualizado((int)(porcentaje * 100));
            if (OnTiempoActualizado != null)
                OnTiempoActualizado(modelo.TiempoActual.ToString(@"mm\:ss"));
        }

        // RENDERIZADO

        public void RenderizarEfecto(Graphics g, int ancho, int alto)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(ColorFondoGrafico);

            if (!modelo.EstaReproduciendo && !modelo.EstaPausado)
                return;

            ActualizarBuffer();

            if (efectoActual != null)
                efectoActual.Renderizar(g, ancho, alto, bufferMuestras);
        }

        public int IndicePistaActual => indicePistaActual;

        // TIMER — corre 100% en el hilo UI, sin hilos secundarios

        private void IniciarTimer()
        {
            DetenerTimer();
            timerFPS = new System.Windows.Forms.Timer();
            timerFPS.Interval = 16; // ~60 FPS
            timerFPS.Tick += TimerTick;
            timerFPS.Start();
        }

        private void DetenerTimer()
        {
            if (timerFPS != null)
            {
                timerFPS.Stop();
                timerFPS.Dispose();
                timerFPS = null;
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            // Ignorar ticks durante cambio de pista
            if (cambiandoPista) return;

            // DETECCIÓN DE FIN DE PISTA por polling:
            // Si había una pista activa y NAudio ya no está reproduciendo ni pausado,
            // significa que la pista terminó naturalmente.
            if (indicePistaActual >= 0
                && !modelo.EstaReproduciendo
                && !modelo.EstaPausado)
            {
                // Avanza a la siguiente según el modo — todo en hilo UI, sin eventos cruzados
                Next();
                return;
            }

            // Actualizar UI normalmente
            if (modelo.EstaReproduciendo)
            {
                double posicion = modelo.TiempoActual.TotalSeconds;
                double duracion = modelo.DuracionTotal.TotalSeconds;

                if (OnTiempoActualizado != null)
                    OnTiempoActualizado(modelo.TiempoActual.ToString(@"mm\:ss"));

                if (duracion > 0 && OnProgresoActualizado != null)
                    OnProgresoActualizado((int)((posicion / duracion) * 100));
            }

            if (OnRedibujarGrafico != null)
                OnRedibujarGrafico();
        }

        private void ActualizarBuffer()
        {
            float volumen = modelo.Volumen;
            if (volumen == 0f) volumen = 0.01f;

            double tiempoFactor = modelo.TiempoActual.TotalMilliseconds * 0.05;

            for (int i = 0; i < FFT_LENGTH; i++)
            {
                bufferMuestras[i] = (float)(
                    Math.Sin(i * 0.1 + tiempoFactor) *
                    Math.Cos(i * 0.05) *
                    volumen *
                    rand.NextDouble()
                );
            }
        }
    }
}