using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    string[] cardSuits = { "Hearts", "Spades", "Diamonds", "Clubs" };
    string[] cardRanks = { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };
    List<Card> deck = new List<Card>();
    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        CreateDeck();
    }

    public void CreateDeck()
    {
        foreach (string suit in cardSuits)
        {
            foreach (string rank in cardRanks)
            {
                Card card = new Card();
                card.setCard(suit, rank);
                deck.Add(card);
            }
        }
        deck = deck.OrderBy(x => Random.value).ToList();
    }

    public Card DrawCard()
    {
        if (deck.Count <= 0)
        {
            Debug.Log("Deck is empty");
            return null;
        }
        Card topCard = deck[0];
        deck.Remove(deck[0]);
        count++;
        return topCard;
    }

    public int getCount()
    {
        return count;
    }
}
