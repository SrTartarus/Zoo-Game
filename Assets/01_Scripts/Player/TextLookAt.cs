using UnityEngine;

namespace Zoo.Player
{
    public class TextLookAt : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}

