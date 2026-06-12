using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ReproductorMusical.EfectosVisuales;
using ReproductorMusical.Modelo;

namespace ReproductorMusical.Controlador
{
    // Orquesta el Modelo y la Vista; contiene el Timer de animación
    public class ReproductorControlador
    {
        // DEPENDENCIAS
        private readonly ReproductorModelo modelo;

        // ESTADO INTERNO
        private List<PistaMusical> listaPistas = new List<PistaMusical>();
        private int indicePistaActual = -1;
        private Dictionary<string, IEfectoVisual> efectos = new Dictionary<string, IEfectoVisual>();
        private IEfectoVisual efectoActual;
        private Timer timerFPS;
        private Random rand = new Random();

        // BUFFER DE AUDIO SIMULADO
        private const int FFT_LENGTH = 128;
        private float[] bufferMuestras = new float[FFT_LENGTH];

        // CALLBACKS HACIA LA VISTA (evita acoplamiento directo)
        public Action<string> OnTiempoActualizado;      // "mm:ss" del tiempo actual
        public Action<string> OnDuracionActualizada;    // "mm:ss" de la duración total
        public Action<int> OnProgresoActualizado;       // valor 0-100 para el ProgressBar
        public Action OnRedibujarGrafico;               // pide Invalidate() al panel
        public Action<string> OnErrorReproduccion;      // muestra errores al usuario

        // COLOR DE FONDO DEL PANEL DE VISUALIZACION (depende del tema)
        public Color ColorFondoGrafico = Color.FromArgb(30, 30, 30);

        public ReproductorControlador(ReproductorModelo modelo)
        {
            this.modelo = modelo;
            RegistrarEfectos();
        }

        // Registra cada efecto en el diccionario con su nombre como clave
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

            // Efecto por defecto
            efectoActual = new EfectoBarrasVerticales();
        }

        private void AgregarEfecto(IEfectoVisual efecto)
        {
            efectos[efecto.Nombre] = efecto;
        }

        // Devuelve la lista de nombres para llenar el ComboBox
        public List<string> ObtenerNombresEfectos()
        {
            List<string> nombres = new List<string>();
            foreach (string clave in efectos.Keys)
                nombres.Add(clave);
            return nombres;
        }

        // Cambia el efecto activo según el nombre recibido desde el ComboBox
        public void CambiarEfecto(string nombre)
        {
            if (efectos.ContainsKey(nombre))
                efectoActual = efectos[nombre];
        }

        // Agrega una o más rutas a la lista interna de pistas
        public void AgregarPistas(string[] rutas)
        {
            foreach (string ruta in rutas)
                listaPistas.Add(new PistaMusical(ruta));
        }

        // Devuelve los nombres de archivo para poblar el ListBox
        public List<string> ObtenerNombresPistas()
        {
            List<string> nombres = new List<string>();
            foreach (PistaMusical p in listaPistas)
                nombres.Add(p.NombreArchivo);
            return nombres;
        }

        // Inicia o reanuda la reproducción del índice indicado
        public void Play(int indicePista)
        {
            if (indicePista < 0 || indicePista >= listaPistas.Count)
                return;

            // Si está pausada la misma pista, solo reanudar
            if (modelo.EstaPausado && indicePista == indicePistaActual)
            {
                modelo.Reanudar();
                IniciarTimer();
                return;
            }

            try
            {
                indicePistaActual = indicePista;
                modelo.Reproducir(listaPistas[indicePista].RutaCompleta);

                // Actualiza la duración total en la vista
                listaPistas[indicePista].Duracion = modelo.DuracionTotal;
                if (OnDuracionActualizada != null)
                    OnDuracionActualizada(modelo.DuracionTotal.ToString(@"mm\:ss"));

                IniciarTimer();
            }
            catch (Exception ex)
            {
                if (OnErrorReproduccion != null)
                    OnErrorReproduccion(ex.Message);
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
        }

        public void Next()
        {
            if (indicePistaActual < listaPistas.Count - 1)
                Play(indicePistaActual + 1);
        }

        public void Previous()
        {
            if (indicePistaActual > 0)
                Play(indicePistaActual - 1);
        }

        // Ajusta el volumen; valor entre 0 y 100 (desde el TrackBar)
        public void CambiarVolumen(int valorTrackBar, int maximoTrackBar)
        {
            float volNormalizado = (float)valorTrackBar / maximoTrackBar;
            modelo.CambiarVolumen(volNormalizado);
        }

        // Salta a una posición según el clic en el ProgressBar
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

        // Renderiza el efecto activo sobre el Graphics del panel
        public void RenderizarEfecto(Graphics g, int ancho, int alto)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(ColorFondoGrafico);

            if (!modelo.EstaReproduciendo && !modelo.EstaPausado)
            {
                DibujarLineaEspera(g, ancho, alto);
                return;
            }

            ActualizarBuffer();

            if (efectoActual != null)
                efectoActual.Renderizar(g, ancho, alto, bufferMuestras);
        }

        // Devuelve el índice actual para sincronizar el ListBox desde la vista
        public int IndicePistaActual => indicePistaActual;

        // PRIVADOS DE APOYO

        private void IniciarTimer()
        {
            DetenerTimer();
            timerFPS = new Timer();
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
            if (!modelo.EstaReproduciendo) return;

            double posicion = modelo.TiempoActual.TotalSeconds;
            double duracion = modelo.DuracionTotal.TotalSeconds;

            if (OnTiempoActualizado != null)
                OnTiempoActualizado(modelo.TiempoActual.ToString(@"mm\:ss"));

            if (duracion > 0 && OnProgresoActualizado != null)
                OnProgresoActualizado((int)((posicion / duracion) * 100));

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

        private void DibujarLineaEspera(Graphics g, int ancho, int alto)
        {
            Color colorLinea = ColorFondoGrafico == Color.FromArgb(30, 30, 30)
                ? Color.FromArgb(0, 80, 0)
                : Color.FromArgb(0, 150, 0);

            using (Pen penBase = new Pen(colorLinea, 2))
            {
                string nombre = efectoActual != null ? efectoActual.Nombre : "";
                if (nombre == "Espectro Circular" || nombre == "Anillos Concéntricos")
                    g.DrawEllipse(penBase, ancho / 2 - 40, alto / 2 - 40, 80, 80);
                else
                    g.DrawLine(penBase, 5, alto - 5, ancho - 5, alto - 5);
            }
        }
    }
}