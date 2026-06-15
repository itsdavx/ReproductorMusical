using System;
using System.Collections.Generic;
using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public class EfectoParticulas : IEfectoVisual
    {
        public string Nombre => "Partículas";

        private class Particula
        {
            public float X, Y, VX, VY, Vida, VidaMax, Tamano;
            public Color Color;
        }

        private readonly List<Particula> _particulas = new List<Particula>();
        private readonly Random _rand = new Random();
        private float _energiaAnt = 0f;

        public void Renderizar(Graphics g, int ancho, int alto, float[] buffer)
        {
            int fftLen = buffer.Length;

            // Energía por rango de frecuencia: bajos [0-15], medios [16-63], agudos [64+]
            float energiaBajos = 0f, energiaMedios = 0f, energiaAgudos = 0f;
            for (int i = 0; i < 16; i++) energiaBajos += Math.Abs(buffer[i]);
            for (int i = 16; i < 64; i++) energiaMedios += Math.Abs(buffer[i]);
            for (int i = 64; i < fftLen; i++) energiaAgudos += Math.Abs(buffer[i]);
            energiaBajos /= 16f;
            energiaMedios /= 48f;
            energiaAgudos /= (fftLen - 64f);

            float energiaTotal = (energiaBajos + energiaMedios + energiaAgudos) / 3f;

            // Beat: pico de bajos supera 1.5× el promedio suavizado (50% nuevo / 50% histórico)
            bool beat = energiaBajos > _energiaAnt * 1.5f && energiaBajos > 0.015f;
            _energiaAnt = energiaBajos * 0.5f + _energiaAnt * 0.5f;

            int centroX = ancho / 2;
            int centroY = alto / 2;

            // Cantidad de partículas nuevas proporcional a energía; ráfaga extra en beat
            int nuevas = Math.Min((int)(energiaTotal * 25) + (beat ? 15 : 0), 40);

            for (int i = 0; i < nuevas && _particulas.Count < 400; i++)
            {
                // Dirección aleatoria uniforme en [0, 2π]; velocidad mayor en beat
                double ang = _rand.NextDouble() * 2.0 * Math.PI;
                float speed = 0.8f + (float)_rand.NextDouble() * (beat ? 4.0f : 2.2f);
                float vida = 20f + (float)_rand.NextDouble() * 35f;

                // Color dominante según rango de frecuencia con mayor energía
                Color col;
                if (energiaBajos >= energiaMedios && energiaBajos >= energiaAgudos)
                    col = Color.FromArgb(_rand.Next(200, 255), 60, 20);       // rojo → bajos
                else if (energiaMedios >= energiaAgudos)
                    col = Color.FromArgb(20, _rand.Next(200, 255), 60);       // verde → medios
                else
                    col = Color.FromArgb(60, 140, _rand.Next(200, 255));      // azul → agudos

                _particulas.Add(new Particula
                {
                    X = centroX,
                    Y = centroY,
                    VX = (float)(Math.Cos(ang) * speed),
                    VY = (float)(Math.Sin(ang) * speed),
                    Vida = vida,
                    VidaMax = vida,
                    Tamano = 2f + (float)_rand.NextDouble() * (beat ? 6f : 3f),
                    Color = col
                });
            }

            for (int i = _particulas.Count - 1; i >= 0; i--)
            {
                Particula p = _particulas[i];

                // Integración de Euler: posición += velocidad; gravedad += 0.045 por frame
                p.X += p.VX;
                p.Y += p.VY;
                p.VY += 0.045f;   // gravedad constante
                p.VX *= 0.99f;    // rozamiento del aire (fricción lineal)
                p.Vida -= 1f;

                if (p.Vida <= 0 || p.X < 0 || p.X > ancho || p.Y < 0 || p.Y > alto)
                {
                    _particulas.RemoveAt(i);
                    continue;
                }

                // Alpha decae linealmente con la vida restante: 255 → 0
                int alpha = Math.Max(0, Math.Min(255, (int)(255f * (p.Vida / p.VidaMax))));
                int tam = Math.Max(1, (int)p.Tamano);

                using (SolidBrush br = new SolidBrush(Color.FromArgb(alpha, p.Color)))
                    g.FillRectangle(br, p.X - tam * 0.5f, p.Y - tam * 0.5f, tam, tam);
            }
        }
    }
}