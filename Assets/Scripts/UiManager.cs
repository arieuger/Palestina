using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private TMP_Text selectedElementText;
    
    private readonly Dictionary<string,string> _elementsDictionary = Dictionaries.ElementsDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void SetSelectedElementText(string text)
    {
        string key = _elementsDictionary.Keys.ToList().Find(text.Contains);
        if (key != null) selectedElementText.text = _elementsDictionary[key];
        else selectedElementText.text = "";
    }

}
