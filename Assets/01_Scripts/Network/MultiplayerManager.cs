using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using Zoo.Web;
using Zoo.Lobby;

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

        public override void OnServerConnect(NetworkConnection conn)
        {

        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {

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

        public static void JoinMatch(bool bOnline, Server server)
        {
            if(!bOnline)
            {
                singleton.networkAddress = server.address;
                singleton.networkPort = server.port;
                singleton.StartClient();
            }
            else
            {
                singleton.StartMatchMaker();
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
                singleton.StartClient(matchInfo);
            }
            else
            {
                Debug.Log(extendedInfo);
            }
        }
    }
}