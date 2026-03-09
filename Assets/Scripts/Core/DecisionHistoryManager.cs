using System;
using System.Collections.Generic;
using UnityEngine;
using BirriaTycoon.Data;

namespace BirriaTycoon.Core
{
    [Serializable]
    public class DecisionRecord
    {
        public int diaGlobal;
        public int diaSemana;
        public int semanaGlobal;
        public DecisionFinancieraEntry entry;
    }

    [Serializable]
    public class WeeklySummary
    {
        public int semanaGlobal;
        public int totalDecisiones;
        public int totalRecomendadas;
        public int totalNoRecomendadas;
        public float porcentajeRecomendadas;
    }

    /// <summary>
    /// Lleva un historial simple de decisiones financieras para usar en el reporte semanal.
    /// </summary>
    public class DecisionHistoryManager : MonoBehaviour
    {
        [SerializeField] private List<DecisionRecord> historial = new List<DecisionRecord>();

        public IReadOnlyList<DecisionRecord> Historial => historial;

        public void RegistrarDecision(int diaGlobal, int diaSemana, int semanaGlobal, DecisionFinancieraEntry entry)
        {
            if (entry == null) return;

            var record = new DecisionRecord
            {
                diaGlobal = diaGlobal,
                diaSemana = diaSemana,
                semanaGlobal = semanaGlobal,
                entry = entry
            };

            historial.Add(record);
        }

        public WeeklySummary CalcularResumenSemana(int semanaGlobal)
        {
            var resumen = new WeeklySummary
            {
                semanaGlobal = semanaGlobal,
                totalDecisiones = 0,
                totalRecomendadas = 0,
                totalNoRecomendadas = 0,
                porcentajeRecomendadas = 0f
            };

            for (int i = 0; i < historial.Count; i++)
            {
                var h = historial[i];
                if (h.semanaGlobal != semanaGlobal) continue;

                resumen.totalDecisiones++;
                if (h.entry.esDecisionRecomendada)
                    resumen.totalRecomendadas++;
                else
                    resumen.totalNoRecomendadas++;
            }

            if (resumen.totalDecisiones > 0)
            {
                resumen.porcentajeRecomendadas =
                    (float)resumen.totalRecomendadas / resumen.totalDecisiones;
            }

            return resumen;
        }
    }
}

