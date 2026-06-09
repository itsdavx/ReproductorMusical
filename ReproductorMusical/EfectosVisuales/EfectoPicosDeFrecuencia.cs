using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoPicosDeFrecuencia : IEfectoVisual
    {
        public string Nombre => "Picos de Frecuencia";

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int puntos = 25;
            float pasoX = (float)ancho / (puntos - 1);
            int fftLength = buffer.Length;
            PointF[] picos = new PointF[puntos];

            for (int i = 0; i < puntos; i++)
            {
                int idx = i * (fftLength / puntos);
                float amplitud = Math.Abs(buffer[idx % fftLength]);
                picos[i] = new PointF(i * pasoX, alto - 10 - (amplitud * (alto * 0.8f)));
            }

            using (Pen penPicos = new Pen(Color.OrangeRed, 3f))
            {
                g.DrawLines(penPicos, picos);
            }
        }
    }
}