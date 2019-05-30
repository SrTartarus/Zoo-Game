using UnityEngine;
using Zoo.Web;

namespace Zoo.Network
{
    public class WebManager : MonoBehaviour
    {
        // Singleton
        private static WebManager instance;
        public static WebManager singleton { get { return instance; } }

        public Mongo db;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            db = new Mongo("mongodb://srtartarus:root@rpgcluster-shard-00-00-v224p.mongodb.net:27017,rpgcluster-shard-00-01-v224p.mongodb.net:27017,rpgcluster-shard-00-02-v224p.mongodb.net:27017/test?ssl=true&replicaSet=RPGCluster-shard-0&authSource=admin&retryWrites=true", "db_rpg");

        }

        private void OnDisable()
        {
            db.Shutdown();
        }
    }
}