using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BirriaTycoon.Core;
using BirriaTycoon.Data;

namespace BirriaTycoon.UI
{
    public class DailyDecisionUIController : MonoBehaviour
    {
        [Header("Referencias de sistema")]
        [SerializeField] private GameLoopManager gameLoopManager;
        [SerializeField] private EconomyManager economyManager;
        [SerializeField] private DecisionHistoryManager decisionHistoryManager;

        [Header("Datos")]
        [Tooltip("Lista de ScriptableObjects, uno por día.")]
        [SerializeField] private List<DecisionesDiarias> decisionesPorDia = new List<DecisionesDiarias>();

        [Header("UI Día")]
        [SerializeField] private TextMeshProUGUI diaLabel;

        [Header("UI Opciones")]
        [SerializeField] private Transform opcionesContainer;
        [SerializeField] private DecisionOptionButton opcionPrefab;

        [Header("Pop-up educativo (opcional)")]
        [SerializeField] private EducationPopupController popupConsejo;

        private readonly Dictionary<int, DecisionesDiarias> _mapDiaADecisiones =
            new Dictionary<int, DecisionesDiarias>();

        private void Awake()
        {
            _mapDiaADecisiones.Clear();
            foreach (var diario in decisionesPorDia)
            {
                if (diario == null) continue;
                if (_mapDiaADecisiones.ContainsKey(diario.diaNumero)) continue;
                _mapDiaADecisiones.Add(diario.diaNumero, diario);
            }
        }

        private void OnEnable()
        {
            if (gameLoopManager != null)
            {
                gameLoopManager.OnNuevoDiaComenzado += OnNuevoDia;
            }
        }

        private void OnDisable()
        {
            if (gameLoopManager != null)
            {
                gameLoopManager.OnNuevoDiaComenzado -= OnNuevoDia;
            }
        }

        private void Start()
        {
            OnNuevoDia();
        }

        private void OnNuevoDia()
        {
            if (gameLoopManager == null) return;

            int diaActual = gameLoopManager.DiaGlobalActual;
            if (diaLabel != null)
            {
                diaLabel.text = $"Día {diaActual}";
            }

            RefrescarOpcionesParaDia(diaActual);
        }

        private void RefrescarOpcionesParaDia(int dia)
        {
            foreach (Transform child in opcionesContainer)
            {
                Destroy(child.gameObject);
            }

            if (!_mapDiaADecisiones.TryGetValue(dia, out var decisionesDiarias)
                || decisionesDiarias == null
                || decisionesDiarias.decisiones == null
                || decisionesDiarias.decisiones.Count == 0)
            {
                Debug.LogWarning($"[DailyDecisionUI] No hay DecisionesDiarias configuradas para el día {dia}.");
                return;
            }

            foreach (var decisionEntry in decisionesDiarias.decisiones)
            {
                var btn = Instantiate(opcionPrefab, opcionesContainer);
                btn.Configurar(decisionEntry, OnDecisionSeleccionada);
            }
        }

        private void OnDecisionSeleccionada(DecisionFinancieraEntry entry)
        {
            if (economyManager == null || gameLoopManager == null || entry == null)
                return;

            economyManager.AplicarCambioFinanciero(
                entry.deltaEfectivoNegocio,
                entry.deltaAhorroPersonal,
                entry.deltaCarteraCrypto
            );

            gameLoopManager.RegistrarDecisionFinancieraTomada();

            if (decisionHistoryManager != null)
            {
                decisionHistoryManager.RegistrarDecision(
                    gameLoopManager.DiaGlobalActual,
                    gameLoopManager.DiaSemanaActual,
                    gameLoopManager.SemanaGlobalActual,
                    entry
                );
            }

            if (popupConsejo != null && !string.IsNullOrWhiteSpace(entry.consejo))
            {
                string titulo = string.IsNullOrWhiteSpace(entry.titulo) ? "Consejo del día" : entry.titulo;
                popupConsejo.Show(titulo, entry.consejo);
            }
        }
    }
}
