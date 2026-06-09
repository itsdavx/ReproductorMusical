using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoOndasDeSonido : IEfectoVisual
    {
        public string Nombre => "Ondas de Sonido";

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int centroY = alto / 2;
            int puntos = 30;
            float pasoX = (float)ancho / puntos;
            int fftLength = buffer.Length;
            PointF[] puntosTop = new PointF[puntos];
            PointF[] puntosBot = new PointF[puntos];

            for (int i = 0; i < puntos; i++)
            {
                float x = i * pasoX;
                float amplitud = Math.Abs(buffer[i * (fftLength / puntos)]);
                float desfase = amplitud * (alto * 0.4f);

                puntosTop[i] = new PointF(x, centroY - desfase);
                puntosBot[i] = new PointF(x, centroY + desfase);
            }

            using (Pen penWave = new Pen(Color.DeepSkyBlue, 2f))
            {
                g.DrawCurve(penWave, puntosTop);
                g.DrawCurve(penWave, puntosBot);
            }
        }
    }
}