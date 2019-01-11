using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
     public Stack gamestack;
     public Stack MakeStack(GameObject [] cards)
    {

        gamestack = new Stack();
        int numberOfCards = cards.Length;
        int stackCount = 0;

        while (stackCount < numberOfCards)
        {
            GameObject newcard =  cards[UnityEngine.Random.Range(0, numberOfCards)];

            //only one of each card
            if (!gamestack.Contains(newcard))
            {
                gamestack.Push(newcard);
                stackCount++;
            }
        }
        
        return gamestack;
    }

    public GameObject drawCard(Stack gamestack)
        //return next card of stack or null if stack is empty;
    {
        if (gamestack.Count > 1)
        {
            GameObject drawncard = (GameObject)gamestack.Pop();
            return drawncard;
        }
        else
        {
            return null;
        }
    }

}


