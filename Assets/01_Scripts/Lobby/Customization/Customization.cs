using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.Web;
using Zoo.Network;

namespace Zoo.Customization
{
    public class Customization : MonoBehaviour
    {
        public Character character;

        private Vector2 lastPosition;

        public float speed = 0.5f;

        [SerializeField] Material[] heads;
        [SerializeField] Material body, clothes;
        [SerializeField] Transform head;

        // Menu
        [SerializeField] private Transform subMenus;
        private int lastIndex = -1;
        private Color32 select, unselect;

        [SerializeField] private GameObject lobby, customization;

        [SerializeField] private Text error;
        [SerializeField] private Button finishBtn;

        // Start is called before the first frame update
        void Start()
        {
            character = new Character();
            character.username = MultiplayerManager.user.username;
            character._id = MongoDB.Bson.ObjectId.GenerateNewId(System.DateTime.Now);
            character.clothesColor = new List<Character.Color>();
            Color defaultSkinColor = new Color(0.7098039f, 0.4078432f, 0f, 1f);
            Color defaultClothesColor = new Color(0.1137255f, 0.1058824f, 0.08235294f, 1f);
            int headLength = heads.Length;
            character.skin = new Character.Color(defaultSkinColor.r, defaultSkinColor.g, defaultSkinColor.b, defaultSkinColor.a);
            character.indexHead = 0;
            
            for(int i = 0; i < 3; i++)
            {
                character.clothesColor.Add(new Character.Color(defaultClothesColor.r, defaultClothesColor.g, defaultClothesColor.b, defaultClothesColor.a));
            }

            for (int i = 0; i < headLength; i++)
            {
                heads[i].SetColor("_Color", defaultSkinColor);
            }

            body.SetColor("_Color", defaultSkinColor);
            clothes.SetColor("_ColorMangas", defaultClothesColor);
            clothes.SetColor("_ColorShort", defaultClothesColor);
            clothes.SetColor("_ColorEspalda", defaultClothesColor);

            select = new Color32(200, 200, 200, 255);
            unselect = new Color32(255, 255, 255, 255);
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0))
            {
                float xDelta = Input.mousePosition.x - lastPosition.x;
                xDelta = (xDelta * -1) * speed;
                Vector3 localRotation = transform.localEulerAngles;
                Vector3 targetRotation = new Vector3(localRotation.x, localRotation.y + xDelta, localRotation.z);
                transform.localEulerAngles = targetRotation;
            }

            lastPosition = Input.mousePosition;
#else
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch(touch.phase)
                {
                    case TouchPhase.Began:
                        lastPosition = Input.mousePosition;
                        break;
                    case TouchPhase.Moved:
                        speed = 0.1f;
                        float xDelta = Input.mousePosition.x - lastPosition.x;
                        xDelta = (xDelta * -1) * speed;
                        Vector3 localRotation = transform.localEulerAngles;
                        Vector3 targetRotation = new Vector3(localRotation.x, localRotation.y + xDelta, localRotation.z);
                        transform.localEulerAngles = targetRotation;
                        break;
                    case TouchPhase.Ended:
                        break;
                }
            }
#endif
        }

        public void SelectMenu(Transform menu)
        {
            int index = menu.GetSiblingIndex();
            if (lastIndex == index)
                return;

            subMenus.GetChild(index).GetComponent<Animator>().SetTrigger("active");
            menu.GetComponent<Image>().color = select;
            if (lastIndex != -1)
            {
                subMenus.GetChild(lastIndex).GetComponent<Animator>().SetTrigger("active");
                menu.parent.GetChild(lastIndex).GetComponent<Image>().color = unselect;
            }

            lastIndex = menu.GetSiblingIndex();
        }

        public void ChangeBody(int index, int lastIndex)
        {
            head.GetChild(index).gameObject.SetActive(true);
            head.GetChild(lastIndex).gameObject.SetActive(false);
        }

        public void ChangeSkinColor(UnityEngine.Color skin)
        {
            int headLength = heads.Length;
            for (int i = 0; i < headLength; i++)
            {
                heads[i].SetColor("_Color", skin);
            }

            body.SetColor("_Color", skin);
        }

        public void ChangeClotheColor(UnityEngine.Color[] colors)
        {
            clothes.SetColor("_ColorMangas", colors[0]);
            clothes.SetColor("_ColorShort", colors[1]);
            clothes.SetColor("_ColorEspalda", colors[2]);
        }

        public void OnCharacterNameChanged()
        {
            error.gameObject.SetActive(false);
        }

        public void Finish(InputField field)
        {
            if (field.text == "")
                return;

            finishBtn.interactable = false;
            character.name = field.text;
            MongoDB.Driver.FilterDefinition<Character> filter = MongoDB.Driver.Builders<Character>.Filter.Eq("name", field.text);
            WebManager.singleton.db.Select<Character>("rpg_characters", filter, OnCharacterExisted);
        }

        private void OnCharacterExisted(bool success, Character character)
        {
            finishBtn.interactable = true;

            if(!success)
            {
                WebManager.singleton.db.Insert<Character>("rpg_characters", this.character, OnCharacter);
            }
            else
            {
                error.gameObject.SetActive(true);
            }
        }

        private void OnCharacter(bool success)
        {
            if(success)
            {
                MultiplayerManager.character = character;
                lobby.SetActive(true);
                customization.SetActive(false);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Somenthing wrong!!");
            }
        }
    }
}
