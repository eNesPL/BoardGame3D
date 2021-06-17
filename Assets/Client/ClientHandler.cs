using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    // Start is called before the first frame update
    Client client = new Client();
    void Start()
    {
        ThreadController TC = GameObject.Find("ThreadController").GetComponent<ThreadController>();

        Thread t = new Thread(() => {
            try
            {
                client.FindServer(client);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });
        t.IsBackground=true;
        TC.Threads.Add(t);
        t.Start();

        Thread c = new Thread(() => {
            try
            {
                client.ConnectionTester(client);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });
        c.IsBackground = true;
        TC.Threads.Add(c);
        c.Start();
        
    }

    void OnApplicationQuit()
    {
#if UNITY_EDITOR
        var constructor = SynchronizationContext.Current.GetType().GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
        var newContext = constructor.Invoke(new object[] { Thread.CurrentThread.ManagedThreadId });
        SynchronizationContext.SetSynchronizationContext(newContext as SynchronizationContext);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getDice()
    {
        new Thread(() => { 
            Debug.Log(client.GetDice());
        }).Start();
        
    }
}
