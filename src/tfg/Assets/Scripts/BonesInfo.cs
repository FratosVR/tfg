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
            _bones.Add(new Tuple<string, float>($"feature_{i}" + "_posx", t.position.x));
            _bones.Add(new Tuple<string, float>($"feature_{i}" + "_posy", t.position.y));
            _bones.Add(new Tuple<string, float>($"feature_{i}" + "_posz", t.position.z));
            _bones.Add(new Tuple<string, float>($"feature_{i}" + "_rotx", t.rotation.eulerAngles.x));
            _bones.Add(new Tuple<string, float>($"feature_{i}" + "_roty", t.rotation.eulerAngles.y));
            _bones.Add(new Tuple<string, float>($"feature_{i}" + "_rotz", t.rotation.eulerAngles.z));
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
