using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [HideInInspector]
    Card choosenCard;
    const int eleven = 11;
    public GameObject cardPrefab;

    [SerializeField]
    public Text scoreTxt;
    [SerializeField]
    public Text fightTxt;

    List<Card> activeCards = new List<Card>();
    List<Card> disabledCards = new List<Card>();
    public PLAYER _player;
    public Card ChoosenCard { get { return choosenCard; } set { choosenCard = value; isCardChoosen = true; LeaveOnlySelectedCard(); } }
    public FORMATION _FORMATION;

    float fightResult = 0;
    public float FightResult { get { UpdateFightTxt(); return fightResult; } }
    int score = 0;
    public int Score { get { return score; } set { score = value; UpdateScoreTxt(); } }

    bool isHisTurn;
    public bool IsHisTurn { get { return isHisTurn; } set { isHisTurn = value; ChangeCardsSettings(); } }

    POS_LINE _action = POS_LINE.HELP;
    public POS_LINE _ACTION { get { return _action; } set { _action = value; } }

    [HideInInspector]
    public bool isCardChoosen;
    [HideInInspector]
    public bool isFirstStatChoosen;
    [HideInInspector]
    public bool isSecondStatChoosen;
    [HideInInspector]
    public bool isOneStatBlocked;
    [HideInInspector]
    public bool isEngagementChoosen;


    public void ResetPlayer()
    {
        isFirstStatChoosen = false;
        isSecondStatChoosen = false;
        isOneStatBlocked = false;
        isEngagementChoosen = false;
        fightResult = 0;

        choosenCard.ResetCard();
        isCardChoosen = false;
        choosenCard = null;
    }

    void UpdateScoreTxt()
    {
        scoreTxt.text = _player + " " + score;
        scoreTxt.GetComponent<Animator>().SetTrigger("Score");
    }

    public void UpdateFightTxt()
    {
        fightTxt.text = _player + " " + fightResult.ToString("F1");
        fightTxt.GetComponent<Animator>().SetTrigger("Fight");
    }

    public void CreateCards()
    {
        for (int i = 0; i < eleven; i++)
        {
            // create card
            GameObject _cardGO = Instantiate(cardPrefab);
            // get card component
            Card _newCard = _cardGO.GetComponent<Card>();
            // set player reference in card
            _newCard._player = this;
            // set position
            PositionManager.instance.AssignCardToPosition(_player, _FORMATION, _newCard.transform, i);
            // initialize new card
            _newCard.Initialize();
            // add new card to active card list
            activeCards.Add(_newCard);
        }
    }

    public int SelectedCardID()
    {
        return choosenCard.GetInstanceID();
    }

    public void AddChoosenStat(float result)
    {
        fightResult += result;
    }

    void ChangeCardsSettings()
    {
        if (choosenCard != null)
            activeCards.Remove(choosenCard);

        // change cards side
        foreach (Card c in activeCards)
            c.SetCardPosiotionDependsOnSide(isHisTurn);

        // change disable cards side
        foreach (Card c in disabledCards)
            c.SetCardPosiotionDependsOnSide(isHisTurn);

        if (choosenCard != null)
        {
            // change visibility depends on his turn
            choosenCard.ChangeStatsVisible();
        }
    }

    /*****************************************
     * Enable cards
     *****************************************/

    public void EnableCards()
    {
        // if it's not your turn check current player state
        POS_LINE _actionOpponent = GameManager.instance.currentPlayer._ACTION;


        if (isHisTurn)
            EnableAllCards();
        else if (!isHisTurn)
        {
            // if opponent attack first line
            if (_actionOpponent == POS_LINE.HELP)
            {
                EnableCardOnPosition(POS_LINE.HELP);
                EnableCardOnPosition(POS_LINE.ATTACK);
            }  // if opponent attack second line
            else
            {
                EnableCardOnPosition(_actionOpponent);
            }
        }
    }

    public void EnableCardOnPosition(POS_LINE selectedAction)
    {
        for (int i = 0; i < disabledCards.Count; i++)
        {
            if (disabledCards[i]._position._POS_LINE == selectedAction)
            {
                disabledCards[i].EnableCard();
                activeCards.Add(disabledCards[i]);
                disabledCards.RemoveAt(i);
                i--;
            }
        }
    }

    void DisableCardOnPosition(POS_LINE selectedAction)
    {
        for (int i = 0; i < activeCards.Count; i++)
        {
            if (activeCards[i]._position._POS_LINE == selectedAction)
            {
                activeCards[i].DisableCard();
                disabledCards.Add(activeCards[i]);
                activeCards.RemoveAt(i);
                i--;
            }
        }
    }

    // disable cards on begining
    public void Player2CardSetUp()
    {
        DisableCardOnPosition(POS_LINE.DEFENDER);
        DisableCardOnPosition(POS_LINE.GOALKEEPER);
    }

    private void EnableAllCards()
    {
        for (int i = 0; i < disabledCards.Count; i++)
        {
            disabledCards[i].EnableCard();
            activeCards.Add(disabledCards[i]);
            disabledCards.Remove(disabledCards[i]);
            i--;
        }
    }

    public void EnableSelectedStats()
    {
        choosenCard.EnableSelectedStats();
    }

    public void DisableChoosenCard()
    {
        choosenCard.DisableCard();
        activeCards.Remove(choosenCard);
        disabledCards.Add(choosenCard);
    }


    // when player selected card, disable all useless cards on map, leave only selected card
    void LeaveOnlySelectedCard()
    {
        activeCards.Remove(choosenCard);

        for (int i = 0; i < activeCards.Count; i++)
        {
            // disable effect 
            activeCards[i].DisableCard();
            // add to disable list
            disabledCards.Add(activeCards[i]);
            // remove from active list
            activeCards.Remove(activeCards[i]);
            i--;
        }
        activeCards.Add(choosenCard);    // add before removed chose card
    }

    // when game is over cards'll be disabled
    public void DisableAllCards()
    {
        foreach (Card c in activeCards)
            c.DisableCard();
    }
}