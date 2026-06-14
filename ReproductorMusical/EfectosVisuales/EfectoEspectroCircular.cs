using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoEspectroCircular : IEfectoVisual
    {
        public string Nombre => "Espectro Circular";

        private float _radioSuavizado = 50f;
        private float _rotacion = 0f;

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int centroX = ancho / 2;
            int centroY = alto / 2;
            int puntos = 72;
            int fftLen = buffer.Length;
            float radioMax = Math.Min(centroX, centroY) - 10f;

            float energiaTotal = 0f;
            for (int i = 0; i < fftLen; i++) energiaTotal += Math.Abs(buffer[i]);
            energiaTotal /= fftLen;

            float energiaBajos = 0f;
            for (int i = 0; i < 8; i++) energiaBajos += Math.Abs(buffer[i]);
            energiaBajos /= 8f;

            float radioObj = 45f + energiaBajos * 55f * (0.7f + energiaTotal * 1.2f);
            _radioSuavizado = radioObj * 0.3f + _radioSuavizado * 0.7f;
            _rotacion += 0.007f + energiaBajos * 0.035f;

            // Precalcular ángulos
            double[] angulos = new double[puntos];
            for (int i = 0; i < puntos; i++)
                angulos[i] = _rotacion + (i * 2.0 * Math.PI / puntos);

            for (int i = 0; i < puntos; i++)
            {
                int idx = i * (fftLen / puntos);
                float amp = Math.Abs(buffer[idx % fftLen]);

                float ext = _radioSuavizado + amp * 115f * (0.7f + energiaTotal * 1.8f);
                if (ext > radioMax) ext = radioMax;

                float x1 = centroX + (float)(_radioSuavizado * Math.Cos(angulos[i]));
                float y1 = centroY + (float)(_radioSuavizado * Math.Sin(angulos[i]));
                float x2 = centroX + (float)(ext * Math.Cos(angulos[i]));
                float y2 = centroY + (float)(ext * Math.Sin(angulos[i]));

                float hue = (float)(i / (double)puntos);
                float sat = 0.7f + amp * 0.3f;
                Color color = HsvColor(hue, sat, 0.95f);
                float grosor = 1.5f + amp * 5f * (0.5f + energiaTotal);

                using (Pen pen = new Pen(color, grosor))
                    g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        private Color HsvColor(float h, float s, float v)
        {
            int hi = (int)(h * 6) % 6;
            float f = h * 6 - (int)(h * 6);
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);
            float r, gc, b;
            switch (hi)
            {
                case 0: r = v; gc = t; b = p; break;
                case 1: r = q; gc = v; b = p; break;
                case 2: r = p; gc = v; b = t; break;
                case 3: r = p; gc = q; b = v; break;
                case 4: r = t; gc = p; b = v; break;
                default: r = v; gc = p; b = q; break;
            }
            return Color.FromArgb((int)(r * 255), (int)(gc * 255), (int)(b * 255));
        }
    }
}