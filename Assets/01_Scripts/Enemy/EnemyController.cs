using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using Zoo.Core;

namespace Zoo.Enemy
{
    public class EnemyController : NetworkBehaviour
    {
        private Material outline;

        private Transform targetTransform;
        private LayerMask raycastLayer;
        private float radius = 10f;
        public float weaponRange = 2f;

        // Use this for initialization
        void Start()
        {
            outline = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
            raycastLayer = 1 << LayerMask.NameToLayer("Player");
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAnimator();
        }

        void FixedUpdate()
        {
            SearchForTarget();
            MoveToTarget();
        }

        void SearchForTarget()
        {
            if (!isServer)
                return;

            if (targetTransform == null)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, raycastLayer);

                if (hitColliders.Length > 0)
                {
                    int randomInt = Random.Range(0, hitColliders.Length);
                    GetComponent<NavMeshAgent>().isStopped = false;
                    targetTransform = hitColliders[randomInt].transform;
                }
            }
            else
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, raycastLayer);

                if (hitColliders.Length <= 0)
                {
                    GetComponent<NavMeshAgent>().isStopped = true;
                    targetTransform = null;
                }
            }
        }

        void MoveToTarget()
        {
            if (targetTransform != null && isServer)
            {
                bool bRange = Vector3.Distance(transform.position, targetTransform.position) < weaponRange;
                if (!bRange)
                {
                    GetComponent<NavMeshAgent>().isStopped = false;
                    GetComponent<NavMeshAgent>().destination = targetTransform.position;
                }
                else
                {
                    GetComponent<NavMeshAgent>().isStopped = true;
                }
            }
        }

        void UpdateAnimator()
        {
            if (!isServer)
                return;

            Vector3 localVelocity = transform.InverseTransformDirection(GetComponent<NavMeshAgent>().velocity);
            GetComponent<Animator>().SetFloat("speed", localVelocity.z);
        }

        private void OnMouseOver()
        {
            Cursor.SetCursor(FindObjectOfType<GameManager>().onEnemy, Vector2.zero, CursorMode.ForceSoftware);
            outline.SetColor("_ASEOutlineColor", Color.red);
        }

        private void OnMouseExit()
        {
            Cursor.SetCursor(FindObjectOfType<GameManager>().exitEnemy, Vector2.zero, CursorMode.ForceSoftware);
            outline.SetColor("_ASEOutlineColor", Color.black);
        }
    }
}