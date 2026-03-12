using UnityEngine;

namespace BirriaTycoon.Data
{
    /// <summary>
    /// Evento de "Noticias del Barrio" que afecta el precio de BirriaCoin.
    /// Ejemplo: +20% si un local acepta la moneda.
    /// </summary>
    [CreateAssetMenu(
        fileName = "BarrioNewsEvent_",
        menuName = "BirriaTycoon/Noticia del Barrio",
        order = 1)]
    public class BarrioNewsEvent : ScriptableObject
    {
        [Header("Contenido")]
        public string id;
        public string titulo;
        [TextArea] public string descripcion;

        [Header("Impacto en precio")]
        [Tooltip("Cambio porcentual en el precio. Ej: 0.2 = +20%, -0.15 = -15%.")]
        public float cambioPorcentualPrecio;

        [Header("UI opcional")]
        public Sprite icono;
    }
}

