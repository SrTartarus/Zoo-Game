using UnityEngine;
using UnityEngine.UI;

namespace Zoo.Customization
{
    public class MenuHead : MonoBehaviour
    {
        [SerializeField] private Customization manager;

        private int lastIndex = 0;

        private Color32 select, unselect;

        // Start is called before the first frame update
        void Start()
        {
            select = new Color32(200, 200, 200, 255);
            unselect = new Color32(255, 255, 255, 255);
        }

        public void SelectHead(Transform head)
        {
            int index = head.GetSiblingIndex();
            if (index == lastIndex)
                return;

            head.GetComponent<Image>().color = select;
            head.parent.GetChild(lastIndex).GetComponent<Image>().color = unselect;
            FindObjectOfType<Customization>().ChangeBody(index, lastIndex);
            head.transform.GetChild(1).gameObject.SetActive(true);
            head.parent.GetChild(lastIndex).GetChild(1).gameObject.SetActive(false);
            lastIndex = index;
            manager.character.indexHead = lastIndex;
        }
    }
}