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
            float pasoX = (float)ancho / fftLen;

            float energiaTotal = 0f;
            for (int i = 0; i < fftLen; i++) energiaTotal += Math.Abs(buffer[i]);
            energiaTotal /= fftLen;

            float escalaVolumen = 0.6f + energiaTotal * 3f;
            if (escalaVolumen > 2.5f) escalaVolumen = 2.5f;

            bool beat = energiaTotal > _energiaAnt * 1.5f && energiaTotal > 0.02f;
            _energiaAnt = energiaTotal * 0.4f + _energiaAnt * 0.6f;

            float ampActual = 0f;
            for (int i = 0; i < fftLen; i++) ampActual = Math.Max(ampActual, Math.Abs(buffer[i]));
            _ampMax = ampActual * 0.4f + _ampMax * 0.6f;

            // DEMOSTRACIÓN DEL ALGORITMO DDA: trazamos una línea horizontal de referencia
            // usando el método DDA (aunque internamente usa DrawLine, el cálculo de pasos es DDA)
            AlgoritmosGraficos.LineaDDA(g, 0, centroY, ancho, centroY, Color.FromArgb(40, 255, 255, 255), 1f);

            float tc = Math.Min(_ampMax * 4f, 1f);
            int cr, cg, cb;
            if (beat) { cr = 255; cg = 100; cb = 200; }
            else { cr = (int)(60 + 150 * tc); cg = (int)(80 + 40 * (1 - tc)); cb = 255; }
            float grosor = 1.5f + _ampMax * 4f;

            PointF[] puntos = new PointF[fftLen];
            for (int i = 0; i < fftLen; i++)
            {
                float x = i * pasoX;
                float y = centroY + buffer[i] * (alto * 0.45f) * escalaVolumen;
                puntos[i] = new PointF(x, y);
            }

            using (Pen pen = new Pen(Color.FromArgb(220, cr, cg, cb), grosor))
                g.DrawLines(pen, puntos);
        }
    }
}