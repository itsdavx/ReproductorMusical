using System;
using System.Collections.Generic;
using ReproductorMusical.Modelo;

namespace ReproductorMusical.Controlador
{
    /// <summary>
    /// Gestiona la lista de pistas y la navegación entre ellas.
    /// </summary>
    public class GestorPlaylist
    {
        // ── Estado interno ───────────────────────────────────────────────
        private readonly List<PistaMusical> _pistas = new List<PistaMusical>();
        private readonly Random _rand = new Random();

        // ── Propiedades de consulta ──────────────────────────────────────
        public int IndicePistaActual { get; private set; } = -1;
        public int CantidadPistas => _pistas.Count;
        public bool HayPistas => _pistas.Count > 0;

        /// <summary>Pista actualmente activa; null si no hay ninguna.</summary>
        public PistaMusical PistaActual => ObtenerPista(IndicePistaActual);

        // ── Acceso a pistas individuales ─────────────────────────────────

        /// <summary>Devuelve la pista en el índice dado, o null si es inválido.</summary>
        public PistaMusical ObtenerPista(int indice)
        {
            if (indice < 0 || indice >= _pistas.Count) return null;
            return _pistas[indice];
        }

        // ── Operaciones sobre la lista ───────────────────────────────────

        /// <summary>Agrega nuevas pistas al final de la lista existente.</summary>
        public void AgregarPistas(string[] rutas)
        {
            foreach (string ruta in rutas)
                _pistas.Add(new PistaMusical(ruta));
        }

        /// <summary>Vacía la lista y reinicia el índice activo.</summary>
        public void LimpiarPistas()
        {
            _pistas.Clear();
            IndicePistaActual = -1;
        }

        /// <summary>Devuelve los nombres de archivo para poblar la Vista.</summary>
        public List<string> ObtenerNombresPistas()
        {
            List<string> nombres = new List<string>();
            foreach (PistaMusical p in _pistas)
                nombres.Add(p.NombreArchivo);
            return nombres;
        }

        /// <summary>
        /// Devuelve todas las pistas como lista de solo lectura
        /// para que la Vista pueda leer Artista y Album al dibujar el ListBox.
        /// </summary>
        public IReadOnlyList<PistaMusical> ObtenerTodasLasPistas()
        {
            return _pistas.AsReadOnly();
        }

        /// <summary>Establece el índice activo. Devuelve true si era válido.</summary>
        public bool SeleccionarPista(int indice)
        {
            if (indice < 0 || indice >= _pistas.Count) return false;
            IndicePistaActual = indice;
            return true;
        }

        /// <summary>Actualiza la duración almacenada de la pista activa.</summary>
        public void ActualizarDuracionActual(TimeSpan duracion)
        {
            if (PistaActual != null)
                PistaActual.Duracion = duracion;
        }

        // ── Navegación ───────────────────────────────────────────────────

        /// <summary>Calcula el índice de la siguiente pista según el modo.</summary>
        public int CalcularSiguiente(ModoReproduccion modo)
        {
            if (!HayPistas) return -1;

            switch (modo)
            {
                case ModoReproduccion.Aleatorio:
                    return _rand.Next(_pistas.Count);
                case ModoReproduccion.RepetirUno:
                    return IndicePistaActual;
                default: // Secuencial
                    return IndicePistaActual < _pistas.Count - 1
                        ? IndicePistaActual + 1 : 0;
            }
        }

        /// <summary>Calcula el índice de la pista anterior según el modo.</summary>
        public int CalcularAnterior(ModoReproduccion modo)
        {
            if (!HayPistas) return -1;

            switch (modo)
            {
                case ModoReproduccion.Aleatorio:
                    return _rand.Next(_pistas.Count);
                case ModoReproduccion.RepetirUno:
                    return IndicePistaActual;
                default: // Secuencial
                    return IndicePistaActual > 0
                        ? IndicePistaActual - 1 : _pistas.Count - 1;
            }
        }
    }
}