using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    [SerializeField]
    private string _uri;
    [SerializeField]
    private List<Transform> _bones;
    //private List<BonesInfo> _bonesInfo;
    private Queue<BonesInfo> _bonesInfo;
    private void Start()
    {
        _bonesInfo = new Queue<BonesInfo>(2);
        PushInfo();
        StartCoroutine(sendInfo());
    }

    //private void Update()
    //{
    //    BonesInfo info = new BonesInfo();
    //    info.AddInfo(_bones);

    //    Debug.Log(info.ToJSON());
    //    _bonesInfo.Add(info);
    //}

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

        UnityWebRequest www = UnityWebRequest.Post(_uri, s, "application/json");
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

    //IEnumerator recieveInfo()
    //{
    //    UnityWebRequest www = UnityWebRequest.Get(_uri);
    //    yield return www.SendWebRequest();

    //    if (www.error != null) {
    //        Debug.LogError(www.error);
    //        Application.Quit();
    //    }
    //    else
    //    {
            
    //        Debug.Log(www.downloadHandler.text);
    //        StartCoroutine(recieveInfo());
    //    }
    //}
}
