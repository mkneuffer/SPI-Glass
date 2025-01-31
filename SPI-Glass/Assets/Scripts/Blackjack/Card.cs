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
    Vector3 nextPosition;
    float speed = 100;

    public void Update()
    {
        if (Vector3.Distance(nextPosition, transform.localPosition) > .5f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextPosition, speed * Time.deltaTime);
        }
    }

    public void GetNextLocation()
    {
        Transform deck = transform.parent;
        Transform left;
        Transform right;
        if (player.Equals("player"))
        {
            left = deck.Find("PlayerAreaLeft");
            right = deck.Find("PlayerAreaRight");
        }
        else
        {
            left = deck.Find("DealerAreaLeft");
            right = deck.Find("DealerAreaRight");
        }
        float length = math.abs(left.localPosition.x - right.localPosition.x);
        float xValue = left.localPosition.x - length / (totalCards + 1) * cardListPos;
        nextPosition = new Vector3(xValue, left.localPosition.y, left.localPosition.z);
    }

    public void InitialSetLocation(int cardListPos, string player, int totalCards)
    {
        this.cardListPos = cardListPos;
        this.player = player;
        this.totalCards = totalCards;
        GetNextLocation();
    }

    //public void SetLocation()



    public void SetTotalCards(int total)
    {
        totalCards = total;
        GetNextLocation();
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
