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

            // Promedio de amplitudes suavizado (30% nuevo / 70% histórico) → reduce parpadeo
            float energiaTotal = 0f;
            for (int i = 0; i < buffer.Length; i++) energiaTotal += Math.Abs(buffer[i]);
            energiaTotal = (energiaTotal / buffer.Length) * 0.3f + _energiaAnterior * 0.7f;
            _energiaAnterior = energiaTotal;

            for (int i = 0; i < COLUMNAS; i++)
            {
                // Muestreo uniforme del buffer; altura objetivo en unidades de bloques
                int idx = i * (buffer.Length / COLUMNAS);
                float amp = Math.Abs(buffer[idx % buffer.Length]);
                float objetivo = Math.Max(0, Math.Min(amp * BLOQUES * 3.5f * (0.7f + energiaTotal * 2.5f), BLOQUES));

                // Subida suave (20% nuevo / 80% histórico); caída lenta × 0.94 por frame
                if (objetivo > _alturasSuavizadas[i])
                    _alturasSuavizadas[i] = objetivo * 0.2f + _alturasSuavizadas[i] * 0.8f;
                else
                    _alturasSuavizadas[i] *= 0.94f;

                // Pico: sube instantáneo, cae 0.03 bloques por frame
                if (_alturasSuavizadas[i] > _picos[i])
                    _picos[i] = _alturasSuavizadas[i];
                else
                    _picos[i] = Math.Max(0, _picos[i] - 0.03f);

                int bloquesActivos = (int)_alturasSuavizadas[i];
                int x = margen + i * anchoCol;

                // Fondo de columna completa en un solo FillRectangle (evita N llamadas)
                int yFondo = alto - margen - BLOQUES * altoBloque;
                using (SolidBrush bApagado = new SolidBrush(Color.FromArgb(28, 255, 255, 255)))
                    g.FillRectangle(bApagado, x, yFondo, anchoCol - espacio, BLOQUES * altoBloque - espacio);

                // LEDs activos de abajo hacia arriba; color por posición relativa (ratio = j/BLOQUES)
                // verde (bajo) → amarillo (medio, >58%) → rojo (alto, >82%)
                for (int j = 0; j < bloquesActivos; j++)
                {
                    int y = alto - margen - (j + 1) * altoBloque;
                    float ratio = (float)j / BLOQUES;
                    Color led = ratio > 0.82f ? Color.FromArgb(255, 30, 30)
                              : ratio > 0.58f ? Color.FromArgb(255, 210, 0)
                                              : Color.FromArgb(0, 210, 70);

                    using (SolidBrush bLed = new SolidBrush(led))
                        g.FillRectangle(bLed, x, y, anchoCol - espacio, altoBloque - espacio);
                }

                // Bloque blanco en la posición del pico retenido
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