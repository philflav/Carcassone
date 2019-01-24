using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : Singleton<Hover>


{
    
    public int gridX { get; private set; }
    public int gridY { get; private set; }

    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
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

            gridX = (int)(transform.position.x/GetComponent<SpriteRenderer>().bounds.size.x);
            gridY = (int)(transform.position.y/GetComponent<SpriteRenderer>().bounds.size.y);
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
