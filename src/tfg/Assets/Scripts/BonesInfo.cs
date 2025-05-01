using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonesInfo
{
    private List<Tuple<string, float>> _bones;

    public BonesInfo()
    {
        _bones = new List<Tuple<string, float>>();
    }

    public void AddInfo(List<Transform> b)
    {
        int i = 0;
        foreach (Transform t in b) {
            _bones.Add(new Tuple<string, float>($"feature_{i}", t.position.x));
            i++;
            _bones.Add(new Tuple<string, float>($"feature_{i}", t.position.y));
            i++;
            _bones.Add(new Tuple<string, float>($"feature_{i}", t.position.z));
            i++;
            _bones.Add(new Tuple<string, float>($"feature_{i}", t.rotation.eulerAngles.x));
            i++;
            _bones.Add(new Tuple<string, float>($"feature_{i}", t.rotation.eulerAngles.y));
            i++;
            _bones.Add(new Tuple<string, float>($"feature_{i}", t.rotation.eulerAngles.z));
            i++;
        }
    }

    public string ToJSON()
    {
        string s = "";

        foreach (Tuple<string, float> t in _bones) {
            s += $"\"{t.Item1}\":{t.Item2},";
        }

        s = s.Remove(s.Length - 1);

        return s;
    }
}
