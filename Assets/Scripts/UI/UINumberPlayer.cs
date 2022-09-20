using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINumberPlayer : MonoBehaviour
{
    public TextMeshProUGUI nbPlayer;

    private void Update()
    {
        nbPlayer.text = Server.GetNbPlayers().ToString();
    }
}
