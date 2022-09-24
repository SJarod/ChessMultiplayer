using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking;
using Serialization;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class EmoteButton : MonoBehaviour
{
    public GameObject prefab;
    public Transform prefabPos;

    private static GameObject p_prefab;
    private static Transform p_prefabPos;

    private void Start()
    {
        p_prefab = prefab;
        p_prefabPos = prefabPos;
    }

    public static void StartEmote()
    {
        EmoteInfo info = new EmoteInfo();
        info.id = 1;
        Client client = FindObjectOfType<Client>();
        client.socket.SendPackageOfType(PackageType.EMOTEINFO, Serializer.ObjectToByteArray(info));
    }

    public static void CreateEmote(int id)
    {
        if(id == 1)
        {
            GameObject Go = Instantiate<GameObject>(p_prefab);
            Go.transform.position = p_prefabPos.position;
        }
    }
}

[Serializable]
public class EmoteInfo
{
    public int id = 0;
}
