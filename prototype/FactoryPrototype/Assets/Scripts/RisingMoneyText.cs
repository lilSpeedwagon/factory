using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class RisingMoneyText : MonoBehaviour
{
    public double Lifetime = 2.0d;
    public double PositionStep = 0.05d;
    public double AlphaStep = 0.02d;

    public static void CreateRisingMoneyAnimation(Vector2 position, int value)
    {
        var moneyAnimation = new GameObject("MoneyAnimation");
        var transform = moneyAnimation.AddComponent<RectTransform>();
        transform.SetParent(CanvasTransform);
        transform.position = position;
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        moneyAnimation.gameObject.AddComponent<RisingMoneyText>();

        var textComponent = moneyAnimation.gameObject.AddComponent<TextMeshProUGUI>();
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.text = (value >= 0 ? "+" : "") + value + '$';
        textComponent.fontSize = 12;
        textComponent.enableWordWrapping = false;

        float lifetime = (float)moneyAnimation.GetComponent<RisingMoneyText>().Lifetime;
        Destroy(moneyAnimation, lifetime); // postponed destruction
    }

    private static RectTransform CanvasTransform
    {
        get
        {
            if (g_canvasTransform == null)
            {
                g_canvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
            }
            return g_canvasTransform;
        }
    }

    private void FixedUpdate()
    {
        var position = transform.position;
        position.y += (float) PositionStep;
        transform.position = position;

        GetComponent<TextMeshProUGUI>().alpha -= (float)AlphaStep;
    }

    private static RectTransform g_canvasTransform;
}
