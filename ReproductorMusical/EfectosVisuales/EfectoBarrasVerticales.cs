using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoBarrasVerticales : IEfectoVisual
    {
        public string Nombre => "Barras Verticales";

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int cantidadBarras = 40;
            int anchoBarra = (ancho - 20) / cantidadBarras;
            int baseY = alto - 5;
            int fftLength = buffer.Length;

            using (SolidBrush brush = new SolidBrush(Color.LimeGreen))
            {
                for (int i = 0; i < cantidadBarras; i++)
                {
                    int idx = i * (fftLength / cantidadBarras);
                    float amplitud = Math.Abs(buffer[idx % fftLength]);
                    int alturaBarra = (int)(amplitud * (alto * 1.8f));

                    if (alturaBarra > alto - 10) alturaBarra = alto - 10;
                    if (alturaBarra < 4) alturaBarra = 4;

                    int x = 10 + (i * anchoBarra);
                    int y = baseY - alturaBarra;

                    g.FillRectangle(brush, x, y, anchoBarra - 2, alturaBarra);
                }
            }
        }
    }
}