using Neuron;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text _text;

    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
    }

    public void SetIP(string ip)
    {
        ServerManager.Instance.SetIP(ip);
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

}
