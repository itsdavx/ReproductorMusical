using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoParticulas : IEfectoVisual
    {
        public string Nombre => "Partículas";

        // Generador interno para movimiento orgánico
        private readonly Random rand = new Random();

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int centroX = ancho / 2;
            int centroY = alto / 2;
            int fftLength = buffer.Length;
            float maxAmp = 0f;

            for (int i = 0; i < fftLength; i++)
            {
                if (Math.Abs(buffer[i]) > maxAmp)
                    maxAmp = Math.Abs(buffer[i]);
            }

            int numParticulas = 5 + (int)(maxAmp * 40);

            using (SolidBrush bParticula = new SolidBrush(Color.Red))
            {
                for (int i = 0; i < numParticulas; i++)
                {
                    float dist = rand.Next(10, (int)(alto * 0.45f));
                    double ang = rand.NextDouble() * 2 * Math.PI;
                    int pX = centroX + (int)(dist * Math.Cos(ang));
                    int pY = centroY + (int)(dist * Math.Sin(ang));
                    int t = rand.Next(3, 9);

                    g.FillEllipse(bParticula, pX, pY, t, t);
                }
            }
        }
    }
}