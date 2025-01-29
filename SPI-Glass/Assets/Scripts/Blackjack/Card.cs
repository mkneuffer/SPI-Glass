using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string rank;
    public string suit;
    int value;
    int cardListPos;
    string player;

    public void Update()
    {

    }

    public void SetLocation(int cardListPos, string player)
    {
        this.cardListPos = cardListPos;
        this.player = player;
    }

    public void setCard(string suit, string rank)
    {
        this.rank = rank;
        this.suit = suit;
        //Converts the string rank into an int value
        //If fails it's an Ace, King, Queen or Jack and we handle accordingly
        if (!System.Int32.TryParse(rank, out value))
        {
            if (rank.Equals("Ace"))
            {
                value = 11;
            }
            else
            {
                value = 10;
            }
        }

    }

    public string getRank()
    {
        return rank;
    }

    public string getSuit()
    {
        return suit;
    }

    public int getValue()
    {
        return value;
    }

    public string toString()
    {
        return rank;// + " of " + suit;
    }

}
