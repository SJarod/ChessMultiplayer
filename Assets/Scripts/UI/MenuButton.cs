using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void CreateServer()
    {
        GameObject goS = new GameObject("Server");
        DontDestroyOnLoad(goS);
        Server sv = goS.AddComponent<Server>();
        sv.Startup();

        CreateClient("127.0.0.1");
    }

    public void CreateClient(string ip)
    {
        GameObject go = new GameObject("Client");
        DontDestroyOnLoad(go);
        Client cl = go.AddComponent<Client>();
        cl.Connect(ip);
    }
}
