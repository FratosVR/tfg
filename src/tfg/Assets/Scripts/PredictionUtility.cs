using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PredictionUtility
{
    private const int NUM_GETURES = 6;
    [Serializable]
    public struct Prediction
    {
        public string[] classes;
        public float[] scores;
    }

    public Prediction prediction;

    public PredictionUtility()
    {
        prediction = new Prediction();
        prediction.classes = new string[NUM_GETURES];
        prediction.scores = new float[NUM_GETURES];
    }
}
