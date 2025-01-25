using System;
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
    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private TextMeshProUGUI gameEndText;
    [SerializeField] private GameObject HitButton;
    [SerializeField] private GameObject StandButton;




    void Start()
    {
        deck.CreateDeck();
        gameEndPanel.SetActive(false);
        DealHands();
    }

    //Deals two cards to both the dealer and the player
    private void DealHands()
    {
        playerHand.Add(deck.DrawCard());
        playerHand.Add(deck.DrawCard());

        dealerHand.Add(deck.DrawCard());
        dealerHand.Add(deck.DrawCard());

        UpdateHandsDisplay(false);
    }

    public void Hit()
    {
        playerHand.Add(deck.DrawCard());
        UpdateHandsDisplay(false);
        if (SumOfHand(playerHand) >= 21)
        {
            DealersTurn();
        }
    }

    public void DealersTurn()
    {
        UpdateHandsDisplay(true);
        while (SumOfHand(dealerHand) < 17)
        {
            dealerHand.Add(deck.DrawCard());
        }
        EndGame();
    }

    private void EndGame()
    {
        HitButton.SetActive(false);
        StandButton.SetActive(false);
        UpdateHandsDisplay(true);
        int playerScore = SumOfHand(playerHand);
        int dealerScore = SumOfHand(dealerHand);
        bool playerBusts = playerScore > 21;
        bool dealerBusts = dealerScore > 21;
        if (playerScore == dealerScore)
        {
            gameEndText.text = "DRAW";
        }
        else if (playerBusts && dealerBusts)
        {
            gameEndText.text = "DRAW";
        }
        else if (playerBusts && !dealerBusts)
        {
            gameEndText.text = "YOU LOSE";
        }
        else if (!playerBusts && dealerBusts)
        {
            gameEndText.text = "YOU WIN";
        }
        else if (playerScore > dealerScore)
        {
            gameEndText.text = "YOU WIN";
        }
        else
        {
            gameEndText.text = "YOU LOSE";
        }
        gameEndPanel.SetActive(true);
    }

    private void UpdateHandsDisplay(bool displayDealersWholeHand)
    {
        playerHandText.text = "Sum=" + SumOfHand(playerHand) + "\nPlayer's Hand: " + HandToString(playerHand);

        if (displayDealersWholeHand)
        {
            dealerHandText.text = "Sum=" + SumOfHand(dealerHand) + "\nDealer's Hand: " + HandToString(dealerHand);
        }
        else
        {
            dealerHandText.text = "Sum=" + SumOfHand(dealerHand) + "\nDealer's Hand: " + dealerHand[0].toString();
        }
    }



    private int SumOfHand(List<Card> hand)
    {
        int sum = 0;
        int aceCount = 0;
        foreach (Card card in hand)
        {
            if (card.getValue() == 11)
            {
                aceCount++;
            }
            sum += card.getValue();
        }
        while (aceCount > 0 && sum > 21)
        {
            sum -= 10;
            aceCount--;
        }
        return sum;
    }

    public void ResetGame()
    {
        HitButton.SetActive(true);
        StandButton.SetActive(true);
        gameEndPanel.SetActive(false);
        deck.CreateDeck();
        playerHand.Clear();
        dealerHand.Clear();
        DealHands();
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
