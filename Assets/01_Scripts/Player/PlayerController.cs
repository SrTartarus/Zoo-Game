using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using Zoo.Core;

namespace Zoo.Player
{
    public class PlayerController : NetworkBehaviour
    {

        // Use this for initialization
        void Start()
        {
            if (isLocalPlayer)
            {

                Camera.main.GetComponent<FollowCamera>().player = gameObject;
                Camera.main.transform.SetParent(null);
                Camera.main.GetComponent<FollowCamera>().offset = Camera.main.transform.position - transform.position;
            }
            else
            {
                Destroy(transform.GetChild(2).gameObject);
                Destroy(this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!isLocalPlayer)
                return;

            UpdateAnimator();

            //if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            if (Physics.Raycast(GetMouseRay(), out hit))
            {
                if (Input.GetMouseButton(0))
                {
                    if (hit.collider != null && hit.transform.name == "terrain")
                    {
                        //GetComponent<Fighter>().Cancel();
                        MoveTo(hit.point);
                        return true;
                    }
                }
            }

            return false;
        }

        /*
        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (var hit in hits)
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                if (enemy == null) continue;

                if (Input.GetMouseButtonDown(0))
                {
                    GetComponent<Fighter>().Attack(enemy);
                    return true;
                }
            }

            return false;
        }
        */

        public void MoveTo(Vector3 position)
        {
            GetComponent<NavMeshAgent>().destination = position;
            GetComponent<NavMeshAgent>().isStopped = false;
        }

        public void Stop()
        {
            GetComponent<NavMeshAgent>().isStopped = true;
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void UpdateAnimator()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(GetComponent<NavMeshAgent>().velocity);
            GetComponent<Animator>().SetFloat("speed", localVelocity.z);
        }
    }
}

