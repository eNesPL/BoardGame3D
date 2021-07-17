using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;

public class RestApiController : MonoBehaviour
{
    // Start is called before the first frame update
    string ip;
    void Start()
    {
    }
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Connect()
    {
        this.ip = GameObject.Find("Text").GetComponent<Text>().text;
        Debug.Log(SendGet("getConnection"));
    }

    string SendRequest(string requestLink)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestLink);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        Debug.Log(jsonResponse);
        return jsonResponse;
    }
    
    string SendGet(string request)
    {
        string ret="";
        Task taskA = new Task(() => ret = SendRequest("http://" + this.ip + ":8000" + "/" + request));
        taskA.Start();
        taskA.Wait();
        return ret;
        
    }
}
