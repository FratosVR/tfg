
using Neuron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _bones;
    [SerializeField]
    NeuronSourceManager _sourceManager;
    private Queue<BonesInfo> _bonesInfo;
    private string _IP;

    private static ServerManager _instance;
    public static ServerManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
    }

    public void SetIP(string ip)
    {
        _IP = ip;
        _sourceManager.address = ip;
        PushInfo();
        StartCoroutine(sendInfo());
    }

    private void Start()
    {
        _bonesInfo = new Queue<BonesInfo>(2);
        SetIP("192.168.1.127");
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
        string s = "{\"instances\": [{";
        int i = 0;
        foreach (BonesInfo info in _bonesInfo) { 
            s += info.ToJSON(i, out i);
            s += ",";
        }
        s = s.Remove(s.Length - 1);
        s += "}]}";
        _bonesInfo.Dequeue();
        UnityWebRequest www = UnityWebRequest.Post($"http://{_IP}:8501/v1/models/rigardu:predict", s, "application/json");
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            Application.Quit();
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            PredictionManager.Instance.NewPrediction(www.downloadHandler.text);
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
