using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zoo.Web;
using Zoo.Network;

namespace Zoo.Lobby
{
    public class Selecter : MonoBehaviour
    {
        public List<Character> characters = new List<Character>();

        [SerializeField] private GameObject character, characterUI, add, selectingCanvas;
        [SerializeField] private Transform uiContainer, objContainer;
        [SerializeField] private GameObject customization, lobby, dynamicCanvas;

        private List<GameObject> charactersObj = new List<GameObject>();

        private int lastIndex = 0;

        // Start is called before the first frame update
        void Start()
        {
            if (characters.Count >= 3)
                add.SetActive(false);

            foreach (Character character in characters)
            {
                // Character
                GameObject c = Instantiate(this.character, objContainer);
                Customization.CustomizerSetup ch = c.GetComponent<Customization.CustomizerSetup>();
                ch.skinColor = new UnityEngine.Color(character.skin.r, character.skin.g, character.skin.b, character.skin.a);
                ch.headIndex = character.indexHead;

                int length = character.clothesColor.Count;
                for (int i = 0; i <= length - 1; i++)
                {
                    ch.clothesColor.Add(new UnityEngine.Color(character.clothesColor[i].r, character.clothesColor[i].g, character.clothesColor[i].b, character.clothesColor[i].a));
                }

                charactersObj.Add(c);

                // UI
                GameObject ui = Instantiate(characterUI, uiContainer);
                ui.transform.GetChild(0).GetComponent<Text>().text = character.name;
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((eventData) => { OnPointerEnter((PointerEventData)eventData, ui.transform.GetSiblingIndex()); });
                ui.GetComponent<EventTrigger>().triggers.Add(entry);
                ui.GetComponent<Button>().onClick.AddListener(() => Select(ui.transform));
            }

            selectingCanvas.SetActive(true);
            charactersObj[lastIndex].SetActive(true);
            add.transform.SetSiblingIndex(add.transform.parent.childCount - 1);
        }

        private void Select(Transform player)
        {
            MultiplayerManager.character = characters[player.GetSiblingIndex()];
            selectingCanvas.SetActive(false);
            gameObject.SetActive(false);
            lobby.SetActive(true);
        }

        public void AddCharacter()
        {
            customization.SetActive(true);
            dynamicCanvas.SetActive(true);
            selectingCanvas.SetActive(false);
            gameObject.SetActive(false);
        }

        private void OnPointerEnter(PointerEventData data, int index)
        {
            if (index == lastIndex)
                return;

            charactersObj[lastIndex].SetActive(false);
            charactersObj[index].SetActive(true);
            lastIndex = index;
        }
    }
}