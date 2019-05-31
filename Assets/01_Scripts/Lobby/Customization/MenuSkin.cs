using UnityEngine;
using UnityEngine.UI;

namespace Zoo.Customization
{
    public class MenuSkin : MonoBehaviour
    {
        [SerializeField] private Customization manager;

        private int lastIndex = 0;

        public void SelectSkin(Transform skin)
        {
            int index = skin.GetSiblingIndex();
            if (index == lastIndex)
                return;

            FindObjectOfType<Customization>().ChangeSkinColor(skin.GetComponent<Image>().color);
            skin.GetChild(0).gameObject.SetActive(true);
            skin.parent.GetChild(lastIndex).GetChild(0).gameObject.SetActive(false);
            lastIndex = index;
            UnityEngine.Color color = skin.GetComponent<Image>().color;
            manager.character.skin = new Web.Character.Color(color.r, color.g, color.b, color.a);
        }
    }
}
