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
            Debug.LogWarning("TTT");
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
