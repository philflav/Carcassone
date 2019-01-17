using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float cameraSpeed = 1;

    private float maxZoom = 30;
    private float minZoom = 2;

    private float xMax;
    private float yMin;
      
    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        if (Input.GetKey(KeyCode.W))
	    {
            transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime);
	    }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * cameraSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * cameraSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime);
        }
           if (Input.GetKey(KeyCode.X))
        {
            Camera.main.orthographicSize = Math.Min(Camera.main.orthographicSize+1 ,maxZoom);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            Camera.main.orthographicSize = Math.Max(minZoom,Camera.main.orthographicSize-1);

        }
    }

    public void SetLimits(Vector3 maxTile)
    {
        Vector3 wp = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));

        xMax = maxTile.x - wp.x; 
        yMin = maxTile.y - wp.y;

    }
}
