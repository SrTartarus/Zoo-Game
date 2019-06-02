using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Zoo.Network;

namespace Zoo.Core
{
    public class Chat : MonoBehaviour
    {
        [SerializeField] TouchkeyboardInputField input;
        [SerializeField] private CanvasGroup canvas;

        [SerializeField] private GameObject textUI;
        [SerializeField] private Transform textContainer;

        private EventSystem current;

        private float time = 0f;

        // Use this for initialization
        void Start()
        {
            current = EventSystem.current;
            input.onValueChanged.AddListener(delegate { OnInputChanged(); });
            input.onKeyboardDone.AddListener(OnTouchkeyboardDone);
        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!GetComponent<Image>().raycastTarget)
                    SendText();
                else
                    ActiveChat();
            }

            if (time >= 25f)
            {
                GetComponent<Image>().raycastTarget = true;
                canvas.alpha = 0.2f;
            }
        }

        private void OnTouchkeyboardDone()
        {
            SendText();
        }

        private void OnInputChanged()
        {
            GetComponent<Image>().raycastTarget = false;
            time = 0f;
            canvas.alpha = 0.8f;
        }

        public void ActiveChat()
        {
            current.SetSelectedGameObject(input.gameObject, new BaseEventData(current));
            input.GetComponent<TouchkeyboardInputField>().OnSelect(new BaseEventData(current));
            GetComponent<Image>().raycastTarget = false;
            time = 0f;
            canvas.alpha = 0.8f;
        }

        public void SendText()
        {
            if (input.text == "")
                return;

            ChatMessage msg = new ChatMessage();
            msg.msg = input.text;
            msg.username = MultiplayerManager.character.name;
            MultiplayerManager.singleton.client.Send(MsgType.Highest + 1, msg);
            input.text = "";
#if UNITY_EDITOR || UNITY_STANDALONE
            current.SetSelectedGameObject(input.gameObject, new BaseEventData(current));
            input.GetComponent<TouchkeyboardInputField>().OnSelect(new BaseEventData(current));
#endif
        }

        public void MessageReceived(NetworkMessage msg)
        {
            GetComponent<Image>().raycastTarget = false;
            time = 0f;
            canvas.alpha = 0.8f;
            ChatMessage json = msg.ReadMessage<ChatMessage>();
            GameObject go = Instantiate(textUI, textContainer);
            go.GetComponent<Text>().text = "[" + json.username + "]: " + json.msg;
        }
    }
}
