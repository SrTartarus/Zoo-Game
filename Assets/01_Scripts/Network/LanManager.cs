using System.Collections;
using System.Text;
using Zoo.Web;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Networking;
using UnityEngine;

namespace Zoo.Network
{
    [RequireComponent(typeof(NetworkManager))]
    public class LanManager : MonoBehaviour
    {

        // Singleton
        private static LanManager instance;
        public static LanManager singleton { get { return instance; } }

        // Socket
        private const int bufferSize = 8 * 1024;
        private State state = new State();
        private IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
        private UdpClient socket;

        public class State
        {
            public byte[] buffer = new byte[bufferSize];
        }

        // Lan
        private List<string> localAddresses = new List<string>();
        private List<string> localSubAdresses = new List<string>();
        private List<string> addresses = new List<string>();
        public string matchName;

        public List<Server> servers = new List<Server>();
        public bool bSearching = false;
        public float percentSearching;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            ScanHost();
        }

        public void StartServer(string matchName)
        {
            try
            {
                this.matchName = matchName;
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 1326);
                socket = new UdpClient(endPoint);
            }
            catch (SocketException e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                Receive();
                Debug.Log("Server Lan Created!!");
            }
        }


        private IEnumerator SendPingInRange(bool bClear = true, Action<bool, List<Server>> callback = null)
        {
            if (bClear)
            {
                servers.Clear();
                addresses.Clear();
            }

            bSearching = true;
            socket = new UdpClient();
            Receive();
            float count = (255 * localAddresses.Count);

            for (int x = 0; x <= 255; x++)
            {
                int subLength = localSubAdresses.Count;
                for (int y = 0; y < subLength; y++)
                {
                    try
                    {
                        LanMessage message = new LanMessage();
                        message.type = LanMessage.MessageType.ping;
                        string data = JsonUtility.ToJson(message);
                        Send(data, new IPEndPoint(IPAddress.Parse(localSubAdresses[y] + "." + x), 1326));
                        float percent = x * subLength;
                        percentSearching = (percent / count) * 100;
                    }
                    catch (SocketException e)
                    {
                        Debug.Log(e.Message);
                    }

                    yield return new WaitForEndOfFrame();
                }
            }

            callback(true, servers);
            bSearching = false;
            Disconnect();
        }

        public void Search(bool bClear, Action<bool, List<Server>> callback)
        {
            if (bSearching)
                return;

            StartCoroutine(SendPingInRange(bClear, callback));
        }

        private void Receive()
        {
            socket.BeginReceive(recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                byte[] bytes = socket.EndReceive(ar, ref remoteEndpoint);
                socket.BeginReceive(recv, so);
                string text = Encoding.ASCII.GetString(bytes);
                LanMessage message = JsonUtility.FromJson<LanMessage>(text);

                if (message.type == LanMessage.MessageType.ping)
                {
                    message.type = LanMessage.MessageType.pong;
                    message.matchName = matchName;
                    message.username = MultiplayerManager.user.username;
                    message.maxPlayers = NetworkManager.singleton.maxConnections;
                    message.numPlayers = NetworkManager.singleton.numPlayers;
                    message.platform = Application.platform.ToString();
                    message.port = NetworkManager.singleton.networkPort;
                    string data = JsonUtility.ToJson(message);
                    Send(data, remoteEndpoint);
                }
                else
                {
                    if (!addresses.Contains(remoteEndpoint.Address.ToString()))
                    {
                        addresses.Add(remoteEndpoint.Address.ToString());
                        Server server = new Server();
                        server.address = remoteEndpoint.Address.ToString();
                        server.user = new User();
                        server.user.username = message.username;
                        server.platform = message.platform;
                        server.port = message.port;
                        server.matchName = message.matchName;
                        server.maxPlayers = message.maxPlayers;
                        server.numPlayers = message.numPlayers;
                        servers.Add(server);
                    }
                }
            }, state);
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            socket.BeginSend(data, data.Length, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = socket.EndSend(ar);
            }, state);
        }

        public void Send(string text, IPEndPoint endPoint)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            socket.Send(data, data.Length, endPoint);
        }

        private void ScanHost()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            int addressLength = host.AddressList.Length;
            for (int i = 0; i < addressLength; i++)
            {
                IPAddress ip = host.AddressList[i];
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    string address = ip.ToString();
                    string subAddress = address.Remove(address.LastIndexOf("."));

                    localAddresses.Add(address);
                    if (!localSubAdresses.Contains(subAddress))
                    {
                        localSubAdresses.Add(subAddress);
                    }
                }
            }
        }

        public void Disconnect()
        {
            StopAllCoroutines();

            if (socket != null)
            {
                socket.Close();
                socket = null;
                bSearching = false;
                Debug.Log("Lan Disconnected!!");
            }
        }

        private void OnDisable()
        {
            Disconnect();
        }
    }

    [Serializable]
    public class LanMessage
    {
        public enum MessageType { ping, pong };

        public MessageType type;
        public string matchName;
        public string username;
        public int port;
        public int maxPlayers;
        public int numPlayers;
        public string platform;
    }
}