using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _bones;
    private Queue<BonesInfo> _bonesInfo;
    private string _IP;

    public void SetIP(string ip)
    {
        _IP = ip;
    }

    private void Start()
    {
        _bonesInfo = new Queue<BonesInfo>(2);
        PushInfo();
        StartCoroutine(sendInfo());
    }

    private void PushInfo()
    {
        BonesInfo info = new BonesInfo();
        info.AddInfo(_bones);
        _bonesInfo.Enqueue(info);
    }

    IEnumerator sendInfo()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        PushInfo();
        string s = "{\"instances\":[{";
        foreach (BonesInfo info in _bonesInfo) { 
            s += info.ToJSON();
        }
        s += "}]}";
        _bonesInfo.Dequeue();

        UnityWebRequest www = UnityWebRequest.Post($"http://{_IP}:8501/v1/models/half_plus_two:predict", s, "application/json");
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            Application.Quit();
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            Debug.Log(www.downloadHandler);
            StartCoroutine(sendInfo());
        }
    }

    public void RetryConnexion()
    {
        _bonesInfo.Clear();
        PushInfo();
        StartCoroutine(sendInfo());
    }
}
