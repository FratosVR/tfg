
using Neuron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Component that manage the communication with the server.
/// </summary>
public class ServerManager : MonoBehaviour
{
    /// <summary>
    /// List of the bones in the character's mesh with the mocap suit.
    /// </summary>
    [SerializeField]
    private List<Transform> _bones;
    /// <summary>
    /// NeuronSourceManager instance, responsible for communication with Axis Studio.
    /// </summary>
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

    /// <summary>
    /// Set the IP where the server with the model and Axis Studio are and begins the communication.
    /// </summary>
    /// <param name="ip">IP where the server and Axis Studio are.</param>
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
    }

    /// <summary>
    /// Enqueue a new BonesInfo instance.
    /// </summary>
    private void PushInfo()
    {
        BonesInfo info = new BonesInfo();
        info.AddInfo(_bones);
        _bonesInfo.Enqueue(info);
    }

    /// <summary>
    /// Coroutine that handles the connection to the server. It waits a specified amount of time to send the bone information in JSON format and receives the prediction.
    /// </summary>
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
            UIManager.Instance.SetText("Se ha perdido la conexión. Vuelve a establecer la IP.");
            _bonesInfo.Clear();
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            PredictionManager.Instance.NewPrediction(www.downloadHandler.text);
            StartCoroutine(sendInfo());
        }
    }
}
