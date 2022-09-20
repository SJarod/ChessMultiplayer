using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public GameObject serverPfb;

    public void CreateServer()
    {
        GameObject.Instantiate<GameObject>(serverPfb);
    }
}
