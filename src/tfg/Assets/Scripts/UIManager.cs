using Neuron;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    NeuronSourceManager _sourceManager;
    [SerializeField]
    TMP_Text _text;

    public void SetIP(string ip)
    {
        _sourceManager.address = ip;
    }


}
