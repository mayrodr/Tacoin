using BirriaTycoon.Data;
using System;
using UnityEngine;

namespace BirriaTycoon.Core
{
    /// <summary>
    /// Simula el precio diario de BirriaCoin con volatilidad y efectos de noticias.
    /// </summary>
    public class CryptoSimSystem : MonoBehaviour
    {
        [Header("Precio actual")]
        [SerializeField] private float precioInicial = 10f; // pesos por BirriaCoin
        [SerializeField] private float precioMinimo = 0.5f;
        [SerializeField] private float precioMaximo = 1000f;

        [Header("Volatilidad diaria")]
        [Tooltip("Cambio porcentual mínimo diario (ej: -0.1 = -10%)")]
        [SerializeField] private float volatilidadMin = -0.1f;
        [Tooltip("Cambio porcentual máximo diario (ej: 0.15 = +15%)")]
        [SerializeField] private float volatilidadMax = 0.15f;

        [Header("Noticias del barrio")]
        [Tooltip("Probabilidad de que ocurra una noticia en un día (0-1).")]
        [Range(0f, 1f)]
        [SerializeField] private float probabilidadNoticiaDiaria = 0.3f;

        [Tooltip("Lista de posibles noticias que afectan el precio.")]
        [SerializeField] private BarrioNewsEvent[] noticiasPosibles;

        public float PrecioActual { get; private set; }
        public BarrioNewsEvent NoticiaDeHoy { get; private set; }

        public event Action<float, BarrioNewsEvent> OnPrecioActualizado; // nuevoPrecio, noticiaAplicada

        private System.Random _rng;

        private void Awake()
        {
            _rng = new System.Random();
            PrecioActual = Mathf.Clamp(precioInicial, precioMinimo, precioMaximo);
        }

        /// <summary>
        /// Llamar una vez por día (GameLoopManager.TerminarDia).
        /// </summary>
        public void ActualizarPrecioDiario()
        {
            NoticiaDeHoy = null;

            // 1) Volatilidad base aleatoria
            float t = (float)_rng.NextDouble(); // 0..1
            float factorBase = Mathf.Lerp(1f + volatilidadMin, 1f + volatilidadMax, t);

            float nuevoPrecio = PrecioActual * factorBase;

            // 2) Noticias del barrio (opcional)
            if (noticiasPosibles != null && noticiasPosibles.Length > 0)
            {
                float roll = (float)_rng.NextDouble();
                if (roll <= probabilidadNoticiaDiaria)
                {
                    var noticia = noticiasPosibles[_rng.Next(0, noticiasPosibles.Length)];
                    if (noticia != null)
                    {
                        NoticiaDeHoy = noticia;
                        float factorNoticia = 1f + noticia.cambioPorcentualPrecio;
                        nuevoPrecio *= factorNoticia;
                    }
                }
            }

            PrecioActual = Mathf.Clamp(nuevoPrecio, precioMinimo, precioMaximo);
            OnPrecioActualizado?.Invoke(PrecioActual, NoticiaDeHoy);
        }
    }
}

