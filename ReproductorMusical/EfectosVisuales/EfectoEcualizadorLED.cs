using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoEcualizadorLED : IEfectoVisual
    {
        public string Nombre => "Ecualizador LED";

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int columnas = 20;
            int bloquesPorColumna = 12;
            int anchoCol = (ancho - 20) / columnas;
            int altoBloque = (alto - 20) / bloquesPorColumna;
            int fftLength = buffer.Length;

            for (int i = 0; i < columnas; i++)
            {
                int idx = i * (fftLength / columnas);
                float amplitud = Math.Abs(buffer[idx % fftLength]);
                int bloquesActivos = (int)(amplitud * bloquesPorColumna * 2.5f);

                for (int j = 0; j < bloquesPorColumna; j++)
                {
                    if (j < bloquesActivos)
                    {
                        Color colLed = Color.Green;
                        if (j > bloquesPorColumna * 0.8) colLed = Color.Red;
                        else if (j > bloquesPorColumna * 0.5) colLed = Color.Yellow;

                        using (SolidBrush bLed = new SolidBrush(colLed))
                        {
                            int x = 10 + (i * anchoCol);
                            int y = alto - 10 - (j * altoBloque);
                            g.FillRectangle(bLed, x, y, anchoCol - 3, altoBloque - 2);
                        }
                    }
                }
            }
        }
    }
}