using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    [SerializeField]
    private string _uri;
    [SerializeField]
    private List<Transform> _bones;
    private List<BonesInfo> _bonesInfo;
    private void Start()
    {
        _bonesInfo = new List<BonesInfo>();
        StartCoroutine(sendInfo());
        StartCoroutine(recieveInfo());
    }

    private void Update()
    {
        BonesInfo info = new BonesInfo();
        info.AddInfo(_bones);

        Debug.Log(info.ToJSON());
        _bonesInfo.Add(info);
    }

    IEnumerator sendInfo()
    {
        yield return new WaitForSeconds(0.8f);
        string s = "";
        foreach (BonesInfo info in _bonesInfo) { 
            s += info.ToJSON();
        }
        _bonesInfo.Clear();

        UnityWebRequest www = UnityWebRequest.Put(_uri, s);
        yield return www.SendWebRequest();

        if(www.error == null)
        {
            Debug.LogError(www.error);
            Application.Quit();
        }
        else
        {
            StartCoroutine(sendInfo());
        }
    }

    IEnumerator recieveInfo()
    {
        UnityWebRequest www = UnityWebRequest.Get(_uri);
        yield return www.SendWebRequest();

        if (www.error != null) {
            Debug.LogError(www.error);
            Application.Quit();
        }
        else
        {
            
            Debug.Log(www.downloadHandler.text);
            StartCoroutine(recieveInfo());
        }
    }
}
