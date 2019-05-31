using UnityEngine;
using UnityEngine.UI;
using Zoo.Web;
using Zoo.Network;

namespace Zoo.Lobby
{
    public class NetworkJoin : MonoBehaviour
    {
        public Server server;

        // Use this for initialization
        void Start()
        {
            transform.Find("name").GetComponent<Text>().text = server.matchName;
            transform.Find("user").GetComponent<Text>().text = server.user.username;
            transform.Find("players").GetComponent<Text>().text = server.numPlayers + " / " + server.maxPlayers;
        }

        public void JoinMatch()
        {
            GetComponent<Button>().interactable = false;
            Searcher search = FindObjectOfType<Searcher>();
            MultiplayerManager.JoinMatch((search.connectionType == 0), server);
        }
    }

}
