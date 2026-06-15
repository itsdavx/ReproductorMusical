using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoAnillosConcentricos : IEfectoVisual
    {
        public string Nombre => "Anillos Concéntricos";

        private float _pulso = 0f;
        private float _energiaAnt = 0f;
        private readonly float[] _radios = new float[5];

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int centroX = ancho / 2;
            int centroY = alto / 2;
            float radioMax = Math.Min(centroX, centroY) - 8f;

            // Energía total: promedio de amplitudes: volumen general
            float energiaTotal = 0f;
            for (int i = 0; i < buffer.Length; i++) energiaTotal += Math.Abs(buffer[i]);
            energiaTotal /= buffer.Length;

            // Energía de bajos: promedio de las primeras 12 bandas (frecuencias graves)
            float energiaBajos = 0f;
            for (int i = 0; i < 12; i++) energiaBajos += Math.Abs(buffer[i]);
            energiaBajos /= 12f;

            // Detección de beat: pico instantáneo supera 1.45× el promedio suavizado
            bool beat = energiaBajos > _energiaAnt * 1.45f && energiaBajos > 0.015f;

            // Suavizado exponencial: 40% nuevo + 60% histórico
            _energiaAnt = energiaBajos * 0.4f + _energiaAnt * 0.6f;

            // _pulso decae linealmente 0.05 por frame; se recarga a 1.0 en cada beat
            if (beat) _pulso = 1f;
            _pulso = Math.Max(0f, _pulso - 0.05f);

            // Propagación de radios: cada anillo hereda 7% del radio anterior (efecto eco)
            for (int i = _radios.Length - 1; i > 0; i--)
                _radios[i] = _radios[i] * 0.93f + _radios[i - 1] * 0.07f;

            // Radio objetivo: base fija + aporte de graves + rebote del pulso, escalado por volumen
            float radioObj = 28f + energiaBajos * 80f + _pulso * 45f;
            radioObj *= (0.7f + energiaTotal * 1.5f);
            _radios[0] = radioObj * 0.4f + _radios[0] * 0.6f; // suavizado del anillo central

            // Separación uniforme entre anillos para distribuirlos en radioMax
            float separacion = radioMax / (_radios.Length + 1f);

            for (int i = 0; i < _radios.Length; i++)
            {
                float radio = _radios[i] + i * separacion;
                if (radio < 2f) continue;
                if (radio > radioMax) radio = radioMax;

                // Hue avanza por índice y se desplaza con el pulso → rotación de color
                float hue = ((float)i / _radios.Length + _pulso * 0.3f) % 1f;
                float brillo = 0.6f + _pulso * 0.4f;
                int alpha = Math.Max(0, 180 - i * 28); // anillos exteriores más transparentes
                float grosor = Math.Max(0.5f, (2.5f - i * 0.3f) + _pulso * 3f);

                using (Pen pen = new Pen(HsvAColor(hue, 0.85f, brillo, alpha), grosor))
                    g.DrawEllipse(pen, centroX - radio, centroY - radio, radio * 2, radio * 2);
            }
        }

        // Conversión HSV → RGB: descompone el hue en 6 sectores de 60°,
        // interpola p/q/t según la fracción f dentro del sector y escala a [0,255].
        private Color HsvAColor(float h, float s, float v, int alpha)
        {
            int hi = (int)(h * 6) % 6;
            float f = h * 6 - (int)(h * 6); // fracción dentro del sector
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
            return Color.FromArgb(
                Math.Max(0, Math.Min(255, alpha)),
                (int)(r * 255), (int)(gc * 255), (int)(b * 255));
        }
    }
}