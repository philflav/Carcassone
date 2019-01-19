using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//Infinite Board

//number of each tile is specified on Unity prefab. Board can extend in any direction.

public class BoardManager : Singleton<BoardManager>
{

    [SerializeField]
    private CameraMovement cameraMovement;

    [SerializeField]
    int Board_size;
    [SerializeField]
    int randomSeed;
    [SerializeField]
    int NumberOfTiles;


    [SerializeField]
    GameObject startTilePrefab;
    [SerializeField]
    GameObject emptyTilePrefab;
    [SerializeField]
    GameObject[] gameTilePrefab;

    public CardManager cardManager;

    public HashSet<Point> availableList;

    TileScript.Direction[] directions;

    private int[] repeats; //an array to hold number of cars repeats allowed for each prefab


    public Stack<GameObject> cardStack;
    public GameObject checkTile;

    public Dictionary<Point, Node> nodes { get; set; }  // a dictionary of placed tiles


    public int score; //to hold cumulative score as each tile is placed.

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
        UnityEngine.Random.seed = randomSeed; //Uncomment here to create a repeatable map

        CardManager cardManager = new CardManager();

        cardStack = cardManager.MakeStack(gameTilePrefab);

        //Create Nodes dictionary
        nodes = new Dictionary<Point, Node>();

        //Create available list
        HashSet<Point> availableList = new HashSet<Point>(); //a node list of avaialable spaces (Points)

        //setup directions array;
        TileScript.Direction[] directions = new TileScript.Direction[] { TileScript.Direction.North, TileScript.Direction.South, TileScript.Direction.East, TileScript.Direction.West };

        PlaceStartTile();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            TakeATurn();
        }
    }

    public void TakeATurn()
    {
        //setup directions array;
        TileScript.Direction[] directions = new TileScript.Direction[] { TileScript.Direction.North, TileScript.Direction.South, TileScript.Direction.East, TileScript.Direction.West };


        //GameObject checkTile = new GameObject();


        if (cardStack.Count > 0) //there are 71 tiles in the deck. Start tile is already placed!
        {
            //we have remianing cards
            //draw next card from stack
            checkTile = cardStack.Peek();

            if (cardStack.Count > 0)
            {
                GameObject drawncard = cardStack.Pop();
            }

            //used placed cards list to create a list of available spaces
            // clear out available list on each pass
            HashSet<Point> availableList = new HashSet<Point>();
            availableList.Clear();
            //for each node in nodelist - check for empty neighbours and add them to available list

            foreach (KeyValuePair<Point, Node> node in nodes)
            {
                if (!nodes.ContainsKey(node.Key.northNeighbour))
                {
                    //no neigbour so add it to available list
                    availableList.Add(node.Key.northNeighbour);
                }
                if (!nodes.ContainsKey(node.Key.eastNeighbour))
                {
                    //no neigbour so add it to available list
                    availableList.Add(node.Key.eastNeighbour);
                }
                if (!nodes.ContainsKey(node.Key.westNeighbour))
                {
                    //no neigbour so add it to available list
                    availableList.Add(node.Key.westNeighbour);
                }
                if (!nodes.ContainsKey(node.Key.southNeighbour))
                {
                    //no neigbour so add it to available list
                    availableList.Add(node.Key.southNeighbour);
                }
            }

            //choose a random orientation
            int PlaceDirection = UnityEngine.Random.Range(0, 4);
            checkTile.GetComponent<TileScript>().placeDirection = directions[PlaceDirection]; //rotate the candiadate tile
                                                                                              //check any neighbouring tiles for a match
                                                                                              //test check tile against each avaialable space in a random orientation
            foreach (var node in availableList)
            {
                if (CheckNeighbours(checkTile.GetComponent<TileScript>(), node))
                {
                    //if there is a fit, place the tile and update nodes list
                    Node newnode = new Node(checkTile.GetComponent<TileScript>());
                    PlaceNewTile(checkTile, node, directions[PlaceDirection]);

                    //  forget the rest
                    break;
                }
            }
        }
        //Debug.Log(availableList);
    }

    private void CreateBoard()
    {
        CardManager cardManager = new CardManager();

        //Create Nodes dictionary
        nodes = new Dictionary<Point, Node>();

        //Assume 20x20 board to start with - make it adaptive later!
        int boardSizeX = Board_size;
        int boardSizeY = Board_size;

        Camera.main.orthographicSize = boardSizeX / 2;


        boardSize = new Point(boardSizeX, boardSizeY);

        Vector3 maxTile = Vector3.zero;


        cameraMovement.SetLimits(new Vector3(boardSizeX, boardSizeY, 0));

        //place the start tile at the centre

        //PlaceStartTile();

        //fill board with random tiles
        for (int y = -boardSizeY / 2; y <= boardSizeY / 2; y++)
        {
            for (int x = -boardSizeX / 2; x <= boardSizeX / 2; x++)
            {
                Point currentPoint = new Point(x, y);

                //does currentPoint already have a tile
                if (nodes.ContainsKey(currentPoint))
                {
                    //Grid position is occupied - move on
                    continue;
                }
                else //contiue with placing tiles
                {
                    while (!nodes.ContainsKey(currentPoint)) //keep looking until we find a suitable tile
                    {
                        //Debug.Log("AT " + currentPoint.x + ":" + currentPoint.y);

                        //pick the next card off the stack

                        GameObject checkTile = cardManager.drawCard(cardStack);

                        //Debug.Log(checkTile + "chosen");
                        if (cardStack.Count > 0)
                        //see if we got a card or that the card stack is exhausted.
                        {



                            //try in each rotation

                            //Choose a placement direction at random

                            int PlaceDirection = UnityEngine.Random.Range(0, 4);
                            //PlaceDirection = 1;
                            TileScript.Direction[] directions = new TileScript.Direction[] { TileScript.Direction.North, TileScript.Direction.South, TileScript.Direction.East, TileScript.Direction.West };

                            if (nodes.ContainsKey(currentPoint))
                            {
                                //Grid position is occupied - move on
                                continue;
                            }
                            else
                            {
                                checkTile.GetComponent<TileScript>().placeDirection = directions[PlaceDirection]; //rotate the candiadate tile
                                                                                                                  //check any neighbouring tiles for a match
                                if (CheckNeighbours(checkTile.GetComponent<TileScript>(), currentPoint))
                                {
                                    //if there is a fit, place the tile and update nodes list
                                    Node newnode = new Node(checkTile.GetComponent<TileScript>());
                                    //Debug.Log(" New instance is -" + checkTile.name);
                                    PlaceNewTile(checkTile, currentPoint, directions[PlaceDirection]);
                                }
                            }
                        }
                    }

                }
            }
        }

    }
    private bool CheckNeighbours(TileScript checkTile, Point checkPoint)
    {
        //Checks any neighbouring Tiles N,S,E & W of check tile

        bool northCheck = true;
        bool southCheck = true;
        bool eastCheck = true;
        bool westCheck = true;

        //Debug.Log("Checking Neighbours for Node at "+checkPoint.x + ":" + checkPoint.y );

        if (nodes.ContainsKey(checkPoint.northNeighbour))
        {

            //CheckDirection North

            TileScript NneighbourNode = nodes[checkPoint.northNeighbour].TileRef.GetComponent<TileScript>();
            //Debug.Log("Checking North :/n");
            northCheck = CheckDirection(checkTile, NneighbourNode, TileScript.Direction.North);
            //Debug.Log(northCheck.ToString());
        }
        else
        {
            //Debug.Log("No North neighbour check required");
        }

        if (nodes.ContainsKey(checkPoint.southNeighbour) && northCheck)
        {
            //CheckDirection South
            TileScript SneighbourNode = nodes[checkPoint.southNeighbour].TileRef.GetComponent<TileScript>();
            //Debug.Log("Checking South :/n");
            southCheck = CheckDirection(checkTile, SneighbourNode, TileScript.Direction.South);
            //Debug.Log(southCheck.ToString());
        }
        else
        {
            //Debug.Log("No South neighbour check required");
        }

        if (nodes.ContainsKey(checkPoint.eastNeighbour) && southCheck)
        {
            //CheckDirection East
            TileScript EneighbourNode = nodes[checkPoint.eastNeighbour].TileRef.GetComponent<TileScript>();
            //Debug.Log("Checking East :/n");
            eastCheck = CheckDirection(checkTile, EneighbourNode, TileScript.Direction.East);
            //Debug.Log(eastCheck.ToString());
        }
        else
        {
            //Debug.Log("No East neighbour check required");
        }

        if (nodes.ContainsKey(checkPoint.westNeighbour) && eastCheck)
        {
            //CheckDirection West
            TileScript WneighbourNode = nodes[checkPoint.westNeighbour].TileRef.GetComponent<TileScript>();
            //Debug.Log("Checking West : /n");
            westCheck = CheckDirection(checkTile, WneighbourNode, TileScript.Direction.West);
            //Debug.Log(westCheck.ToString());
        }
        else
        {
            //Debug.Log("No West neighbour check required");
        }


        //Debug.Log("Returning : " + (northCheck && southCheck && eastCheck && westCheck));

        return northCheck && southCheck && eastCheck && westCheck;

    }

    private bool CheckDirection(TileScript CheckTile, TileScript NeighbourTile, TileScript.Direction direction)
    {
        //Debug.Log("Checking " + CheckTile + " with " + NeighbourTile + " in direction " + direction);



        if (direction == TileScript.Direction.North && CheckTile.Up() != NeighbourTile.Down())
        {
            //Debug.Log(" N Testing " + CheckTile.Up() + " with " + NeighbourTile.Down());
            return false;
        }
        if (direction == TileScript.Direction.South && CheckTile.Down() != NeighbourTile.Up())
        {

            //Debug.Log(" S Testing " + CheckTile.Down() + " with " + NeighbourTile.Up());
            return false;
        }
        if (direction == TileScript.Direction.East && CheckTile.Right() != NeighbourTile.Left())
        {
            //Debug.Log(" E Testing " + CheckTile.Right() + " with " + NeighbourTile.Left());
            return false;
        }
        if (direction == TileScript.Direction.West && CheckTile.Left() != NeighbourTile.Right())
        {
            //Debug.Log(" W Testing " + CheckTile.Left() + " with " + NeighbourTile.Right());
            return false;
        }

        //Debug.Log(CheckTile + " matches " + NeighbourTile );
        return true;

    }

    private void PlaceStartTile()
    {
        Point startSpawn = new Point(0, 0);

        PlaceNewTile(startTilePrefab, startSpawn, TileScript.Direction.North);

    }

    public void PlaceNewTile(GameObject tile, Point point, TileScript.Direction direction)
    {

        //setup tile placement rotation vector based on which direction is Up

        Vector3 rot = new Vector3(0, 0, 0);

        if (direction == TileScript.Direction.North) { rot = new Vector3(0, 0, 0); }
        if (direction == TileScript.Direction.East) { rot = new Vector3(0, 0, 270); }
        if (direction == TileScript.Direction.South) { rot = new Vector3(0, 0, 180); }
        if (direction == TileScript.Direction.West) { rot = new Vector3(0, 0, 90); }


        GameObject newTile = Instantiate(tile, new Vector2(TileSize * point.x, TileSize * point.y), Quaternion.identity);

        newTile.transform.Rotate(rot);



        TileScript newTileScript = newTile.GetComponent<TileScript>();
        //Debug.Log("Placed " + newTile.name + " at " + point.x +":"+point.y + "pointing "+direction);
        newTileScript.placeDirection = direction;
        newTileScript.Setup(point, newTile.transform.position); //setup the tiles grid position
        nodes.Add(point, new Node(newTile.GetComponent<TileScript>()));
        score += inPlayScore(newTile);
    }



    public int inPlayScore(GameObject currentCard)
    {

        //Debug.Log("Scoring :" + currentCard.name);


        //Check for Completed Road

        return roadComplete(currentCard);

        //Check for completed city

        //Check for completed monastry 

        // 
        //return 0;  //returns score for currentCards
    }

    public int roadComplete(GameObject currentCard)

    //check if current card completes a road and return score if its incomnplete retrun -score
    //This assumes current card is one claimed by a Meeple and therefor only one road is to be examined

    //First follow path to the start of road
    //
    //Now using road start follow path to the end of a road inclementing the score as we go.
    {
        Point currentPoint = new Point();

        int roadScore = 0;

        Stack<TileScript> tilesToCheck = new Stack<TileScript>();  //tiles to be checked
        Stack<TileScript> tileprevious = new Stack<TileScript>();  //the previous tile checked
        tilesToCheck.Clear();
        tileprevious.Clear();
        TileScript currentTile = currentCard.GetComponent<TileScript>();
        List<TileScript.Direction> edges = new List<TileScript.Direction> { };


        edges = currentTile.hasRoadEdges();  //find the number of road edges on the current tile
        currentPoint = currentTile.GridPosition;

        //check each road edge found for complete road

        if (edges.Count() == 1)
        {
            Debug.Log("1 road edge");
            roadScore=isCompleteRoad(currentTile, edges[0]);
        }
        if (edges.Count() ==2)
        {
            Debug.Log("2 road edge .... need to follow to the end to start complede road check");
            roadScore=isCompleteRoad(currentTile, edges[0]);
            if (roadScore > 0)
            {
                int tmp = isCompleteRoad(currentTile, edges[1]);
               if (tmp > 0)
                    //if 2 road ends found
                {
                    roadScore += tmp -1 ;//subtract 1 becasue we start checking from the same tile twice
                }
                else
                {
                    roadScore = 0;
                }
            }

        }

        if (edges.Count() == 3)
        {
            Debug.Log("3 road edges");
            roadScore = isCompleteRoad(currentTile, edges[0]);
            roadScore += isCompleteRoad(currentTile, edges[1]);
            roadScore += isCompleteRoad(currentTile, edges[2]);
        }
        if (edges.Count() == 4)
        {
            Debug.Log("4 road edges");

            roadScore = isCompleteRoad(currentTile, edges[0]);
            roadScore += isCompleteRoad(currentTile, edges[1]);
            roadScore += isCompleteRoad(currentTile, edges[2]);
            roadScore += isCompleteRoad(currentTile, edges[3]);

        }

        //not a road start tile
        if(roadScore >0)Debug.Log("roadScore " + roadScore);
        return roadScore;


    }

    TileScript hasRoadNeighbour(TileScript currentTile, TileScript.Direction edge)
    {
        //returns reference to neighbouring tile on edge OR null

        Point currentPoint = currentTile.GridPosition;

        if (edge == TileScript.Direction.North && nodes.ContainsKey(currentPoint.northNeighbour))
        {
            return nodes[currentPoint.northNeighbour].TileRef.GetComponent<TileScript>();
        }
        else if (edge == TileScript.Direction.East && nodes.ContainsKey(currentPoint.eastNeighbour))
        {
            return nodes[currentPoint.eastNeighbour].TileRef.GetComponent<TileScript>();
        }
        else if (edge == TileScript.Direction.South && nodes.ContainsKey(currentPoint.southNeighbour))
        {
            return nodes[currentPoint.southNeighbour].TileRef.GetComponent<TileScript>();
        }
        else if (edge == TileScript.Direction.West && nodes.ContainsKey(currentPoint.westNeighbour))
        {
            return nodes[currentPoint.westNeighbour].TileRef.GetComponent<TileScript>();
        }
        else
        {
            return null;
        }
    }
    int isCompleteRoad(TileScript startTile, TileScript.Direction edge)
    {
        //returns score for a complete road starting from currentTile on edge
        //retruns - score of incomplete road segments.

 
        bool roadEndFound = false;



        Point currentPoint = new Point();
        Point startPoint = new Point();
        Point endPoint = new Point();

        Stack<TileScript> tilesToCheck = new Stack<TileScript>();  //tiles to be checked
        Stack<TileScript> tileprevious = new Stack<TileScript>();  //the previous tile checked
        List<TileScript.Direction> edges = new List<TileScript.Direction> { };
        tilesToCheck.Clear();
        tileprevious.Clear();
        TileScript tmp, currentTile;

        int safety = 0;
        int roadScore = 1;
  

        startPoint = startTile.GridPosition;
        tmp = hasRoadNeighbour(startTile, edge);  //is there a neighbour on the edge
        if (tmp)
        {
            tilesToCheck.Push(tmp); //push neigbour onto stack
        }
        tileprevious.Push(startTile); //mark startTile as checked

        Debug.Log(">>>Checking " + startTile + "from " + startPoint.x + "," + startPoint.y+" in direction "+edge);

        while (tilesToCheck.Count > 0 && !roadEndFound && safety < 10)
        {
            safety++;  //prevent infinite loop
            currentTile = tilesToCheck.Pop();
            if (!tileprevious.Contains(currentTile))
            {

                roadScore++;
                edges = currentTile.hasRoadEdges();  //road edges on the current tile
                currentPoint = currentTile.GridPosition;
                //Debug.Log(safety+"Next tile " + currentTile + " " + edges[0]+" "+edges[1]);
                if (edges.Count() == 1 || edges.Count() >= 3)
                {
                    endPoint = currentPoint;
                    roadEndFound = true;
                    if (!(startPoint == endPoint))
                    {
                        Debug.Log(">>>>Road found from :" + startPoint.x + "," + startPoint.y + " to " + endPoint.x + "," + endPoint.y);
                        return roadScore;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else if (edges.Count() == 2)
                {
                    tmp = hasRoadNeighbour(currentTile, edges[0]);
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp);
                        Debug.Log("Neighbour" + edges[0]);
                    }
                    tmp = hasRoadNeighbour(currentTile, edges[1]);
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp);
                        Debug.Log("Neighbour" + edges[1]);
                    } 
                }
                tileprevious.Push(currentTile);

            }
        }
            return 0;
    }
}

    
    



     














