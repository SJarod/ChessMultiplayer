using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emote : MonoBehaviour
{
    public float time = 2f;

    private void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
            Destroy(this.gameObject);
    }
}
