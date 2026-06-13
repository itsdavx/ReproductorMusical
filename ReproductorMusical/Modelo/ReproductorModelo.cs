using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ReproductorMusical.Modelo
{
    public class ReproductorModelo
    {
        // ESTADO INTERNO
        private WaveStream waveStream;
        private VolumeSampleProvider volumeProvider;
        private IWavePlayer outputDevice;

        // PROPIEDADES DE CONSULTA
        public bool EstaReproduciendo =>
            outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing;
        public bool EstaPausado =>
            outputDevice != null && outputDevice.PlaybackState == PlaybackState.Paused;
        public TimeSpan TiempoActual =>
            waveStream != null ? waveStream.CurrentTime : TimeSpan.Zero;
        public TimeSpan DuracionTotal =>
            waveStream != null ? waveStream.TotalTime : TimeSpan.Zero;
        public float Volumen =>
            volumeProvider != null ? volumeProvider.Volume : 1f;

        // Reproduce una pista desde su ruta en disco
        public void Reproducir(string rutaCompleta)
        {
            // Detiene limpiamente cualquier reproducción anterior
            Detener();

            if (rutaCompleta.ToLower().EndsWith(".mp3"))
                waveStream = new Mp3FileReader(rutaCompleta);
            else if (rutaCompleta.ToLower().EndsWith(".wav"))
                waveStream = new WaveFileReader(rutaCompleta);
            else
                throw new NotSupportedException("Formato de audio no soportado.");

            ISampleProvider sampleProvider = waveStream.ToSampleProvider();
            volumeProvider = new VolumeSampleProvider(sampleProvider);

            outputDevice = new WaveOutEvent();
            outputDevice.Init(volumeProvider);
            outputDevice.Play();

            // SIN PlaybackStopped — el controlador detecta fin por polling en el timer
        }

        public void Reanudar()
        {
            if (EstaPausado)
                outputDevice.Play();
        }

        public void Pausar()
        {
            if (EstaReproduciendo)
                outputDevice.Pause();
        }

        // Detiene y libera todos los recursos NAudio limpiamente
        public void Detener()
        {
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

        public void CambiarVolumen(float valor)
        {
            if (volumeProvider != null)
                volumeProvider.Volume = Math.Max(0f, Math.Min(1f, valor));
        }

        public void CambiarPosicion(double segundos)
        {
            if (waveStream != null)
                waveStream.CurrentTime = TimeSpan.FromSeconds(segundos);
        }
    }
}