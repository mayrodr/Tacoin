using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirriaTycoon.Data
{
    [Serializable]
    public class DecisionFinancieraEntry
    {
        [Header("Texto")]
        public string id;             // Ej: "DIA1_OPCION_A"
        public string titulo;         // Ej: "Reinvertir en más birria"
        [TextArea] public string descripcion;
        [TextArea] public string consejo; // Tip financiero/educativo que se mostrará al jugador

        [Header("Impacto financiero")]
        public float deltaEfectivoNegocio; // + o - en pesos
        public float deltaAhorroPersonal;  // + o - en pesos
        public float deltaCarteraCrypto;   // + o - en BirriaCoin

        [Header("Metadatos opcionales")]
        public bool esDecisionRecomendada; // Para el reporte semanal (acierto/error)
        public Sprite icono;              // Para UI si lo necesitas
    }

    [CreateAssetMenu(
        fileName = "DecisionesDiarias_",
        menuName = "BirriaTycoon/Decisiones Diarias",
        order = 0)]
    public class DecisionesDiarias : ScriptableObject
    {
        [Tooltip("Número de día en el juego (1, 2, 3...).")]
        public int diaNumero;

        [Tooltip("Lista de decisiones disponibles para este día.")]
        public List<DecisionFinancieraEntry> decisiones = new List<DecisionFinancieraEntry>();
    }
}
