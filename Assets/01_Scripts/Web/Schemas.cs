using System;
using System.Collections.Generic;
using MongoDB.Bson;
using UnityEngine.Networking.Types;

namespace Zoo.Web
{
    public class User
    {
        public ObjectId _id;

        public string username { set; get; }
        public string email { set; get; }
        public string password { set; get; }
        public string token { set; get; }
        public DateTime lastLogin { set; get; }
    }

    public class Character
    {
        public ObjectId _id;

        public User user { set; get; }
        public string name { set; get; }
        public int indexHead { set; get; }
        public Color skin { set; get; }
        public List<Color> clothesColor { set; get; }

        public class Color
        {
            public Color(float r, float g, float b, float a)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }

            public float r;
            public float g;
            public float b;
            public float a;
        }
    }

    public class Server
    {
        public ObjectId _id;

        public User user { set; get; }
        public string address { set; get; }
        public int port { set; get; }
        public NetworkID networkId { set; get; }
        public string matchName { set; get; }
        public string platform { set; get; }
        public int numPlayers { set; get; }
        public int maxPlayers { set; get; }
    }
}