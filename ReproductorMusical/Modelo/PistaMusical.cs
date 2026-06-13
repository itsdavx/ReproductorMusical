using System;

namespace ReproductorMusical.Modelo
{
    public class PistaMusical
    {
        // PROPIEDADES
        public string RutaCompleta { get; private set; }
        public string NombreArchivo { get; private set; }
        public string NombreAlbum { get; private set; }
        public TimeSpan Duracion { get; set; }

        public PistaMusical(string rutaCompleta)
        {
            RutaCompleta = rutaCompleta;
            NombreArchivo = System.IO.Path.GetFileName(rutaCompleta);
            Duracion = TimeSpan.Zero;

            // Lee el tag Album del mp3 con TagLib
            NombreAlbum = LeerAlbum(rutaCompleta);
        }

        private string LeerAlbum(string ruta)
        {
            try
            {
                TagLib.File tag = TagLib.File.Create(ruta);
                string album = tag.Tag.Album;
                tag.Dispose();

                // Si el tag existe y no está vacío, lo devuelve limpio
                if (!string.IsNullOrWhiteSpace(album))
                    return album.Trim();
            }
            catch { }

            return string.Empty; // si falla o no tiene tag, devuelve vacío
        }
    }
}