using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinnerPrinter : MonoBehaviour
{
    SceneChanger SC;


    // Start is called before the first frame update
    TextMeshProUGUI player;
    void Start()
    {
        SC = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
        player = GameObject.Find("Player").GetComponent<TextMeshProUGUI>();
        Debug.Log(SC.winner.ToString());
        player.text = ""+SC.winner.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
