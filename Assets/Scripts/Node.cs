﻿using System;
using System.Collections;
using UnityEngine;

public class Node
{
    public Point GridPosition { get; private set; }

    public TileScript TileRef { get; private set; }

    public Node Parent  {get; private set;}

 
    public Node(TileScript tileRef)
    {
        this.TileRef = tileRef;
        this.GridPosition = tileRef.GridPosition;

    }

 } 
