using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoOsciloscopio : IEfectoVisual
    {
        public string Nombre => "Osciloscopio";

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int centroY = alto / 2;
            int fftLength = buffer.Length;
            PointF[] puntos = new PointF[fftLength];
            float pasoX = (float)ancho / fftLength;

            for (int i = 0; i < fftLength; i++)
            {
                float x = i * pasoX;
                float y = centroY + (buffer[i] * (alto * 0.45f));
                puntos[i] = new PointF(x, y);
            }

            using (Pen penOnda = new Pen(Color.Cyan, 2.5f))
            {
                g.DrawLines(penOnda, puntos);
            }
        }
    }
}