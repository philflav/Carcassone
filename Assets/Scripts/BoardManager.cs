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

    private int[] repeats; //an array to hold number of cars repeats allowed for each prefab


    private Stack cardStack;
 
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


        //CreateBoard();
        TakeATurn();

    }

    // Update is called once per frame
    void Update()
    {

    }

        private void TakeATurn()
    {
        //this loops untill card stack is empty
        CardManager cardManager = new CardManager();



        //Create Nodes dictionary
        nodes = new Dictionary<Point, Node>();
        HashSet<Point> availableList = new HashSet<Point>(); //a node list of avaialable spaces (Points)
        //setup directions array;
        TileScript.Direction[] directions = new TileScript.Direction[] { TileScript.Direction.North, TileScript.Direction.South, TileScript.Direction.East, TileScript.Direction.West };

        PlaceStartTile();

 
        while (cardStack.Count>(70-NumberOfTiles)) //there are 71 tiles in the deck. Start tile is already placed!
        {
            //we have remianing cards
            //draw next card from stack
            GameObject checkTile = cardManager.drawCard(cardStack);
            


            //used placed cards list to create a list of available spaces
            // clear out available list on each pass
            availableList.Clear();
                    //for each node in nodelist - check for empty neighbours and add them to available list

                    foreach(KeyValuePair<Point,Node> node in nodes)
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
                else //contiue with placing tiles
                {
                    while (!nodes.ContainsKey(currentPoint)) //keep looking until we find a suitable tile
                    {
                        //Debug.Log("AT " + currentPoint.x + ":" + currentPoint.y);
                        
                        //pick the next card off the stack

                        GameObject checkTile = cardManager.drawCard(cardStack);

                        //Debug.Log(checkTile + "chosen");
                        if (cardStack.Count>0)
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

        if (nodes.ContainsKey(checkPoint.northNeighbour)){

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

        if (nodes.ContainsKey(checkPoint.southNeighbour)&&northCheck)
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

        if (nodes.ContainsKey(checkPoint.eastNeighbour)&&southCheck)
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

        if (nodes.ContainsKey(checkPoint.westNeighbour)&&eastCheck)
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
  
   

        if(direction == TileScript.Direction.North && CheckTile.Up() != NeighbourTile.Down() )
        {
            //Debug.Log(" N Testing " + CheckTile.Up() + " with " + NeighbourTile.Down());
            return false;
        }
        if (direction == TileScript.Direction.South && CheckTile.Down() != NeighbourTile.Up() )
        {

            //Debug.Log(" S Testing " + CheckTile.Down() + " with " + NeighbourTile.Up());
            return false;
        }
        if (direction == TileScript.Direction.East && CheckTile.Right() != NeighbourTile.Left() )
        {
            //Debug.Log(" E Testing " + CheckTile.Right() + " with " + NeighbourTile.Left());
            return false;
        }
        if (direction == TileScript.Direction.West && CheckTile.Left() != NeighbourTile.Right() )
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

        PlaceNewTile(startTilePrefab, startSpawn , TileScript.Direction.North);

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

   

        int inPlayScore(GameObject currentCard)
        {

            //Debug.Log("Scoring :" + currentCard.name);


            //Check for Completed Road

            return roadComplete(currentCard);

            //Check for completed city

            //Check for completed monastry 

            //  }
            //return 0;  //returns score for currentCards
        }

    int roadComplete(GameObject currentCard)
    {
        int roadScore = 0;
        int roadEdges;

        bool roadEndFound = false;

        Stack<TileScript> tilesToCheck = new Stack<TileScript>();  //tiles to be checked
        Stack<TileScript> tilesChecked = new Stack<TileScript>();  //checked tiles
        tilesToCheck.Clear();
        tilesChecked.Clear();
        TileScript currentTile = currentCard.GetComponent<TileScript>();
        tilesToCheck.Push(currentTile); //start by pushing the current tile onto the stack

        while (tilesToCheck.Count > 0 )
        {

            currentTile = tilesToCheck.Pop(); //get the next tile off the stack
            Point currentPoint = currentTile.GridPosition;
            if (!tilesChecked.Contains(currentTile))
            {

                List<TileScript.Direction> edges = currentTile.hasRoadEdges(); //check the next one on the stack
                roadEdges = edges.Count;  //get current tile's road edges

                if (roadEdges == 1)
                {
                    //we have found the start of a single road path
                    //now test the whole road path

                    roadScore = checkPath(currentCard, edges[0]);
                    if (roadScore > 0)
                    {
                        roadEndFound = true; //success
                        Debug.Log("Complete Road found " + currentTile + "@" + currentPoint.x + "," + currentPoint.y + " SCORE :" + roadScore);
                    }
                }
                else if (roadEdges == 2)
                {
                    //we are somewhere in the middle
                    //search both ways

                    roadScore = checkPath(currentCard, edges[0]);
                    if (roadScore > 0)
                    {
 
                        int tmp = checkPath(currentCard, edges[1]);
                        if (tmp > 0)
                        {
                            roadScore += tmp - 1; //subtract 1 becasue starting point is checked twice
                            roadEndFound = true;
                        Debug.Log("Complete Road found " + currentTile + "@" + currentPoint.x + "," + currentPoint.y+" SCORE :" + roadScore);
                        }
                        else
                        {
                            roadScore = 0;
                            roadEndFound = false;
                        }
                    }

                }
                else
                {
                    //we are at a three way junction
                    //search in each direction
                }


            }
        }
            //all done

            if (roadEndFound)
            {
                Debug.Log("Score :" + roadScore);
                Debug.Log("=====");
                return roadScore;
            }
            else
            {
                Debug.Log("=====");
                return 0;
            }
        
    
             

    }

    public int checkPath(GameObject currentCard , TileScript.Direction direction)
    {
        //returns number of steps found until road end of 0 if not found.
        //initally searches in the direction specified

        int roadScore = 0;
        int roadEdges;

        bool roadStartFound = true;
        bool roadEndFound = false;
        bool firstPass = true; //flag set on first pass - check tile in one direction only

        Stack<TileScript> tilesToCheck = new Stack<TileScript>();  //tiles to be checked
        Stack<TileScript> tilesChecked = new Stack<TileScript>();  //checked tiles
        List<TileScript.Direction> edges = new List<TileScript.Direction>{};
        tilesToCheck.Clear();
        tilesChecked.Clear();
        TileScript currentTile = currentCard.GetComponent<TileScript>();
        tilesToCheck.Push(currentTile); //start by pushing the current tile onto the stack

        while (tilesToCheck.Count > 0 && !roadEndFound)
        {
            
            currentTile = tilesToCheck.Pop(); //get the next tile off the stack
            Point currentPoint = currentTile.GridPosition;
            if (!tilesChecked.Contains(currentTile))
            {

                edges.Clear();
                edges = currentTile.hasRoadEdges(); //check the next one on the stack
                roadEdges = edges.Count;  //get current tile's road edges

                

                Debug.Log(" Checking "+currentTile.name+" @"+currentPoint.x+","+currentPoint.y+" in direction "+edges[0]);


                if (roadEdges == 1 || roadEdges >= 3) //must be start or end of road
                {
                    roadScore++;
                    Debug.Log("Road end found" + currentTile + "@" + currentPoint.x + "," + currentPoint.y + " :" + roadEdges);
                    roadEndFound = true; //we have found one end the road

                }
                else if (roadEdges == 2)
                {
                    // Debug.Log("Road section found" + currentTile.name);
                    roadScore++;
                }
                //find neighbours on each road edge

                if (firstPass)
                {
                    //reset edges to specfied direction only
                    edges.Clear();
                    edges.Add(direction);
                    firstPass = false;
                }

                for (int i = 0; i < edges.Count(); i++)
                {
                    if (edges[i] == TileScript.Direction.North && nodes.ContainsKey(currentPoint.northNeighbour))
                    {
                        tilesToCheck.Push(nodes[currentPoint.northNeighbour].TileRef.GetComponent<TileScript>());
                    }
                    else if (edges[i] == TileScript.Direction.East && nodes.ContainsKey(currentPoint.eastNeighbour))
                    {
                        tilesToCheck.Push(nodes[currentPoint.eastNeighbour].TileRef.GetComponent<TileScript>());
                    }
                    else if (edges[i] == TileScript.Direction.South && nodes.ContainsKey(currentPoint.southNeighbour))
                    {
                        tilesToCheck.Push(nodes[currentPoint.southNeighbour].TileRef.GetComponent<TileScript>());
                    }
                    else if (edges[i] == TileScript.Direction.West && nodes.ContainsKey(currentPoint.westNeighbour))
                    {
                        tilesToCheck.Push(nodes[currentPoint.westNeighbour].TileRef.GetComponent<TileScript>());
                    }
                }

                tilesChecked.Push(currentTile);  //make sure we don't check it again

            }
            else
            {
                if (tilesToCheck.Count > 0)
                {
                    tilesChecked.Push(tilesToCheck.Pop());
                }
            }

        }
        //all done

        if (roadEndFound)
        {
            Debug.Log("Score :" + roadScore);
            Debug.Log("=====");
            return roadScore;
        }
        else
        {
            Debug.Log("=====");
            return 0;
        }
    }

    }











