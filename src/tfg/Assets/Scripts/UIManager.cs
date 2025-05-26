using Neuron;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Component that manage the UI.
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Text for predictions.
    /// </summary>
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

    /// <summary>
    /// Method that notifies the ServerManager of the change in the text for entering the IP. 
    /// </summary>
    /// <param name="ip">Entered IP.</param>
    public void SetIP(string ip)
    {
        ServerManager.Instance.SetIP(ip);
    }

    /// <summary>
    /// Set the text in the block that informs the predicted gesture.
    /// </summary>
    /// <param name="text">Predictions of the gestures.</param>
    public void SetText(string text)
    {
        _text.text = text;
    }

}
