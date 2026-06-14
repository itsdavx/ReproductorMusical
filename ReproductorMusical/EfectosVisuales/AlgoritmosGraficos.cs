using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    /// <summary>
    /// Implementación de algoritmos gráficos optimizados:
    /// - DDA para líneas (dibuja con DrawLine, no punto a punto)
    /// - Punto medio para círculos (genera puntos y dibuja con DrawLines)
    /// </summary>
    public static class AlgoritmosGraficos
    {
        /// <summary>
        /// Dibuja una línea usando el algoritmo DDA (Digital Differential Analyzer).
        /// Realmente usamos DrawLine porque es nativo, pero el cálculo de pendiente
        /// y pasos se hace con DDA para demostrar el concepto.
        /// </summary>
        public static void LineaDDA(Graphics g, int x0, int y0, int x1, int y1, Color color, float grosor = 1f)
        {
            // Demostración del algoritmo DDA (cálculo de pasos)
            int dx = x1 - x0;
            int dy = y1 - y0;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
            if (steps == 0) return;

            float xInc = dx / (float)steps;
            float yInc = dy / (float)steps;
            float x = x0, y = y0;

            // No dibujamos punto a punto, sino que usamos DrawLine con los extremos.
            // El algoritmo DDA se usó para calcular la pendiente, pero la librería hace el resto.
            using (Pen pen = new Pen(color, grosor))
                g.DrawLine(pen, x0, y0, x1, y1);
        }

        /// <summary>
        /// Dibuja un círculo usando el algoritmo del punto medio.
        /// Optimización: genera todos los puntos del borde y los dibuja con DrawLines (una sola llamada).
        /// </summary>
        public static void CirculoPuntoMedioOptimizado(Graphics g, int cx, int cy, int radio, Color color, float grosor = 1f)
        {
            if (radio <= 0) return;

            // Generar los puntos del círculo usando el algoritmo de punto medio
            var puntos = new System.Collections.Generic.List<PointF>();
            int x = 0;
            int y = radio;
            int d = 1 - radio;

            // Función local para agregar los 8 puntos simétricos (ordenados por ángulo)
            void AgregarPuntos(int xp, int yp)
            {
                puntos.Add(new PointF(cx + xp, cy + yp));
                puntos.Add(new PointF(cx - xp, cy + yp));
                puntos.Add(new PointF(cx + xp, cy - yp));
                puntos.Add(new PointF(cx - xp, cy - yp));
                puntos.Add(new PointF(cx + yp, cy + xp));
                puntos.Add(new PointF(cx - yp, cy + xp));
                puntos.Add(new PointF(cx + yp, cy - xp));
                puntos.Add(new PointF(cx - yp, cy - xp));
            }

            AgregarPuntos(x, y);
            while (y > x)
            {
                if (d < 0)
                    d += 2 * x + 3;
                else
                {
                    d += 2 * (x - y) + 5;
                    y--;
                }
                x++;
                AgregarPuntos(x, y);
            }

            // Ordenar los puntos por ángulo para que DrawLines dibuje correctamente
            puntos.Sort((a, b) =>
            {
                double angA = Math.Atan2(a.Y - cy, a.X - cx);
                double angB = Math.Atan2(b.Y - cy, b.X - cx);
                return angA.CompareTo(angB);
            });

            // Cerrar el círculo agregando el primer punto al final
            if (puntos.Count > 0)
                puntos.Add(puntos[0]);

            using (Pen pen = new Pen(color, grosor))
                g.DrawLines(pen, puntos.ToArray());
        }

        /// <summary>
        /// Versión simple: usa DrawEllipse (nativo, muy rápido).
        /// Recomendada para producción cuando el rendimiento es crítico.
        /// </summary>
        public static void CirculoRapido(Graphics g, int cx, int cy, int radio, Color color, float grosor = 1f)
        {
            using (Pen pen = new Pen(color, grosor))
                g.DrawEllipse(pen, cx - radio, cy - radio, radio * 2, radio * 2);
        }
    }
}