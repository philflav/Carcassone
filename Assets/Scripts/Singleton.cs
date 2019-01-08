using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;

    public static T Instance //note: capitalisation indicates property

    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
            }
            return instance;
        }
    }
}

/* See You Tube for explanation
 * https://www.youtube.com/watch?v=ibOBHDgg2kg&list=PLX-uZVK_0K_4uNwvKian1bscP9mVvOp1M&index=12&t=0s
 * */

    
