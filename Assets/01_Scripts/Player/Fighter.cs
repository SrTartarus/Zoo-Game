using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Zoo.Enemy;

namespace Zoo.Player
{
    public class Fighter : NetworkBehaviour
    {
        [SerializeField] float weaponRange = 2f;

        Transform target;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!isLocalPlayer)
                return;

            if (target != null)
            {
                if (!IsInRange())
                {
                    GetComponent<PlayerController>().MoveTo(target.position);
                }
                else
                {
                    GetComponent<PlayerController>().Stop();
                    GetComponent<NetworkAnimator>().SetTrigger("attack");
                    transform.LookAt(target);
                }
            }
        }

        private bool IsInRange()
        {
            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public void Attack(EnemyController enemy)
        {
            if (!isLocalPlayer)
                return;

            target = enemy.transform;
        }

        // Animation event
        void Hit()
        {
            if (target == null || !isLocalPlayer)
                return;

            target.GetComponent<EnemyController>().CmdReduceLife();
        }

        public void Cancel()
        {
            target = null;
            GetComponent<Animator>().Play("Locomotion");
        }
    }
}

