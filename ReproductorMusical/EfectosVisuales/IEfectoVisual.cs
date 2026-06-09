using System.Drawing;

namespace ReproductorMusical.EfectosVisuales
{
    // Contrato que todos los efectos visuales deben cumplir
    public interface IEfectoVisual
    {
        // Nombre legible para mostrarlo en el ComboBox
        string Nombre { get; }

        // Dibuja el efecto sobre el Graphics recibido
        void Renderizar(Graphics g, int ancho, int alto, float[] buffer);
    }
}