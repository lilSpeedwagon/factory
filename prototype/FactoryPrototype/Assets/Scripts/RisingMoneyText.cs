using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEngine;

public class RisingMoneyText : MonoBehaviour
{
    public double Lifetime = 2.0d;
    public double PositionStep = 0.0001d;
    public double AlphaStep = 0.0001d;
    
    private void FixedUpdate()
    {
        var position = transform.position;
        position.y += (float) PositionStep;
        transform.position = position;

        GetComponent<TextMeshProUGUI>().alpha -= (float)AlphaStep;
    }
    
}
