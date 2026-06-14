using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ReproductorMusical.EfectosVisuales;
using ReproductorMusical.Modelo;

namespace ReproductorMusical.Controlador
{
    /// <summary>
    /// Orquesta el modelo, el gestor de playlist y los efectos visuales.
    /// Solo coordina: no sabe nada de controles WinForms ni de colores de UI.
    /// </summary>
    public class ReproductorControlador
    {
        // ── Dependencias ─────────────────────────────────────────────────
        private readonly ReproductorModelo _modelo;
        private readonly GestorPlaylist _playlist;

        // ── Efectos visuales ─────────────────────────────────────────────
        private readonly Dictionary<string, IEfectoVisual> _efectos
            = new Dictionary<string, IEfectoVisual>();
        private IEfectoVisual _efectoActual;

        // ── Timer de animación (hilo UI, sin threads secundarios) ────────
        private Timer _timerFPS;

        // ── Buffer de audio simulado para la visualización ───────────────
        private const int FFT_LENGTH = 128;
        private readonly float[] _bufferMuestras = new float[FFT_LENGTH];
        private readonly Random _rand = new Random();

        // ── Flag que evita detección falsa de fin de pista ───────────────
        private bool _cambiandoPista = false;

        // ── Estado de reproducción ───────────────────────────────────────
        public ModoReproduccion ModoReproduccion { get; set; } = ModoReproduccion.Secuencial;

        // ── Color de fondo del panel (cedido por la Vista vía propiedad) ─
        // Se mantiene aquí únicamente para pasárselo al método de renderizado
        // sin que la Vista tenga que pasarlo en cada Paint.
        public Color ColorFondoGrafico { get; set; } = Color.FromArgb(20, 20, 20);

        // ── Consulta pública del índice activo ───────────────────────────
        public int IndicePistaActual => _playlist.IndicePistaActual;

        // ── Eventos hacia la Vista ───────────────────────────────────────
        // Usando 'event' para que solo este controlador pueda dispararlos.
        public event Action<string> OnTiempoActualizado;
        public event Action<string> OnDuracionActualizada;
        public event Action<int> OnProgresoActualizado;
        public event Action OnRedibujarGrafico;
        public event Action<string> OnErrorReproduccion;
        public event Action OnStopReproduccion;
        public event Action OnSincronizarLista;
        public event Action<Image> OnAlbumCambiado;

        // ── Constructor ──────────────────────────────────────────────────
        public ReproductorControlador(ReproductorModelo modelo)
        {
            _modelo = modelo;
            _playlist = new GestorPlaylist();
            RegistrarEfectos();
        }

        // ── Registro de efectos ──────────────────────────────────────────

        private void RegistrarEfectos()
        {
            Registrar(new EfectoBarrasVerticales());
            Registrar(new EfectoOsciloscopio());
            Registrar(new EfectoEspectroCircular());
            Registrar(new EfectoEcualizadorLED());
            Registrar(new EfectoOndasDeSonido());
            Registrar(new EfectoPicosDeFrecuencia());
            Registrar(new EfectoParticulas());
            Registrar(new EfectoAnillosConcentricos());

            _efectoActual = new EfectoBarrasVerticales();
        }

        private void Registrar(IEfectoVisual efecto)
        {
            _efectos[efecto.Nombre] = efecto;
        }

        // ── Consultas para la Vista ──────────────────────────────────────

        public List<string> ObtenerNombresEfectos()
        {
            return new List<string>(_efectos.Keys);
        }

        public List<string> ObtenerNombresPistas()
        {
            return _playlist.ObtenerNombresPistas();
        }

        /// <summary>
        /// Expone la lista completa de pistas (solo lectura) para que la Vista
        /// pueda leer Artista y Album al dibujar el ListBox con OwnerDraw.
        /// </summary>
        public IReadOnlyList<Modelo.PistaMusical> ObtenerTodasLasPistas()
        {
            return _playlist.ObtenerTodasLasPistas();
        }

        // ── Gestión de la playlist ───────────────────────────────────────

        /// <summary>
        /// Agrega pistas nuevas a la lista. Si ya había pistas, se añaden al final.
        /// Llama a LimpiarYAgregar si quieres reemplazar la lista completa.
        /// </summary>
        public void AgregarPistas(string[] rutas)
        {
            _playlist.AgregarPistas(rutas);
        }

        /// <summary>Reemplaza la lista completa con nuevas pistas.</summary>
        public void ReemplazarPistas(string[] rutas)
        {
            _playlist.LimpiarPistas();
            _playlist.AgregarPistas(rutas);
        }

        // ── Reproducción ─────────────────────────────────────────────────

        public void Play(int indicePista)
        {
            if (!_playlist.SeleccionarPista(indicePista)) return;

            // Si es la misma pista pausada, solo reanudamos
            if (_modelo.EstaPausado && indicePista == _playlist.IndicePistaActual
                && _bufferMuestras != null)
            {
                _modelo.Reanudar();
                IniciarTimer();
                return;
            }

            _cambiandoPista = true;

            try
            {
                PistaMusical pista = _playlist.PistaActual;

                _modelo.Reproducir(pista.RutaCompleta);
                _playlist.ActualizarDuracionActual(_modelo.DuracionTotal);

                OnDuracionActualizada?.Invoke(_modelo.DuracionTotal.ToString(@"mm\:ss"));
                OnSincronizarLista?.Invoke();
                OnAlbumCambiado?.Invoke(pista.Portada);

                IniciarTimer();
            }
            catch (Exception ex)
            {
                OnErrorReproduccion?.Invoke(ex.Message);
            }
            finally
            {
                _cambiandoPista = false;
            }
        }

        public void Pause()
        {
            _modelo.Pausar();
            DetenerTimer();
        }

        public void Stop()
        {
            _modelo.Detener();
            DetenerTimer();

            OnTiempoActualizado?.Invoke("00:00");
            OnDuracionActualizada?.Invoke("00:00");
            OnProgresoActualizado?.Invoke(0);
            OnRedibujarGrafico?.Invoke();
            OnStopReproduccion?.Invoke();
        }

        public void Next()
        {
            if (!_playlist.HayPistas) return;
            Play(_playlist.CalcularSiguiente(ModoReproduccion));
        }

        public void Previous()
        {
            if (!_playlist.HayPistas) return;
            Play(_playlist.CalcularAnterior(ModoReproduccion));
        }

        public void CambiarVolumen(int valorTrackBar, int maximoTrackBar)
        {
            float volNormalizado = (float)valorTrackBar / maximoTrackBar;
            _modelo.CambiarVolumen(volNormalizado);
        }

        public void CambiarPosicion(int clickX, int anchoPBar)
        {
            if (!_modelo.EstaReproduciendo && !_modelo.EstaPausado) return;

            double porcentaje = (double)clickX / anchoPBar;
            double nuevoSegundo = porcentaje * _modelo.DuracionTotal.TotalSeconds;
            _modelo.CambiarPosicion(nuevoSegundo);

            OnProgresoActualizado?.Invoke((int)(porcentaje * 100));
            OnTiempoActualizado?.Invoke(_modelo.TiempoActual.ToString(@"mm\:ss"));
        }

        public void CambiarEfecto(string nombre)
        {
            if (_efectos.ContainsKey(nombre))
                _efectoActual = _efectos[nombre];
        }

        // ── Renderizado ──────────────────────────────────────────────────

        public void RenderizarEfecto(Graphics g, int ancho, int alto)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(ColorFondoGrafico);

            if (!_modelo.EstaReproduciendo && !_modelo.EstaPausado) return;

            ActualizarBuffer();
            _efectoActual?.Renderizar(g, ancho, alto, _bufferMuestras);
        }

        // ── Timer ────────────────────────────────────────────────────────

        private void IniciarTimer()
        {
            DetenerTimer();
            _timerFPS = new Timer();
            _timerFPS.Interval = 16; // ~60 FPS
            _timerFPS.Tick += TimerTick;
            _timerFPS.Start();
        }

        private void DetenerTimer()
        {
            if (_timerFPS == null) return;
            _timerFPS.Stop();
            _timerFPS.Dispose();
            _timerFPS = null;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (_cambiandoPista) return;

            // Detección de fin de pista por polling
            if (_playlist.IndicePistaActual >= 0
                && !_modelo.EstaReproduciendo
                && !_modelo.EstaPausado)
            {
                Next();
                return;
            }

            if (_modelo.EstaReproduciendo)
            {
                double posicion = _modelo.TiempoActual.TotalSeconds;
                double duracion = _modelo.DuracionTotal.TotalSeconds;

                OnTiempoActualizado?.Invoke(_modelo.TiempoActual.ToString(@"mm\:ss"));

                if (duracion > 0)
                    OnProgresoActualizado?.Invoke((int)((posicion / duracion) * 100));
            }

            OnRedibujarGrafico?.Invoke();
        }

        // ── Buffer de visualización ──────────────────────────────────────

        private void ActualizarBuffer()
        {
            float volumen = _modelo.Volumen;
            if (volumen == 0f) volumen = 0.01f;

            double tiempoFactor = _modelo.TiempoActual.TotalMilliseconds * 0.05;

            for (int i = 0; i < FFT_LENGTH; i++)
            {
                _bufferMuestras[i] = (float)(
                    Math.Sin(i * 0.1 + tiempoFactor) *
                    Math.Cos(i * 0.05) *
                    volumen *
                    _rand.NextDouble()
                );
            }
        }
    }
}