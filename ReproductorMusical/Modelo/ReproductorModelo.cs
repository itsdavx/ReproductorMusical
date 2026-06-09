using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ReproductorMusical.Modelo
{
    // Encapsula todo el motor de audio NAudio
    public class ReproductorModelo
    {
        // ESTADO INTERNO
        private WaveStream waveStream;
        private VolumeSampleProvider volumeProvider;
        private IWavePlayer outputDevice;

        // PROPIEDADES DE CONSULTA
        public bool EstaReproduciendo => outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing;
        public bool EstaPausado => outputDevice != null && outputDevice.PlaybackState == PlaybackState.Paused;
        public TimeSpan TiempoActual => waveStream != null ? waveStream.CurrentTime : TimeSpan.Zero;
        public TimeSpan DuracionTotal => waveStream != null ? waveStream.TotalTime : TimeSpan.Zero;
        public float Volumen => volumeProvider != null ? volumeProvider.Volume : 1f;

        // Reproduce una pista desde su ruta en disco
        public void Reproducir(string rutaCompleta)
        {
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
        }

        // Reanuda si estaba pausado
        public void Reanudar()
        {
            if (EstaPausado)
                outputDevice.Play();
        }

        // Pausa la reproducción activa
        public void Pausar()
        {
            if (EstaReproduciendo)
                outputDevice.Pause();
        }

        // Detiene y libera todos los recursos de NAudio
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

        // Ajusta el volumen entre 0.0 y 1.0
        public void CambiarVolumen(float valor)
        {
            if (volumeProvider != null)
                volumeProvider.Volume = Math.Max(0f, Math.Min(1f, valor));
        }

        // Salta a una posición expresada en segundos
        public void CambiarPosicion(double segundos)
        {
            if (waveStream != null)
                waveStream.CurrentTime = TimeSpan.FromSeconds(segundos);
        }
    }
}