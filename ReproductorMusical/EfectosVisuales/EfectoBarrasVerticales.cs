using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoBarrasVerticales : IEfectoVisual
    {
        public string Nombre => "Barras Verticales";

        private float[] _alturasSuavizadas = new float[48];
        private float[] _picos = new float[48];
        private float _energiaAnt = 0f;

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int n = 48;
            int margen = 12;
            int espacio = 2;
            int anchoBarra = (ancho - margen * 2) / n - espacio;
            int baseY = alto - 8;
            int alturaMax = alto - 16;

            // Promedio de amplitudes → energía global del frame
            float energiaTotal = 0f;
            for (int i = 0; i < buffer.Length; i++) energiaTotal += Math.Abs(buffer[i]);
            energiaTotal /= buffer.Length;

            // Beat: pico instantáneo supera 1.4× el promedio suavizado (60% nuevo / 40% histórico)
            bool beat = energiaTotal > _energiaAnt * 1.4f && energiaTotal > 0.02f;
            _energiaAnt = energiaTotal * 0.6f + _energiaAnt * 0.4f;

            for (int i = 0; i < n; i++)
            {
                // Muestreo uniforme: cada barra toma una banda equidistante del buffer
                int idx = i * (buffer.Length / n);
                float amp = Math.Abs(buffer[idx % buffer.Length]);

                // Altura objetivo: amplitud escalada por volumen y energía global
                float objetivo = amp * (alto * 1.9f) * (0.8f + energiaTotal * 2f);
                objetivo = Math.Max(3, Math.Min(objetivo, alturaMax));

                // Subida rápida (75% nuevo), bajada lenta (decaimiento × 0.84)
                if (objetivo > _alturasSuavizadas[i])
                    _alturasSuavizadas[i] = objetivo * 0.75f + _alturasSuavizadas[i] * 0.25f;
                else
                    _alturasSuavizadas[i] *= 0.84f;

                // Pico: sube instantáneo, cae 1.8 px por frame
                if (_alturasSuavizadas[i] > _picos[i])
                    _picos[i] = _alturasSuavizadas[i];
                else
                    _picos[i] = Math.Max(0, _picos[i] - 1.8f);

                int x = margen + i * (anchoBarra + espacio);
                int alturaBarra = (int)_alturasSuavizadas[i];
                int y = baseY - alturaBarra;

                // Color por umbral de ratio: verde → naranja → rojo/beat
                float ratio = _alturasSuavizadas[i] / (float)alturaMax;
                Color colorTop;
                if (beat && ratio > 0.5f) colorTop = Color.FromArgb(100, 180, 255);
                else if (ratio > 0.75f) colorTop = Color.FromArgb(255, (int)(80 * (1 - ratio)), 40);
                else if (ratio > 0.4f) colorTop = Color.FromArgb(255, (int)(200 * ratio), 0);
                else colorTop = Color.FromArgb(50, 220, 80);

                // Base del gradiente: mismo tono oscurecido ÷ 4
                Color colorBot = Color.FromArgb(colorTop.R / 4, colorTop.G / 4, colorTop.B / 4);

                // Gradiente lineal vertical: colorTop (cima) → colorBot (base)
                Rectangle rectBarra = new Rectangle(x, y, anchoBarra, alturaBarra);
                using (LinearGradientBrush grad = new LinearGradientBrush(rectBarra, colorTop, colorBot, LinearGradientMode.Vertical))
                    g.FillRectangle(grad, rectBarra);

                // Línea de pico horizontal en la posición máxima retenida
                int yPico = baseY - (int)_picos[i];
                using (Pen penPico = new Pen(Color.FromArgb(200, colorTop), 1.5f))
                    g.DrawLine(penPico, x, yPico, x + anchoBarra, yPico);
            }
        }
    }
}