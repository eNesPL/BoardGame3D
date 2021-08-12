using Newtonsoft.Json.Linq;
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
    
    PlayersController PC;
    ThreadController TC;
    SceneChanger SC;
    private JObject cmd = null;
    string ReturnValue;
    string SendingCommand;

    private void Awake()
    {
        client.OnGetDiceValue += PC.MakeTurn;
        DontDestroyOnLoad(this.gameObject);
    }

    private void StartAsync()
    {
            
            bool status = client.FindServer(client);
            if (status == true)
            {
                Debug.Log("connected");
                var JsonReply = client.ReplyHandler();
                Debug.Log(JsonReply);
                SC.SetJson(JsonReply);
             }
        

    }
    private void Start()
    {
        SC = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
        TC = GameObject.Find("ThreadController").GetComponent<ThreadController>();

        Thread SocketHandler = new Thread(StartAsync);
        SocketHandler.Start();
        TC.Threads.Add(SocketHandler);
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
           return client.GetDice();
    }


   public int SpawnOrMoveQuestion()
    {       
            string option = client.SendCommand("SpawnOrMove");
            if (option == "spawn") { return 1; }
            if (option == "move") { return 2; }
            return 2;
    }

    internal int SendQuestionMovablePawns(List<GameObject> movablepawns)
    {
        string command = "";
        foreach(var pawn in movablepawns)
        {
            var r = pawn.GetComponent<PlayerController>().GetPawnNumber();
            command = command + r + ";";
        }
        string reply = client.SendCommand(command);
        return int.Parse(reply);
    }

    public JObject WaitForStart()
    {
            JObject reply = client.ReplyHandler();
        return reply;

    }


}
