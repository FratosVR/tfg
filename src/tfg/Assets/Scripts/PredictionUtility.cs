using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class in order to manage the predictions returned by the server.
/// </summary>
[Serializable]
public class PredictionUtility
{
    private const int NUM_GETURES = 6;

    /// <summary>
    /// Struct containing the information returned by the model. These fields must be public for JSONUtility to populate them.
    /// </summary>
    [Serializable]
    public struct Prediction
    {
        /// <summary>
        /// Nombre de las clases que predice el modelo.
        /// Name of the classes that the model predicts.
        /// </summary>
        public string[] classes;
        /// <summary>
        /// Scores of the classes that the model predicts. The position in this array corresponds to the class in the same position as the "classes" array.
        /// </summary>
        public float[] scores;
    }

    /// <summary>
    /// Prediction returned by the model. It must be an array on size 1 because that is how the model returns the predictions. 
    /// </summary>
    public Prediction[] predictions;

    /// <summary>
    /// Constructor of the class. Initialize the "predictions" array at size 1 and the "classes" and "scores" array at the size of the number of gestures.
    /// </summary>
    public PredictionUtility()
    {
        predictions = new Prediction[1];
        predictions[0].classes = new string[NUM_GETURES];
        predictions[0].scores = new float[NUM_GETURES];
    }

    /// <summary>
    /// Bubble Short to order the predictions from highest to lowest depending on the score.
    /// </summary>
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

    /// <summary>
    /// Generates a text with the predictions.
    /// </summary>
    /// <returns>Text string with predictions in the format "ClassName : Score".</returns>
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
