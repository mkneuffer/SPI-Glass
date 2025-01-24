using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string rank;
    public string suit;

    public void setCard(string suit, string rank)
    {
        this.rank = rank;
        this.suit = suit;
    }

    public string getRank()
    {
        return rank;
    }

    public string getSuit()
    {
        return suit;
    }

    public string toString()
    {
        return rank + " of " + suit;
    }

}
