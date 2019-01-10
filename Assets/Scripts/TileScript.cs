using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
    public Point GridPosition { get; private set; }

    private SpriteRenderer spriteRenderer;


    public Vector2 WorldPosition
    {
        get
        {
            return new Vector2(transform.position.x + GetComponent<SpriteRenderer>().bounds.size.x / 2,
                               transform.position.y - GetComponent<SpriteRenderer>().bounds.size.y / 2);
        }
    }
    public enum Structure { Road, City, Field, Monastry, Shield, River, Village };
    public enum Direction { North, South, East, West }
    [SerializeField]
    Structure N, E, S, W, C;  //edges and centre of tile 

    public Direction placeDirection; //tile placement direction for Up


    void start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void Setup(Point gridPos, Vector3 worldPos) {

        this.GridPosition = gridPos;
        transform.position = worldPos;

        BoardManager.Instance.tiles.Add(gridPos, this);

    }
    public void Move(Vector2 newposition)
    {
        transform.position = newposition;
        Rotation();

    }

    public float Rotation()
    {
        Vector3 rot = this.transform.eulerAngles;

        Debug.Log(this.name + this.transform.rotation + "direction " + this.placeDirection);

        return rot.z;
    }

    public Structure East { get => this.E; }
    public Structure West { get => this.W; }
    public Structure North { get => this.N; }
    public Structure South { get => this.S; }
    public Structure Centre { get => this.C; }



    public Structure Up ()
    //what is edge Structure on Up on this instance.
   {       
    
        if (this.placeDirection == Direction.North)
        {
        Debug.Log("Checking Up on " + this.name+" pointing "+this.placeDirection+"  its " + this.North);
        return this.North;
        }
        else if(this.placeDirection == Direction.East)
        {
        Debug.Log("Checking Up on " + this.name+" pointing "+this.placeDirection+"  its " + this.West);
        return this.West;
        }
        else if(this.placeDirection == Direction.South)
        {
        Debug.Log("Checking Up on " + this.name+" pointing "+this.placeDirection+"  its " + this.South);
        return this.South;
        }
        else
        {
        Debug.Log("Checking Up on " + this.name+" pointing "+this.placeDirection+"  its " + this.East);
        return this.East;
        }
    } 


    public Structure Down()
    //what is edge Structure on Down on this instance.
    {        
        if (this.placeDirection == Direction.North)
        {
            Debug.Log("Checking Down on " + this.name+" pointing "+this.placeDirection+"  its " + this.South);
            return this.South;
        }
        else if (this.placeDirection == Direction.East)
        {
            Debug.Log("Checking Down on " + this.name+" pointing "+this.placeDirection+"  its " + this.East);
            return this.East;
        }
        else if (this.placeDirection == Direction.South)
        {
            Debug.Log("Checking Down on " + this.name+" pointing "+this.placeDirection+"  its " + this.North);
            return this.North;
        }
        else
        {
            Debug.Log("Checking Down on " + this.name+" pointing "+this.placeDirection+"  its " + this.West);
            return this.West;
        }
    }
    public Structure Right()
    //what is edge Structure on  Right on this instance.
    {     
        if (this.placeDirection == Direction.North)
        {
            Debug.Log("Checking Right on " + this.name+" pointing "+this.placeDirection+"  its " + this.East);
            return this.East;
        }
        else if (this.placeDirection == Direction.East)
        {
            Debug.Log("Checking Right on " + this.name+" pointing "+this.placeDirection+"  its " + this.North);
            return this.North;
        }
        else if (this.placeDirection == Direction.South)
        {
            Debug.Log("Checking RIght on " + this.name+" pointing "+this.placeDirection+"  its " + this.West);
            return this.West;
        }
        else
        {
            Debug.Log("Checking RIght on " + this.name+" pointing "+this.placeDirection+"  its " + this.South);

            return this.South;
        }
    }
    public Structure Left()
    //what is edge Structure on the Left on this instance.
    {
        if (this.placeDirection == Direction.North)
        {
            Debug.Log("Checking Left on " + this.name+" pointing "+this.placeDirection+"  its " + this.West);

            return this.West;
        }
        else if (this.placeDirection == Direction.East)
        {
            Debug.Log("Checking Left on " + this.name+" pointing "+this.placeDirection+"  its " + this.South);

            return this.South;
        }
        else if (this.placeDirection == Direction.South)
        {
            Debug.Log("Checking Left on " + this.name+" pointing "+this.placeDirection+"  its " + this.East);

            return this.East;
        }
        else
        {
            Debug.Log("Checking Left on " + this.name+" pointing "+this.placeDirection+"  its " + this.North);

            return this.North;
        }
    }
    //for debugging
    public String EdgeString()
    {
        String EdgeString = "";
        EdgeString += "  N: "+this.N.ToString();
        EdgeString += "  E: "+this.E.ToString();
        EdgeString += "  S: "+this.S.ToString();
        EdgeString += "  W: "+this.W.ToString();

        return EdgeString;
    }

    public bool ThroughRoad()
    {
        if (this.S == Structure.Road && this.N == Structure.Road  && this.C != Structure.Village)
        {
            return true;
        }
        else if (this.S == Structure.Road && this.E == Structure.Road && this.C != Structure.Village)
        {
            return true;
        }
        else if (this.S == Structure.Road && this.W == Structure.Road && this.C != Structure.Village)
        {
            return true;
        }
        else if (this.E == Structure.Road && this.W == Structure.Road && this.C != Structure.Village)
        {
            return true;
        }
        else
        {
            return false;
        }
            
    }
    public bool TerminatesRoad()
    {
        if((this.S==Structure.Road || this.N==Structure.Road || this.E==Structure.Road ||this.W == Structure.Road) && !this.ThroughRoad())
        {
            return true;
        }
        else
        {
            return false;
        }
            
    }
    public bool IsMonastry()
    {
        if (this.C == Structure.Monastry)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    
        
    
}

