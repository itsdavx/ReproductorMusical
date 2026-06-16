using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoOndasDeSonido : IEfectoVisual // Herencia de la interfaz
    {
        public string Nombre => "Ondas de Sonido"; // Devuelve el nombre del efecto

        private float _fase = 0f;                   // Guarda la fase actual de las ondas
        private PointF[][] _puntosSuperiores;       // Almacena los puntos de las ondas superiores para cada capa
        private PointF[][] _puntosInferiores;       // Almacena los puntos de las ondas inferiores para cada capa
        private int _ultimoAncho = 0;               // Guarda el último ancho del panel para saber cuándo reasignar los arreglos de puntos

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer) // Implementación del método heredado
        {
            int centroY = alto / 2;                // Centro vertical del panel
            int fftLen = buffer.Length;            // Obtiene el tamaño del buffer de audio
            
            // Energía de bajos (primeras 16 bandas) y agudos (bandas 64 en adelante)
            float energiaBajos = 0f, energiaAgudos = 0f;                            //Acumuladores

            for (int i = 0; i < 16; i++) energiaBajos += Math.Abs(buffer[i]);       //Recorre las primeras 16 bandas
            for (int i = 64; i < fftLen; i++) energiaAgudos += Math.Abs(buffer[i]); //Recorre las bandas 64 en adelante

            energiaBajos /= 16f;                   // Promedio de energía en bajas frecuencias
            energiaAgudos /= (fftLen - 64f);       // Promedio de energía en altas frecuencias

            float energiaTotal = (energiaBajos + energiaAgudos) / 2f; // Energía global

            _fase += 0.035f + energiaBajos * 0.12f; // Incrementa la fase para animar las ondas y se acelera con los bajos

            int capas = 3;                         // Número de ondas superpuestas
            int paso = 2;                          // Muestrea cada 2 píxeles para reducir cálculos (optimización)
            int numPuntos = ancho / paso + 1;      // Cantidad total de puntos de cada línea

            // Reasigna los arreglos únicamente cuando cambia el ancho de la ventana (Solo sirve si se cambia el tamaño del panel)
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

            // Recorre cada capa de ondas y las dibuja
            for (int capa = 0; capa < capas; capa++)
            {
                float offsetFase = _fase + capa * (float)(Math.PI * 2.0 / capas); // Cada capa se desplaza 2π/3 radianes para evitar que coincidan visualmente (Desfase)

                int idxPunto = 0; // Contador

                // Genera los puntos de la onda a lo largo del ancho de la pantalla
                for (int x = 0; x < ancho; x += paso)
                {
                    // Relaciona la posición horizontal con una banda del buffer
                    int idxBuf = (int)((float)x / ancho * fftLen);
                    float amp = Math.Abs(buffer[idxBuf % fftLen]);

                    // Onda principal senoidal
                    float seno = (float)Math.Sin(x * 0.045f + offsetFase);

                    // Segunda onda cosenoidal
                    float coseno = (float)Math.Cos(x * 0.022f + offsetFase * 0.7f);

                    // Desplazamiento vertical de la onda
                    float desplazamiento = (0.04f + amp * 0.55f + seno * 0.035f + coseno * 0.018f) * (alto * 0.45f) * (0.8f + energiaTotal * 1.5f);

                    // Punto superior de la onda
                    _puntosSuperiores[capa][idxPunto] = new PointF(x, centroY - desplazamiento);

                    // Punto inferior simétrico respecto al centro
                    _puntosInferiores[capa][idxPunto] = new PointF(x, centroY + desplazamiento);

                    idxPunto++; //Avanza al siguiente punto
                }

                // Color HSV (Tono, Saturación, Valor)
                float hue = (capa / (float)capas + energiaAgudos * 0.5f) % 1f;

                // Las capas más internas son más transparentes
                int alpha = 190 - capa * 45;

                // Las capas interiores también son más delgadas
                float grosor = 2.2f - capa * 0.4f;

                // Se crea el lápiz y se dibujan las ondas superior e inferior
                using (Pen pen = new Pen(HsvAColor(hue, 0.85f, 1f, alpha), grosor))
                {
                    g.DrawLines(pen, _puntosSuperiores[capa]);
                    g.DrawLines(pen, _puntosInferiores[capa]);
                }
            }
        }

        // Conversión HSV a RGB: 6 sectores de 60° e interpolación lineal mediante p/q/t
        private Color HsvAColor(float h, float s, float v, int alpha)
        {
            int hi = (int)(h * 6) % 6; // Determina el sector del hue (0-5)

            float f = h * 6 - (int)(h * 6);
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);

            float r, gc, b;

            switch (hi) // Asigna r, g y b según el sector correspondiente
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
                (int)(r * 255),
                (int)(gc * 255),
                (int)(b * 255));
        }
    }
}