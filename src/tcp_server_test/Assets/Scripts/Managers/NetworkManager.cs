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
using Unity.Networking.Transport;
using Unity.Collections;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private string _IP = "127.0.0.1";
    private ushort _port = 1234;
    [SerializeField]
    Transform _obj;

    Thread mThread;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector3 receivedPos = Vector3.zero;
    public GameObject robot;
    public Vector3 robot_pos;



    bool running;

    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

    public class JsonData
    {
        public List<Vector3> robot_pos = new List<Vector3>();
    }

    private void Update()
    {
        transform.position = receivedPos; //assigning receivedPos in SendAndReceiveData()
        robot_pos = robot.transform.position;

    }

    private void Start()
    {
        robot = GameObject.Find("robot");
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();

        running = true;
        while (running)
        {
            SendData();
            //SendAndReceiveData();
        }
        listener.Stop();
    }

    void SendData()
    {

        NetworkStream nwStream = client.GetStream();
        StreamWriter sw = new StreamWriter(nwStream) { AutoFlush = true };
        var testData = new JsonData();
        testData.robot_pos = new List<Vector3>()
        {
            robot_pos
        };
        var result = JsonUtility.ToJson(testData);

        //---Sending Data to Host----
        sw.WriteLine(result);
        //string data = robot_pos.ToString();
        //sw.Write(data, 0, data.Length);

    }








    //private string ip;
    //[SerializeField]
    //private int port;

    //private string studioIP;
    //private int studioPort;

    //private string headsetIP;
    //private int headsetPort;

    //[SerializeField]
    //private TMPro.TMP_Text displayText;

    //private TcpListener server;
    //private TcpClient client;

    //private UdpClient _client;

    //private NetworkDriver _driver;
    //private NetworkConnection _connection;
    //private bool _connected;

    //// Start is called before the first frame update
    //private void Start()
    //{
    //    //_client = new UdpClient();
    //    //try
    //    //{
    //    //    _client.Connect(IPAddress.Parse(_IP), _port);
    //    //    _connected = true;
    //    //}
    //    //catch (Exception e) { 
    //    //    Debug.LogException(e);
    //    //    _connected = false;
    //    //}


    //    //Thread serverThread = new Thread(new ThreadStart(StartClient));
    //    //serverThread.IsBackground = true;
    //    //serverThread.Start();
    //    //InvokeRepeating("changeValue", 1, 1);

    //    _connected = false;
    //    _connection = default;
    //    _driver = NetworkDriver.Create();
    //    var endpoint = NetworkEndpoint.Parse(_IP, _port, NetworkFamily.Ipv4);
    //    try
    //    {
    //        _connection = _driver.Connect(endpoint);
    //        _connected = true;
    //        Debug.Log(_connection);
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogException(ex);
    //    }

    //}

    //private void StartClient()
    //{
    //    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(_IP), _port);
    //    Byte[] sendBytes = Encoding.ASCII.GetBytes("Hola!");

    //    _client.Send(sendBytes, sendBytes.Length);

    //    Byte[] receiveBytes = _client.Receive(ref RemoteIpEndPoint);
    //    string returnData = Encoding.ASCII.GetString(receiveBytes);

    //    // Uses the IPEndPoint object to determine which of these two hosts responded.
    //    Debug.Log("This is the message you received " +
    //                                 returnData.ToString());

    //}



    //private void OnDestroy()
    //{
    //    _client?.Close();
    //}

    ////void StartServer()
    ////{
    ////    System.Net.IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

    ////    foreach (System.Net.IPAddress ip in host.AddressList)
    ////    {
    ////        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    ////        {
    ////            this.ip = ip.ToString();
    ////        }
    ////    }

    ////    server = new(System.Net.IPAddress.Parse(this.ip), port);
    ////    server.Start();

    ////    while(client == null)
    ////    {
    ////        client = server.AcceptTcpClient();

    ////    }

    ////}

    ////void changeValue()
    ////{
    ////    if (client != null)
    ////    {
    ////        int random = UnityEngine.Random.Range(0,9);
    ////        Debug.Log(random);
    ////        byte[] buffer = new byte[1024];
    ////        NetworkStream stream = client.GetStream();
    ////        stream.Write(Encoding.UTF8.GetBytes($"{random}"));
    ////    }
    ////}



    //// Update is called once per frame
    //void Update()
    //{
    //    _driver.ScheduleUpdate().Complete();
    //    if (!_connected)
    //        return;

    //    DataStreamReader stream;
    //    NetworkEvent.Type cmd;
    //    DataStreamWriter writer = new DataStreamWriter();
    //    _driver.BeginSend(_connection, out writer);
    //    NativeArray<byte> bytes = new NativeArray<byte>(Encoding.ASCII.GetBytes(transform.position.ToString() + "," + transform.rotation.eulerAngles.ToString()), Allocator.Persistent);
    //    writer.WriteBytes(bytes);
    //    _driver.EndSend(writer);
    //    while ((cmd = _connection.PopEvent(_driver, out stream)) != NetworkEvent.Type.Empty)
    //    {
    //        if (cmd == NetworkEvent.Type.Data)
    //        {
    //            NativeArray<byte> buff = new NativeArray<byte>();
    //            stream.ReadBytes(buff);
    //            Debug.Log($"Got the value {buff} back from the server.");
    //        }
    //        else if (cmd == NetworkEvent.Type.Disconnect)
    //        {
    //            Debug.Log("Client got disconnected from server.");
    //            _connection = default;
    //            _connected = false;
    //        }
    //    }

    //        //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(_IP), _port);
    //        //Byte[] sendBytes = Encoding.ASCII.GetBytes(_obj.position.ToString() + _obj.rotation.eulerAngles.ToString());

    //        //_client.Send(sendBytes, sendBytes.Length);

    //        //Byte[] receiveBytes = _client.Receive(ref RemoteIpEndPoint);
    //        //string returnData = Encoding.ASCII.GetString(receiveBytes);

    //        //System.Threading.Tasks.Task.Run(async () =>
    //        //{
    //        //    using (var udpClient = new UdpClient(11000))
    //        //    {
    //        //        while (true)
    //        //        {
    //        //            //IPEndPoint object will allow us to read datagrams sent from any source.
    //        //            var receivedResults = await udpClient.ReceiveAsync();
    //        //            Debug.Log("This is the message you received " +
    //        //                             Encoding.ASCII.GetString(receivedResults.Buffer).ToString());
    //        //        }
    //        //    }
    //        //});

    //        //// Uses the IPEndPoint object to determine which of these two hosts responded.
    //        //Debug.Log("This is the message you received " +
    //        //                             returnData.ToString());
    //    }

    ////private void OnApplicationQuit()
    ////{
    ////    server.Stop();
    ////    client?.Close();
    ////}
}
