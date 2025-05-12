using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public void NewPrediction(string json)
    {
        PredictionUtility pred = new PredictionUtility();
        pred = JsonUtility.FromJson<PredictionUtility>(json);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
