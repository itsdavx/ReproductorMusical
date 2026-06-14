using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoEcualizadorLED : IEfectoVisual
    {
        public string Nombre => "Ecualizador LED";

        private const int COLUMNAS = 24;
        private const int BLOQUES = 16;

        private float[] _alturasSuavizadas = new float[COLUMNAS];
        private float[] _picos = new float[COLUMNAS];
        private float _energiaAnterior = 0f;

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int margen = 10;
            int espacio = 2;
            int anchoCol = (ancho - margen * 2) / COLUMNAS;
            int altoBloque = (alto - margen * 2) / BLOQUES;

            // Energía total para ajuste dinámico (el buffer ya viene escalado por volumen)
            float energiaTotal = 0f;
            for (int i = 0; i < buffer.Length; i++) energiaTotal += Math.Abs(buffer[i]);
            energiaTotal /= buffer.Length;

            // Aplicamos un suavizado adicional para evitar cambios bruscos
            energiaTotal = energiaTotal * 0.3f + _energiaAnterior * 0.7f;
            _energiaAnterior = energiaTotal;

            for (int i = 0; i < COLUMNAS; i++)
            {
                int idx = i * (buffer.Length / COLUMNAS);
                float amp = Math.Abs(buffer[idx % buffer.Length]);

                // Objetivo con sensibilidad al volumen (ya incluido en amp)
                float objetivo = amp * BLOQUES * 3.5f * (0.7f + energiaTotal * 2.5f);
                if (objetivo > BLOQUES) objetivo = BLOQUES;
                if (objetivo < 0) objetivo = 0;

                // Suavizado extra fuerte (más inercia) para eliminar parpadeo
                if (objetivo > _alturasSuavizadas[i])
                    _alturasSuavizadas[i] = objetivo * 0.2f + _alturasSuavizadas[i] * 0.8f;
                else
                    _alturasSuavizadas[i] *= 0.94f; // caída lenta

                // Pico (también más lento)
                if (_alturasSuavizadas[i] > _picos[i])
                    _picos[i] = _alturasSuavizadas[i];
                else
                    _picos[i] = Math.Max(0, _picos[i] - 0.03f);

                int bloquesActivos = (int)_alturasSuavizadas[i];
                int x = margen + i * anchoCol;

                // 1. Dibujar fondo de toda la columna de una sola vez (un rectángulo grande)
                using (SolidBrush bApagado = new SolidBrush(Color.FromArgb(28, 255, 255, 255)))
                {
                    int yFondo = alto - margen - BLOQUES * altoBloque;
                    g.FillRectangle(bApagado, x, yFondo, anchoCol - espacio, BLOQUES * altoBloque - espacio);
                }

                // 2. Dibujar LEDs activos de abajo hacia arriba
                for (int j = 0; j < bloquesActivos; j++)
                {
                    int y = alto - margen - (j + 1) * altoBloque;
                    float ratio = (float)j / BLOQUES;
                    Color led;
                    if (ratio > 0.82f)
                        led = Color.FromArgb(255, 30, 30);
                    else if (ratio > 0.58f)
                        led = Color.FromArgb(255, 210, 0);
                    else
                        led = Color.FromArgb(0, 210, 70);

                    using (SolidBrush bLed = new SolidBrush(led))
                        g.FillRectangle(bLed, x, y, anchoCol - espacio, altoBloque - espacio);
                }

                // 3. Pico blanco
                int yPico = alto - margen - ((int)_picos[i] + 1) * altoBloque;
                if (yPico >= 0 && yPico < alto)
                {
                    using (SolidBrush bPico = new SolidBrush(Color.White))
                        g.FillRectangle(bPico, x, yPico, anchoCol - espacio, altoBloque - espacio);
                }
            }
        }
    }
}