using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// TACOIN TYCOON — UI Builder automático
/// 
/// INSTRUCCIONES:
/// 1. Adjunta este script a tu Canvas
/// 2. Dale Play — construye toda la UI automáticamente
/// 3. Detén Play — la UI queda guardada en la escena
/// 4. Quita este script del Canvas cuando termines
/// 
/// FUENTES REQUERIDAS (importar antes de correr):
/// - Bangers-Regular.ttf   → Assets/Fonts/
/// - Outfit-Bold.ttf       → Assets/Fonts/
/// Descárgalas gratis en fonts.google.com
/// Si no las tienes, el script usa la fuente default de TMP.
/// </summary>
[ExecuteInEditMode]
public class UIBuilder : MonoBehaviour
{
    [Header("Fuentes (asignar en Inspector)")]
    public TMP_FontAsset fontBangers;   // Bangers-Regular SDF
    public TMP_FontAsset fontOutfit;    // Outfit-Bold SDF

    [Header("Ejecutar")]
    public bool buildUI = false;        // Activa desde Inspector para construir

    // ── Colores ──
    static readonly Color CAFE     = Hex("#2C1A0E");
    static readonly Color CAFE2    = Hex("#1a0e08");
    static readonly Color ROJO     = Hex("#C41E3A");
    static readonly Color VERDE    = Hex("#1B5E38");
    static readonly Color DORADO   = Hex("#F0A500");
    static readonly Color CREMA    = Hex("#FDF6EC");
    static readonly Color GRIS     = Hex("#F0EBE3");
    static readonly Color BLANCO   = Color.white;
    static readonly Color SUB      = Hex("#8a7060");
    static readonly Color TEXTO    = Hex("#3D2314");

    void Update()
    {
        if (buildUI)
        {
            buildUI = false;
            Build();
        }
    }

    void Build()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas == null) { Debug.LogError("Adjunta este script al Canvas"); return; }

        // Limpiar hijos existentes
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        // Fondo azulejo
        var bg = MakeImage("Background", transform, CREMA);
        Stretch(bg.rectTransform);

        // ── HEADER ──
        var header = MakePanel("Header", transform, CAFE, new Vector2(0,1), new Vector2(1,1),
            new Vector2(0,-88), new Vector2(0,0));
        MakeText("LogoTitle",  header.transform, "TACOIN",
            fontBangers, 36, DORADO, TextAlignmentOptions.Left,
            new Vector2(18,8), new Vector2(240,44));
        MakeText("LogoSub", header.transform, "Birria · Blockchain · Tycoon",
            fontOutfit,  10, new Color(1,1,1,.4f), TextAlignmentOptions.Left,
            new Vector2(18,-46), new Vector2(260,22));
        var coin = MakePanel("CoinBtn", header.transform, DORADO,
            new Vector2(1,0.5f), new Vector2(1,0.5f),
            new Vector2(-18,0), new Vector2(52,52));
        coin.GetComponent<RectTransform>().pivot = new Vector2(1,.5f);
        MakeText("CoinEmoji", coin.transform, "🌮", fontOutfit, 26, BLANCO,
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(52,52));
        coin.GetComponent<Image>().sprite = null; // circulo via code
        // Animación de la moneda
        coin.gameObject.AddComponent<CoinFloatAnim>();

        // ── TICKER ──
        var ticker = MakePanel("TickerBar", transform, VERDE,
            new Vector2(0,1), new Vector2(1,1),
            new Vector2(0,-132), new Vector2(0,-88));
        MakeText("TickerLabel", ticker.transform, "BirriaCoin",
            fontOutfit, 10, new Color(1,1,1,.6f), TextAlignmentOptions.Left,
            new Vector2(18, 14), new Vector2(140,16));
        var precioTxt = MakeText("PrecioVal", ticker.transform, "$10.00",
            fontOutfit, 20, BLANCO, TextAlignmentOptions.Left,
            new Vector2(18, -8), new Vector2(140, 28));
        precioTxt.fontStyle = FontStyles.Bold;
        var precioBadge = MakePanel("PrecioBadge", ticker.transform, new Color(1,1,1,.15f),
            new Vector2(0,0.5f), new Vector2(0,0.5f),
            new Vector2(170, 0), new Vector2(80,26));
        MakeText("BadgeTxt", precioBadge.transform, "▲ +0.0%",
            fontOutfit, 11, Hex("#7FFFA0"), TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(80,26));
        var diaBadge = MakePanel("DiaBadge", ticker.transform, new Color(0,0,0,.2f),
            new Vector2(1,.5f), new Vector2(1,.5f),
            new Vector2(-18,0), new Vector2(120,28));
        diaBadge.GetComponent<RectTransform>().pivot = new Vector2(1,.5f);
        MakeText("DiaTxt", diaBadge.transform, "DÍA 1 · SEM 1",
            fontOutfit, 11, new Color(1,1,1,.8f), TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(120,28));

        // ── SALDO CARDS ──
        float cardY = -132f;
        var saldoStrip = MakeEmpty("SaldoStrip", transform);
        var sr = saldoStrip.GetComponent<RectTransform>();
        sr.anchorMin = new Vector2(0,1); sr.anchorMax = new Vector2(1,1);
        sr.offsetMin = new Vector2(14,-196); sr.offsetMax = new Vector2(-14,-132);
        var hGroup = saldoStrip.AddComponent<HorizontalLayoutGroup>();
        hGroup.spacing = 8; hGroup.childForceExpandWidth = true;
        hGroup.childControlHeight = true; hGroup.padding = new RectOffset(0,0,8,8);

        string[] saldoLabels = {"💵 Efectivo","🏦 Ahorro","🪙 BirriaCoin"};
        string[] saldoIds    = {"EfectivoVal","AhorroVal","CryptoVal"};
        string[] saldoVals   = {"$500","$200","0 BC"};
        Color[]  saldoColors = {ROJO, VERDE, DORADO};
        for (int i=0;i<3;i++)
        {
            var card = MakeImage($"SaldoCard{i}", saldoStrip.transform, BLANCO);
            card.GetComponent<Image>().sprite = null;
            AddShadow(card.gameObject);
            var vg = card.gameObject.AddComponent<VerticalLayoutGroup>();
            vg.padding = new RectOffset(12,12,10,10);
            vg.childForceExpandWidth = true;
            // Accent top bar
            var accent = MakeImage("Accent", card.transform, saldoColors[i]);
            accent.GetComponent<RectTransform>().sizeDelta = new Vector2(0,3);
            MakeText(saldoLabels[i]+"Lbl", card.transform, saldoLabels[i],
                fontOutfit, 9, SUB, TextAlignmentOptions.Left, Vector2.zero, new Vector2(0,16));
            MakeText(saldoIds[i], card.transform, saldoVals[i],
                fontOutfit, 16, TEXTO, TextAlignmentOptions.Left, Vector2.zero, new Vector2(0,24));
        }

        // ── ACCIONES DOTS ──
        var accionesRow = MakeEmpty("AccionesRow", transform);
        var ar = accionesRow.GetComponent<RectTransform>();
        ar.anchorMin = new Vector2(0,1); ar.anchorMax = new Vector2(1,1);
        ar.offsetMin = new Vector2(14,-226); ar.offsetMax = new Vector2(-14,-196);
        MakeText("AccionesLabel", accionesRow.transform, "ACCIONES RESTANTES",
            fontOutfit, 10, SUB, TextAlignmentOptions.Left,
            new Vector2(0,0), new Vector2(200,30));
        var dotsContainer = MakeEmpty("DotsContainer", accionesRow.transform);
        var dc = dotsContainer.GetComponent<RectTransform>();
        dc.anchorMin = new Vector2(1,.5f); dc.anchorMax = new Vector2(1,.5f);
        dc.pivot = new Vector2(1,.5f);
        dc.anchoredPosition = new Vector2(0,0);
        dc.sizeDelta = new Vector2(110,20);
        var hg2 = dotsContainer.AddComponent<HorizontalLayoutGroup>();
        hg2.spacing = 5; hg2.childForceExpandWidth = false;
        for (int i=0;i<3;i++)
        {
            var dot = MakeImage($"Dot{i}", dotsContainer.transform, ROJO);
            dot.rectTransform.sizeDelta = new Vector2(28,8);
            var le = dot.gameObject.AddComponent<LayoutElement>();
            le.minWidth=28; le.minHeight=8; le.preferredWidth=28;
        }

        // ── SCROLL CONTENT (decisiones) ──
        var scrollObj = MakeEmpty("ScrollView", transform);
        var scrollRT = scrollObj.GetComponent<RectTransform>();
        scrollRT.anchorMin = new Vector2(0,0); scrollRT.anchorMax = new Vector2(1,1);
        scrollRT.offsetMin = new Vector2(0,72); scrollRT.offsetMax = new Vector2(0,-226);
        var scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;

        var viewport = MakeImage("Viewport", scrollObj.transform, new Color(0,0,0,0));
        Stretch(viewport.rectTransform);
        viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
        scrollRect.viewport = viewport.rectTransform;

        var content = MakeEmpty("Content", viewport.transform);
        var contentRT = content.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0,1); contentRT.anchorMax = new Vector2(1,1);
        contentRT.pivot     = new Vector2(.5f,1);
        contentRT.offsetMin = new Vector2(14,0); contentRT.offsetMax = new Vector2(-14,0);
        contentRT.sizeDelta = new Vector2(-28,0);
        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing=10; vlg.childForceExpandWidth=true;
        vlg.childControlHeight=false;
        vlg.padding = new RectOffset(0,0,10,10);
        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.content = contentRT;

        // Section title
        var secTitle = MakeEmpty("SectionTitle", content.transform);
        var secRT = secTitle.GetComponent<RectTransform>();
        secRT.sizeDelta = new Vector2(0,32);
        var secLE = secTitle.AddComponent<LayoutElement>();
        secLE.minHeight=32; secLE.preferredHeight=32;
        MakeText("SecH2", secTitle.transform, "¿QUÉ HACES HOY?",
            fontBangers, 22, TEXTO, TextAlignmentOptions.Left,
            new Vector2(0,0), new Vector2(220,32));
        var pillBg = MakeImage("Pill", secTitle.transform, ROJO);
        pillBg.rectTransform.anchorMin = new Vector2(0,.5f);
        pillBg.rectTransform.anchorMax = new Vector2(0,.5f);
        pillBg.rectTransform.pivot     = new Vector2(0,.5f);
        pillBg.rectTransform.anchoredPosition = new Vector2(225,0);
        pillBg.rectTransform.sizeDelta = new Vector2(55,18);
        MakeText("PillTxt", pillBg.transform, "Elige 1",
            fontOutfit, 9, BLANCO, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(55,18));

        // 3 option cards
        string[] opTitulos = {"Reinvertir en birria","Guardar en el banco","Comprar BirriaCoin"};
        string[] opDescs   = {
            "Más ingredientes = más tacos mañana.",
            "Seguro pero el dinero no trabaja por ti.",
            "Puede subir mucho… o bajar. ¿Te arriesgas?"
        };
        string[] opEmojis = {"🥩","🏦","🪙"};
        Color[]  opColors = {ROJO, VERDE, DORADO};

        for (int i=0;i<3;i++)
        {
            var card = MakeImage($"OpCard{i}", content.transform, BLANCO);
            AddShadow(card.gameObject);
            card.rectTransform.sizeDelta = new Vector2(0,80);
            var le = card.gameObject.AddComponent<LayoutElement>();
            le.minHeight=80; le.preferredHeight=80;
            var btn = card.gameObject.AddComponent<Button>();
            btn.targetGraphic = card;
            var cb = btn.colors;
            cb.highlightedColor = new Color(.98f,.96f,.96f,1f);
            cb.pressedColor     = new Color(.95f,.93f,.93f,1f);
            btn.colors = cb;

            // Left accent stripe
            var stripe = MakeImage("Stripe", card.transform, opColors[i]);
            stripe.rectTransform.anchorMin = new Vector2(0,0);
            stripe.rectTransform.anchorMax = new Vector2(0,1);
            stripe.rectTransform.offsetMin = new Vector2(0,0);
            stripe.rectTransform.offsetMax = new Vector2(5,0);

            // Emoji box
            var emojiBox = MakeImage("EmojiBox", card.transform, GRIS);
            emojiBox.rectTransform.anchorMin = new Vector2(0,.5f);
            emojiBox.rectTransform.anchorMax = new Vector2(0,.5f);
            emojiBox.rectTransform.pivot     = new Vector2(0,.5f);
            emojiBox.rectTransform.anchoredPosition = new Vector2(16,0);
            emojiBox.rectTransform.sizeDelta = new Vector2(48,48);
            MakeText("Emoji", emojiBox.transform, opEmojis[i],
                fontOutfit, 24, BLANCO, TextAlignmentOptions.Center,
                Vector2.zero, new Vector2(48,48));

            // Title + desc
            MakeText($"OpTitulo{i}", card.transform, opTitulos[i],
                fontOutfit, 14, TEXTO, TextAlignmentOptions.Left,
                new Vector2(76, 14), new Vector2(260, 22));
            var tituloTxt = card.transform.Find($"OpTitulo{i}").GetComponent<TMP_Text>();
            tituloTxt.fontStyle = FontStyles.Bold;

            MakeText($"OpDesc{i}", card.transform, opDescs[i],
                fontOutfit, 11, SUB, TextAlignmentOptions.Left,
                new Vector2(76, -12), new Vector2(255, 34));

            // Arrow
            MakeText("Arrow", card.transform, "›",
                fontOutfit, 24, new Color(.8f,.8f,.8f,1), TextAlignmentOptions.Right,
                new Vector2(-16,0), new Vector2(30,40));
            card.transform.Find("Arrow").GetComponent<RectTransform>().anchorMin = new Vector2(1,.5f);
            card.transform.Find("Arrow").GetComponent<RectTransform>().anchorMax = new Vector2(1,.5f);
            card.transform.Find("Arrow").GetComponent<RectTransform>().pivot     = new Vector2(1,.5f);
        }

        // Fin día button
        var finBtn = MakeImage("FinDiaBtn", content.transform, CAFE);
        AddShadow(finBtn.gameObject);
        finBtn.rectTransform.sizeDelta = new Vector2(0,56);
        var finLE = finBtn.gameObject.AddComponent<LayoutElement>();
        finLE.minHeight=56; finLE.preferredHeight=56;
        var finButton = finBtn.gameObject.AddComponent<Button>();
        finButton.targetGraphic = finBtn;
        MakeText("FinTxt", finBtn.transform, "🌙  CERRAR EL PUESTO",
            fontBangers, 20, DORADO, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(0,56));
        Stretch(finBtn.transform.Find("FinTxt").GetComponent<RectTransform>());

        // ── BOTTOM NAV ──
        var nav = MakePanel("BottomNav", transform, BLANCO,
            new Vector2(0,0), new Vector2(1,0),
            new Vector2(0,0), new Vector2(0,72));
        // top border
        var border = MakeImage("Border", nav.transform, new Color(.17f,.1f,.055f,.08f));
        border.rectTransform.anchorMin = new Vector2(0,1);
        border.rectTransform.anchorMax = new Vector2(1,1);
        border.rectTransform.offsetMin = new Vector2(0,-1);
        border.rectTransform.offsetMax = new Vector2(0,0);
        string[] navLabels = {"Puesto","Mercado","Historial","Config"};
        string[] navEmojis = {"🌮","📈","📋","⚙️"};
        var navHG = nav.gameObject.AddComponent<HorizontalLayoutGroup>();
        navHG.childForceExpandWidth = true;
        navHG.childControlHeight    = true;
        navHG.padding = new RectOffset(0,0,6,8);
        for (int i=0;i<4;i++)
        {
            var nb = MakeEmpty($"NavBtn{i}", nav.transform);
            var nbVG = nb.AddComponent<VerticalLayoutGroup>();
            nbVG.childAlignment = TextAnchor.MiddleCenter;
            nbVG.childForceExpandWidth = true;
            nbVG.spacing = 2;
            var nBtn = nb.AddComponent<Button>();
            nBtn.targetGraphic = nb.AddComponent<Image>();
            nBtn.targetGraphic.color = new Color(0,0,0,0);
            MakeText($"NavIco{i}", nb.transform, navEmojis[i],
                fontOutfit, 22, i==0?ROJO:new Color(.73f,.73f,.73f,1),
                TextAlignmentOptions.Center, Vector2.zero, new Vector2(0,28));
            MakeText($"NavLbl{i}", nb.transform, navLabels[i],
                fontOutfit, 9, i==0?ROJO:new Color(.73f,.73f,.73f,1),
                TextAlignmentOptions.Center, Vector2.zero, new Vector2(0,14));
        }

        // ── EDUCATION POPUP ──
        var eduOverlay = MakeImage("EducationPopup", transform, new Color(.17f,.1f,.055f,.6f));
        Stretch(eduOverlay.rectTransform);
        eduOverlay.gameObject.SetActive(false);
        var popupPanel = MakeImage("PopupPanel", eduOverlay.transform, BLANCO);
        popupPanel.rectTransform.anchorMin = new Vector2(0,0);
        popupPanel.rectTransform.anchorMax = new Vector2(1,0);
        popupPanel.rectTransform.pivot     = new Vector2(.5f,0);
        popupPanel.rectTransform.offsetMin = new Vector2(0,0);
        popupPanel.rectTransform.offsetMax = new Vector2(0,0);
        popupPanel.rectTransform.sizeDelta = new Vector2(0,340);
        var pvg = popupPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        pvg.padding = new RectOffset(22,22,20,28);
        pvg.spacing=10; pvg.childForceExpandWidth=true;
        var pcsf = popupPanel.gameObject.AddComponent<ContentSizeFitter>();
        pcsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        MakeText("PopupHandle", popupPanel.transform, "—",
            fontOutfit, 16, new Color(.88f,.88f,.88f,1), TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(0,20));
        MakeText("PopupEmoji", popupPanel.transform, "💡",
            fontOutfit, 48, BLANCO, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(0,60));
        var ptitle = MakeText("PopupTitle", popupPanel.transform, "CONSEJO",
            fontBangers, 26, TEXTO, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(0,36));
        var pbody = MakeText("PopupBody", popupPanel.transform, "...",
            fontOutfit, 14, SUB, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(0,80));
        pbody.enableWordWrapping=true;
        var popupOkImg = MakeImage("PopupOkBtn", popupPanel.transform, ROJO);
        popupOkImg.rectTransform.sizeDelta = new Vector2(0,52);
        var okLE = popupOkImg.gameObject.AddComponent<LayoutElement>();
        okLE.minHeight=52; okLE.preferredHeight=52;
        var okBtn = popupOkImg.gameObject.AddComponent<Button>();
        okBtn.targetGraphic = popupOkImg;
        MakeText("OkTxt", popupOkImg.transform, "✅  ENTENDIDO",
            fontBangers, 18, BLANCO, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(0,52));
        Stretch(popupOkImg.transform.Find("OkTxt").GetComponent<RectTransform>());
        // Adjunta EducationPopupController manualmente desde Inspector

        // ── WEEKLY REPORT ──
        var repOverlay = MakeImage("WeeklyReportPanel", transform, new Color(.17f,.1f,.055f,.7f));
        Stretch(repOverlay.rectTransform);
        repOverlay.gameObject.SetActive(false);
        var repCard = MakeImage("ReportCard", repOverlay.transform, BLANCO);
        repCard.rectTransform.anchorMin = new Vector2(.05f,.1f);
        repCard.rectTransform.anchorMax = new Vector2(.95f,.9f);
        repCard.rectTransform.offsetMin = Vector2.zero;
        repCard.rectTransform.offsetMax = Vector2.zero;
        var rvg = repCard.gameObject.AddComponent<VerticalLayoutGroup>();
        rvg.padding = new RectOffset(20,20,24,24);
        rvg.spacing=10; rvg.childForceExpandWidth=true;
        MakeText("RTitulo", repCard.transform, "REPORTE SEMANAL",
            fontBangers, 28, TEXTO, TextAlignmentOptions.Center, Vector2.zero, new Vector2(0,40));
        MakeText("RSubtitle", repCard.transform, "Semana 1 completada",
            fontOutfit, 12, SUB, TextAlignmentOptions.Center, Vector2.zero, new Vector2(0,20));
        // stats grid
        var statsGrid = MakeEmpty("StatsGrid", repCard.transform);
        var sgLE = statsGrid.AddComponent<LayoutElement>();
        sgLE.minHeight=90; sgLE.preferredHeight=90;
        var sgg = statsGrid.AddComponent<GridLayoutGroup>();
        sgg.cellSize    = new Vector2(130,40);
        sgg.spacing     = new Vector2(8,8);
        sgg.constraint  = GridLayoutGroup.Constraint.FixedColumnCount;
        sgg.constraintCount = 2;
        string[] statLbls = {"Decisiones","Efectivo","✅ Aciertos","❌ Errores"};
        string[] statIds  = {"RTotal","REfectivo","RAciertos","RErrores"};
        for (int i=0;i<4;i++)
        {
            var sc = MakeImage($"Stat{i}", statsGrid.transform, GRIS);
            MakeText(statIds[i], sc.transform, "0",
                fontOutfit, 22, TEXTO, TextAlignmentOptions.Left,
                new Vector2(14,2), new Vector2(110,28));
            MakeText(statLbls[i]+"L", sc.transform, statLbls[i],
                fontOutfit, 9, SUB, TextAlignmentOptions.Left,
                new Vector2(14,-18), new Vector2(110,18));
        }
        // barras
        CreateBarra("BarraAciertos", repCard.transform, VERDE, "Decisiones acertadas", "PctA");
        CreateBarra("BarraErrores",  repCard.transform, ROJO,  "Decisiones riesgosas", "PctE");
        var repBtnImg = MakeImage("ReportCerrarBtn", repCard.transform, CAFE);
        repBtnImg.rectTransform.sizeDelta = new Vector2(0,52);
        var rbLE = repBtnImg.gameObject.AddComponent<LayoutElement>();
        rbLE.minHeight=52; rbLE.preferredHeight=52;
        var rbBtn = repBtnImg.gameObject.AddComponent<Button>();
        rbBtn.targetGraphic = repBtnImg;
        MakeText("RBtnTxt", repBtnImg.transform, "🌮  SEGUIR COCINANDO",
            fontBangers, 18, DORADO, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(0,52));
        Stretch(repBtnImg.transform.Find("RBtnTxt").GetComponent<RectTransform>());
        // Adjunta WeeklyReportUIController manualmente desde Inspector

        Debug.Log("✅ Tacoin UI construida exitosamente. ¡Recuerda quitar UIBuilder del Canvas!");
    }

    // ═══════════════════════ HELPERS ═══════════════════════
    Image MakePanel(string name, Transform parent, Color color,
        Vector2 ancMin, Vector2 ancMax, Vector2 offMin, Vector2 offMax)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        var rt  = img.rectTransform;
        rt.anchorMin = ancMin; rt.anchorMax = ancMax;
        rt.offsetMin = offMin; rt.offsetMax = offMax;
        return img;
    }

    Image MakeImage(string name, Transform parent, Color color)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        return img;
    }

    TMP_Text MakeText(string name, Transform parent, string text,
        TMP_FontAsset font, float size, Color color,
        TextAlignmentOptions align, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = size;
        tmp.color     = color;
        tmp.alignment = align;
        tmp.enableWordWrapping = false;
        if (font != null) tmp.font = font;
        var rt = tmp.rectTransform;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = sizeDelta;
        return tmp;
    }

    GameObject MakeEmpty(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    void AddShadow(GameObject go)
    {
        var shadow = go.AddComponent<Shadow>();
        shadow.effectColor    = new Color(0,0,0,.08f);
        shadow.effectDistance = new Vector2(0,-3);
    }

    void CreateBarra(string name, Transform parent, Color fillColor, string label, string pctId)
    {
        var wrap = MakeEmpty(name+"Wrap", parent);
        var wLE  = wrap.AddComponent<LayoutElement>();
        wLE.minHeight=38; wLE.preferredHeight=38;
        var wvg  = wrap.AddComponent<VerticalLayoutGroup>();
        wvg.spacing=4; wvg.childForceExpandWidth=true;
        // info row
        var info = MakeEmpty(name+"Info", wrap.transform);
        info.AddComponent<LayoutElement>().minHeight=16;
        var ihg  = info.AddComponent<HorizontalLayoutGroup>();
        MakeText(name+"Lbl", info.transform, label,
            fontOutfit, 11, SUB, TextAlignmentOptions.Left, Vector2.zero, new Vector2(0,16));
        MakeText(pctId, info.transform, "0%",
            fontOutfit, 11, fillColor, TextAlignmentOptions.Right, Vector2.zero, new Vector2(50,16));
        // bar
        var bg   = MakeImage(name+"Bg", wrap.transform, Hex("#EDE8E0"));
        bg.rectTransform.sizeDelta = new Vector2(0,10);
        bg.gameObject.AddComponent<LayoutElement>().minHeight=10;
        var fill = MakeImage(name+"Fill", bg.transform, fillColor);
        fill.rectTransform.anchorMin = new Vector2(0,.5f);
        fill.rectTransform.anchorMax = new Vector2(0,.5f);
        fill.rectTransform.pivot     = new Vector2(0,.5f);
        fill.rectTransform.sizeDelta = new Vector2(0,10);
    }

    static Color Hex(string h)
    {
        ColorUtility.TryParseHtmlString(h, out Color c);
        return c;
    }
}

/// <summary>Animación flotante para la moneda del header</summary>
public class CoinFloatAnim : MonoBehaviour
{
    float _t;
    Vector2 _orig;
    RectTransform _rt;

    void Start()
    {
        _rt   = GetComponent<RectTransform>();
        _orig = _rt.anchoredPosition;
    }

    void Update()
    {
        _t += Time.deltaTime;
        float y = Mathf.Sin(_t * 2.4f) * 4f;
        float r = Mathf.Sin(_t * 2.4f) * 5f;
        _rt.anchoredPosition = _orig + new Vector2(0, y);
        _rt.localRotation    = Quaternion.Euler(0, 0, r);
    }
}
