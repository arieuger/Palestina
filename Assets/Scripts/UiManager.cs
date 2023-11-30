using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private TMP_Text selectedElementText;
    [SerializeField] private Button cancelActionButton;
    [SerializeField] private List<Button> actionButtons; 
    
    private readonly Dictionary<string,string> _elementsDictionary = Dictionaries.ElementsDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        cancelActionButton.gameObject.SetActive(false);
    }

    public void SetSelectedElementText(string text)
    {
        string key = _elementsDictionary.Keys.ToList().Find(text.Contains);
        selectedElementText.text = key != null ? _elementsDictionary[key] : "";
    }

    public void ActivateActionButtons(bool active)
    {
        actionButtons.ForEach(b => b.gameObject.SetActive(active));
        Debug.Log(active);
        cancelActionButton.gameObject.SetActive(!active);
    }

}
