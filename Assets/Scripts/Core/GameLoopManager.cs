using System;
using UnityEngine;

namespace BirriaTycoon.Core
{
    public class GameLoopManager : MonoBehaviour
    {
        public static GameLoopManager Instance { get; private set; }

        [Header("Configuración de ciclo")]
        [SerializeField] private int accionesFinancierasPorDia = 5;
        [SerializeField] private int diasPorSemana = 6;

        [Header("Referencia al sistema de cripto")]
        [SerializeField] private CryptoSimSystem cryptoSimSystemRef;

        public int DiaGlobalActual { get; private set; } = 1;
        public int DiaSemanaActual { get; private set; } = 1; // 1..diasPorSemana
        public int SemanaGlobalActual { get; private set; } = 1;
        public int AccionesRestantesEnDia { get; private set; }

        public event Action OnNuevoDiaComenzado;
        public event Action OnDiaTerminado;
        public event Action OnNuevaSemanaComenzada;
        public event Action<int> OnSemanaTerminada;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
        if (EconomyManager.Instance !=null)
        {EconomyManager.Instance.Inicializar(
            efectivoInicial: 500f,
            ahorroInicial: 200f,
            cryptoInicial: 0f
        );
        }
            IniciarNuevoDia();
        }

        private void IniciarNuevoDia()
        {
            AccionesRestantesEnDia = accionesFinancierasPorDia;
            OnNuevoDiaComenzado?.Invoke();
        }

        /// <summary>
        /// Llamar cada vez que el jugador toma una decisión financiera.
        /// </summary>
        public void RegistrarDecisionFinancieraTomada()
        {
            if (AccionesRestantesEnDia <= 0)
                return;

            AccionesRestantesEnDia--;

            if (AccionesRestantesEnDia <= 0)
            {
                TerminarDia();
            }
        }

        private void TerminarDia()
        {
            OnDiaTerminado?.Invoke();

            if (cryptoSimSystemRef != null)
            {
                cryptoSimSystemRef.ActualizarPrecioDiario();
            }

            AvanzarCalendario();
            IniciarNuevoDia();
        }

        private void AvanzarCalendario()
        {
            DiaGlobalActual++;
            DiaSemanaActual++;

            if (DiaSemanaActual > diasPorSemana)
            {
                int semanaTerminada = SemanaGlobalActual;
                OnSemanaTerminada?.Invoke(semanaTerminada);

                DiaSemanaActual = 1;
                SemanaGlobalActual++;
                OnNuevaSemanaComenzada?.Invoke();
            }
        }

        /// <summary>
        /// Permite forzar terminar el día desde UI (botón "Cerrar puesto", por ejemplo).
        /// </summary>
        public void ForzarFinDeDia()
        {
            if (AccionesRestantesEnDia > 0)
            {
                AccionesRestantesEnDia = 0;
                TerminarDia();
            }
        }
    }
}
