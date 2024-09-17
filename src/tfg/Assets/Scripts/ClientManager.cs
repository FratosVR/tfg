using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System;
using System.Text;
using System.Threading;

public class ClientManager : MonoBehaviour
{
    // Start is called before the first frame update

    private int random;

    [SerializeField]
    private String ip;
    [SerializeField]
    private int port;

    [SerializeField]
    private TMPro.TMP_Text text;

    private TcpClient client;
    void Start()
    {

        Thread t = new Thread(new ThreadStart(ConnectToServer));
        t.IsBackground = true;
        t.Start();

    }

    private void ConnectToServer()
    {
        client = new TcpClient();

        while (!client.Connected)
        {
            client.Connect(ip, port);
        }

    }

    // Update is called once per frame
    void Update()
    {
        byte[] buffer = new byte[1024];
        NetworkStream stream = client.GetStream();

        stream.Read(buffer);
        string response = Encoding.UTF8.GetString(buffer);

        text.text = response;

        Debug.Log(response);

    }

    private void OnApplicationQuit()
    {
        client.Close();
    }
}
