using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoPicosDeFrecuencia : IEfectoVisual
    {
        public string Nombre => "Picos de Frecuencia";

        private const int PUNTOS = 32;
        private float[] _suavizado = new float[PUNTOS];

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            // pasoX distribuye los 32 puntos de control uniformemente en el ancho
            float pasoX = (float)ancho / (PUNTOS - 1);
            int fftLen = buffer.Length;

            // Energía global: promedio de amplitudes del frame
            float energiaTotal = 0f;
            for (int i = 0; i < fftLen; i++) energiaTotal += Math.Abs(buffer[i]);
            energiaTotal /= fftLen;

            // Escala dinámica: amplifica la altura con el volumen, tope en 2×
            float escalaVolumen = Math.Min(0.6f + energiaTotal * 2f, 2f);

            // Suavizado 50/50 por punto; posición Y invertida (0 = arriba en pantalla)
            float[] posY = new float[PUNTOS];
            for (int i = 0; i < PUNTOS; i++)
            {
                int idx = i * (fftLen / PUNTOS);
                float amp = Math.Abs(buffer[idx % fftLen]);
                _suavizado[i] = amp * 0.5f + _suavizado[i] * 0.5f;
                posY[i] = Math.Max(4f, alto - 8 - _suavizado[i] * (alto * 0.88f) * escalaVolumen);
            }

            // Color interpolado de azul frío (silencio) a naranja/rojo (señal fuerte) según tc
            float tc = Math.Min(energiaTotal * 6f, 1f);
            Color colorLinea = Color.FromArgb(255, (int)(80 + 175 * tc), (int)(120 * (1 - tc)), (int)(255 * (1 - tc)));
            Color colorRelleno = Color.FromArgb(60, colorLinea.R, colorLinea.G, colorLinea.B);
            float grosor = 2f + energiaTotal * 5f;

            // Relleno: cada segmento forma un trapecio entre dos puntos consecutivos y la base
            for (int i = 0; i < PUNTOS - 1; i++)
            {
                float x1 = i * pasoX, x2 = (i + 1) * pasoX;
                PointF[] trapecio = { new PointF(x1, posY[i]), new PointF(x2, posY[i + 1]),
                                      new PointF(x2, alto),    new PointF(x1, alto) };
                using (SolidBrush brRelleno = new SolidBrush(colorRelleno))
                    g.FillPolygon(brRelleno, trapecio);
            }

            // Línea de contorno superior conectando los 32 puntos de control
            for (int i = 0; i < PUNTOS - 1; i++)
            {
                float x1 = i * pasoX, x2 = (i + 1) * pasoX;
                using (Pen pen = new Pen(colorLinea, grosor))
                    g.DrawLine(pen, x1, posY[i], x2, posY[i + 1]);
            }

            // Círculo en cada vértice para suavizar visualmente las uniones entre segmentos
            float radioVertice = grosor * 0.6f;
            for (int i = 0; i < PUNTOS; i++)
            {
                float x = i *