namespace ReproductorMusical.Modelo
{
    /// <summary>
    /// Define los modos de navegación entre pistas.
    /// Reemplaza el int mágico (0, 1, 2) que existía antes en el Controlador y la Vista.
    /// </summary>
    public enum ModoReproduccion
    {
        Aleatorio = 0,
        Secuencial = 1,
        RepetirUno = 2
    }
}