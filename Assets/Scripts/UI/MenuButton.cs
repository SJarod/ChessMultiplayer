using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    private string IP = "127.0.0.1";
    public void CreateServerFor(string gameScene)
    {
        GameObject goS = new GameObject("Server");
        DontDestroyOnLoad(goS);
        Server sv = goS.AddComponent<Server>();
        sv.gameScene = gameScene;
        sv.Startup();

        CreateClient("127.0.0.1");
    }

    public void CreateClient(string ip)
    {
        GameObject go = new GameObject("Client");
        DontDestroyOnLoad(go);
        Client cl = go.AddComponent<Client>();
        cl.ConnectTo(ip);
    }

    public void CreateClient()
    {
        GameObject go = new GameObject("Client");
        DontDestroyOnLoad(go);
        Client cl = go.AddComponent<Client>();
        cl.ConnectTo(IP);
    }

    public void RenameIP(string ip) => IP = ip;
}
