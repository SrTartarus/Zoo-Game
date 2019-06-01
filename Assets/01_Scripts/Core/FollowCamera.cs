using UnityEngine;

namespace Zoo.Core
{
    public class FollowCamera : MonoBehaviour
    {
        public GameObject player;
        public Vector3 offset;

        // Update is called once per frame
        void LateUpdate()
        {
            if (player != null)
                transform.position = player.transform.position + offset;
        }
    }
}

