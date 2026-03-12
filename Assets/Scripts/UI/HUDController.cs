using UnityEngine;
using TMPro;
using BirriaTycoon.Core;

/// <summary>
/// HUDController — muestra efectivo, ahorro, BirriaCoin y precio en tiempo real.
///
/// SETUP:
/// 1. Adjunta este script al objeto "HUD" dentro del Canvas
/// 2. Asigna los 4 TextMeshProUGUI en el Inspector
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("Labels de economía")]
    public TextMeshProUGUI efectivoText;
    public TextMeshProUGUI ahorroText;
    public TextMeshProUGUI cryptoText;
    public TextMeshProUGUI precioText;

    [Header("Label de día (opcional)")]
    public TextMeshProUGUI diaText;

    private EconomyManager  _eco;
    private CryptoSimSystem _crypto;
    private GameLoopManager _loop;

    void Start()
    {
        _eco    = EconomyManager.Instance;
        _crypto = FindAnyObjectByType<CryptoSimSystem>();
        _loop   = GameLoopManager.Instance;

        if (_eco != null)
            _eco.OnValoresActualizados += RefrescarHUD;

        if (_crypto != null)
            _crypto.OnPrecioActualizado += OnPrecioActualizado;

        if (_loop != null)
            _loop.OnNuevoDiaComenzado += RefrescarDia;

        RefrescarHUD();
        RefrescarDia();
    }

    void OnDestroy()
    {
        if (_eco    != null) _eco.OnValoresActualizados      -= RefrescarHUD;
        if (_crypto != null) _crypto.OnPrecioActualizado     -= OnPrecioActualizado;
        if (_loop   != null) _loop.OnNuevoDiaComenzado       -= RefrescarDia;
    }

    void OnPrecioActualizado(float nuevoPrecio, BirriaTycoon.Data.BarrioNewsEvent noticia)
    {
        RefrescarHUD();
    }

    void RefrescarHUD()
    {
        if (_eco == null) return;

        if (efectivoText != null)
            efectivoText.text = $"💵 ${_eco.EfectivoNegocio:0.00}";

        if (ahorroText != null)
            ahorroText.text = $"🏦 ${_eco.AhorroPersonal:0.00}";

        if (cryptoText != null)
            cryptoText.text = $"🪙 {_eco.CarteraCrypto:0.000} BC";

        if (precioText != null && _crypto != null)
            precioText.text = $"1 BC = ${_crypto.PrecioActual:0.00}";
    }

    void RefrescarDia()
    {
        if (diaText != null && _loop != null)
            diaText.text = $"Día {_loop.DiaGlobalActual}  |  Semana {_loop.SemanaGlobalActual}";
    }
}
