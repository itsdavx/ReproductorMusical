using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoOsciloscopio : IEfectoVisual // Herencia de la interfaz
    {
        public string Nombre => "Osciloscopio"; // Devuelve el nombre del efecto

        private float _ampMax = 0f;      // Guarda la amplitud máxima suavizada
        private float _energiaAnt = 0f;  // Guarda la energía del frame anterior suavizada

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer) // Implementación del método heredado
        {
            int centroY = alto / 2;          // Centro vertical del panel
            int fftLen = buffer.Length;      // Obtiene el tamaño del buffer de audio

            float pasoX = (float)ancho / fftLen; // Distribuye las muestras uniformemente a lo largo del ancho

            // Energía global: promedio de amplitudes del frame
            float energiaTotal = 0f; // Acumulador

            for (int i = 0; i < fftLen; i++)
                energiaTotal += Math.Abs(buffer[i]); // Recorre todas las muestras del buffer

            energiaTotal /= fftLen; // Promedio de energía del audio

            // Escala la amplitud de la señal según el volumen general
            float escalaVolumen = Math.Min(0.6f + energiaTotal * 3f, 2.5f);

            // Detecta un beat cuando la energía actual supera significativamente la anterior
            bool beat = energiaTotal > _energiaAnt * 1.5f && energiaTotal > 0.02f;

            // Suaviza la energía para evitar cambios bruscos
            _energiaAnt = energiaTotal * 0.4f + _energiaAnt * 0.6f;

            // Obtiene la amplitud máxima del frame actual
            float ampActual = 0f;

            for (int i = 0; i < fftLen; i++)
                ampActual = Math.Max(ampActual, Math.Abs(buffer[i]));

            // Suaviza la amplitud máxima para estabilizar el grosor de la línea
            _ampMax = ampActual * 0.4f + _ampMax * 0.6f;

            // Color RGB dinámico según la intensidad de la señal
            float tc = Math.Min(_ampMax * 4f, 1f);

            int cr, cg, cb;

            // Si detecta un beat utiliza un color magenta fijo
            if (beat)
            {
                cr = 255;
                cg = 100;
                cb = 200;
            }
            else
            {
                cr = (int)(60 + 150 * tc);
                cg = (int)(80 + 40 * (1 - tc));
                cb = 255;
            }

            // A mayor amplitud, mayor grosor de la línea
            float grosor = 1.5f + _ampMax * 4f;

            // Convierte cada muestra de audio en un punto de la pantalla
            PointF[] puntos = new PointF[fftLen];

            for (int i = 0; i < fftLen; i++)
            {
                puntos[i] = new PointF(
                    i * pasoX,
                    centroY + buffer[i] * (alto * 0.45f) * escalaVolumen
                );
            }

            // Se crea el lápiz y se dibuja la forma de onda
            using (Pen pen = new Pen(Color.FromArgb(220, cr, cg, cb), grosor))
                g.DrawLines(pen, puntos);
        }
    }
}