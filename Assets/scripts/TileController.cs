using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    List<GameObject> ListOfTiles = new List<GameObject>();
    List<GameObject> ListOfRedWinningTiles = new List<GameObject>();
    List<GameObject> ListOfGreenWinningTiles = new List<GameObject>();
    List<GameObject> ListOfBlueWinningTiles = new List<GameObject>();
    List<GameObject> ListOfYellowWinningTiles = new List<GameObject>();
    List<List<GameObject>> ListOfListsOfWinningTiles = new List<List<GameObject>>();
    private Dictionary<int, GameObject> Tiles = new Dictionary<int, GameObject>();
    [SerializeField]
    bool loading = true;
    void Awake()
    {
        ListOfTiles = GameObject.FindGameObjectsWithTag("Tile").ToList();
        foreach (GameObject g in ListOfTiles)
        {
            Tiles.Add(g.GetComponent<Tile>().GetID(), g);
        }
        GetWinningTiles();

    }

    public GameObject GetTile(int TileID)
    {
        Debug.Log("loading");
        GameObject te;
        Tiles.TryGetValue(TileID, out te);
        return te;
    }

    public int GetEndingTile(int PlayerID)
    {
        int TileID = 0;
        if (PlayerID == 1) TileID = 40;
        if (PlayerID == 2) TileID = 10;
        if (PlayerID == 3) TileID = 20;
        if (PlayerID == 4) TileID = 30;
        Debug.Log(TileID);
        return TileID;
    }
    public int GetStartingTile(int PlayerID)
    {
        int TileID = 0;
        if (PlayerID == 1) TileID = 1;
        if (PlayerID == 2) TileID = 11;
        if (PlayerID == 3) TileID = 21;
        if (PlayerID == 4) TileID = 31;
        Debug.Log(TileID);
        return TileID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal bool isOnWinning(int playerID, int tileID)
    {
        if(tileID > 50 * playerID && tileID <= 50 * playerID + 4)
        {
            return true;
        }
        return false;
    }
    private void GetWinningTiles()
    {
        ListOfYellowWinningTiles = getWinningTilesPerPlayer(1);
        ListOfRedWinningTiles = getWinningTilesPerPlayer(2);
        ListOfBlueWinningTiles = getWinningTilesPerPlayer(3);
        ListOfGreenWinningTiles = getWinningTilesPerPlayer(4);
        ListOfListsOfWinningTiles.Add(ListOfYellowWinningTiles);
        ListOfListsOfWinningTiles.Add(ListOfRedWinningTiles);
        ListOfListsOfWinningTiles.Add(ListOfBlueWinningTiles);
        ListOfListsOfWinningTiles.Add(ListOfGreenWinningTiles);
    }
    public int CheckWinningStatus()
    {
        int playerid = 1;
        int status = 0;
        foreach(var listOfWinningTiles in ListOfListsOfWinningTiles)
        {
            status = 0;
            foreach(var winningtile in listOfWinningTiles)
            {
                if (winningtile.GetComponent<Tile>().IsOccupied())
                {
                    status += 1;
                }
                if (status == 4)
                {
                    return playerid;
                }
            }
            playerid += 1;
        }
        return 0;
    }
    private List<GameObject> getWinningTilesPerPlayer(int playerID)
    {
        List<GameObject>listofWinningTiles = new List<GameObject>();
        for (int i =1;i < 5; i++)
        {
            listofWinningTiles.Add(GetTile(50 * playerID + i));
        }
        return listofWinningTiles;
    }
}
