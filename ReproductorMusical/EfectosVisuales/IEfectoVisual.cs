using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    public interface IEfectoVisual
    {
        string Nombre { get; }
        void Renderizar(Graphics g, int ancho, int alto, float[] buffer);
    }
}