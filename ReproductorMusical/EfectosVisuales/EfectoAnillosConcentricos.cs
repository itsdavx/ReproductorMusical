using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoAnillosConcentricos : IEfectoVisual
    {
        public string Nombre => "Anillos Concéntricos";

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int centroX = ancho / 2;
            int centroY = alto / 2;
            float ampTotal = 0f;

            for (int i = 0; i < 20; i++)
                ampTotal += Math.Abs(buffer[i]);
            ampTotal /= 20f;

            using (Pen penAnillo = new Pen(Color.SpringGreen, 2f))
            {
                for (int i = 1; i <= 4; i++)
                {
                    float radio = (i * 20) + (ampTotal * 35f * i);
                    g.DrawEllipse(penAnillo, centroX - radio, centroY - radio, radio * 2, radio * 2);
                }
            }
        }
    }
}