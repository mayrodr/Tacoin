using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BirriaTycoon.Data;

namespace BirriaTycoon.UI
{
    public class DecisionOptionButton : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI tituloText;
        [SerializeField] private TextMeshProUGUI descripcionText;
        [SerializeField] private Button boton;

        private DecisionFinancieraEntry _entry;
        private Action<DecisionFinancieraEntry> _onClick;

        private void Awake()
        {
            if (boton != null)
            {
                boton.onClick.AddListener(HandleClick);
            }
        }

        public void Configurar(DecisionFinancieraEntry entry, Action<DecisionFinancieraEntry> onClick)
        {
            _entry = entry;
            _onClick = onClick;

            if (tituloText != null)
            {
                tituloText.text = entry.titulo;
            }

            if (descripcionText != null)
            {
                descripcionText.text = entry.descripcion;
            }
        }

        private void HandleClick()
        {
            _onClick?.Invoke(_entry);
        }
    }
}
