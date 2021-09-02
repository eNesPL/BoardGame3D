using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityToolbag;

public class ClientHandler : MonoBehaviour
{
    // Start is called before the first frame update
    Client client = new Client();
    
    PlayersController PC;
    ThreadController TC;
    SceneChanger SC;
   

    private void Awake()
    {
        
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

    internal void AddOnGetDiceValue(Action<DiceData> makeTurn)
    {
        client.OnGetDiceValue += makeTurn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getDice()
    {
        var t = new Thread(() => { client.GetDice(); });
        TC.Threads.Add(t);
        t.Start();
        
    }

    public void setWaiting()
    {
        SendCommand("WaitForEnd");
    }
   public int SpawnOrMoveQuestion()
    {       
            string option = client.SendCommand("SpawnOrMove");
            var JsonReply = JObject.Parse(option);
            option = JsonReply["Option"].ToString();
             Debug.Log(option);
            if (option == "spawn") { return 1; }
            if (option == "move") { return 2; }
            return 2;
    }

    internal int SendQuestionMovablePawns(List<GameObject> movablepawns)
    {
        string command = "MoveQuestion;";
        foreach (var pawn in movablepawns)
        {
            var r = pawn.GetComponent<PlayerController>().GetPawnNumber();
            command = command + r + ";";
        }
        string reply = client.SendCommand(command);
        var JsonReply = JObject.Parse(reply);
        return int.Parse(JsonReply["MovePawn"].ToString());
    }

    public JObject WaitForStart()
    {
            JObject reply = client.ReplyHandler();
        return reply;

    }

    public void SendCommand(string cmd, Action func)
    {
        var t = new Thread(() => { client.CommandHandler(new Cmd(cmd,false, func)); });
        TC.Threads.Add(t);
        t.Start();
    }

    public void SendCommand(string cmd,bool hasReturn, Action<JObject> func)
    {
        var t = new Thread(() => { client.CommandHandler(new Cmd(cmd, hasReturn, func)); });
        TC.Threads.Add(t);
        t.Start();
    }

    public void SendCommand(string cmd)
    {
        var t = new Thread(() => { client.CommandHandler(new Cmd(cmd)); });
        TC.Threads.Add(t);
        t.Start();
    }

}