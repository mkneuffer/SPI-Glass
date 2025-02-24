using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Ink.Parsed;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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

    //[SerializeField] private GameObject gameEndPanel;
    //[SerializeField] private TextMeshProUGUI gameEndText;
    [SerializeField] private GameObject HitButton;
    [SerializeField] private GameObject StandButton;
    [SerializeField] private GameObject ResetButton;
    [SerializeField] private Slider bettingSlider;
    [SerializeField] private TextMeshProUGUI bettingAmountText;
    [SerializeField] private GameObject bettingUIParent;
    [SerializeField] private TextMeshProUGUI chipsAmountText;
    [SerializeField] private TextMeshProUGUI chipsChangeText;
    [SerializeField] private GameObject chipsPanel;
    private int totalChips;
    private int bettingChips;
    [SerializeField] private int startingChips = 500;
    [SerializeField] private int chipsNeededToLose = 250;
    [SerializeField] private int chipsNeededToWin = 1500;


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
    private GameObject table;
    private bool tableActive = false;

    void Start()
    {
        HitButton.SetActive(false);
        StandButton.SetActive(false);
        ResetButton.SetActive(false);
        bettingUIParent.SetActive(false);
        chipsChangeText.text = "";
        totalChips = startingChips;
        chipsPanel.SetActive(false);
        deck.CreateDeck();
        //gameEndPanel.SetActive(false);
    }

    void Update()
    {
        if (!tableActive) //Only set if table is not child already
        {
            table = transform.GetChild(0).gameObject;
        }
        if (table != null && table.activeInHierarchy)
        {
            //Only actives once when the table spawns
            if (!tableActive)
            {
                chipsAmountText.text = "Chips: " + totalChips.ToString();
                chipsPanel.SetActive(true);
                tableActive = true;
                StartBetting();
            }
        }
    }

    //Actually start the round of blackjack
    private void StartPlayingGame()
    {
        HitButton.SetActive(true);
        StandButton.SetActive(true);

        DealHands();
    }

    //Deals two cards to both the dealer and the player
    private void DealHands()
    {
        DrawCards("player");
        DrawCards("player");

        DrawCards("dealer");
        DrawCards("dealer");
    }

    //Player hits once, and draws one card
    //If they bust, then automatically go to dealer
    public void Hit()
    {
        //Prevents the player from spamming the button and drawing more cards after already busting
        if (SumOfHand(playerHand) > 21)
            return;
        DrawCards("player");
        if (SumOfHand(playerHand) > 21)
        {
            DealersTurn();
        }
    }

    //Button needs to call this so it needs to call a coroutine
    //This is "Stand"
    public void DealersTurn()
    {
        StartCoroutine(DealerTurnCoroutine());
    }

    //Dealer's second card flips over and draws cards until they have over 17
    IEnumerator DealerTurnCoroutine()
    {
        dealerHandModel[1].GetComponent<Card>().flipY(180);
        yield return new WaitForSeconds(.25f);
        while (SumOfHand(dealerHand) < 17)
        {
            DrawCards("dealer");
            yield return new WaitForSeconds(.5f);
        }
        EndGame();
    }

    //Get's results of the game and determines who wins
    private void EndGame()
    {
        HitButton.SetActive(false);
        StandButton.SetActive(false);

        int playerScore = SumOfHand(playerHand);
        int dealerScore = SumOfHand(dealerHand);
        bool playerBusts = playerScore > 21;
        bool dealerBusts = dealerScore > 21;
        if (playerScore == dealerScore)
        {
            //gameEndText.text = "DRAW";
            StartCoroutine(AddChips(0));
        }
        else if (playerBusts && dealerBusts)
        {
            //gameEndText.text = "DRAW";
            StartCoroutine(AddChips(0));
        }
        else if (playerBusts && !dealerBusts)
        {
            //gameEndText.text = "YOU LOSE";
            StartCoroutine(AddChips(-bettingChips));

        }
        else if (!playerBusts && dealerBusts)
        {
            //gameEndText.text = "YOU WIN";
            StartCoroutine(AddChips(bettingChips));
        }
        else if (playerScore > dealerScore)
        {
            //gameEndText.text = "YOU WIN";
            StartCoroutine(AddChips(bettingChips));
        }
        else
        {
            //gameEndText.text = "YOU LOSE";
            StartCoroutine(AddChips(-bettingChips));
        }
    }



    //Adds chips over time
    private IEnumerator AddChips(int amount)
    {
        //Sets up string and sets color
        //Losing Chips
        if (amount < 0)
        {
            chipsChangeText.color = Color.red;
            chipsChangeText.text = amount.ToString();
        }
        //Gaining Chips
        else if (amount > 0)
        {
            chipsChangeText.color = Color.green;
            chipsChangeText.text = "+" + amount.ToString();
        }
        //Draw
        else
        {
            chipsChangeText.color = Color.white;
            chipsChangeText.text = "+" + amount.ToString();
        }
        yield return new WaitForSeconds(.75f);

        int chipsChangePerStep = 1;
        if (Math.Abs(amount) > 400)
            chipsChangePerStep = 5;
        else if (Math.Abs(amount) > 250)
            chipsChangePerStep = 5;
        else if (Math.Abs(amount) > 100)
            chipsChangePerStep = 2;

        //Increment/Decrement Chips
        while (totalChips != totalChips + amount)
        {
            totalChips += (int)Mathf.Sign(amount) * chipsChangePerStep;
            amount -= (int)Mathf.Sign(amount) * chipsChangePerStep;

            chipsAmountText.text = "Chips: " + totalChips.ToString();
            if (chipsChangeText.color == Color.red)
                chipsChangeText.text = amount.ToString();
            else
                chipsChangeText.text = "+" + amount.ToString();
            yield return new WaitForSeconds(.0001f);
        }
        chipsChangeText.text = "";

        //Check for win/lose conditions
        if (totalChips >= chipsNeededToWin) //win
        {
            //gameEndPanel.SetActive(true);
            //gameEndText.text = "YOU WIN";
            PlayerPrefs.SetInt("DialogueState", 1); // Win dialogue
            PlayerPrefs.Save();
            StartCoroutine(Quit());
        }
        else if (totalChips <= chipsNeededToLose) //lose
        {
            //gameEndPanel.SetActive(true);
            //gameEndText.text = "YOU LOSE";
            PlayerPrefs.SetInt("DialogueState", 2); // Loss dialogue
            PlayerPrefs.Save();
            StartCoroutine(Quit());
        }
        else
        {
            ResetButton.SetActive(true); //game is still going
        }
    }

    //Stops the game of blackjack
    private IEnumerator Quit()
    {
        yield return new WaitForSeconds(2f);
        foreach (GameObject card in playerHandModel)
        {
            card.GetComponent<Card>().Reset();
        }
        foreach (GameObject card in dealerHandModel)
        {
            card.GetComponent<Card>().Reset();
        }
        yield return new WaitForSeconds(1f);
        foreach (GameObject card in playerHandModel)
        {
            Destroy(card);
        }
        foreach (GameObject card in dealerHandModel)
        {
            Destroy(card);
        }

        yield return new WaitForSeconds(.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    
    //Adds one card to the given hand
    private void DrawCards(string hand)
    {
        Card card = deck.DrawCard();
        GameObject cardModel = CardToModel(card);
        cardModel = Instantiate(cardModel, table.transform.GetChild(0));
        Card cardScript = cardModel.AddComponent<Card>();

        if (hand.Equals("player")) //player
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
        else //dealer
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

    //Get the total value of the given hand
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


    //Restarts the game
    public void ResetGame()
    {
        ResetButton.SetActive(false);
        StartCoroutine(RestartGameCoroutine());
    }

    //Restarts the game
    IEnumerator RestartGameCoroutine()
    {
        //Moves cards back into deck
        foreach (GameObject card in playerHandModel)
        {
            card.GetComponent<Card>().Reset();
        }
        foreach (GameObject card in dealerHandModel)
        {
            card.GetComponent<Card>().Reset();
        }
        yield return new WaitForSeconds(1f);

        //Destroys all card objects
        foreach (GameObject card in playerHandModel)
        {
            Destroy(card);
        }
        foreach (GameObject card in dealerHandModel)
        {
            Destroy(card);
        }

        deck.CreateDeck();
        playerHand.Clear();
        dealerHand.Clear();

        playerHandModel.Clear();
        dealerHandModel.Clear();
        //gameEndPanel.SetActive(false);
        StartBetting();
    }

    //Sets up the betting phase
    private void StartBetting()
    {
        bettingUIParent.SetActive(true);
        bettingSlider.maxValue = totalChips / 10;
        UpdateFromSlider();
    }

    //Updates the betting text given the value of the slider
    public void UpdateFromSlider()
    {
        bettingAmountText.text = "Betting: " + (bettingSlider.value * 10).ToString() + " Chips";
    }

    //Called from "Bet" Button
    public void Bet()
    {
        bettingChips = (int)bettingSlider.value * 10;
        bettingUIParent.SetActive(false);
        StartPlayingGame();
    }

    //Converts the given hand into a readable string
    private string HandToString(List<Card> hand)
    {
        string output = "";
        int index = 0;
        foreach (Card card in hand)
        {
            if (index < hand.Count - 1)
                output += card.toString() + ", ";
            else
                output += card.toString();
            index++;
        }
        return output;
    }

    //Gets the model of the card given the card object
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
