using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    public string scene;
    public void ChangeScene()
    {
        SceneManager.LoadScene(scene);
    }
}
