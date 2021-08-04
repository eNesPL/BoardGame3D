using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayersController : MonoBehaviour
{

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
    void Start()
    {
        CH = GameObject.FindWithTag("Client").GetComponent<ClientHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ChangeTurn()
    {
        playerTurn++;
        if (playerTurn > players)
        {
            playerTurn = 1;
        }
    }
    public void GetPawns()
    {
        
        List<GameObject>ListOfPawns = GameObject.FindGameObjectsWithTag("RedPawn").ToList();
        try
        {
            foreach (GameObject g in ListOfPawns)
            {
                redPawns.Add(g.GetComponent<PlayerController>().GetPawnNumber(), g);
            }
        }
        catch(Exception e)
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

    private Dictionary<int,GameObject> GetDict(int playerID)
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

    public void MakeTurn()
    {
        int dice = 0;
        dice = CH.getDice();
        if(dice == 6)
        {
            if (HaveUnSpawnedPawns(playerTurn)){
                int option = CH.SpawnOrMoveQuestion();
                if(option == 1)
                {
                    SpawnPawn();
                }
                else
                {
                    var spawnedpawns = GetSpawnedPawns();
                    var movablepawns = GetMovablePawns(spawnedpawns, dice);
                    CH.SendQuestionMovablePawns(movablepawns);
                }
            }
        }
        MovePawn(1, playerTurn, dice);
        ChangeTurn();
    }

    private object GetMovablePawns(List<GameObject> spawnedpawns, int dice)
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
            if (!pawn.GetComponent<PlayerController>().isOnSpawn())
            {
                listofpawns.Add(pawn);
            }
            
        }
        return pawn;
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
        for(int i = 1; i < 5; i++)
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
}
