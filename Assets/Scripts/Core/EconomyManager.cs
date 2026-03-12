using System;
using UnityEngine;

namespace BirriaTycoon.Core
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        [Header("Saldos actuales (en pesos)")]
        [SerializeField] private float efectivoNegocio;
        [SerializeField] private float ahorroPersonal;

        [Header("Cripto (BirriaCoin)")]
        [SerializeField] private float carteraCrypto; // Cantidad de BirriaCoin, no pesos

        public float EfectivoNegocio => efectivoNegocio;
        public float AhorroPersonal => ahorroPersonal;
        public float CarteraCrypto => carteraCrypto;

        public event Action OnValoresActualizados;
        public event Action OnBancarrota;

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

        public void Inicializar(float efectivoInicial, float ahorroInicial, float cryptoInicial = 0f)
        {
            efectivoNegocio = Mathf.Max(0f, efectivoInicial);
            ahorroPersonal  = Mathf.Max(0f, ahorroInicial);
            carteraCrypto   = Mathf.Max(0f, cryptoInicial);

            NotificarCambioValores();
            VerificarBancarrota();
        }

        /// <summary>
        /// Suma pesos al efectivo del negocio (venta de birria, premios, etc.).
        /// </summary>
        public void RegistrarVenta(float ingresoPesos)
        {
            if (ingresoPesos <= 0f) return;

            efectivoNegocio += ingresoPesos;
            NotificarCambioValores();
        }

        /// <summary>
        /// Mueve dinero del efectivo del negocio al ahorro personal.
        /// </summary>
        public bool MoverAEhorro(float monto)
        {
            if (monto <= 0f || monto > efectivoNegocio)
                return false;

            efectivoNegocio -= monto;
            ahorroPersonal  += monto;

            NotificarCambioValores();
            VerificarBancarrota();
            return true;
        }

        /// <summary>
        /// Mueve pesos del efectivo del negocio a BirriaCoin.
        /// </summary>
        /// <param name="montoPesos">Pesos a convertir.</param>
        /// <param name="precioActualBirriaCoin">Precio actual 1 BirriaCoin en pesos.</param>
        public bool MoverAInversionCrypto(float montoPesos, float precioActualBirriaCoin)
        {
            if (montoPesos <= 0f || montoPesos > efectivoNegocio || precioActualBirriaCoin <= 0f)
                return false;

            efectivoNegocio -= montoPesos;

            float birriaCoinsCompradas = montoPesos / precioActualBirriaCoin;
            carteraCrypto += birriaCoinsCompradas;

            NotificarCambioValores();
            VerificarBancarrota();
            return true;
        }

        /// <summary>
        /// Vende BirriaCoin a pesos, sumándolos al efectivo del negocio.
        /// </summary>
        public bool VenderCrypto(float birriaCoinsAVender, float precioActualBirriaCoin)
        {
            if (birriaCoinsAVender <= 0f || birriaCoinsAVender > carteraCrypto || precioActualBirriaCoin <= 0f)
                return false;

            carteraCrypto   -= birriaCoinsAVender;
            float pesosObtenidos = birriaCoinsAVender * precioActualBirriaCoin;
            efectivoNegocio += pesosObtenidos;

            NotificarCambioValores();
            VerificarBancarrota();
            return true;
        }

        /// <summary>
        /// Aplica cambios directos (por decisiones, eventos, etc.).
        /// Valores pueden ser positivos o negativos.
        /// </summary>
        public void AplicarCambioFinanciero(float deltaEfectivo, float deltaAhorro, float deltaBirriaCoins)
        {
            efectivoNegocio = Mathf.Max(0f, efectivoNegocio + deltaEfectivo);
            ahorroPersonal  = Mathf.Max(0f, ahorroPersonal  + deltaAhorro);
            carteraCrypto   = Mathf.Max(0f, carteraCrypto   + deltaBirriaCoins);

            NotificarCambioValores();
            VerificarBancarrota();
        }

        private void NotificarCambioValores()
        {
            OnValoresActualizados?.Invoke();
        }

        private void VerificarBancarrota()
        {
            if (efectivoNegocio <= 0f && ahorroPersonal <= 0f)
            {
                OnBancarrota?.Invoke();
            }
        }
    }
}
