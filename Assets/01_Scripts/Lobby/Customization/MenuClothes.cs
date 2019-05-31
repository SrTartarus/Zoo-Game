using UnityEngine;
using UnityEngine.UI;

namespace Zoo.Customization
{
    public class MenuClothes : MonoBehaviour
    {
        [SerializeField] private Customization manager;

        private int[] lastIndex = new int[3] { 0, 0, 0 };

        public void SelectClothe(Transform clothe)
        {
            int indexParent = clothe.parent.parent.GetSiblingIndex();
            int index = clothe.GetSiblingIndex();
            if (index == lastIndex[indexParent])
                return;

            clothe.GetChild(0).gameObject.SetActive(true);
            clothe.parent.GetChild(lastIndex[indexParent]).GetChild(0).gameObject.SetActive(false);
            lastIndex[indexParent] = index;

            Transform parent = clothe.parent.parent.parent;
            Color[] colors = new Color[3];
            int length = colors.Length - 1;
            for (int i = 0; i <= length; i++)
            {
                Transform obj = parent.GetChild(i).GetChild(1).GetChild(lastIndex[i]);
                colors[i] = obj.GetComponent<Image>().color;
                manager.character.clothesColor[i] = new Web.Character.Color(colors[i].r, colors[i].g, colors[i].b, colors[i].a);
            }

            FindObjectOfType<Customization>().ChangeClotheColor(colors);
        }
    }
}