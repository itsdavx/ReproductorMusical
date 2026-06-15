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
        private readonly ReproductorModelo _modelo;
        private readonly GestorPlaylist _playlist;

        private readonly Dictionary<string, IEfectoVisual> _efectos = new Dictionary<string, IEfectoVisual>();
        private IEfectoVisual _efectoActual;

        private Timer _timerFPS;
        private bool _cambiandoPista = false;

        public ModoReproduccion ModoReproduccion { get; set; } = ModoReproduccion.Secuencial;
        public Color ColorFondoGrafico { get; set; } = Color.FromArgb(20, 20, 20);
        public int IndicePistaActual => _playlist.IndicePistaActual;

        public event Action<string> OnTiempoActualizado;
        public event Action<string> OnDuracionActualizada;
        public event Action<int> OnProgresoActualizado;
        public event Action OnRedibujarGrafico;
        public event Action<string> OnErrorReproduccion;
        public event Action OnStopReproduccion;
        public event Action OnSincronizarLista;
        public event Action<Image> OnAlbumCambiado;

        public ReproductorControlador(ReproductorModelo modelo)
        {
            _modelo = modelo;
            _playlist = new GestorPlaylist();
            RegistrarEfectos();
        }

        // Registra todos los efectos visuales disponibles.
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

        private void Registrar(IEfectoVisual efecto) => _efectos[efecto.Nombre] = efecto;

        public List<string> ObtenerNombresEfectos() => new List<string>(_efectos.Keys);
        public List<string> ObtenerNombresPistas() => _playlist.ObtenerNombresPistas();

        // Expone la lista completa de pistas para que la Vista pueda leer Artista y Album.
        public IReadOnlyList<Modelo.PistaMusical> ObtenerTodasLasPistas() => _playlist.ObtenerTodasLasPistas();

        // Agrega pistas al final de la lista actual.
        public void AgregarPistas(string[] rutas) => _playlist.AgregarPistas(rutas);

        // Reemplaza la lista completa con nuevas pistas.
        public void ReemplazarPistas(string[] rutas)
        {
            _playlist.LimpiarPistas();
            _playlist.AgregarPistas(rutas);
        }

        // Reproduce la pista en el índice dado; si está pausada en esa misma pista, la reanuda.
        public void Play(int indicePista)
        {
            if (!_playlist.SeleccionarPista(indicePista)) return;

            if (_modelo.EstaPausado && indicePista == _playlist.IndicePistaActual)
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

        // Detiene la reproducción y resetea todos los indicadores de la UI.
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

        // Limpia el panel y renderiza el efecto actual con las bandas del modelo.
        public void RenderizarEfecto(Graphics g, int ancho, int alto)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(ColorFondoGrafico);

            if (!_modelo.EstaReproduciendo && !_modelo.EstaPausado) return;

            float[] bandas = _modelo.ObtenerBandas();
            if (bandas == null) return;

            _efectoActual?.Renderizar(g, ancho, alto, bandas);
        }

        // Inicia el timer de animación (~60 FPS), deteniéndolo antes si ya corría.
        private void IniciarTimer()
        {
            DetenerTimer();
            _timerFPS = new Timer();
            _timerFPS.Interval = 16;
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

        // Tick del timer: detecta fin de pista por polling y actualiza tiempo y progreso.
        private void TimerTick(object sender, EventArgs e)
        {
            if (_cambiandoPista) return;

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

        public void QuitarPista(int indice) => _playlist.QuitarPista(indice);
    }
}