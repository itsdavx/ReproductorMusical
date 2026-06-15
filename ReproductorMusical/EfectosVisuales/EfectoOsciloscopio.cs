using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoOsciloscopio : IEfectoVisual
    {
        public string Nombre => "Osciloscopio";

        private float _ampMax = 0f;
        private float _energiaAnt = 0f;

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int centroY = alto / 2;
            int fftLen = buffer.Length;

            // pasoX distribuye las fftLen muestras uniformemente en el ancho de píxeles
            float pasoX = (float)ancho / fftLen;

            // Energía global: promedio de amplitudes del frame
            float energiaTotal = 0f;
            for (int i = 0; i < fftLen; i++) energiaTotal += Math.Abs(buffer[i]);
            energiaTotal /= fftLen;

            // Escala dinámica: aumenta con el volumen, tope en 2.5 para no saturar
            float escalaVolumen = Math.Min(0.6f + energiaTotal * 3f, 2.5f);

            // Beat: pico instantáneo supera 1.5× el promedio suavizado (40% nuevo / 60% histórico)
            bool beat = energiaTotal > _energiaAnt * 1.5f && energiaTotal > 0.02f;
            _energiaAnt = energiaTotal * 0.4f + _energiaAnt * 0.6f;

            // Amplitud de pico suavizada (40% nuevo / 60% histórico) → estabiliza el grosor de línea
            float ampActual = 0f;
            for (int i = 0; i < fftLen; i++) ampActual = Math.Max(ampActual, Math.Abs(buffer[i]));
            _ampMax = ampActual * 0.4f + _ampMax * 0.6f;

            // Color: tc interpola de azul frío (silencio) a rojo/magenta (señal fuerte); beat → magenta fijo
            float tc = Math.Min(_ampMax * 4f, 1f);
            int cr, cg, cb;
            if (beat) { cr = 255; cg = 100; cb = 200; }
            else { cr = (int)(60 + 150 * tc); cg = (int)(80 + 40 * (1 - tc)); cb = 255; }
            float grosor = 1.5f + _ampMax * 4f;

            // Mapeo de muestras a píxeles: y = centroY + muestra × semiAlto × escala
            PointF[] puntos = new PointF[fftLen];
            for (int i = 0; i < fftLen; i++)
                puntos[i] = new PointF(i * pasoX, centroY + buffer[i] * (alto * 0.45f) * escalaVolumen);

            using (Pen pen = new Pen(Color.FromArgb(220, cr, cg, cb), grosor))
                g.DrawLines(pen, puntos);
        }
    }
}