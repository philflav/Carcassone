using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//Master branch
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
        int boardSizeX = 10;
        int boardSizeY = 10;

        Camera.main.orthographicSize = boardSizeX / 2;


        boardSize = new Point(boardSizeX, boardSizeY);

        Vector3 maxTile = Vector3.zero;


        cameraMovement.SetLimits(new Vector3(boardSizeX, boardSizeY, 0));

        //initalise a current node at the centre

       Node currentNode = new Node(startTilePrefab.GetComponent<TileScript>());
       nodes.Add(new Point(0, 0), currentNode);

        //place the start tile
        PlaceStartTile();

        //fill board with random tiles
        for (int y = -boardSizeY/2 ; y <= boardSizeY/2 ; y++)
        {
            for (int x = -boardSizeX/2; x <= boardSizeX/2 ; x++)
            {
                Point currentPoint = new Point(x, y);
                GameObject newTile = drawTile();              
                Node newnode = new Node(newTile.GetComponent<TileScript>());

                //does currentPoint already have a tile

                if (nodes.ContainsKey(currentPoint))
                {
                    //Grid position is occupied - move on
                    continue;
                }
                else
                {
                    int i = 0;//loop counter
                    //pick a random tile to place here
                    while (!CheckNeighbours(newTile.GetComponent<TileScript>(), currentPoint))
                    {
                        newTile= drawTile();

                        newnode = new Node(newTile.GetComponent<TileScript>());
                        i++;
                            if (i > randomTilePrefab.Length)
                        {
                            //move on if too many attempts
                            newTile = emptyTilePrefab;
                            break;
                        }
                    }
                }
                PlaceTile(newTile, currentPoint);
                nodes.Add(currentPoint, new Node(newTile.GetComponent<TileScript>()));
                DictDebug();

            }
        }
    }
    private GameObject drawTile()
    {
        //draw a tile at random for now

        GameObject tile = randomTilePrefab[Random.Range(0, randomTilePrefab.Length)];

        return tile;
    }
    private bool CheckNeighbours(TileScript checkTile, Point checkPoint)
    {
        //Checks any neighbouring Tiles N,S,E & W of check tile

        bool northCheck = true;
        bool southCheck = true;
        bool eastCheck = true;
        bool westCheck = true;

        Debug.Log("Checking Neighbours for Node at "+checkPoint.x + ":" + checkPoint.y);

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

    private bool CheckDirection(TileScript checkTile, TileScript refTile, TileScript.Direction direction)
    {
        //returns true if checktile  matches refTile in Diretion specified

        //test of code revertion. If this worked this line has gone!


        Debug.Log("Checking " + checkTile + " with " + refTile + " in direction " + direction);

        if(direction == TileScript.Direction.North && checkTile.North != refTile.South)
        {
            return false;
        }
        if (direction == TileScript.Direction.South && checkTile.South != refTile.North)
        {
            return false;
        }
        if (direction == TileScript.Direction.East && checkTile.East != refTile.West)
        {
            return false;
        }
        if (direction == TileScript.Direction.West && checkTile.West != refTile.East)
        {
            return false;
        }

        Debug.Log(checkTile + " matches " + refTile );
        return true;

    }
    
    private void PlaceStartTile()
    {
        Point startSpawn = new Point(0, 0);

        PlaceTile(startTilePrefab, startSpawn);

    }

    public void PlaceTile(GameObject tilePrefab, Point point)
    {
        TileScript newTile = Instantiate(tilePrefab).GetComponent<TileScript>();
        newTile.Move(new Vector2(TileSize * point.x, TileSize * point.y));

        Debug.Log("Placed " + tilePrefab.name + " at " + point.x +":"+point.y);
        
        //Debugging
        if (newTile.ThroughRoad())
        {
            //Debug.Log("ThroughRoad found" + tilePrefab);
        }
        if (newTile.TerminatesRoad())
        {
            //Debug.Log("Terminates road found" + tilePrefab);
        }
        if (newTile.IsMonastry())
        {
            //Debug.Log("Monastry found" + tilePrefab);
        }



    }
    private TileScript RotateTile(TileScript tile, int rot)
    {
        //Rotates sprite and edges

        Debug.Log("Rotating " + tile.name + " by: " + rot);
        Vector3 angle = new Vector3(0, 0, 0);
        if (rot == 90)
        {
            angle = new Vector3(0, 0, 90);
            tile.RotateEdges90();
            Debug.Log(tile.ToString() + "R" + tile.EdgeString());
        }
        else if (rot == 180)
        {
            angle = new Vector3(0, 0, 180);
            tile.RotateEdges180();
            Debug.Log(tile.ToString() + "R" + tile.EdgeString());
        }
        else if (rot == 270 || rot == -90)
        {
            angle = new Vector3(0, 0, 270);
            tile.RotateEdges270();
            Debug.Log(tile.ToString() + "R" + tile.EdgeString());
        }

        tile.transform.eulerAngles = angle;

        return tile;
    }

     
    private void DictDebug()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Debug.Log("[" + nodes.Keys.ElementAt(i).x+","+ nodes.Keys.ElementAt(i).y + "] : "+  nodes[nodes.Keys.ElementAt(i)].TileRef);
        }
    }
}
