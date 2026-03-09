using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BirriaTycoon.Core;

namespace BirriaTycoon.UI
{
    /// <summary>
    /// Muestra un reporte semanal sencillo con barras de aciertos y errores.
    /// </summary>
    public class WeeklyReportUIController : MonoBehaviour
    {
        [Header("Referencias de sistema")]
        [SerializeField] private GameLoopManager gameLoopManager;
        [SerializeField] private DecisionHistoryManager decisionHistoryManager;

        [Header("Raíz del panel")]
        [SerializeField] private GameObject root;

        [Header("Textos")]
        [SerializeField] private TextMeshProUGUI tituloSemanaText;
        [SerializeField] private TextMeshProUGUI resumenText;

        [Header("Barras (0-1, usando Image.fillAmount)")]
        [SerializeField] private Image barraAciertos;
        [SerializeField] private Image barraErrores;

        [Header("Botón")]
        [SerializeField] private Button cerrarButton;

        private void Awake()
        {
            if (cerrarButton != null)
            {
                cerrarButton.onClick.AddListener(Hide);
            }

            if (root != null)
            {
                root.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (gameLoopManager != null)
            {
                gameLoopManager.OnSemanaTerminada += OnSemanaTerminada;
            }
        }

        private void OnDisable()
        {
            if (gameLoopManager != null)
            {
                gameLoopManager.OnSemanaTerminada -= OnSemanaTerminada;
            }
        }

        private void OnSemanaTerminada(int semanaGlobal)
        {
            if (decisionHistoryManager == null) return;

            var resumen = decisionHistoryManager.CalcularResumenSemana(semanaGlobal);
            MostrarResumen(resumen);
        }

        private void MostrarResumen(WeeklySummary resumen)
        {
            if (root != null)
                root.SetActive(true);

            if (tituloSemanaText != null)
            {
                tituloSemanaText.text = $"Reporte Semana {resumen.semanaGlobal}";
            }

            if (resumenText != null)
            {
                resumenText.text =
                    $"Decisiones totales: {resumen.totalDecisiones}\n" +
                    $"Aciertos (recomendadas): {resumen.totalRecomendadas}\n" +
                    $"Errores (no recomendadas): {resumen.totalNoRecomendadas}\n" +
                    $"Porcentaje aciertos: {(resumen.porcentajeRecomendadas * 100f):0}%";
            }

            float total = Mathf.Max(1, resumen.totalDecisiones); // evitar división por 0
            float ratioAciertos = resumen.totalRecomendadas / total;
            float ratioErrores = resumen.totalNoRecomendadas / total;

            if (barraAciertos != null)
            {
                barraAciertos.fillAmount = ratioAciertos;
            }

            if (barraErrores != null)
            {
                barraErrores.fillAmount = ratioErrores;
            }
        }

        public void Hide()
        {
            if (root != null)
                root.SetActive(false);
        }
    }
}

