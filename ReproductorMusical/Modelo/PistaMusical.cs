using System;

namespace ReproductorMusical.Modelo
{
    // Entidad que representa una canción cargada en la lista
    public class PistaMusical
    {
        // PROPIEDADES
        public string RutaCompleta { get; private set; }
        public string NombreArchivo { get; private set; }
        public TimeSpan Duracion { get; set; }

        public PistaMusical(string rutaCompleta)
        {
            RutaCompleta = rutaCompleta;
            NombreArchivo = System.IO.Path.GetFileName(rutaCompleta);
            Duracion = TimeSpan.Zero;
        }
    }
}