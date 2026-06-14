using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ReproductorMusical.Modelo
{
    // AGREGADOR DE MUESTRAS
    // Captura bloques del pipeline de audio y calcula RMS + pico por banda
    public class SampleAggregator : ISampleProvider
    {
        private readonly ISampleProvider _fuente;
        private readonly int _ventana;
        private readonly float[] _acumulador;
        private int _posicion;

        // Buffer público que el Controlador lee en cada frame
        public readonly float[] BandasFFT = new float[128];

        public WaveFormat WaveFormat => _fuente.WaveFormat;

        public SampleAggregator(ISampleProvider fuente, int ventana = 1024)
        {
            _fuente = fuente;
            _ventana = ventana;
            _acumulador = new float[ventana];
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int leidos = _fuente.Read(buffer, offset, count);

            for (int i = 0; i < leidos; i++)
            {
                _acumulador[_posicion % _ventana] = buffer[offset + i];
                _posicion++;

                // Cada vez que llenamos la ventana, recalculamos las bandas
                if (_posicion % _ventana == 0)
                    CalcularBandas();
            }

            return leidos;
        }

        private void CalcularBandas()
        {
            int bandas = BandasFFT.Length;
            int porBanda = _ventana / bandas;

            for (int b = 0; b < bandas; b++)
            {
                float suma = 0f;
                int inicio = b * porBanda;

                for (int i = 0; i < porBanda; i++)
                    suma += Math.Abs(_acumulador[inicio + i]);

                // Suavizado exponencial: 70% nuevo, 30% anterior
                BandasFFT[b] = (suma / porBanda) * 0.7f + BandasFFT[b] * 0.3f;
            }
        }
    }

    public class ReproductorModelo
    {
        // ESTADO INTERNO
        private WaveStream _waveStream;
        private VolumeSampleProvider _volumeProvider;
        private SampleAggregator _aggregator;
        private IWavePlayer _outputDevice;
        private float _volumenActual = 1.0f;

        // PROPIEDADES DE CONSULTA
        public bool EstaReproduciendo =>
            _outputDevice != null && _outputDevice.PlaybackState == PlaybackState.Playing;
        public bool EstaPausado =>
            _outputDevice != null && _outputDevice.PlaybackState == PlaybackState.Paused;
        public TimeSpan TiempoActual => _waveStream != null ? _waveStream.CurrentTime : TimeSpan.Zero;
        public TimeSpan DuracionTotal => _waveStream != null ? _waveStream.TotalTime : TimeSpan.Zero;
        public float Volumen => _volumenActual;

        // Acceso al buffer de bandas reales (null si no hay reproducción)
        public float[] ObtenerBandas()
        {
            if (_aggregator == null) return null;
            float[] bandas = _aggregator.BandasFFT;
            float[] bandasEscaladas = new float[bandas.Length];
            for (int i = 0; i < bandas.Length; i++)
                bandasEscaladas[i] = bandas[i] * _volumenActual;
            return bandasEscaladas;
        }

        // REPRODUCCIÓN
        public void Reproducir(string rutaCompleta)
        {
            Detener();

            if (rutaCompleta.ToLower().EndsWith(".mp3"))
                _waveStream = new Mp3FileReader(rutaCompleta);
            else if (rutaCompleta.ToLower().EndsWith(".wav"))
                _waveStream = new WaveFileReader(rutaCompleta);
            else
                throw new NotSupportedException("Formato de audio no soportado.");

            ISampleProvider baseProvider = _waveStream.ToSampleProvider();
            _aggregator = new SampleAggregator(baseProvider, 1024);
            _volumeProvider = new VolumeSampleProvider(_aggregator);
            _volumeProvider.Volume = _volumenActual;

            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(_volumeProvider);
            _outputDevice.Play();
        }

        public void Reanudar() { if (EstaPausado) _outputDevice.Play(); }

        public void Pausar() { if (EstaReproduciendo) _outputDevice.Pause(); }

        public void Detener()
        {
            if (_outputDevice != null) { _outputDevice.Stop(); _outputDevice.Dispose(); _outputDevice = null; }
            if (_waveStream != null) { _waveStream.Dispose(); _waveStream = null; }
            _volumeProvider = null;
            _aggregator = null;
        }

        public void CambiarVolumen(float valor)
        {
            _volumenActual = Math.Max(0f, Math.Min(1f, valor));
            if (_volumeProvider != null) _volumeProvider.Volume = _volumenActual;
        }

        public void CambiarPosicion(double segundos)
        {
            if (_waveStream != null)
                _waveStream.CurrentTime = TimeSpan.FromSeconds(segundos);
        }
    }
}