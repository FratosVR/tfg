using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PredictionUtility
{
    private const int NUM_GETURES = 6;

    //Deben ser campos públicos para que JSONUtility lo rellene
    [Serializable]
    public struct Prediction
    {
        public string[] classes;
        public float[] scores;
    }

    public Prediction[] predictions;

    public PredictionUtility()
    {
        predictions = new Prediction[1];
        predictions[0].classes = new string[NUM_GETURES];
        predictions[0].scores = new float[NUM_GETURES];
    }

    public void BubbleShort()
    {
        float temporaryScores;
        string temporaryClasses;

        for (int j = 0; j <= NUM_GETURES - 2; j++)
        {
            for (int i = 0; i <= NUM_GETURES - 2; i++)
            {
                if (predictions[0].scores[i] < predictions[0].scores[i + 1])
                {
                    temporaryScores = predictions[0].scores[i + 1];
                    temporaryClasses = predictions[0].classes[i + 1];


                    predictions[0].scores[i + 1] = predictions[0].scores[i];
                    predictions[0].classes[i + 1] = predictions[0].classes[i];

                    predictions[0].scores[i] = temporaryScores;
                    predictions[0].classes[i] = temporaryClasses;
                }
            }
        }
    }

    public string PredsToText()
    {
        string s = "";
        for(int i = 0; i < NUM_GETURES; i++)
        {
            s += $"{predictions[0].classes[i]} : {predictions[0].scores[i]}\n";
        }
        return s;
    }
}
