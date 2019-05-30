using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Zoo.Web;

namespace Zoo.Network
{
    public class MultiplayerManager : NetworkManager
    {
        public static User user = null;
        public static Server server = null;

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
    }
}