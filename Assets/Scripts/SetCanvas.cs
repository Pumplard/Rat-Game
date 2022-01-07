using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCanvas : MonoBehaviour
{
    public CanvasScaler scaler;

    void Awake() {
        
        scaler.scaleFactor = 1;
    }
}
