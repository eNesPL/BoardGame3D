using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using CielaSpike;

public class PlayersController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI StatusText;
    Dictionary<int, GameObject> redPawns = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> bluePawns = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> greenPawns = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> yellowPawns = new Dictionary<int, GameObject>();
    // Start is called before the first frame update
    [SerializeField]
    private int players = 2;
    [SerializeField]
    private int playerTurn = 1;
    [SerializeField]
    int[] playerScore = new int[4] { 0, 0, 0, 0 };
    [SerializeField]
    ClientHandler CH;
    SceneChanger SC;
    ThreadController TC;
    int returnDice = 0;


    void Start()
    {
    }
    private void Awake()
    {
        TC = GameObject.Find("ThreadController").GetComponent<ThreadController>();
        SC = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
        CH = GameObject.Find("ClientHandler").GetComponent<ClientHandler>();
        GetPawns();
        StartGame(SC.GetData());
    }

    private void StartGame(JObject jObject)
    {
        Debug.Log(jObject);
        if (jObject["Type"].ToString() == "New") 
        {
            NewGame(int.Parse(jObject["Players"].ToString()));
        }
        if (jObject["Type"].ToString() == "Continue")
        {
            ContinueGame();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewGame(int Players)
    {
        this.players = Players;

        WaitForStartAsync(); 
    }

    private void WaitForStartAsync()
    {
        var reply = CH.WaitForStart();
        if (reply["Type"].ToString() == "start")
        {
            MakeTurn();

        }
    }

    public void ContinueGame()
    {
        var JsonReply = LoadFile();
        this.players = int.Parse(JsonReply["Players"].ToString());
        
        GameObject pawn;
        int tile;
        for (int i = 1; i < 5; i++)
        {
            for (int j = 1; j < 5; j++)
            {
                switch (i)
                {
                    case 1:
                        yellowPawns.TryGetValue(j, out pawn);
                        tile = int.Parse(JsonReply[i.ToString()][j.ToString()].ToString());
                        if (tile != 0)
                        {
                            pawn.GetComponent<PlayerController>().SetPosition(tile);
                        }
                        break;
                    case 2:
                        redPawns.TryGetValue(j, out pawn);
                        tile = int.Parse(JsonReply[i.ToString()][j.ToString()].ToString());
                        if (tile != 0)
                        {
                            pawn.GetComponent<PlayerController>().SetPosition(tile);
                        }
                        break;
                    case 3:
                        bluePawns.TryGetValue(j, out pawn);
                        tile = int.Parse(JsonReply[i.ToString()][j.ToString()].ToString());
                        if (tile != 0)
                        {
                            pawn.GetComponent<PlayerController>().SetPosition(tile);
                        }
                        break;
                    case 4:
                        greenPawns.TryGetValue(j, out pawn);
                        tile = int.Parse(JsonReply[i.ToString()][j.ToString()].ToString());
                        if (tile != 0)
                        {
                            pawn.GetComponent<PlayerController>().SetPosition(tile);
                        }
                        break;
                }
            }
        }

    }
    private void ChangeTurn()
    {
        this.playerTurn++;
        if (this.playerTurn > this.players)
        {
            this.playerTurn = 1;
        }
        StatusText.text = "Player: " + this.playerTurn;
        StartTurn();
    }
    public void GetPawns()
    {

        List<GameObject> ListOfPawns = GameObject.FindGameObjectsWithTag("RedPawn").ToList();
        try
        {
            foreach (GameObject g in ListOfPawns)
            {
                redPawns.Add(g.GetComponent<PlayerController>().GetPawnNumber(), g);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        ListOfPawns = new List<GameObject>();
        ListOfPawns = GameObject.FindGameObjectsWithTag("BluePawn").ToList();
        try
        {
            foreach (GameObject g in ListOfPawns)
            {
                bluePawns.Add(g.GetComponent<PlayerController>().GetPawnNumber(), g);
            }
        }
        catch
        {
            Debug.Log("No Blue Players");
        }
        ListOfPawns = new List<GameObject>();
        ListOfPawns = GameObject.FindGameObjectsWithTag("GreenPawn").ToList();
        try
        {
            foreach (GameObject g in ListOfPawns)
            {
                greenPawns.Add(g.GetComponent<PlayerController>().GetPawnNumber(), g);
            }
        }
        catch
        {
            Debug.Log("No Green Players");
        }

        ListOfPawns = GameObject.FindGameObjectsWithTag("YellowPawn").ToList();
        try
        {
            foreach (GameObject g in ListOfPawns)
            {
                yellowPawns.Add(g.GetComponent<PlayerController>().GetPawnNumber(), g);
            }
        }
        catch
        {
            Debug.Log("No Yellow Players");
        }
    }

    private Dictionary<int, GameObject> GetDict(int playerID)
    {
        switch (playerID)
        {
            case 1:
                return yellowPawns;
            case 2:
                return redPawns;
            case 3:
                return bluePawns;
            case 4:
                return greenPawns;
            default:
                return null;
        }
    }

    private void MovePawn(int pawnID, int playerID, int diceRoll)
    {
        Dictionary<int, GameObject> pawns = GetDict(playerID);
        GameObject pawn;
        pawns.TryGetValue(pawnID, out pawn);
        pawn.GetComponent<PlayerController>().MovePawn(diceRoll);
    }


    void StartTurn()
    {
        CH.getDice();
    }
    public void MakeTurn(DiceData diceData)
{
        int dice = diceData.dice;
        if (dice == 6)
        {
            if (HaveUnSpawnedPawns(playerTurn))
            {
                var spawnedpawns = GetSpawnedPawns();
                Debug.Log(spawnedpawns.Count);
                if (spawnedpawns.Count > 0) { 
                    int option = CH.SpawnOrMoveQuestion();
                    if (option == 1)
                    {
                        SpawnPawn();
                    }
                    else
                    {
                        var movablepawns = GetMovablePawns(spawnedpawns, dice);
                        int selected = CH.SendQuestionMovablePawns(movablepawns);
                        MovePawn(selected, playerTurn, dice);
                    }
                }
                else
                {
                    SpawnPawn();
                }
            }
        }
        else
        {
            var spawnedpawns = GetSpawnedPawns();
            if (spawnedpawns.Count > 0)
            { 
                var movablepawns = GetMovablePawns(spawnedpawns, dice);
                if (movablepawns.Count > 0)
                {
                    int selected = CH.SendQuestionMovablePawns(movablepawns);
                    MovePawn(selected, playerTurn, dice);
                }
            }
        }
        ChangeTurn();
    }

    private List<GameObject> GetMovablePawns(List<GameObject> spawnedpawns, int dice)
    {
        var MovablePawns = new List<GameObject>();
        for (int i = 0; i < spawnedpawns.Count; i++)
        {
            GameObject pawn = spawnedpawns[i];
            if (pawn.GetComponent<PlayerController>().CanIMove(dice))
            {
                MovablePawns.Add(pawn);
            }

        }
        return MovablePawns;
    }

    private List<GameObject> GetSpawnedPawns()
    {
        
        var listofpawns = new List<GameObject>();
        GameObject pawn;
        Dictionary<int, GameObject> Pawns = new Dictionary<int, GameObject>();
        switch (playerTurn)
        {
            case 1:
                Pawns = yellowPawns;
                break;
            case 2:
                Pawns = redPawns;
                break;
            case 3:
                Pawns = bluePawns;
                break;
            case 4:
                Pawns = greenPawns;
                break;
        }
        for (int i = 1; i < 5; i++)
        {
            Pawns.TryGetValue(i, out pawn);
            bool stan = pawn.GetComponent<PlayerController>().isOnSpawn();
            if (stan == true)
            {
                listofpawns.Add(pawn);
            }

        }
        return listofpawns;
    }

    private void SpawnPawn()
    {
        Dictionary<int, GameObject> Pawns = new Dictionary<int, GameObject>();
        switch (playerTurn)
        {
            case 1:
                Pawns = yellowPawns;
                break;
            case 2:
                Pawns = redPawns;
                break;
            case 3:
                Pawns = bluePawns;
                break;
            case 4:
                Pawns = greenPawns;
                break;
        }
        for (int i = 1; i < 5; i++)
        {
            GameObject pawn;
            Pawns.TryGetValue(1, out pawn);
            var pwn = pawn.GetComponent<PlayerController>();
            if (pwn.isOnSpawn())
            {
                pwn.Spawn();
                break;
            }
        }
    }

    private bool HaveUnSpawnedPawns(int playerTurn)
    {
       
        Dictionary<int, GameObject> Pawns = new Dictionary<int, GameObject>();
        switch (playerTurn)
        {
            case 1:
                Pawns = yellowPawns;
                break;
            case 2:
                Pawns = redPawns;
                break;
            case 3:
                Pawns = bluePawns;
                break;
            case 4:
                Pawns = greenPawns;
                break;
        }
        for (int i = 1; i < 5; i++)
        {
            GameObject pawn;
            Pawns.TryGetValue(1, out pawn);
            if (pawn.GetComponent<PlayerController>().isOnSpawn())
            {
                return true;
            }
        }
        return false;
    }

    public void SaveFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;
        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);
        file.Close();
        string data = "{ \"Players\": " + players + ",";
        int tile;
        GameObject pawn;
        
        for (int i = 1; i < 5; i++)
        {
            data = data + "\"" + i + "\":{";
            for (int j = 1; j < 5; j++)
            {

                switch (i)
                {
                    case 1:
                        yellowPawns.TryGetValue(j, out pawn);
                        tile = pawn.GetComponent<PlayerController>().GetActualTile();
                        data = data + "\"" + j + "\":" + tile;
                        break;
                    case 2:
                        redPawns.TryGetValue(j, out pawn);
                        tile = pawn.GetComponent<PlayerController>().GetActualTile();
                        data = data + "\"" + j + "\":" + tile;
                        break;
                    case 3:
                        bluePawns.TryGetValue(j, out pawn);
                        tile = pawn.GetComponent<PlayerController>().GetActualTile();
                        data = data + "\"" + j + "\":" + tile;
                        break;
                    case 4:
                        greenPawns.TryGetValue(j, out pawn);
                        tile = pawn.GetComponent<PlayerController>().GetActualTile();
                        data = data + "\"" + j + "\":" + tile;
                        break;
                }
                if (j != 4)
                {
                    data = data + ",";
                }
            }
            if (i != 4)
            {
                data = data + "},";
            }
            else
            {
                data = data + "}";
            }
        }
        data = data + "}";
        StreamWriter writer = new StreamWriter(destination, true);
        writer.WriteLine(data);
        writer.Close();
        Debug.Log(data);
    }

    public JObject LoadFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return null;
        }
        BinaryFormatter bf = new BinaryFormatter();
        StreamReader reader = new StreamReader(destination);
        string data = reader.ReadLine();
        var JsonData = JObject.Parse(data);
        file.Close();
        return JsonData;
    }
}
