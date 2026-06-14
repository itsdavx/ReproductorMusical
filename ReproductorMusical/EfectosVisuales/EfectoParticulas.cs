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

            float energiaBajos = 0f, energiaMedios = 0f, energiaAgudos = 0f;
            for (int i = 0; i < 16; i++) energiaBajos += Math.Abs(buffer[i]);
            for (int i = 16; i < 64; i++) energiaMedios += Math.Abs(buffer[i]);
            for (int i = 64; i < fftLen; i++) energiaAgudos += Math.Abs(buffer[i]);
            energiaBajos /= 16f;
            energiaMedios /= 48f;
            energiaAgudos /= (fftLen - 64f);

            float energiaTotal = (energiaBajos + energiaMedios + energiaAgudos) / 3f;
            bool beat = energiaBajos > _energiaAnt * 1.5f && energiaBajos > 0.015f;
            _energiaAnt = energiaBajos * 0.5f + _energiaAnt * 0.5f;

            int centroX = ancho / 2;
            int centroY = alto / 2;
            int nuevas = (int)(energiaTotal * 25) + (beat ? 15 : 0);
            if (nuevas > 40) nuevas = 40;

            for (int i = 0; i < nuevas && _particulas.Count < 400; i++)
            {
                double ang = _rand.NextDouble() * 2.0 * Math.PI;
                float speed = 0.8f + (float)_rand.NextDouble() * (beat ? 4.0f : 2.2f);
                float vida = 20f + (float)_rand.NextDouble() * 35f;

                Color col;
                if (energiaBajos >= energiaMedios && energiaBajos >= energiaAgudos)
                    col = Color.FromArgb(_rand.Next(200, 255), 60, 20);
                else if (energiaMedios >= energiaAgudos)
                    col = Color.FromArgb(20, _rand.Next(200, 255), 60);
                else
                    col = Color.FromArgb(60, 140, _rand.Next(200, 255));

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
                p.X += p.VX;
                p.Y += p.VY;
                p.VY += 0.045f;
                p.VX *= 0.99f;
                p.Vida -= 1f;

                if (p.Vida <= 0 || p.X < 0 || p.X > ancho || p.Y < 0 || p.Y > alto)
                {
                    _particulas.RemoveAt(i);
                    continue;
                }

                int alpha = (int)(255f * (p.Vida / p.VidaMax));
                alpha = Math.Max(0, Math.Min(255, alpha));
                int tam = Math.Max(1, (int)p.Tamano);

                using (SolidBrush br = new SolidBrush(Color.FromArgb(alpha, p.Color)))
                    g.FillRectangle(br, p.X - tam * 0.5f, p.Y - tam * 0.5f, tam, tam);
            }
        }
    }
}