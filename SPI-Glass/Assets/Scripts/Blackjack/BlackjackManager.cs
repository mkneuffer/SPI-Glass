using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Parsed;
using TMPro;
using UnityEngine;

public class BlackjackManager : MonoBehaviour
{
    [SerializeField] Deck deck;
    List<Card> playerHand = new List<Card>();
    List<Card> dealerHand = new List<Card>();
    [SerializeField] private TextMeshProUGUI playerHandText;
    [SerializeField] private TextMeshProUGUI dealerHandText;


    void Start()
    {
        DealHands();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            DealHands();
        }
    }

    private void DealHands()
    {
        playerHand.Add(deck.DrawCard());
        playerHand.Add(deck.DrawCard());

        dealerHand.Add(deck.DrawCard());
        dealerHand.Add(deck.DrawCard());

        playerHandText.text = "Player's Hand: " + HandToString(playerHand);
        dealerHandText.text = "Dealer's Hand: " + HandToString(dealerHand);


        Debug.Log("Player's Hand: " + HandToString(playerHand));
        Debug.Log("Dealer's Hand: " + HandToString(dealerHand));
    }

    private string HandToString(List<Card> hand)
    {
        string output = "";
        foreach (Card card in hand)
        {
            output += card.toString() + ", ";
        }
        return output;
    }
}
