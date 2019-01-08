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
    public enum Direction { North, South, East, West}
    [SerializeField]
    Structure N, E, S, W, C;  //edges and centre of tile 

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
    }

    public void RotateEdges90()
    //this function cycles edge properties by 90 in CW direction
    {
        Structure tmp = new Structure();
        tmp = this.N;
        this.N = this.E;
        this.E = this.S;
        this.S = this.W;
        this.W = tmp;

    }
    public void RotateEdges180()
    {
        RotateEdges90();
        RotateEdges90();
    }

    public void RotateEdges270()
    {
        RotateEdges90();
        RotateEdges90();
        RotateEdges90();
    }

    public Structure East { get => this.E; }
    public Structure West { get => this.W; }
    public Structure North { get => this.N; }
    public Structure South{ get => this.S; }
    public Structure Centre { get => this.C; }



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
