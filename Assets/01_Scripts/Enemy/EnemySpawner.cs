using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Zoo.Enemy
{
    public class EnemySpawner : NetworkBehaviour
    {

        [SerializeField] private GameObject enemyPrefab;
        private List<GameObject> spots = new List<GameObject>();

        public int numberOfEnemies = 3;
        private float time;

        void Start()
        {
            if (!isServer)
                return;

            int length = transform.childCount;
            for (int i = 0; i < length; i++)
            {
                spots.Add(transform.GetChild(i).gameObject);
            }

            time = Random.Range(3, 6);
        }

        void Update()
        {
            if (!isServer)
                return;

            if(numberOfEnemies > 0)
            {
                time -= Time.deltaTime;
                if(time <= 0f)
                {
                    time = Random.Range(3, 6);
                    SpawnEnemy();
                }
            }
        }

        void SpawnEnemy()
        {
            numberOfEnemies -= 1;
            int randomInt = Random.Range(0, transform.childCount);
            GameObject go = GameObject.Instantiate(enemyPrefab);
            go.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(spots[randomInt].transform.position);
            NetworkServer.Spawn(go);
        }
    }
}

