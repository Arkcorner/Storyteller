using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TextDisaperance : MonoBehaviour
{

    public TMP_Text text1; // Assign your UI Text component
    public TMP_Text text2; // Assign your UI Text component

    public float fadeDuration = 2f;

    void Start()
    {
        if (text1 == null)
            text1 = GetComponent<TMP_Text>();
        if (text2 == null)
            text2 = GetComponent<TMP_Text>();
    }

    public void StartFadeOut1()
    {
        StartCoroutine(FadeOutText(text1));
    }
    public void StartFadeIn1()
    {
        StartCoroutine(FadeInText(text1));
    }
    public void StartFadeOut2()
    {
        StartCoroutine(FadeOutText(text2));
    }
    public void StartFadeIn2()
    {
        StartCoroutine(FadeInText(text2));
    }

    private IEnumerator FadeOutText(TMP_Text text)
    {
        Color originalColor = text.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Ensure it's fully transparent
    }
    private IEnumerator FadeInText(TMP_Text text)
    {
        Color originalColor = text.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Ensure it's fully Visible
    }
}