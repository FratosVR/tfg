using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Jobs;

/// <summary>
/// Helper class to facilitate the storage of the bones information in order to send it to the server.
/// </summary>
public class BonesInfo
{
    /// <summary>
    /// List of the pairs that make up the name of the bone component (must be "feature_x" to the server) with its value.
    /// </summary>
    private List<Tuple<string, float>> _bones;
    /// <summary>
    /// Native C# class for formatting numbers. Required to change the "," in floats to ".".
    /// </summary>
    private NumberFormatInfo nfi;

    /// <summary>
    /// Constructor for the class that initializes the NumberFormatInfo and the list of bones.
    /// </summary>
    public BonesInfo()
    {
        nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        _bones = new List<Tuple<string, float>>();
    }

    /// <summary>
    /// Adds the information from the list passed as a parameter to the class list.
    /// </summary>
    /// <param name="b">List of Transforms to storage.</param>
    public void AddInfo(List<Transform> b)
    {
        foreach (Transform t in b) {
            _bones.Add(new Tuple<string, float>($"feature_", t.position.x));
            _bones.Add(new Tuple<string, float>($"feature_", t.position.y));
            _bones.Add(new Tuple<string, float>($"feature_", t.position.z));
            _bones.Add(new Tuple<string, float>($"feature_", t.rotation.eulerAngles.x));
            _bones.Add(new Tuple<string, float>($"feature_", t.rotation.eulerAngles.y));
            _bones.Add(new Tuple<string, float>($"feature_", t.rotation.eulerAngles.z));
        }
    }

    /// <summary>
    /// Transforms the list of bones into a text string in JSON format.
    /// </summary>
    /// <param name="index">Index from where it should start.</param>
    /// <param name="lastIndex">Index where it ends as an output parameter. This parameter is required to ensure consistency with the number in the bone names sent to the server when called multiple times in a row.</param>
    /// <returns>Text string of bone information in JSON format.</returns>
    public string ToJSON(int index, out int lastIndex)
    {
        string s = "";
        int i = index;
        foreach (Tuple<string, float> t in _bones) {
            s += $"\"{t.Item1}{i}\":{t.Item2.ToString(nfi)},";
            i++;
        }
        lastIndex = i;
        s = s.Remove(s.Length - 1);

        return s;
    }
}
