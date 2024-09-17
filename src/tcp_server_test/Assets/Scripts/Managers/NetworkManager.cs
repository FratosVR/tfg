using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;
using System.Threading;
using UnityEditor.PackageManager;
using Unity.VisualScripting;

public class NetworkManager : MonoBehaviour
{
    private string ip;
    [SerializeField]
    private int port;

    private string studioIP;
    private int studioPort;

    private string headsetIP;
    private int headsetPort;

    [SerializeField]
    private TMPro.TMP_Text displayText;

    private TcpListener server;
    private TcpClient client;

    // Start is called before the first frame update
    private void Start()
    {
        Thread serverThread = new Thread(new ThreadStart(StartServer));
        serverThread.IsBackground = true;
        serverThread.Start();
        InvokeRepeating("changeValue", 1, 1);
    }
    void StartServer()
    {
        System.Net.IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (System.Net.IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                this.ip = ip.ToString();
            }
        }

        server = new(System.Net.IPAddress.Parse(this.ip), port);
        server.Start();

        while(client == null)
        {
            client = server.AcceptTcpClient();

        }

    }

    void changeValue()
    {
        if (client != null)
        {
            int random = UnityEngine.Random.Range(0,9);
            Debug.Log(random);
            byte[] buffer = new byte[1024];
            NetworkStream stream = client.GetStream();
            stream.Write(Encoding.UTF8.GetBytes($"{random}"));
        }
    }



    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnApplicationQuit()
    {
        server.Stop();
        client?.Close();
    }
}
