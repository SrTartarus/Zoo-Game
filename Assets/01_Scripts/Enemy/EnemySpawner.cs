using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Zoo.Enemy
{
    public class EnemySpawner : NetworkBehaviour
    {

        [SerializeField] private GameObject enemyPrefab;
        private List<GameObject> spots = new List<GameObject>();

        private int numberOfEnemies = 1;

        public override void OnStartServer()
        {
            int length = transform.childCount;
            for (int i = 0; i < length; i++)
            {
                spots.Add(transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < numberOfEnemies; i++)
            {
                SpawnEnemies();
            }
        }

        void SpawnEnemies()
        {
            int randomInt = Random.Range(0, transform.childCount);
            GameObject go = GameObject.Instantiate(enemyPrefab);
            go.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(spots[randomInt].transform.position);
            NetworkServer.Spawn(go);
        }
    }
}

