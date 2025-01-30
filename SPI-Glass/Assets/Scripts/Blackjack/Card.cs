using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string rank;
    public string suit;
    int value;
    int cardListPos;
    string player = "";
    int totalCards;

    public void Update()
    {
        UpdateLocation();
    }

    private void UpdateLocation()
    {
        Transform deck = transform.parent;
        Transform left;
        Transform right;
        int mult = 1;
        if (player.Equals("player"))
        {
            left = deck.Find("PlayerAreaLeft");
            right = deck.Find("PlayerAreaRight");
            //mult = -1;
        }
        else
        {
            left = deck.Find("DealerAreaLeft");
            right = deck.Find("DealerAreaRight");
        }
        float length = math.abs(left.localPosition.x - right.localPosition.x);
        Debug.Log("Length: " + length + " leftX: " + left.localPosition.x + " rigthx: " + right.localPosition.x);
        float xValue = left.localPosition.x - length / (totalCards + 1) * cardListPos * mult;
        transform.localPosition = new Vector3(xValue, left.localPosition.y, left.localPosition.z);
    }

    public void SetLocation(int cardListPos, string player, int totalCards)
    {
        this.cardListPos = cardListPos;
        this.player = player;
        this.totalCards = totalCards;
    }

    public void SetTotalCards(int total)
    {
        totalCards = total;
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
