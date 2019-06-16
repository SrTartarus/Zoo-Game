using System;
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

        [SerializeField]
        private Transform eraser;
        [SerializeField]
        private GameObject fillImage;

        private bool bHover = false;
        private float time;

        // Start is called before the first frame update
        void Start()
        {
            if (characters.Count >= 3)
                add.SetActive(false);

            CreateCharacters();

            selectingCanvas.SetActive(true);
            charactersObj[lastIndex].SetActive(true);
            add.transform.SetSiblingIndex(add.transform.parent.childCount - 1);
        }

        void Update()
        {
            if (!bHover)
                return;

            Vector2 mousePosition = Input.mousePosition;
            mousePosition.x += 10f;
            mousePosition.y += 10f;
            eraser.GetComponent<RectTransform>().anchoredPosition = mousePosition;

            if(Input.GetMouseButton(1))
            {
                time += Time.deltaTime;
                fillImage.SetActive(true);
            }

            if (Input.GetMouseButtonUp(1))
            {
                time = 0f;
                fillImage.SetActive(false);
            }

            float timer = time / 3f;
            fillImage.transform.GetChild(0).GetComponent<Image>().fillAmount = timer;

            if(timer > 1f)
            {
                time = 0f;
                Delete();
            }
        }

        private void Select(Transform player)
        {
            MultiplayerManager.character = characters[player.GetSiblingIndex()];
            selectingCanvas.SetActive(false);
            gameObject.SetActive(false);
            lobby.SetActive(true);
        }

        private void Delete()
        {
            if (!bHover)
                return;

            WebManager.singleton.db.AsyncDelete<Character>("rpg_characters", characters[lastIndex]._id, OnDelete);
        }

        private void OnDelete(bool success)
        {
            if(success)
            {
                if (characters.Count <= 0)
                    AddCharacter();
                else
                {
                    characters.Remove(characters[lastIndex]);
                    charactersObj.Remove(charactersObj[lastIndex]);
                    charactersObj.Clear();

                    int length = objContainer.childCount;
                    for (int i = 0; i < length; i++)
                    {
                        if (uiContainer.GetChild(i).name != "add")
                            Destroy(uiContainer.GetChild(i).gameObject);

                        Destroy(objContainer.GetChild(i).gameObject);
                    }

                    CreateCharacters();

                    add.gameObject.SetActive(true);
                    add.transform.SetSiblingIndex(add.transform.parent.childCount - 1);
                    lastIndex = -1;
                }
            }
        }

        void CreateCharacters()
        {
            foreach (Character character in characters)
            {
                // Character
                GameObject c = Instantiate(this.character, objContainer);
                Customization.CustomizerSetup ch = c.GetComponent<Customization.CustomizerSetup>();
                ch.skinColor = new Color(character.skin.r, character.skin.g, character.skin.b, character.skin.a);
                ch.headIndex = character.indexHead;

                int length = character.clothesColor.Count;
                for (int i = 0; i <= length - 1; i++)
                {
                    ch.clothesColor.Add(new Color(character.clothesColor[i].r, character.clothesColor[i].g, character.clothesColor[i].b, character.clothesColor[i].a));
                }

                charactersObj.Add(c);

                // UI
                GameObject ui = Instantiate(characterUI, uiContainer);
                ui.transform.GetChild(0).GetComponent<Text>().text = character.name;
                EventTrigger.Entry enterEntry = new EventTrigger.Entry();
                enterEntry.eventID = EventTriggerType.PointerEnter;
                enterEntry.callback.AddListener((eventData) => { OnPointerEnter((PointerEventData)eventData, ui.transform.GetSiblingIndex()); });
                EventTrigger.Entry exitEntry = new EventTrigger.Entry();
                exitEntry.eventID = EventTriggerType.PointerExit;
                exitEntry.callback.AddListener((eventData) => { OnPointerExit((PointerEventData)eventData, ui.transform.GetSiblingIndex()); });
                ui.GetComponent<EventTrigger>().triggers.Add(exitEntry);
                ui.GetComponent<EventTrigger>().triggers.Add(enterEntry);
                ui.GetComponent<Button>().onClick.AddListener(() => Select(ui.transform));
            }
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
            bHover = true;
            eraser.gameObject.SetActive(true);

            if (index == lastIndex)
                return;

            try
            {
                charactersObj[lastIndex].SetActive(false);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                charactersObj[index].SetActive(true);
                lastIndex = index;
            }
        }

        private void OnPointerExit(PointerEventData data, int index)
        {
            bHover = false;
            eraser.gameObject.SetActive(false);
        }
    }
}