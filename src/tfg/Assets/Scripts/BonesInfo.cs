using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Jobs;

public class BonesInfo
{
    private List<Tuple<string, float>> _bones;
    private NumberFormatInfo nfi;

    public BonesInfo()
    {
        nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        _bones = new List<Tuple<string, float>>();
    }

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
