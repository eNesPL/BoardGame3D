using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    // Start is called before the first frame update
    Client client = new Client();
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    async Task StartAsync()
    {
        ThreadController TC = GameObject.Find("ThreadController").GetComponent<ThreadController>();

        bool status = await Task.Run(() => {
            return client.FindServer(client);
        });
        Task.WaitAll();
        if (status == true)
        {
            Debug.Log("connected");
            Thread T = new Thread(() =>
            {
                client.ReplyHandler();
            });
            TC.Threads.Add(T);
            T.Start();
        }
    }
    private void Start()
    {
        StartAsync();
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

    public int getDice()
    {
        var dicereader = Task<int>.Run(() => {
           return client.GetDice();
        });
        return dicereader.Result;
    }

    public void SendTest()
    {
        new Thread(() => {
            client.Test();
        }).Start();
    }

   public int SpawnOrMoveQuestion()
    {
        // TODO:Spawn Or move Send Question to pi
        throw new NotImplementedException();
    }

    internal void SendQuestionMovablePawns(object movablepawns)
    {
        // TODO: SendQuestion Movalbe pawns to pi
        throw new NotImplementedException();
    }
}
