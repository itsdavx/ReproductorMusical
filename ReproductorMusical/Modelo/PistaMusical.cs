using System;
using TagLib;

namespace ReproductorMusical.Modelo
{
    public class PistaMusical
    {
        public string RutaCompleta { get; private set; }
        public string NombreArchivo { get; private set; }
        public TimeSpan Duracion { get; set; }
        public string Artista { get; private set; }
        public string Album { get; private set; }
        public System.Drawing.Image Portada { get; private set; }

        public PistaMusical(string rutaCompleta)
        {
            RutaCompleta = rutaCompleta;
            NombreArchivo = System.IO.Path.GetFileName(rutaCompleta);
            Duracion = TimeSpan.Zero;
            LeerMetadatos(rutaCompleta);
        }

        // Lee Artista, Album y Portada desde los tags ID3; deja vacíos/null si falla.
        private void LeerMetadatos(string ruta)
        {
            try
            {
                using (TagLib.File tag = TagLib.File.Create(ruta))
                {
                    // Artista: prefiere AlbumArtists, luego Performers
                    if (tag.Tag.AlbumArtists != null && tag.Tag.AlbumArtists.Length > 0
                        && !string.IsNullOrWhiteSpace(tag.Tag.AlbumArtists[0]))
                        Artista = tag.Tag.AlbumArtists[0].Trim();
                    else if (tag.Tag.Performers != null && tag.Tag.Performers.Length > 0
                        && !string.IsNullOrWhiteSpace(tag.Tag.Performers[0]))
                        Artista = tag.Tag.Performers[0].Trim();
                    else
                        Artista = string.Empty;

                    Album = !string.IsNullOrWhiteSpace(tag.Tag.Album)
                        ? tag.Tag.Album.Trim()
                        : string.Empty;

                    // Copia la portada en un Bitmap independiente para no retener el stream.
                    if (tag.Tag.Pictures != null && tag.Tag.Pictures.Length > 0)
                    {
                        IPicture imagen = tag.Tag.Pictures[0];
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imagen.Data.Data))
                        using (System.Drawing.Image imgTemp = System.Drawing.Image.FromStream(ms))
                            Portada = new System.Drawing.Bitmap(imgTemp);
                    }
                }
            }
            catch
            {
                Artista = string.Empty;
                Album = string.Empty;
                Portada = null;
            }
        }
    }
}