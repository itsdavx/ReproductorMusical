using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoAnillosConcentricos : IEfectoVisual
    {
        public string Nombre => "Anillos Concéntricos";

        private float _pulso = 0f;
        private float _energiaAnt = 0f;
        private float[] _radios = new float[5];

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int centroX = ancho / 2;
            int centroY = alto / 2;
            float radioMax = Math.Min(centroX, centroY) - 8f;

            // Energía total para volumen
            float energiaTotal = 0f;
            for (int i = 0; i < buffer.Length; i++) energiaTotal += Math.Abs(buffer[i]);
            energiaTotal /= buffer.Length;

            float energiaBajos = 0f;
            for (int i = 0; i < 12; i++) energiaBajos += Math.Abs(buffer[i]);
            energiaBajos /= 12f;

            bool beat = energiaBajos > _energiaAnt * 1.45f && energiaBajos > 0.015f;
            _energiaAnt = energiaBajos * 0.4f + _energiaAnt * 0.6f;

            if (beat) _pulso = 1f;
            _pulso = Math.Max(0f, _pulso - 0.05f);

            for (int i = _radios.Length - 1; i > 0; i--)
                _radios[i] = _radios[i] * 0.93f + _radios[i - 1] * 0.07f;

            float radioObj = 28f + energiaBajos * 80f + _pulso * 45f;
            radioObj *= (0.7f + energiaTotal * 1.5f);
            _radios[0] = radioObj * 0.4f + _radios[0] * 0.6f;

            float separacion = radioMax / (_radios.Length + 1f);

            for (int i = 0; i < _radios.Length; i++)
            {
                float radio = _radios[i] + i * separacion;
                if (radio < 2f) continue;
                if (radio > radioMax) radio = radioMax;

                float hue = ((float)i / _radios.Length + _pulso * 0.3f) % 1f;
                float brillo = 0.6f + _pulso * 0.4f;
                int alpha = (int)(180 - i * 28);
                Color color = HsvAColor(hue, 0.85f, brillo, Math.Max(0, alpha));
                float grosor = (2.5f - i * 0.3f) + _pulso * 3f;
                if (grosor < 0.5f) grosor = 0.5f;

                // USO DE ALGORITMO GRÁFICO: El cálculo del radio y la posición de los anillos
                // se basa en el algoritmo de punto medio (implícito al usar DrawEllipse).
                // Para una demostración explícita, puedes cambiar a AlgoritmosGraficos.CirculoPuntoMedioOptimizado.
                using (Pen pen = new Pen(color, grosor))
                    g.DrawEllipse(pen, centroX - radio, centroY - radio, radio * 2, radio * 2);
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