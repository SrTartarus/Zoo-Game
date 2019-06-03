using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using Zoo.Network;

namespace Zoo.Player
{
    public class SetupPlayer : NetworkBehaviour
    {
        private struct playerStruct
        {
            public int indexHead;
            public Color skinColor;
            public Color[] clothesColor;
            public string username;
        }

        [SyncVar]
        private playerStruct player;

        // Use this for initialization
        void Start()
        {
            if (!isLocalPlayer)
            {
                GetComponent<CapsuleCollider>().isTrigger = true;
                Destroy(GetComponent<NavMeshAgent>());
                //Destroy(GetComponent<Fighter>());
                Setup(player);
            }
            else
            {
                transform.Find("name").gameObject.SetActive(false);
                playerStruct player = new playerStruct();
                player.indexHead = MultiplayerManager.character.indexHead;
                Web.Character.Color skin = MultiplayerManager.character.skin;
                player.skinColor = new Color(skin.r, skin.g, skin.b, skin.a);
                int length = MultiplayerManager.character.clothesColor.Count;
                player.clothesColor = new Color[length];
                for (int i = 0; i < length; i++)
                {
                    Web.Character.Color clothes = MultiplayerManager.character.clothesColor[i];
                    player.clothesColor[i] = new Color(clothes.r, clothes.g, clothes.b, clothes.a);
                }

                player.username = MultiplayerManager.character.name;

                CmdPlayerChanged(player);
            }
        }

        [Command]
        private void CmdPlayerChanged(playerStruct player)
        {
            this.player = player;
            RpcPlayerChanged(player);
        }

        [ClientRpc]
        private void RpcPlayerChanged(playerStruct player)
        {
            Setup(player);
        }

        private void Setup(playerStruct player)
        {
            gameObject.name = netId.ToString();
            Transform localPlayer = transform.Find("model:geo");
            Transform headTransform = localPlayer.Find("Heads");
            Transform bodyTransform = localPlayer.Find("Body");
            Transform clothesTransform = localPlayer.Find("Clothes");
            transform.Find("name").GetComponent<TextMesh>().text = player.username;
            headTransform.GetChild(0).gameObject.SetActive(false);
            headTransform.GetChild(player.indexHead).gameObject.SetActive(true);
            Material headMaterial = Instantiate(headTransform.GetChild(player.indexHead).GetComponent<SkinnedMeshRenderer>().material);
            Material bodyMaterial = Instantiate(bodyTransform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material);
            Material clothesMaterial = Instantiate(clothesTransform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material);
            headMaterial.SetColor("_Color", player.skinColor);
            bodyMaterial.SetColor("_Color", player.skinColor);

            if (player.clothesColor != null && player.clothesColor.Length >= 2)
            {
                clothesMaterial.SetColor("_ColorMangas", player.clothesColor[0]);
                clothesMaterial.SetColor("_ColorShort", player.clothesColor[1]);
                clothesMaterial.SetColor("_ColorEspalda", player.clothesColor[2]);
            }

            headTransform.GetChild(player.indexHead).GetComponent<SkinnedMeshRenderer>().material = headMaterial;

            int length = bodyTransform.childCount;
            for (int i = 0; i < length; i++)
            {
                bodyTransform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material = bodyMaterial;
            }

            length = clothesTransform.childCount;
            for (int i = 0; i < length; i++)
            {
                clothesTransform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material = clothesMaterial;
            }
        }
    }
}

