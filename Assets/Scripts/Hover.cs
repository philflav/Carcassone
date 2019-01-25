using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hover : Singleton<Hover>


{
    
    public int gridX { get; private set; }
    public int gridY { get; private set; }

    public float tileSizeX;
    public float tileSizeY;

    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        tileSizeX = GetComponent<SpriteRenderer>().bounds.size.x;
        tileSizeY = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        if (spriteRenderer.enabled)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            if (transform.position.x > 0)
            {
            gridX = (int)((transform.position.x+tileSizeX/2)/tileSizeX);
            }
            else
            {
                gridX = (int)((transform.position.x - tileSizeX / 2) / tileSizeX);
            }
            if (transform.position.y > 0)
            {
                gridY = (int)((transform.position.y + tileSizeY / 2) / tileSizeY);
            }
            else
            {
                gridY = (int)((transform.position.y - tileSizeY / 2) / tileSizeY);
            }
            //show it on screen
            GameObject.Find("Tile").GetComponent<Text>().text = gridX.ToString() + " " + gridY.ToString();
        }
    }


    public void Activate(Sprite sprite)
    {
        this.spriteRenderer.sprite = sprite;
        this.spriteRenderer.sortingOrder = 1;
        this.spriteRenderer.enabled = true;
    }
    public void Rotate(Sprite sprite, int direction)
    {
        this.spriteRenderer.transform.eulerAngles = new Vector3(0, 0, -90 * direction);     
    }

    public void Deactivate()
    {
        this.spriteRenderer.enabled = false;
  
    }

}
