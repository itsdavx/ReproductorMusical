using System;
using TagLib;

namespace ReproductorMusical.Modelo
{
    public class PistaMusical
    {
        // PROPIEDADES
        public string RutaCompleta { get; private set; }
        public string NombreArchivo { get; private set; }
        public TimeSpan Duracion { get; set; }

        // Portada embebida en el mp3; null si no tiene
        public System.Drawing.Image Portada { get; private set; }

        public PistaMusical(string rutaCompleta)
        {
            RutaCompleta = rutaCompleta;
            NombreArchivo = System.IO.Path.GetFileName(rutaCompleta);
            Duracion = TimeSpan.Zero;
            Portada = LeerPortada(rutaCompleta);
        }

        private System.Drawing.Image LeerPortada(string ruta)
        {
            try
            {
                TagLib.File tag = TagLib.File.Create(ruta);

                // Tag.Pictures contiene todas las imágenes embebidas del mp3
                if (tag.Tag.Pictures != null && tag.Tag.Pictures.Length > 0)
                {
                    IPicture imagen = tag.Tag.Pictures[0];
                    tag.Dispose();

                    // Convierte los bytes de la imagen a un objeto Image de WinForms
                    using (System.IO.MemoryStream ms =
                        new System.IO.MemoryStream(imagen.Data.Data))
                    {
                        return System.Drawing.Image.FromStream(ms);
                    }
                }

                tag.Dispose();
            }
            catch { }

            return null; // sin portada, la vista pondrá la imagen predeterminada
        }
    }
}