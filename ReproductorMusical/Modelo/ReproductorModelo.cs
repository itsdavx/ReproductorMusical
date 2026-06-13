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

        // VOLUMEN PERSISTENTE: se mantiene entre cambios de pista
        // Inicia en 1.0f pero se actualiza cada vez que el usuario mueve el TrackBar
        private float volumenActual = 1.0f;

        // PROPIEDADES DE CONSULTA
        public bool EstaReproduciendo =>
            outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing;
        public bool EstaPausado =>
            outputDevice != null && outputDevice.PlaybackState == PlaybackState.Paused;
        public TimeSpan TiempoActual =>
            waveStream != null ? waveStream.CurrentTime : TimeSpan.Zero;
        public TimeSpan DuracionTotal =>
            waveStream != null ? waveStream.TotalTime : TimeSpan.Zero;

        // Devuelve el volumen persistente, no el del provider (que puede ser null)
        public float Volumen => volumenActual;

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

            // Aplica el volumen que el usuario tenía antes de cambiar de pista
            volumeProvider.Volume = volumenActual;

            outputDevice = new WaveOutEvent();
            outputDevice.Init(volumeProvider);
            outputDevice.Play();
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
            // volumenActual NO se toca aquí — debe persistir entre pistas
        }

        // Guarda el volumen en volumenActual Y lo aplica al provider si existe
        public void CambiarVolumen(float valor)
        {
            volumenActual = Math.Max(0f, Math.Min(1f, valor));

            if (volumeProvider != null)
                volumeProvider.Volume = volumenActual;
        }

        public void CambiarPosicion(double segundos)
        {
            if (waveStream != null)
                waveStream.CurrentTime = TimeSpan.FromSeconds(segundos);
        }
    }
}