using System;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoEspectroCircular : IEfectoVisual //Herencia de la interfaz
    {
        public string Nombre => "Espectro Circular"; //Devolve el nombre del efecto

        private float _radioSuavizado = 50f; //Guarda el radio actual del circulo, suavizado para evitar cambios bruscos
        private float _rotacion = 0f;        //Guarda el ángulo de rotación acumulado

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer) //Implementacion del metodo de herencia
        {
            int centroX = ancho / 2;    //Determina el centro del panel
            int centroY = alto / 2; 

            int puntos = 72;            //Circulo esta formado por 72 barras, cada una representa un rango de frecuencia del espectro

            int fftLen = buffer.Length; //Obtiene el tamaño del buffer de audio

            float radioMax = Math.Min(centroX, centroY) - 10f; //Calcula el radio máximo del círculo con un padding de 10 px

            // Calcula la energía global del audio
            float energiaTotal = 0f;
            for (int i = 0; i < fftLen; i++) energiaTotal += Math.Abs(buffer[i]);
            energiaTotal /= fftLen;

            // Calcula la energía de bajos con las primeras 8 bandas
            float energiaBajos = 0f;
            for (int i = 0; i < 8; i++) energiaBajos += Math.Abs(buffer[i]);
            energiaBajos /= 8f;

            float radioObj = 45f + energiaBajos * 55f * (0.7f + energiaTotal * 1.2f);   // El radio del circulo exterior
            _radioSuavizado = radioObj * 0.3f + _radioSuavizado * 0.7f;                 // Suavizado del radio para evitar cambios bruscos
            _rotacion += 0.007f + energiaBajos * 0.035f;                                // Velocidad del giro

            // Arreglo de Angulos
            double[] angulos = new double[puntos];
            for (int i = 0; i < puntos; i++)
                angulos[i] = _rotacion + (i * 2.0 * Math.PI / puntos); //Calcula el angulo con 2π/puntos (72) por la posicion, mas la rotacion para que gire

            //Dibuja cada barra del espectro circular
            for (int i = 0; i < puntos; i++)
            {
                // Toma una muestra del buffer cada cierto intervalo (por ejemplo 512/72 ≈ 7 bandas)  para representar el espectro completo
                int idx = i * (fftLen / puntos);
                float amp = Math.Abs(buffer[idx % fftLen]);

                // Radio exterior: Hasta donde llega la barra, proporcional a la amplitud de esa banda y al volumen general
                float ext = Math.Min(_radioSuavizado + amp * 115f * (0.7f + energiaTotal * 1.8f), radioMax);

                // Coordenadas polares a cartesianas: x = r·cos(θ), y = r·sin(θ)
                float x1 = centroX + (float)(_radioSuavizado * Math.Cos(angulos[i]));
                float y1 = centroY + (float)(_radioSuavizado * Math.Sin(angulos[i]));
                float x2 = centroX + (float)(ext * Math.Cos(angulos[i]));
                float y2 = centroY + (float)(ext * Math.Sin(angulos[i]));

                // Color HSV (Tono, Saturacion, Valor)
                float hue = (float)(i / (double)puntos);                    // Distribución uniforme de tonos a lo largo del círculo
                float sat = 0.7f + amp * 0.3f;                              // Mayor amplitud da color mas intenso
                float grosor = 1.5f + amp * 5f * (0.5f + energiaTotal);     // Mayor amplitud y volumen general hacen las barras más gruesas

                using (Pen pen = new Pen(HsvColor(hue, sat, 0.95f), grosor)) // Se crea el lapiz y se dibuja la barra
                    g.DrawLine(pen, x1, y1, x2, y2); 
            }
        }

        // Conversión HSV a RGB: 6 sectores de 60°, interpolación lineal con p/q/t
        private Color HsvColor(float h, float s, float v)
        {
            int hi = (int)(h * 6) % 6; // Sector del hue (0-5)
            float f = h * 6 - (int)(h * 6);
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);
            float r, gc, b;
            switch (hi) // Asigna r, g, b según el sector del hue
            {
                case 0: r = v; gc = t; b = p; break;
                case 1: r = q; gc = v; b = p; break;
                case 2: r = p; gc = v; b = t; break;
                case 3: r = p; gc = q; b = v; break;
                case 4: r = t; gc = p; b = v; break;
                default: r = v; gc = p; b = q; break;
            }
            return Color.FromArgb((int)(r * 255), (int)(gc * 255), (int)(b * 255));
        }
    }
}