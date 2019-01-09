using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TIle Rotation branch

public class BoardManager : Singleton<BoardManager>
{

    [SerializeField]
    private CameraMovement cameraMovement;

    [SerializeField]
    GameObject startTilePrefab;
    [SerializeField]
    GameObject emptyTilePrefab;
    [SerializeField]
    GameObject[] randomTilePrefab;

    public Dictionary<Point, TileScript> tiles { get; set; }

    public Dictionary<Point, Node> nodes { get; set; }

    //use starttile prefab to set Tilesize
    //assume square tiles for this game
    public float TileSize
    {
        get { return startTilePrefab.GetComponent<SpriteRenderer>().bounds.size.x; }
    }

    private Point boardSize;

    private Vector3 worldOrigin;

    // Start is called before the first frame update
    void Start()
    {
        CreateBoard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateBoard()
    {
        //create Tiles dictionary
        tiles = new Dictionary<Point, TileScript>();

        //Create Nodes disctionary
        nodes = new Dictionary<Point, Node>();

        //Assume 20x20 board to start with - make it adaptive later!
        int boardSizeX = 9;
        int boardSizeY = 9;

        Camera.main.orthographicSize = boardSizeX / 2;


        boardSize = new Point(boardSizeX, boardSizeY);

        Vector3 maxTile = Vector3.zero;


        cameraMovement.SetLimits(new Vector3(boardSizeX, boardSizeY, 0));

        //place the start tile at the centre

        PlaceStartTile();

        //fill board with random tiles
        for (int y = -boardSizeY/2 ; y <= boardSizeY/2 ; y++)
        {
            for (int x = -boardSizeX/2; x <= boardSizeX/2 ; x++)
            {
                Point currentPoint = new Point(x, y);

                //does currentPoint already have a tile
                if (nodes.ContainsKey(currentPoint))
                {
                    //Grid position is occupied - move on
                    continue;
                }
                else
                {
                    Debug.Log("AT "+ currentPoint.x + ":" + currentPoint.y);
                    //pick a random tile to place here

                    GameObject checkTile = drawRandomTile(); 
                    
                    //try in each rotation
                    //pick a random rotation for now!
                    int [] randomrot =new int[4] { 0, 90, 180, 270 }; 
                    Vector3 rot = new Vector3(0, 0, randomrot[0]);
                    checkTile.transform.Rotate(rot);

                    //check any neighbouring tiles for a match
                    if (CheckNeighbours(checkTile.GetComponent<TileScript>(), currentPoint, rot))
                    {
                           //if there is a fit place the tile and update nodes list
                            Node newnode = new Node(checkTile.GetComponent<TileScript>());
                            Debug.Log(" New instance is -" + checkTile.name + " rotation :" + rot.z);
                            PlaceNewTile(checkTile, currentPoint, rot);
                        
                    }
                }
            }
        }
        //finished - write out the node list
        DictDebug();
    }
    private GameObject drawRandomTile()
    {
        //draw a tile at random for now

        GameObject newtile = Instance.randomTilePrefab[Random.Range(0, randomTilePrefab.Length)];
 

        return newtile;
    }
    private bool CheckNeighbours(TileScript checkTile, Point checkPoint, Vector3 rot)
    {
        //Checks any neighbouring Tiles N,S,E & W of check tile with rotation -rot

        bool northCheck = true;
        bool southCheck = true;
        bool eastCheck = true;
        bool westCheck = true;

        Debug.Log("Checking Neighbours for Node at "+checkPoint.x + ":" + checkPoint.y + " with rotation "+rot.z);

        if (nodes.ContainsKey(checkPoint.northNeighbour)){

            //CheckDirection North
            
            TileScript NneighbourNode = nodes[checkPoint.northNeighbour].TileRef.GetComponent<TileScript>();
            Debug.Log("Checking North :");
            northCheck = CheckDirection(checkTile, NneighbourNode, TileScript.Direction.North);
            Debug.Log(northCheck.ToString());
        }
        else
        {
            Debug.Log("No North neighbour check");
        }

        if (nodes.ContainsKey(checkPoint.southNeighbour)&&northCheck)
        {
            //CheckDirection South
            TileScript SneighbourNode = nodes[checkPoint.southNeighbour].TileRef.GetComponent<TileScript>();
            Debug.Log("Checking South : ");
            southCheck = CheckDirection(checkTile, SneighbourNode, TileScript.Direction.South);
            Debug.Log(southCheck.ToString());
        }
        else
        {
            Debug.Log("No South neighbour check");
        }

        if (nodes.ContainsKey(checkPoint.eastNeighbour)&&southCheck)
        {
            //CheckDirection East
            TileScript EneighbourNode = nodes[checkPoint.eastNeighbour].TileRef.GetComponent<TileScript>();
            Debug.Log("Checking East :");
            eastCheck = CheckDirection(checkTile, EneighbourNode, TileScript.Direction.East);
            Debug.Log(eastCheck.ToString());
        }
        else
        {
            Debug.Log("No East neighbour check");
        }

        if (nodes.ContainsKey(checkPoint.westNeighbour)&&eastCheck)
        { 
            //CheckDirection West
            TileScript WneighbourNode = nodes[checkPoint.westNeighbour].TileRef.GetComponent<TileScript>();
            Debug.Log("Checking West :");
            westCheck = CheckDirection(checkTile, WneighbourNode, TileScript.Direction.West);
            Debug.Log(westCheck.ToString());
        }
        else
        {
            Debug.Log("No West neighbour check");
        }


        Debug.Log("Returning : " + (northCheck && southCheck && eastCheck && westCheck));

        return northCheck && southCheck && eastCheck && westCheck;

    }

      private bool CheckDirection(TileScript CheckTile, TileScript NeighbourTile, TileScript.Direction direction)
    {
        Debug.Log("Checking " + CheckTile + " with " + NeighbourTile + " in direction " + direction);
   

        if(direction == TileScript.Direction.North && CheckTile.Up() != NeighbourTile.Down() )
        {
            Debug.Log(" N Testing " + CheckTile.Up() + " with " + NeighbourTile.Down());
            return false;
        }
        if (direction == TileScript.Direction.South && CheckTile.Down() != NeighbourTile.Up() )
        {

            Debug.Log(" S Testing " + CheckTile.Down() + " with " + NeighbourTile.Up());
            return false;
        }
        if (direction == TileScript.Direction.East && CheckTile.Right() != NeighbourTile.Left() )
        {
            Debug.Log(" E Testing " + CheckTile.Right() + " with " + NeighbourTile.Left());
            return false;
        }
        if (direction == TileScript.Direction.West && CheckTile.Left() != NeighbourTile.Right() )
        {
            Debug.Log(" W Testing " + CheckTile.Left() + " with " + NeighbourTile.Right());
            return false;
        }

        Debug.Log(CheckTile + " matches " + NeighbourTile );
        return true;

    }
    
    private void PlaceStartTile()
    {
        Point startSpawn = new Point(0, 0);    

        PlaceNewTile(startTilePrefab, startSpawn , new Vector3(0,0,0));

    }

    public void PlaceNewTile(GameObject tile, Point point, Vector3 rot)
    {
        GameObject newTile = Instantiate(tile, new Vector2(TileSize * point.x, TileSize * point.y), Quaternion.Euler(rot)); 

        TileScript newTileScript = newTile.GetComponent<TileScript>();
        Debug.Log("Placed " + newTile.name + " at " + point.x +":"+point.y);
        nodes.Add(point, new Node(newTile.GetComponent<TileScript>()));

        //Debugging
        if (newTileScript.ThroughRoad())
        {
            //Debug.Log("ThroughRoad found" + tilePrefab);
        }
        if (newTileScript.TerminatesRoad())
        {
            //Debug.Log("Terminates road found" + tilePrefab);
        }
        if (newTileScript.IsMonastry())
        {
            //Debug.Log("Monastry found" + tilePrefab);
        }



    }
    

     
    private void DictDebug()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Debug.Log("[" + nodes.Keys.ElementAt(i).x+","+ nodes.Keys.ElementAt(i).y + "] : "+  nodes[nodes.Keys.ElementAt(i)].TileRef);
        }
    }
}
