using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
     public List<GameObject> deck = new List<GameObject>();
     public Stack <GameObject> gamestack;
     public Stack<GameObject> MakeStack(GameObject[] cards)
    {


        gamestack = new Stack <GameObject>();

        //for each card type
        for (int i = 0; i < cards.Length; i++)
        {
            //add the correct number of cards to the deck

            for (int j = 0; j < cards[i].GetComponent<TileScript>().CardsInDeck; j++)
            {
                deck.Add(cards[i]);
            }
        }
        //now shuffle
        while (deck.Count>0)
        {
            //choose a random card
            int rnd = UnityEngine.Random.Range(0, deck.Count);
            gamestack.Push(deck[rnd]);
            //now remove it from the list
            deck.RemoveAt(rnd);

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
public static class StackExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var temp = list[i];
            var index = UnityEngine.Random.Range(0, list.Count);
            list[i] = list[index];
            list[index] = temp;
        }
    }
}


