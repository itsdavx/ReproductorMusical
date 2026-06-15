using System;
using System.Collections.Generic;
using ReproductorMusical.Modelo;

namespace ReproductorMusical.Controlador
{
    public class GestorPlaylist
    {
        private readonly List<PistaMusical> _pistas = new List<PistaMusical>();
        private readonly Random _rand = new Random();

        public int IndicePistaActual { get; private set; } = -1;
        public int CantidadPistas => _pistas.Count;
        public bool HayPistas => _pistas.Count > 0;

        public PistaMusical PistaActual => ObtenerPista(IndicePistaActual);

        // Devuelve la pista en el índice dado, o null si es inválido.
        public PistaMusical ObtenerPista(int indice)
        {
            if (indice < 0 || indice >= _pistas.Count) return null;
            return _pistas[indice];
        }

        // Agrega nuevas pistas al final de la lista.
        public void AgregarPistas(string[] rutas)
        {
            foreach (string ruta in rutas)
                _pistas.Add(new PistaMusical(ruta));
        }

        // Vacía la lista y reinicia el índice activo.
        public void LimpiarPistas()
        {
            _pistas.Clear();
            IndicePistaActual = -1;
        }

        // Devuelve los nombres de archivo para poblar la Vista.
        public List<string> ObtenerNombresPistas()
        {
            List<string> nombres = new List<string>();
            foreach (PistaMusical p in _pistas)
                nombres.Add(p.NombreArchivo);
            return nombres;
        }

        // Devuelve todas las pistas para que la Vista pueda leer Artista y Album.
        public IReadOnlyList<PistaMusical> ObtenerTodasLasPistas()
        {
            return _pistas.AsReadOnly();
        }

        // Establece el índice activo. Devuelve true si era válido.
        public bool SeleccionarPista(int indice)
        {
            if (indice < 0 || indice >= _pistas.Count) return false;
            IndicePistaActual = indice;
            return true;
        }

        // Actualiza la duración almacenada de la pista activa.
        public void ActualizarDuracionActual(TimeSpan duracion)
        {
            if (PistaActual != null)
                PistaActual.Duracion = duracion;
        }

        // Calcula el índice de la siguiente pista según el modo de reproducción.
        public int CalcularSiguiente(ModoReproduccion modo)
        {
            if (!HayPistas) return -1;

            switch (modo)
            {
                case ModoReproduccion.Aleatorio:
                    return _rand.Next(_pistas.Count);
                case ModoReproduccion.RepetirUno:
                    return IndicePistaActual;
                default:
                    return IndicePistaActual < _pistas.Count - 1
                        ? IndicePistaActual + 1 : 0;
            }
        }

        // Calcula el índice de la pista anterior según el modo de reproducción.
        public int CalcularAnterior(ModoReproduccion modo)
        {
            if (!HayPistas) return -1;

            switch (modo)
            {
                case ModoReproduccion.Aleatorio:
                    return _rand.Next(_pistas.Count);
                case ModoReproduccion.RepetirUno:
                    return IndicePistaActual;
                default:
                    return IndicePistaActual > 0
                        ? IndicePistaActual - 1 : _pistas.Count - 1;
            }
        }

        // Elimina una pista y ajusta el índice activo para que no quede desincronizado.
        public void QuitarPista(int indice)
        {
            if (indice < 0 || indice >= _pistas.Count) return;

            _pistas.RemoveAt(indice);

            if (_pistas.Count == 0)
                IndicePistaActual = -1;
            else if (indice < IndicePistaActual)
                IndicePistaActual--;
            else if (indice == IndicePistaActual)
                IndicePistaActual = -1;
        }
    }
}