using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoOndasDeSonido : IEfectoVisual
    {
        public string Nombre => "Ondas de Sonido";

        private float _fase = 0f;
        private PointF[][] _puntosSuperiores;
        private PointF[][] _puntosInferiores;
        private int _ultimoAncho = 0;

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int centroY = alto / 2;
            int fftLen = buffer.Length;

            float energiaBajos = 0f, energiaAgudos = 0f;
            for (int i = 0; i < 16; i++) energiaBajos += Math.Abs(buffer[i]);
            for (int i = 64; i < fftLen; i++) energiaAgudos += Math.Abs(buffer[i]);
            energiaBajos /= 16f;
            energiaAgudos /= (fftLen - 64f);
            float energiaTotal = (energiaBajos + energiaAgudos) / 2f;

            _fase += 0.035f + energiaBajos * 0.12f;

            int capas = 3;
            int paso = 2; // dibujar cada 2 píxeles (ahorro del 50%)
            int numPuntos = ancho / paso + 1;

            if (_puntosSuperiores == null || _ultimoAncho != ancho)
            {
                _puntosSuperiores = new PointF[capas][];
                _puntosInferiores = new PointF[capas][];
                for (int c = 0; c < capas; c++)
                {
                    _puntosSuperiores[c] = new PointF[numPuntos];
                    _puntosInferiores[c] = new PointF[numPuntos];
                }
                _ultimoAncho = ancho;
            }

            for (int capa = 0; capa < capas; capa++)
            {
                float offsetFase = _fase + capa * (float)(Math.PI * 2.0 / capas);
                int idxPunto = 0;

                for (int x = 0; x < ancho; x += paso)
                {
                    int idxBuf = (int)((float)x / ancho * fftLen);
                    float amp = Math.Abs(buffer[idxBuf % fftLen]);
                    float seno = (float)Math.Sin(x * 0.045f + offsetFase);
                    float coseno = (float)Math.Cos(x * 0.022f + offsetFase * 0.7f);
                    float desplazamiento = (0.04f + amp * 0.55f + seno * 0.035f + coseno * 0.018f) * (alto * 0.45f);
                    desplazamiento *= (0.8f + energiaTotal * 1.5f);

                    _puntosSuperiores[capa][idxPunto] = new PointF(x, centroY - desplazamiento);
                    _puntosInferiores[capa][idxPunto] = new PointF(x, centroY + desplazamiento);
                    idxPunto++;
                }

                float hue = (capa / (float)capas + energiaAgudos * 0.5f) % 1f;
                int alpha = 190 - capa * 45;
                Color color = HsvAColor(hue, 0.85f, 1f, alpha);
                float grosor = 2.2f - capa * 0.4f;

                using (Pen pen = new Pen(color, grosor))
                {
                    g.DrawLines(pen, _puntosSuperiores[capa]);
                    g.DrawLines(pen, _puntosInferiores[capa]);
                }
            }
        }

        private Color HsvAColor(float h, float s, float v, int alpha)
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
            return Color.FromArgb(Math.Max(0, Math.Min(255, alpha)),
                (int)(r * 255), (int)(gc * 255), (int)(b * 255));
        }
    }
}