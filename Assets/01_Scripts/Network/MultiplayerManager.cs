using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using MongoDB.Driver;
using Zoo.Web;
using Zoo.Lobby;
using Zoo.Core;

namespace Zoo.Network
{
    public class MultiplayerManager : NetworkManager
    {
        public static User user = null;
        public static Character character = null;
        public static Server server = null;

        public static bool bMatchMaker = false;
        public static bool bServer = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler(MsgType.Highest + 1, OnServerChat);
        }

        public override void OnStartClient(NetworkClient client)
        {
            client.RegisterHandler(MsgType.Highest + 1, OnClientChat);
        }

        private void OnServerChat(NetworkMessage netMsg)
        {
            ChatMessage msg = netMsg.ReadMessage<ChatMessage>();
            NetworkServer.SendToAll(MsgType.Highest + 1, msg);
        }

        private void OnClientChat(NetworkMessage netMsg)
        {
            FindObjectOfType<Chat>().MessageReceived(netMsg);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (bMatchMaker)
            {
                UpdateDefinition<Server> update = Builders<Server>.Update
                .Set(it => it.numPlayers, singleton.numPlayers + 1);
                WebManager.singleton.db.Update<Server>("rpg_servers", server._id, update);
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (bMatchMaker)
            {
                UpdateDefinition<Server> update = Builders<Server>.Update
                .Set(it => it.numPlayers, singleton.numPlayers);
                WebManager.singleton.db.Update<Server>("rpg_servers", server._id, update);
            }
        }

        public static void CreateMatch(bool bOnline, string matchName)
        {
            if(!bOnline)
            {
                bServer = true;
                LanManager.singleton.StartServer(matchName);
                singleton.StartHost();
            }
            else
            {
                singleton.StartMatchMaker();
                singleton.matchMaker.CreateMatch(matchName, (uint)singleton.maxConnections, true, "", "", "", 0, 0, OnInternetMatchCreate);
            }
        }

        public static void JoinMatch(bool bOnline, Server localServer)
        {
            if(!bOnline)
            {
                singleton.networkAddress = localServer.address;
                singleton.networkPort = localServer.port;
                singleton.StartClient();
            }
            else
            {
                singleton.StartMatchMaker();
                server = localServer;
                singleton.matchMaker.JoinMatch(server.networkId, "", "", "", 0, 0, OnJoinInternetMatch);
            }
        }

        private static void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if(success)
            {
                Searcher search = FindObjectOfType<Searcher>();
                search.RegisterServer(search.matchName, matchInfo);
            }
            else
            {
                Debug.Log(extendedInfo);
            }
        }

        private static void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if(success)
            {
                server = null;
                singleton.StartClient(matchInfo);
            }
            else
            {
                WebManager.singleton.db.AsyncDelete<Server>("rpg_servers", server._id, OnDeleteServer);
                Debug.Log(extendedInfo);
            }
        }

        private static void OnDeleteServer(bool success)
        {
            Destroy(GameObject.Find(server._id.ToString()));
            server = null;
        }

        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                Stop();
            }
        }

        private void OnApplicationQuit()
        {
            Stop();
        }

        private static void Stop()
        {
            if(singleton.IsClientConnected())
            {
                if(bServer)
                {
                    if(bMatchMaker)
                        WebManager.singleton.db.Delete<Server>("rpg_servers", server._id);

                    bServer = false;
                    bMatchMaker = false;
                    singleton.StopMatchMaker();
                    singleton.StopHost();
                }
                else
                {
                    singleton.StopMatchMaker();
                    singleton.StopHost();
                }

                user = null;
                server = null;
                character = null;
            }
        }
    }

    public class ChatMessage : MessageBase
    {
        public string username;
        public string msg;
    }
}