using System;
using UnityEngine;
using UnityEngine.Networking;
using Zoo.Enemy;

namespace Zoo.Player
{
    public class Fighter : NetworkBehaviour
    {
        [SerializeField] float weaponRange = 2f;

        Transform target;

        [SyncVar]
        private NetworkInstanceId targetName = new NetworkInstanceId();

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
            target = enemy.transform;
            CmdAttack(enemy.GetComponent<NetworkIdentity>().netId);
        }

        [Command]
        void CmdAttack(NetworkInstanceId enemy)
        {
            targetName = enemy;
        }

        // Animation event
        void Hit()
        {
            if (targetName.ToString() == "0" || !isServer)
                return;

            try
            {
                NetworkServer.FindLocalObject(targetName).GetComponent<EnemyController>().TakeDamage(UnityEngine.Random.Range(5, 20));
            }
            catch(Exception e)
            {
                targetName = new NetworkInstanceId();
                Debug.Log(e.Message);
            }
        }

        public void Cancel()
        {
            target = null;
            CmdAttack(new NetworkInstanceId());
            GetComponent<Animator>().Play("Locomotion");
        }
    }
}

