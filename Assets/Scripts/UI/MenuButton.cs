using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public GameObject serverPfb;
    public GameObject clientPfb;
    public string IP;

    public void CreateServer()
    {
        Instantiate<GameObject>(serverPfb);
    }

    public void CreateClient()
    {
        Instantiate<GameObject>(clientPfb);
        clientPfb.GetComponent<Client>().SetIP(IP);
    }
}
