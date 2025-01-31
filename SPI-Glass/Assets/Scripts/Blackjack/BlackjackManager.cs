using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Ink.Parsed;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class BlackjackManager : MonoBehaviour
{
    [SerializeField] Deck deck;
    List<Card> playerHand = new List<Card>();
    List<Card> dealerHand = new List<Card>();
    List<GameObject> playerHandModel = new List<GameObject>();
    List<GameObject> dealerHandModel = new List<GameObject>();

    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private TextMeshProUGUI gameEndText;
    [SerializeField] private GameObject HitButton;
    [SerializeField] private GameObject StandButton;
    [SerializeField] private GameObject ResetButton;

    [Header("Cards")]
    [SerializeField] private GameObject twoClub;
    [SerializeField] private GameObject twoDiamond;
    [SerializeField] private GameObject twoHeart;
    [SerializeField] private GameObject twoSpade;
    [SerializeField] private GameObject threeClub;
    [SerializeField] private GameObject threeDiamond;
    [SerializeField] private GameObject threeHeart;
    [SerializeField] private GameObject threeSpade;
    [SerializeField] private GameObject fourClub;
    [SerializeField] private GameObject fourDiamond;
    [SerializeField] private GameObject fourHeart;
    [SerializeField] private GameObject fourSpade;
    [SerializeField] private GameObject fiveClub;
    [SerializeField] private GameObject fiveDiamond;
    [SerializeField] private GameObject fiveHeart;
    [SerializeField] private GameObject fiveSpade;
    [SerializeField] private GameObject sixClub;
    [SerializeField] private GameObject sixDiamond;
    [SerializeField] private GameObject sixHeart;
    [SerializeField] private GameObject sixSpade;
    [SerializeField] private GameObject sevenClub;
    [SerializeField] private GameObject sevenDiamond;
    [SerializeField] private GameObject sevenHeart;
    [SerializeField] private GameObject sevenSpade;
    [SerializeField] private GameObject eightClub;
    [SerializeField] private GameObject eightDiamond;
    [SerializeField] private GameObject eightHeart;
    [SerializeField] private GameObject eightSpade;
    [SerializeField] private GameObject nineClub;
    [SerializeField] private GameObject nineDiamond;
    [SerializeField] private GameObject nineHeart;
    [SerializeField] private GameObject nineSpade;
    [SerializeField] private GameObject tenClub;
    [SerializeField] private GameObject tenDiamond;
    [SerializeField] private GameObject tenHeart;
    [SerializeField] private GameObject tenSpade;
    [SerializeField] private GameObject aceClub;
    [SerializeField] private GameObject aceDiamond;
    [SerializeField] private GameObject aceHeart;
    [SerializeField] private GameObject aceSpade;
    [SerializeField] private GameObject jackClub;
    [SerializeField] private GameObject jackDiamond;
    [SerializeField] private GameObject jackHeart;
    [SerializeField] private GameObject jackSpade;
    [SerializeField] private GameObject kingClub;
    [SerializeField] private GameObject kingDiamond;
    [SerializeField] private GameObject kingHeart;
    [SerializeField] private GameObject kingSpade;
    [SerializeField] private GameObject queenClub;
    [SerializeField] private GameObject queenDiamond;
    [SerializeField] private GameObject queenHeart;
    [SerializeField] private GameObject queenSpade;
    [SerializeField] private GameObject cardBox;
    private bool gameRunning;
    private GameObject table;
    private bool tableActive = false;

    void Start()
    {
        HitButton.SetActive(false);
        StandButton.SetActive(false);
        ResetButton.SetActive(false);

        deck.CreateDeck();
        gameEndPanel.SetActive(false);
        gameRunning = true;
    }

    void Update()
    {
        if (!tableActive) //Only set if table is not child already
        {
            table = transform.GetChild(0).gameObject;
        }
        if (table != null && table.activeInHierarchy)
        {
            if (!tableActive)
            {
                HitButton.SetActive(true);
                StandButton.SetActive(true);
                ResetButton.SetActive(true);
                tableActive = true;
                DealHands();
            }
        }
    }

    //Deals two cards to both the dealer and the player
    private void DealHands()
    {
        gameRunning = true;
        DrawCards("player", true);
        DrawCards("player", true);

        DrawCards("dealer", true);
        DrawCards("dealer", false);
    }

    public void Hit()
    {
        DrawCards("player", true);
        if (SumOfHand(playerHand) > 21)
        {
            DealersTurn();
        }
    }

    public void DealersTurn()
    {
        //Make corutine later so takes time
        while (SumOfHand(dealerHand) < 17)
        {
            DrawCards("dealer", false);
        }
        EndGame();
    }

    private void EndGame()
    {
        gameRunning = false;
        HitButton.SetActive(false);
        StandButton.SetActive(false);
        dealerHandModel[1].GetComponent<Card>().flipY(180);
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

    private void DrawCards(string hand, bool visible)
    {
        Card card = deck.DrawCard();
        GameObject cardModel = CardToModel(card);
        cardModel = Instantiate(cardModel, table.transform.GetChild(0));
        Card cardScript = cardModel.AddComponent<Card>();

        if (hand.Equals("player"))
        {
            playerHand.Add(card);
            playerHandModel.Add(cardModel);
            cardScript.InitialSetLocation(playerHand.Count, "player", playerHand.Count);
            foreach (GameObject card1 in playerHandModel)
            {
                Card c = card1.GetComponent<Card>();
                c.SetTotalCards(playerHand.Count);
            }
        }
        else
        {
            dealerHand.Add(card);
            dealerHandModel.Add(cardModel);
            cardScript.InitialSetLocation(dealerHand.Count, "dealer", dealerHand.Count);
            if (dealerHand.Count == 2)
            {
                cardModel.transform.Rotate(0, 180, 0);
            }
            foreach (GameObject card1 in dealerHandModel)
            {
                Card c = card1.GetComponent<Card>();
                c.SetTotalCards(dealerHand.Count);
            }
        }
        cardModel.transform.localScale = new Vector3(150, 150, 150);
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
        foreach (GameObject card in playerHandModel)
        {
            Destroy(card);
        }
        foreach (GameObject card in dealerHandModel)
        {
            Destroy(card);
        }
        playerHandModel.Clear();
        dealerHandModel.Clear();
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

    private GameObject CardToModel(Card card)
    {
        if (card.getSuit() == "Hearts")
        {
            switch (card.getRank())
            {
                case "Ace":
                    return aceHeart;
                case "2":
                    return twoHeart;
                case "3":
                    return threeHeart;
                case "4":
                    return fourHeart;
                case "5":
                    return fiveHeart;
                case "6":
                    return sixHeart;
                case "7":
                    return sevenHeart;
                case "8":
                    return eightHeart;
                case "9":
                    return nineHeart;
                case "10":
                    return tenHeart;
                case "Jack":
                    return jackHeart;
                case "Queen":
                    return queenHeart;
                case "King":
                    return kingHeart;
                default:
                    Debug.Log("Error displying card: Attempting to display " + card.getRank() + " of " + card.getSuit());
                    return cardBox;
            }
        }
        else if (card.getSuit() == "Clubs")
        {
            switch (card.getRank())
            {
                case "Ace":
                    return aceClub;
                case "2":
                    return twoClub;
                case "3":
                    return threeClub;
                case "4":
                    return fourClub;
                case "5":
                    return fiveClub;
                case "6":
                    return sixClub;
                case "7":
                    return sevenClub;
                case "8":
                    return eightClub;
                case "9":
                    return nineClub;
                case "10":
                    return tenClub;
                case "Jack":
                    return jackClub;
                case "Queen":
                    return queenClub;
                case "King":
                    return kingClub;
                default:
                    Debug.Log("Error displying card: Attempting to display " + card.getRank() + " of " + card.getSuit());
                    return cardBox;
            }
        }
        else if (card.getSuit() == "Diamonds")
        {
            switch (card.getRank())
            {
                case "Ace":
                    return aceDiamond;
                case "2":
                    return twoDiamond;
                case "3":
                    return threeDiamond;
                case "4":
                    return fourDiamond;
                case "5":
                    return fiveDiamond;
                case "6":
                    return sixDiamond;
                case "7":
                    return sevenDiamond;
                case "8":
                    return eightDiamond;
                case "9":
                    return nineDiamond;
                case "10":
                    return tenDiamond;
                case "Jack":
                    return jackDiamond;
                case "Queen":
                    return queenDiamond;
                case "King":
                    return kingDiamond;
                default:
                    Debug.Log("Error displying card: Attempting to display " + card.getRank() + " of " + card.getSuit());
                    return cardBox;
            }
        }
        else if (card.getSuit() == "Spades")
        {
            switch (card.getRank())
            {
                case "Ace":
                    return aceSpade;
                case "2":
                    return twoSpade;
                case "3":
                    return threeSpade;
                case "4":
                    return fourSpade;
                case "5":
                    return fiveSpade;
                case "6":
                    return sixSpade;
                case "7":
                    return sevenSpade;
                case "8":
                    return eightSpade;
                case "9":
                    return nineSpade;
                case "10":
                    return tenSpade;
                case "Jack":
                    return jackSpade;
                case "Queen":
                    return queenSpade;
                case "King":
                    return kingSpade;
                default:
                    Debug.Log("Error displying card: Attempting to display " + card.getRank() + " of " + card.getSuit());
                    return cardBox;
            }
        }
        else
        {
            Debug.Log("Error displying card: Attempting to display " + card.getRank() + " of " + card.getSuit());
            return cardBox;
        }
    }
}
