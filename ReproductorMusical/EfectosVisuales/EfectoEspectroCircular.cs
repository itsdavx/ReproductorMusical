using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoEspectroCircular : IEfectoVisual
    {
        public string Nombre => "Espectro Circular";

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int centroX = ancho / 2;
            int centroY = alto / 2;
            int puntosTotales = 60;
            float radioBase = 50f;
            int fftLength = buffer.Length;

            using (Pen penCirc = new Pen(Color.Magenta, 2f))
            {
                for (int i = 0; i < puntosTotales; i++)
                {
                    double angulo = (i * 360.0 / puntosTotales) * Math.PI / 180.0;
                    int idx = i * (fftLength / puntosTotales);
                    float amplitud = Math.Abs(buffer[idx % fftLength]);
                    float ext = radioBase + (amplitud * 90f);

                    float x1 = centroX + (float)(radioBase * Math.Cos(angulo));
                    float y1 = centroY + (float)(radioBase * Math.Sin(angulo));
                    float x2 = centroX + (float)(ext * Math.Cos(angulo));
                    float y2 = centroY + (float)(ext * Math.Sin(angulo));

                    g.DrawLine(penCirc, x1, y1, x2, y2);
                }
            }
        }
    }
}