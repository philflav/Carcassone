using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
     public Stack <GameObject> gamestack;
     public Stack <GameObject> MakeStack(GameObject [] cards)
    {


        gamestack = new Stack <GameObject>();
        int numberOfCards = cards.Length;
        int stackSize = 0; //Stack size will be >= numberOfCards of cards
        int stackCount = 0;

        int[] deckCount = new int[numberOfCards];

        //read repeats and setup number of cars to be sum of repeats
        for (int i = 0; i < numberOfCards; i++)
        {
            deckCount[i] = cards[i].GetComponent<TileScript>().CardsInDeck;
            stackSize += deckCount[i];
            
        }
        while (stackCount < stackSize)
        {
            //choose a random card
            int rnd = UnityEngine.Random.Range(0, numberOfCards);
            GameObject newcard =  cards[rnd];
            //are there still any of this card available
            if (deckCount[rnd]>0)
            {
                gamestack.Push(newcard);
                stackCount++;
                //reduce the count of this card available for adding to stack
                deckCount[rnd]--;
            }
        }

        return  gamestack;
    }

    public GameObject drawCard(Stack <GameObject> gamestack)
        //return next card of stack or null if stack is empty;
    {
        if (gamestack.Count > 0)
        {
            GameObject drawncard = gamestack.Pop();
            return drawncard;
        }
        else
        {
            return null;
        }
    }

}


