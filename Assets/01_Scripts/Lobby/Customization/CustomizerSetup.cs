using System.Collections.Generic;
using UnityEngine;

namespace Zoo.Customization
{
    public class CustomizerSetup : MonoBehaviour
    {
        public Color skinColor;
        public List<Color> clothesColor = new List<Color>();
        public int headIndex;

        // Start is called before the first frame update
        void Start()
        {
            Transform heads = transform.Find("model:geo").Find("Heads");
            Transform body = transform.Find("model:geo").Find("Body");
            Transform clothes = transform.Find("model:geo").Find("Clothes");
            heads.GetChild(headIndex).gameObject.SetActive(true);
            Material headMaterial = Instantiate(heads.GetChild(headIndex).GetComponent<SkinnedMeshRenderer>().material);
            headMaterial.SetColor("_Color", skinColor);
            Material bodyMaterial = Instantiate(body.GetChild(0).GetComponent<SkinnedMeshRenderer>().material);
            bodyMaterial.SetColor("_Color", skinColor);
            Material clotheMaterial = Instantiate(clothes.GetChild(0).GetComponent<SkinnedMeshRenderer>().material);
            clotheMaterial.SetColor("_ColorMangas", clothesColor[0]);
            clotheMaterial.SetColor("_ColorShort", clothesColor[1]);
            clotheMaterial.SetColor("_ColorEspalda", clothesColor[2]);

            // Set Material
            heads.GetChild(headIndex).GetComponent<SkinnedMeshRenderer>().material = headMaterial;
            int length = body.childCount;
            for(int i = 0; i < length; i++)
            {
                body.GetChild(i).GetComponent<SkinnedMeshRenderer>().material = bodyMaterial;
            }

            length = clothes.childCount;
            for(int i = 0; i < length; i++)
            {
                clothes.GetChild(i).GetComponent<SkinnedMeshRenderer>().material = clotheMaterial;
            }
        }
    }
}

