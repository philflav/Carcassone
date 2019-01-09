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

   public float Rotation()
    {
        Vector3 rot =  this.transform.eulerAngles;      
        
        Debug.Log(this.name + " :rot = " + rot.z);

        return rot.z;
    }

        


    public Structure East { get => this.E; }
    public Structure West { get => this.W; }
    public Structure North { get => this.N; }
    public Structure South{ get => this.S; }
    public Structure Centre { get => this.C; }

    public Structure Up()
    //what is edge Structure on Up on this instance.
    {
        float rotation = transform.rotation.eulerAngles.z;
    Debug.Log("Checking Up on " + this.name + " rotation "+ rotation);
        if (rotation <45)
        {
            return this.North;
        }
        else if(rotation > 45 && rotation < 135)
        {
            return this.West;
        }
        else if(rotation > 135 && rotation < 215)
        {
            return this.South;
        }
        else
        {
            return this.East;
        }
    }
    public Structure Down()
    //what is edge Structure on Down on this instance.
    {
        float rotation = transform.rotation.eulerAngles.z;
        Debug.Log("Checking Down on " + this.name + " rotation " + rotation);
        if (rotation < 45)
        {
            return this.South;
        }
        else if (rotation > 45 && this.Rotation() < 135)
        {
            return this.East;
        }
        else if (rotation> 135 && this.Rotation() < 215)
        {
            return this.North;
        }
        else
        {
            return this.West;
        }
    }
    public Structure Right()
    //what is edge Structure on  Right on this instance.
    {
        float rotation = transform.rotation.eulerAngles.z;
        Debug.Log("Checking Right on " + this.name + " rotation " + rotation);

        if (rotation < 45)
        {
            return this.East;
        }
        else if (rotation > 45 && this.Rotation() < 135)
        {
            return this.North;
        }
        else if (rotation > 135 && this.Rotation() < 215)
        {
            return this.West;
        }
        else
        {
            return this.South;
        }
    }
    public Structure Left()
    //what is edge Structure on the Left on this instance.
    {
        float rotation = transform.rotation.eulerAngles.z;
        Debug.Log("Checking Left on " + this.name + " rotation " + rotation);

        if (rotation < 45)
        {
            return this.West;
        }
        else if (rotation > 45 && this.Rotation() < 135)
        {
            return this.South;
        }
        else if (rotation > 135 && this.Rotation() < 215)
        {
            return this.East;
        }
        else
        {
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
