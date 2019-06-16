using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using Zoo.Network;
using Zoo.Web;

namespace Zoo.Lobby
{
    public class Searcher : MonoBehaviour
    {
        [SerializeField] GameObject serverUI;

        [SerializeField] private Animator modal;

        [SerializeField] private Transform sync, serverContainer;

        [SerializeField] private InputField input;
        [SerializeField] private Dropdown players;
        [SerializeField] private Dropdown type;
        [SerializeField] private Button createBtn, onlineBtn, lanBtn;
        [SerializeField] private Text refreshText;

        private bool bSearching = false;

        private MatchInfo hostInfo;
        [HideInInspector]
        public string matchName;
        [HideInInspector]
        public int connectionType = 0;

        // Update is called once per frame
        void Update()
        {
            if (bSearching)
            {
                sync.transform.Rotate(0, 0, 180 * Time.deltaTime);
            }
        }

        public void Switch(bool bOnline)
        {
            if (bSearching)
                return;

            foreach (Transform child in serverContainer)
            {
                Destroy(child.gameObject);
            }

            if (!bOnline)
            {
                connectionType = 1;
                onlineBtn.interactable = true;
                lanBtn.interactable = false;
                refreshText.text = "Refresh (LAN)";
            }
            else
            {
                connectionType = 0;
                onlineBtn.interactable = false;
                lanBtn.interactable = true;
                refreshText.text = "Refresh (ONLINE)";
            }
        }

        public void Refresh()
        {
            if (bSearching)
                return;

            foreach (Transform child in serverContainer)
            {
                Destroy(child.gameObject);
            }

            bSearching = true;

            if (!onlineBtn.interactable)
                WebManager.singleton.db.SelectAll<Server>("rpg_servers", OnRefresh);
            else
                LanManager.singleton.Search(true, OnRefresh);
        }

        private void OnRefresh(bool success, List<Server> servers)
        {
            if (success)
            {
                foreach (Server server in servers)
                {
                    GameObject go = Instantiate(serverUI, serverContainer);
                    go.name = server._id.ToString();
                    NetworkJoin join = go.GetComponent<NetworkJoin>();
                    join.server = server;
                }
            }
            else
            {
                Debug.Log("Somenthing wrong!!");
            }

            bSearching = false;
        }

        public void CreateMatch()
        {
            if (input.text == "")
                return;

            createBtn.interactable = false;
            matchName = input.text;
            int menuIndex = players.value;
            List<Dropdown.OptionData> menuOptions = players.options;
            string value = menuOptions[menuIndex].text;
            MultiplayerManager.singleton.maxConnections = int.Parse(value);
            MultiplayerManager.CreateMatch((type.value == 0), input.text);
        }

        public void RegisterServer(string matchName, MatchInfo matchInfo)
        {
            if (MultiplayerManager.user == null)
                return;

            hostInfo = matchInfo;
            Server server = new Server();
            server.user = MultiplayerManager.user;
            server.address = matchInfo.address;
            server.port = matchInfo.port;
            server.matchName = matchName;
            server.networkId = matchInfo.networkId;
            server.maxPlayers = MultiplayerManager.singleton.maxConnections;
            server.numPlayers = 0;
            server.platform = Application.platform.ToString();
            MultiplayerManager.server = server;
            WebManager.singleton.db.Insert<Server>("rpg_servers", server, OnRegisterServer);
        }

        private void OnRegisterServer(bool success)
        {
            if (success)
            {
                if (hostInfo == null)
                    return;

                createBtn.interactable = true;
                MultiplayerManager.bMatchMaker = true;
                MultiplayerManager.bServer = true;
                NetworkServer.Listen(hostInfo, 7777);
                MultiplayerManager.singleton.StartHost(hostInfo);
            }
            else
            {
                Debug.Log("Something Wrong");
            }
        }

        public void ActionMatch(bool active)
        {
            modal.SetBool("active", active);
        }
    }
}