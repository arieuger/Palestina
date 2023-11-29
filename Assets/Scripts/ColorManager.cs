using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    
    public Color darkenedTilesColor; // 9DB1DA
    public Color dangeredTilesColor; //
    public Color darkDangeredTilesColor;
    
    public static ColorManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public Color GetDarkenedColor(Color originalColor)
    {
        if (originalColor.Equals(Color.white)) return darkenedTilesColor;
        if (originalColor.Equals(dangeredTilesColor)) return darkDangeredTilesColor;

        return Color.white;
    }

}
