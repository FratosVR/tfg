using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Component that manage the predictions of the server.
/// </summary>
public class PredictionManager : MonoBehaviour
{
    private static PredictionManager _instance;
    public static PredictionManager Instance {  get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
    }

    /// <summary>
    /// Method that should be called when a new prediction arrives. It uses the text returned by the server to create the text indicating the prediction of the classes and informs the avatar of the prediccted class.
    /// </summary>
    /// <param name="json">Text returned by the server that is in JSON format.</param>
    public void NewPrediction(string json)
    {
        PredictionUtility pred = new PredictionUtility();
        pred = JsonUtility.FromJson<PredictionUtility>(json);
        pred.BubbleShort();

        Debug.Log(pred.PredsToText());
        UIManager.Instance.SetText(pred.PredsToText());
        AnimationManager.Instance.SetAnimationType(pred.predictions[0].classes[0]);
    }
}
