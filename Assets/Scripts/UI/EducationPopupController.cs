using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BirriaTycoon.UI
{
    public class EducationPopupController : MonoBehaviour
    {
        [Header("Raíz del pop-up")]
        [SerializeField] private GameObject root;

        [Header("Texto")]
        [SerializeField] private TextMeshProUGUI tituloText;
        [SerializeField] private TextMeshProUGUI cuerpoText;

        [Header("Botones")]
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

        public void Show(string titulo, string cuerpo)
        {
            if (tituloText != null)
                tituloText.text = titulo;

            if (cuerpoText != null)
                cuerpoText.text = cuerpo;

            if (root != null)
                root.SetActive(true);
        }

        public void Hide()
        {
            if (root != null)
                root.SetActive(false);
        }
    }
}
