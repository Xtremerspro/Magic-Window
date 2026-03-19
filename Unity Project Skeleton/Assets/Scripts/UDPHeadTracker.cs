using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Globalization;

public class UDPHeadTracker : MonoBehaviour
{
    private Thread receiveThread;
    private UdpClient client;
    public int port = 5005;
    public float phoneOffset = -0.5f;

    public Vector3 LatestHeadPos;
    public bool IsTracking = false;
    public bool IsPhone = false;

    // Thread safety
    private readonly object _lock = new object();
    private Vector3 _threadPos;
    private bool _hasNewData = false;
    private float _privatePhoneOffset = 0;

    void Start()
    {
        // Lower the frame rate to give the CPU room to breathe for the UDP thread
        Application.targetFrameRate = 60;

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Priority = System.Threading.ThreadPriority.AboveNormal; // Give the tracker higher priority
        receiveThread.Start();
        Debug.Log("UDP Receiver Started with High Priority...");
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);

                // PROCESS DATA IN BACKGROUND THREAD
                // This saves the Main Thread from doing expensive string splitting/parsing
                string[] points = text.Split(',');
                if (points.Length == 3)
                {
                    float x = float.Parse(points[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(points[1], CultureInfo.InvariantCulture) + _privatePhoneOffset;
                    float z = float.Parse(points[2], CultureInfo.InvariantCulture);

                    // Use a lock to safely pass the Vector3 to the main thread
                    lock (_lock)
                    {
                        _threadPos = new Vector3(x, y, -z);
                        _hasNewData = true;
                    }
                }
            }
            catch (System.Exception err)
            {
                // Silently catch socket errors on close
                if (err is SocketException) break;
                Debug.LogWarning("UDP Error: " + err.Message);
            }
        }
    }

    void Update()
    {
        // Quickly grab the data and release the lock
        lock (_lock)
        {
            if (_hasNewData)
            {
                LatestHeadPos = _threadPos;
                IsTracking = true;
                _hasNewData = false; // Reset so we don't re-process the same frame
            }
        }

        if (IsPhone)
            _privatePhoneOffset = phoneOffset;
        else
            _privatePhoneOffset = 0;
    }

    void OnApplicationQuit()
    {
        if (client != null) client.Close();
        if (receiveThread != null && receiveThread.IsAlive) receiveThread.Abort();
    }
}