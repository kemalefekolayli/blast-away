using UnityEngine;
using TMPro;
using System.Collections;

public class ComboPopup : MonoBehaviour
{
    public TextMeshProUGUI comboText;
    public float displayDuration = 1.5f;
    public float fadeSpeed = 2f;
    public float floatSpeed = 50f; // Yukarı kayma hızı (piksel/sn)

    private Coroutine activeRoutine;
    private Vector2 startPos;

    void OnEnable()
    {
        GameEvents.OnComboUpdate += ShowCombo;
    }

    void OnDisable()
    {
        GameEvents.OnComboUpdate -= ShowCombo;
    }

    void Start()
    {
        if (comboText != null)
        {
            startPos = comboText.rectTransform.anchoredPosition;
            comboText.gameObject.SetActive(false);
        }
    }

    void ShowCombo(int multiplier)
    {
        if (comboText == null) return;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(AnimateCombo(multiplier));
    }

    IEnumerator AnimateCombo(int multiplier)
    {
        comboText.text = "x" + multiplier;
        comboText.rectTransform.anchoredPosition = startPos;
        comboText.gameObject.SetActive(true);

        // Boyutu combo'ya göre büyüt
        float scale = 1f + (multiplier - 2) * 0.15f;
        comboText.rectTransform.localScale = Vector3.one * scale;

        Color color = comboText.color;
        color.a = 1f;
        comboText.color = color;

        float elapsed = 0f;

        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;

            // Yukarı kay
            Vector2 pos = comboText.rectTransform.anchoredPosition;
            pos.y += floatSpeed * Time.deltaTime;
            comboText.rectTransform.anchoredPosition = pos;

            // Son yarıda fade out
            if (elapsed > displayDuration * 0.5f)
            {
                float fadeProgress = (elapsed - displayDuration * 0.5f) / (displayDuration * 0.5f);
                color.a = 1f - fadeProgress;
                comboText.color = color;
            }

            yield return null;
        }

        comboText.gameObject.SetActive(false);
        comboText.rectTransform.anchoredPosition = startPos;
        activeRoutine = null;
    }
}
