using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Ink.Parsed;
using TMPro;
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

    [SerializeField] private TextMeshProUGUI playerHandText;
    [SerializeField] private TextMeshProUGUI dealerHandText;
    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private TextMeshProUGUI gameEndText;
    [SerializeField] private GameObject HitButton;
    [SerializeField] private GameObject StandButton;
    [SerializeField] private ARCameraManager cameraManager;

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

    void Start()
    {
        deck.CreateDeck();
        gameEndPanel.SetActive(false);
        DealHands();
    }

    void Update()
    {
        DisplayPlayerCards();
    }

    private void DisplayPlayerCards()
    {
        playerHandModel[0].transform.position = cameraManager.transform.position + cameraManager.transform.forward;

        playerHandModel[0].transform.LookAt(cameraManager.transform);
        GameObject card1 = playerHandModel[0];
        card1.transform.Rotate(90, 0, 0.0f, Space.Self);
        card1.transform.position = playerHandModel[0].transform.position + card1.transform.forward * .25f;
        card1.transform.Rotate(-90f, 0, 0.0f, Space.Self);
        card1.transform.Rotate(0, 90f, 0.0f, Space.Self);
        card1.transform.position = playerHandModel[0].transform.position + card1.transform.forward * .25f;
        card1.transform.Rotate(0, -90f, 0.0f, Space.Self);

        for (int i = 1; i < playerHandModel.Count; i++)
        {
            if (i <= 2)
            {
                card1.transform.Rotate(0f, -90f, 0.0f, Space.Self);
                playerHandModel[i].transform.position = playerHandModel[0].transform.position + card1.transform.forward * .25f * i;
                card1.transform.Rotate(0f, 90f, 0.0f, Space.Self);
            }
            else if (i == 3)
            {
                card1.transform.Rotate(90, 0, 0.0f, Space.Self);
                playerHandModel[i].transform.position = playerHandModel[0].transform.position + card1.transform.forward * .25f;
                card1.transform.Rotate(-90f, 0, 0.0f, Space.Self);
            }
            else
            {
                playerHandModel[3].transform.LookAt(cameraManager.transform);
                playerHandModel[3].transform.Rotate(0f, -90f, 0.0f, Space.Self);
                playerHandModel[i].transform.position = playerHandModel[3].transform.position + playerHandModel[3].transform.forward * .25f * (i - 3);
                playerHandModel[3].transform.Rotate(0f, 90f, 0.0f, Space.Self);
            }
            playerHandModel[i].transform.LookAt(cameraManager.transform);
        }
    }

    //Deals two cards to both the dealer and the player
    private void DealHands()
    {
        DrawCards("player", true);
        DrawCards("player", true);

        DrawCards("dealer", true);
        DrawCards("dealer", false);

        UpdateHandsDisplay(false);
    }

    public void Hit()
    {
        DrawCards("player", true);
        UpdateHandsDisplay(false);
        if (SumOfHand(playerHand) > 21)
        {
            //DealersTurn();
        }
    }

    public void DealersTurn()
    {
        UpdateHandsDisplay(true);
        //Make corutine later so takes time
        while (SumOfHand(dealerHand) < 17)
        {
            DrawCards("dealer", false);
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

    private void DrawCards(string hand, bool visible)
    {
        Card card = deck.DrawCard();
        GameObject cardModel = CardToModel(card);
        cardModel = Instantiate(cardModel, cameraManager.GetComponent<Transform>().position + Camera.main.transform.forward * 0.5f, Quaternion.identity);
        if (hand.Equals("player"))
        {
            playerHand.Add(card);
            playerHandModel.Add(cardModel);
            cardModel.transform.position = playerHandModel[0].transform.position + new Vector3(.2f, 0, 0) * (playerHandModel.Count - 1);
        }
        else
        {
            dealerHand.Add(card);
            dealerHandModel.Add(cardModel);
        }
        // Quaternion rotation = Quaternion.identity;
        // rotation.Set(Camera.main.transform.rotation.x, Camera.main.transform.rotation.y + 180, Camera.main.transform.rotation.z, rotation.w);
        cardModel.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        cardModel.transform.rotation = cameraManager.transform.rotation;
    }

    private void UpdateHandsDisplay(bool endOfGameDisplay)
    {

        if (endOfGameDisplay)
        {
            playerHandText.text = "Sum=" + SumOfHand(playerHand) + "\nPlayer's Hand: " + HandToString(playerHand);
            dealerHandText.text = "Sum=" + SumOfHand(dealerHand) + "\nDealer's Hand: " + HandToString(dealerHand);
        }
        else
        {
            playerHandText.text = "Player's Hand: " + HandToString(playerHand);
            dealerHandText.text = "Dealer's Hand: " + dealerHand[0].toString();
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
