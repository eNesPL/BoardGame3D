using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    ClientHandler CH;
    JObject JsonConfig;
    // Start is called before the first frame update
    void Start()
    {
        CH = GameObject.Find("ClientHandler").GetComponent<ClientHandler>();
    }
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }


    public JObject GetData()
    {
        return JsonConfig;
    }
    
    public void SetJson(JObject Json)
    {
        this.JsonConfig = Json;
        SceneManager.LoadScene(1);
    }
}
