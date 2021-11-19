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
                Dispatcher.Invoke(() => Starter());
        }


    }
    void Starter()
    {
        var JsonReply = client.ReplyHandler();
        Debug.Log(JsonReply);
        SC.SetJson(JsonReply);
    }
    public void AfterSceneChane()
    {
       PC = GameObject.Find("PlayersController").GetComponent<PlayersController>();
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
   public void SpawnOrMoveQuestion(List<GameObject> spawnedpawns, int dice)
    {
        var t = new Thread(() => { SpawnOrMoveQuestion_r(spawnedpawns,  dice); });
        TC.Threads.Add(t);
        t.Start();
    }
    void SpawnOrMoveQuestion_r(List<GameObject> spawnedpawns, int dice)
    {
        string option = client.SendCommand("SpawnOrMove");
        var JsonReply = JObject.Parse(option);
        option = JsonReply["Option"].ToString();
        Debug.Log(option);
        if (option == "spawn") { Dispatcher.Invoke(() => PC.SpawnPawn()); }
        if (option == "move") { PC.MovablePawnsHandler(spawnedpawns, dice); }
    }
    public void SendQuestionMovablePawns(int dice, string command)
    {
        Debug.LogError(command+" "+dice);
        SendCommand(command, true, SendQuestionMovablePawns_continue, dice);
    }

    public void SendQuestionMovablePawns_continue(JObject JsonReply)
    {
        Debug.LogError(JsonReply);
        int selected = int.Parse(JsonReply["MovePawn"].ToString());
        int dice = int.Parse(JsonReply["dice"].ToString());
        Dispatcher.Invoke(() => PC.MovePawn(selected, PC.GetPlayerTurn(), dice));
        PC.EndTurn();
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
    public void SendCommand(string cmd, bool hasReturn, Action<JObject> func, int dice)
    {
        
        var t = new Thread(() => { client.CommandHandler(new Cmd(cmd, hasReturn, func),dice); });
        TC.Threads.Add(t);
        t.Start();
    }

}