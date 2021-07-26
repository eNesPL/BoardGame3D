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
        MovePawn(1, playerTurn, dice);
        ChangeTurn();
    }


}
