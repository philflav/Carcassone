﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


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
    public Text txt;

    public int roadTot, cityTot, monastryTot, gameTot; //Score totals

    public HashSet<Point> availableList;

    TileScript.Direction[] directions;

    public int PlaceDirection = 0;

    private int[] repeats; //an array to hold number of cars repeats allowed for each prefab


    public Stack<GameObject> cardStack;
    public GameObject checkTile;
    public GameObject nextCard;
    public bool cardWasPlaced = false;
    public bool gameStarted = false;

    public Sprite nextSprite = null;
    public Point nextPoint;
    public TileScript nextTile;

    public Dictionary<Point, Node> nodes { get; set; }  // a dictionary of placed tiles


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
        //UnityEngine.Random.seed = randomSeed; //Uncomment here to create a repeatable map
        //GameObject.Find("GameOver").GetComponent<Text>().enabled = false;
        CardManager cardManager = gameObject.AddComponent(typeof(CardManager)) as CardManager;

        cardStack = cardManager.MakeStack(gameTilePrefab);

        //Create Nodes dictionary
        nodes = new Dictionary<Point, Node>();

        //Create available list
        HashSet<Point> availableList = new HashSet<Point>(); //a node list of avaialable spaces (Points)
        availableList.Clear();
        //setup directions array;
        TileScript.Direction[] directions = new TileScript.Direction[] { TileScript.Direction.North, TileScript.Direction.South, TileScript.Direction.East, TileScript.Direction.West };

   }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.R))
        {
            PlaceDirection++;
            if (PlaceDirection > 3) PlaceDirection = 0;
            nextTile.placeDirection = (TileScript.Direction)PlaceDirection;
            Hover.Instance.Rotate(nextSprite, PlaceDirection);
            ClearMarkers();
            MarkAvailable(nextPoint, nextTile);
        }
        if (Input.GetMouseButtonDown(0)||Input.GetKeyDown(KeyCode.Space))
            
        {
            nextPoint.x = Hover.Instance.gridX;
            nextPoint.y = Hover.Instance.gridY;
            //Sprite nextSprite = nextCard.GetComponent<SpriteRenderer>().sprite;
            //Check legal placement          
            if (MarkAvailable(nextPoint, nextTile))
                {
                        PlaceNewTile(nextCard, nextPoint, nextTile.placeDirection);
                //Debug.Log(" Placing tile @ " + nextPoint.x + "," + nextPoint.y);
                        cardWasPlaced = true;
                        ClearMarkers();
                        Hover.Instance.Deactivate();
                }
                else
                {
                    Debug.Log("Can't place here! - illegal placement");
                }
            
 
        }
    }
    public void ClickonDeck()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            GameObject.Find("Instructions").GetComponent<Text>().enabled = false;
            PlaceStartTile();
            cardWasPlaced = true;
        }

        if (cardWasPlaced)
        {
            //only do this if previous card was placed
            if (cardStack.Count() > 0  )
            {
                nextCard = cardStack.Pop();
                //txt.text = cardStack.Count().ToString();
                GameObject.Find("CardDeck").GetComponentInChildren<Text>().text = cardStack.Count().ToString();
                nextTile = nextCard.GetComponent<TileScript>();
                PlaceDirection = 0;
                cardWasPlaced = false;
                nextTile.placeDirection = (TileScript.Direction)PlaceDirection;
                nextSprite = nextTile.GetComponent<SpriteRenderer>().sprite;
                Hover.Instance.Activate(nextSprite);
                Hover.Instance.Rotate(nextSprite, PlaceDirection);
                MarkAvailable(nextPoint, nextTile);
            }
            else
            {
                //Game Over - do something.

            }

        }
    }
    public void ClearMarkers()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("empty");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }
    public bool MarkAvailable(Point checkpoint, TileScript checkTile)
    {
        HashSet<Point> availableList = new HashSet<Point>();
        availableList.Clear();
        //delete all emptyTile Game objects first
        //ClearMarkers();


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
        foreach (Point point in availableList)
        {
            if (CheckNeighbours(checkTile, point)&&!cardWasPlaced)
            {
                //show an empty tile if legal placement available
                GameObject emptytile = Instantiate(emptyTilePrefab, new Vector2(TileSize * point.x, TileSize * point.y), Quaternion.identity);
            }

        }
        if (availableList.Contains(checkpoint) && CheckNeighbours(checkTile, checkpoint))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void TakeATurn()
    {
        //setup directions array;
        TileScript.Direction[] directions = new TileScript.Direction[] { TileScript.Direction.North, TileScript.Direction.South, TileScript.Direction.East, TileScript.Direction.West };
   
                     
        if (cardStack.Count > 0) //there are 71 tiles in the deck. Start tile is already placed!
        {
            //we have remianing cards
            //draw next card from stack
            checkTile = cardStack.Peek();
            PlaceDirection = 0;

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
           //checkTile.GetComponent<TileScript>().placeDirection = directions[PlaceDirection]; //rotate the candiadate tile
            //Hover.Instance.Rotate(checkTile.GetComponent<SpriteRenderer>().sprite, PlaceDirection);                                                                                 //check any neighbouring tiles for a match
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
        //ToDo = check for all cards drawn.
        nextCard = cardStack.Peek();
        Sprite nextSprite = nextCard.GetComponent<SpriteRenderer>().sprite;
        Hover.Instance.Activate(nextSprite);
        Hover.Instance.Rotate(nextSprite,0);
    }

        private bool CheckNeighbours(TileScript checkTile, Point checkPoint)
    {
        //Checks any neighbouring Tiles N,S,E & W of check tile

        bool northCheck = true;
        bool southCheck = true;
        bool eastCheck = true;
        bool westCheck = true;

        //Debug.Log("Checking Neighbours of "+checkTile+ " @ "+checkPoint.x + ":" + checkPoint.y );

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
           // Debug.Log("No North neighbour check required");
        }

        if (nodes.ContainsKey(checkPoint.southNeighbour) && northCheck)
        {
            //CheckDirection South
            TileScript SneighbourNode = nodes[checkPoint.southNeighbour].TileRef.GetComponent<TileScript>();
           // Debug.Log("Checking South :/n");
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
           // Debug.Log("No East neighbour check required");
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


        //Debug.Log("Neighbour check: " + checkTile+"@ "+checkPoint.x+","+checkPoint.y+"  :"+(northCheck && southCheck && eastCheck && westCheck));

        return northCheck && southCheck && eastCheck && westCheck;

    }

    private bool CheckDirection(TileScript CheckTile, TileScript NeighbourTile, TileScript.Direction direction)
    {
       // Debug.Log("Checking " + CheckTile + " with " + NeighbourTile + " in direction " + direction);



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
        gameTot += inPlayScore(newTile);
        GameObject.Find("TotalScore").GetComponent<Text>().text = "Game Total:" + gameTot.ToString();
    }



    public int inPlayScore(GameObject currentCard)
    {

        //ToDo  City with Shield
        //ToDo  Partial scores for end game
        //ToDo  Assign score to claims (Meeples)
        int roadScore = roadComplete(currentCard);
        int cityScore = cityComplete(currentCard);
        int monastryScore = monastryCompleted(currentCard);

        roadTot += roadScore;
        cityTot += cityScore;
        monastryTot += monastryScore;

        GameObject.Find("RoadScore").GetComponent<Text>().text = "Roads:"+roadTot.ToString() +" ("+roadScore+")";
        GameObject.Find("CityScore").GetComponent<Text>().text = "Cities:"+cityTot.ToString() + " (" + cityScore + ")";
        GameObject.Find("MonastryScore").GetComponent<Text>().text = "Monastries:"+monastryTot.ToString() + " (" + monastryScore + ")";

        gameTot = roadTot + cityTot + monastryTot;


        return roadScore + cityScore + monastryScore;

    }

    public int roadComplete(GameObject currentCard)
    {

        //check if current card completes a road and return score if its incomnplete retrun -score
        //This assumes current card is one claimed by a Meeple and therefor only one road is to be examined

        //First follow path to the start of road
        //
        //Now using road start follow path to the end of a road inclementing the score as we go.
        Point currentPoint = new Point();

        int roadScore = 0;

        //  Stack<TileScript> tilesToCheck = new Stack<TileScript>();  //tiles to be checked
        //   Stack<TileScript> tileprevious = new Stack<TileScript>();  //the previous tile checked
        //   tilesToCheck.Clear();
        //  tileprevious.Clear();
        TileScript currentTile = currentCard.GetComponent<TileScript>();
        List<TileScript.Direction> edges = new List<TileScript.Direction> { };

        Tuple<int, bool> result;
      
        edges = currentTile.hasRoadEdges();  //find the number of road edges on the current tile
        currentPoint = currentTile.GridPosition;

        //check each road edge found for complete road

        if (edges.Count() == 1)
        {
            //Debug.Log("1 road edge");
            result = isCompleteRoad(currentTile, edges[0]);
            roadScore = result.Item1;
        }
        if (edges.Count() == 2)
        {
            // Debug.Log("2 road edge .... need to follow to the end to start complete road check");
            result = isCompleteRoad(currentTile, edges[0]);
            roadScore = result.Item1;
            if (roadScore > 0 && !result.Item2) //look the other way if road is not a loop
            {
                int tmp = isCompleteRoad(currentTile, edges[1]).Item1;
                if (tmp > 0)
                //if both road ends found
                {
                    roadScore += tmp - 1;//subtract 1 becasue we start checking from the same tile twice
                }
                else
                {
                    roadScore = 0;
                }
            }

        }

        if (edges.Count() == 3)
        {
            // Debug.Log("3 road edges");
            roadScore = isCompleteRoad(currentTile, edges[0]).Item1;
            roadScore += isCompleteRoad(currentTile, edges[1]).Item1;
            roadScore += isCompleteRoad(currentTile, edges[2]).Item1;
        }
        if (edges.Count() == 4)
        {
            // Debug.Log("4 road edges");

            roadScore = isCompleteRoad(currentTile, edges[0]).Item1;
            roadScore += isCompleteRoad(currentTile, edges[1]).Item1;
            roadScore += isCompleteRoad(currentTile, edges[2]).Item1;
            roadScore += isCompleteRoad(currentTile, edges[3]).Item1;

        }

        //not a road start tile
        if (roadScore > 0) Debug.Log("RoadScore " + roadScore);
        return roadScore;


    }

    TileScript hasNeighbour(TileScript currentTile, TileScript.Direction edge)
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
    public Tuple<int ,bool> isCompleteRoad(TileScript startTile, TileScript.Direction edge)
    {
        //returns score for a complete road starting from currentTile on edge
        //returns zero score for incomplete road segments.
        //returns boolean = true if roadloop found


        bool roadEndFound = false;
        bool roadStartFound = false;



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
        tmp = hasNeighbour(startTile, edge);  //is there a neighbour on the edge
        if (tmp)
        {
            tilesToCheck.Push(tmp); //push neigbour onto stack
        }
        tileprevious.Push(startTile); //mark startTile as checked

        //Debug.Log(">>>Checking " + startTile + "from " + startPoint.x + "," + startPoint.y+" in direction "+edge);

        while (tilesToCheck.Count > 0 && !roadEndFound && safety < 100)
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
                        //Debug.Log(">>>>Road found from :" + startPoint.x + "," + startPoint.y + " to " + endPoint.x + "," + endPoint.y);
                        return Tuple.Create(roadScore, false);
                    }
                    else
                    {
                        return Tuple.Create(0, false);
                    }
                }
                else if (edges.Count() == 2)
                {
                    tmp = hasNeighbour(currentTile, edges[0]);
                    if(tmp.GridPosition==startPoint && roadScore > 3)
                    {
                        //we've gone in a loop
                        roadEndFound = true;
                        return Tuple.Create(roadScore, true); //return true to indicate loop
                    }
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp);
                        //Debug.Log("Neighbour" + edges[0]);
                    }
                    tmp = hasNeighbour(currentTile, edges[1]);
                    if (tmp.GridPosition == startPoint && roadScore > 3)
                    {
                        //we've gone in a loop
                        roadEndFound = true;
                        return Tuple.Create(roadScore, true); //return true to indicate loop
                    }
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp);
                        //Debug.Log("Neighbour" + edges[1]);
                    }
                }
                tileprevious.Push(currentTile);

            }
        }
        return Tuple.Create(0, false);
    }

    public int cityComplete(GameObject currentCard)
    {
        //does the current card complete one or more cities. If so return the city score(s) or -score for incomplete cities.
        Point currentPoint = new Point();

        int cityScore = 0;

        int tmp = 0;

        TileScript currentTile = currentCard.GetComponent<TileScript>();
        List<TileScript.Direction> edges = new List<TileScript.Direction> { };

        edges = currentTile.hasCityEdges();  //find the number of city edges on the current tile
        currentPoint = currentTile.GridPosition;

        if (edges.Count() == 1)
        {
            cityScore = isCompletedCity(currentTile, edges[0]); //does this tile complete a city
        }
        if (edges.Count() == 2 && !currentTile.HasCityCentre())
        {
            //Debug.Log("Here "+currentTile);
            //explore both edges for 2 completed cities
            // Debug.Log(currentTile + " does not have connected city edges");
            cityScore = isCompletedCity(currentTile, edges[0]); //does this tile complete a city
            //Debug.Log("City score " + cityScore);
            cityScore = isCompletedCity(currentTile, edges[1]); //does this tile complete a city
            //Debug.Log("City score " + cityScore);
        }
        if (edges.Count() == 2 && currentTile.HasCityCentre())
        {
            //explore both edges for 1 completed city
            cityScore = isCompletedCity(currentTile, edges[0]); //just test the first edge
        }
        if (edges.Count() == 3)
        {
            //explore all edges for 1 completed city
            tmp = isCompletedCity(currentTile, edges[0]); //does this tile complete a city
            if (tmp > 0)
            {
                cityScore = tmp;
                tmp = isCompletedCity(currentTile, edges[1]); //does this tile complete a city
                if (tmp > 0)
                {
                    cityScore += tmp;
                    tmp = isCompletedCity(currentTile, edges[2]); //does this tile complete a city
                    if (tmp > 0)
                    {
                        cityScore += tmp;
                    }
                    else
                    {
                        cityScore = 0;
                    }
                }
                else
                {
                    cityScore = 0;
                }
            }
        }
        if (edges.Count() == 4)
        {
            //explore all edges for 1 completed city
            tmp = isCompletedCity(currentTile, edges[0]); //does this tile complete a citysegment
            if (tmp > 0)
            {
                cityScore = tmp;
                tmp = isCompletedCity(currentTile, edges[1]); //does this tile complete a citysegment
                if (tmp > 0)
                {
                    cityScore += tmp;
                    tmp = isCompletedCity(currentTile, edges[2]); //does this tile complete a citysegment
                    if (tmp > 0)
                    {
                        cityScore += tmp;
                        tmp = isCompletedCity(currentTile, edges[3]); //does this tile complete the citysegment
                        if (tmp > 0)
                        {
                            cityScore += tmp;
                        }
                        else cityScore = 0;
                    }
                    else
                    {
                        cityScore = 0;
                    }
                }
                else
                {
                    cityScore = 0;
                }
            }
        }




        return cityScore;
    }

    public int isCompletedCity(TileScript startTile, TileScript.Direction edge)
    {

        //returns score for a completed city segment starting from the startTile.
        //Assumes the start tile is itself closed when calculating open or closed segments.



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
        int cityScore = 0;
        int T = 0; //closed edge counter
        int S = 0; //open edge counter

        tilesToCheck.Push(startTile);
        tmp = hasNeighbour(startTile, edge);  //is there a neighbour on the edge    
        if (tmp)
        {
            tilesToCheck.Push(tmp); //push neigbour onto stack

        }
        //tileprevious.Push(startTile); //mark startTile as checked

        while (tilesToCheck.Count > 0 && safety < 10)
        {
            safety++;  //prevent infinite loop
            currentTile = tilesToCheck.Pop(); //get the next tile to check
            if (!tileprevious.Contains(currentTile))
            {

                edges = currentTile.hasCityEdges();  //city edges on the current tile

                // Debug.Log(safety+"Checking :"+currentTile);

                currentPoint = currentTile.GridPosition;
                if (edges.Count() == 1)
                {
                    cityScore++;
                    if (currentTile.HasShield()) cityScore++;
                    T++; //closing edge
                    tmp = hasNeighbour(currentTile, edges[0]);
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp); //push neigbour onto stack
                    }
                }
                else if (edges.Count() == 2)
                {
                    //if edges are not connected increment T
                    cityScore++;
                    if (currentTile.HasShield()) cityScore++;
                    if (!currentTile.HasCityCentre())
                    {
                        //Debug.Log(currentTile + " does not have connected city edges");
                        T++;
                    }
                    else
                    {
                        //Debug.Log(currentTile + "has connected city edges");
                        tmp = hasNeighbour(currentTile, edges[0]);
                        if (tmp)
                        {
                            tilesToCheck.Push(tmp); //push neigbour onto stack
                        }
                        tmp = hasNeighbour(currentTile, edges[1]);
                        if (tmp)
                        {
                            tilesToCheck.Push(tmp); //push neigbour onto stack
                        }

                    }
                }
                else if (edges.Count() == 3)
                {
                    cityScore++;
                    if (currentTile.HasShield()) cityScore++;
                    S++; //add a split
                    tmp = hasNeighbour(currentTile, edges[0]);
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp); //push neigbour onto stack
                    }
                    tmp = hasNeighbour(currentTile, edges[1]);
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp); //push neigbour onto stack
                    }
                    tmp = hasNeighbour(currentTile, edges[2]);
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp); //push neigbour onto stack
                    }

                }
                else if (edges.Count() == 4)
                {
                    cityScore++;
                    if (currentTile.HasShield()) cityScore++;
                    S += 2; //add 2 splits
                    tmp = hasNeighbour(currentTile, edges[0]);
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp); //push neigbour onto stack
                    }
                    tmp = hasNeighbour(currentTile, edges[1]);
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp); //push neigbour onto stack
                    }
                    tmp = hasNeighbour(currentTile, edges[2]);
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp); //push neigbour onto stack
                    }
                    tmp = hasNeighbour(currentTile, edges[3]);
                    if (tmp)
                    {
                        tilesToCheck.Push(tmp); //push neigbour onto stack
                    }

                }
                tileprevious.Push(currentTile); //make sure we don't check it again
            }
        }
        if (T - 2 == S)
        {
            //We found a complete segment

            //Debug.Log("Completed City @ " + currentPoint.x + "," + currentPoint.y + " T:" + T + " S:" + S + " CityScore:" + cityScore);

            return cityScore;
        }
        else
        {
            return 0;
        }
    }
    public int monastryCompleted(GameObject currentCard)
    {
        //does the current tile complete a  monastry

        TileScript currentTile = currentCard.GetComponent<TileScript>();
        Node tmp;

        int monastryScore = 0;

        Point currentPoint = currentTile.GridPosition;

        Stack<TileScript> tilesToCheck = new Stack<TileScript>();  //tiles to be checked

        tilesToCheck.Push(currentTile);

        if (nodes.TryGetValue(currentPoint.northNeighbour,out tmp))tilesToCheck.Push(tmp.TileRef);
        if (nodes.TryGetValue(currentPoint.northeastNeighbour, out tmp)) tilesToCheck.Push(tmp.TileRef);
        if (nodes.TryGetValue(currentPoint.eastNeighbour, out tmp)) tilesToCheck.Push(tmp.TileRef);
        if (nodes.TryGetValue(currentPoint.southeastNeighbour, out tmp)) tilesToCheck.Push(tmp.TileRef);
        if (nodes.TryGetValue(currentPoint.southNeighbour, out tmp)) tilesToCheck.Push(tmp.TileRef);
        if (nodes.TryGetValue(currentPoint.southwestNeighbour, out tmp)) tilesToCheck.Push(tmp.TileRef);
        if (nodes.TryGetValue(currentPoint.westNeighbour, out tmp)) tilesToCheck.Push(tmp.TileRef);
        if (nodes.TryGetValue(currentPoint.northwestNeighbour, out tmp)) tilesToCheck.Push(tmp.TileRef);

        while (tilesToCheck.Count() > 0)
        {
            currentTile = tilesToCheck.Pop();
            monastryScore += CompletedMonastry(currentTile);
        }
        if (monastryScore == 8) monastryScore++; //add one for the central tile.
        return monastryScore ;

    }
    public int CompletedMonastry(TileScript currentTile)
    {
        //if a tile is a completed monastry return 8 otherwise return 0

        if (!currentTile.IsMonastry()) return 0;

        Point currentPoint = currentTile.GridPosition;

        int neighbours = 0;

        if (nodes.ContainsKey(currentPoint.northNeighbour))
        {
            neighbours++;
        }
        if (nodes.ContainsKey(currentPoint.northeastNeighbour))
        {
            neighbours++;
        }
        if (nodes.ContainsKey(currentPoint.eastNeighbour))
        {
            neighbours++;
        }
        if (nodes.ContainsKey(currentPoint.southeastNeighbour))
        {
            neighbours++;
        }
        if (nodes.ContainsKey(currentPoint.southNeighbour))
        {
            neighbours++;
        }
        if (nodes.ContainsKey(currentPoint.southwestNeighbour))
        {
            neighbours++;
        }
        if (nodes.ContainsKey(currentPoint.westNeighbour))
        {
            neighbours++;
        }
        if (nodes.ContainsKey(currentPoint.northwestNeighbour))
        {
            neighbours++;
        }
        if (neighbours == 8)
        {
            Debug.Log("Completed Monastry @ " + currentPoint.x + "," + currentPoint.y);
            return neighbours;
        }
        else
        {
            return 0;
        }
    }
    
}




   
    
    



     














