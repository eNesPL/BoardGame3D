using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    List<GameObject> ListOfTiles = new List<GameObject>();
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
        this.loading = false;
        
    }

    public GameObject GetTile(int TileID)
    {
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
}
