using System;
using TagLib;

namespace ReproductorMusical.Modelo
{
    public class PistaMusical
    {
        // ── Propiedades ──────────────────────────────────────────────────
        public string RutaCompleta { get; private set; }
        public string NombreArchivo { get; private set; }
        public TimeSpan Duracion { get; set; }

        // Metadatos ID3 — cadena vacía si el tag no existe
        public string Artista { get; private set; }
        public string Album { get; private set; }

        // Portada embebida; null si el mp3 no tiene imagen
        public System.Drawing.Image Portada { get; private set; }

        // ── Constructor ──────────────────────────────────────────────────
        public PistaMusical(string rutaCompleta)
        {
            RutaCompleta = rutaCompleta;
            NombreArchivo = System.IO.Path.GetFileName(rutaCompleta);
            Duracion = TimeSpan.Zero;

            LeerMetadatos(rutaCompleta);
        }

        // ── Lectura de metadatos (una sola apertura del tag) ─────────────
        private void LeerMetadatos(string ruta)
        {
            try
            {
                using (TagLib.File tag = TagLib.File.Create(ruta))
                {
                    // Artista: primero AlbumArtists, luego Performers, luego vacío
                    if (tag.Tag.AlbumArtists != null && tag.Tag.AlbumArtists.Length > 0
                        && !string.IsNullOrWhiteSpace(tag.Tag.AlbumArtists[0]))
                        Artista = tag.Tag.AlbumArtists[0].Trim();
                    else if (tag.Tag.Performers != null && tag.Tag.Performers.Length > 0
                        && !string.IsNullOrWhiteSpace(tag.Tag.Performers[0]))
                        Artista = tag.Tag.Performers[0].Trim();
                    else
                        Artista = string.Empty;

                    // Álbum
                    Album = !string.IsNullOrWhiteSpace(tag.Tag.Album)
                        ? tag.Tag.Album.Trim()
                        : string.Empty;

                    // Portada
                    if (tag.Tag.Pictures != null && tag.Tag.Pictures.Length > 0)
                    {
                        IPicture imagen = tag.Tag.Pictures[0];
                        using (System.IO.MemoryStream ms =
                            new System.IO.MemoryStream(imagen.Data.Data))
                        {
                            Portada = System.Drawing.Image.FromStream(ms);
                        }
                    }
                }
            }
            catch
            {
                // Si TagLib falla (archivo corrupto, formato raro, etc.)
                // dejamos las propiedades en sus valores por defecto (empty / null)
                Artista = string.Empty;
                Album = string.Empty;
                Portada = null;
            }
        }
    }
}