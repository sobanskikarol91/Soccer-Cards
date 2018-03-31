using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isCurtain;
    public bool message;
    public bool reflectors;
    public static GameManager instance;
    public CurtainController curtain;
    public STATES CURRENT_STATE;
    public Player Player1;
    public Player Player2;
    [HideInInspector]
    public Player currentPlayer;
    [HideInInspector]
    public Player opponentPlayer;
    const int roundNr = 12;

    public Text roundTxt;
    public GameObject gameOverPanel;
    public GameObject formationPanel;
    public Text formationPaneltxt;


    public LightManager _lightManager;
    #region Players Actions
    public Card ChooseCard { get { return currentPlayer.ChoosenCard; } set { currentPlayer.ChoosenCard = value; } }
    public bool BlockOpponentStat { get { return opponentPlayer.isOneStatBlocked; } set { opponentPlayer.isOneStatBlocked = value; } }
    public bool ChooseEngagement { get { return currentPlayer.isEngagementChoosen; } set { currentPlayer.isEngagementChoosen = value; } }
    public bool ChooseFirstStat { get { return currentPlayer.isFirstStatChoosen; } set { currentPlayer.isFirstStatChoosen = value; } }
    public bool ChooseSecondStat { get { return currentPlayer.isSecondStatChoosen; } set { currentPlayer.isSecondStatChoosen = value; } }
    public Card ChooseEnemyCard { get { return opponentPlayer.ChoosenCard; } set { opponentPlayer.ChoosenCard = value; } }
    #endregion


    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        GameEnd();
    }

    IEnumerator RoundStarting()
    {
        Messages.instance.ShowMessage = message;
        // set players order


        currentPlayer = Player1;
        while (currentPlayer._FORMATION == FORMATION.NONE) yield return null;

        formationPaneltxt.text = "Player 2 Formation:";

        currentPlayer = Player2;
        while (currentPlayer._FORMATION == FORMATION.NONE) yield return null;


        formationPanel.SetActive(false);
        
        currentPlayer = Player1;
        opponentPlayer = Player2;

        currentPlayer.CreateCards();
        opponentPlayer.CreateCards();


        if (reflectors)
            yield return StartCoroutine(_lightManager.IETurnOnReflectors());

        yield return new WaitForSeconds(0.5f);
        SoundManager.instance.Whistle();
        yield return new WaitForSeconds(0.3f);
        if (isCurtain)
        {
            // wait to not register last click mouse
            yield return null;
            curtain.gameObject.SetActive(true);
            curtain.ChangeColor(Color.red);
            yield return StartCoroutine(curtain.CheckCurtainDisable());
        }


        

        currentPlayer.IsHisTurn = true;
        opponentPlayer.IsHisTurn = false;

        // disable cards on begining
        opponentPlayer.Player2CardSetUp();

        yield return null;
    }

    IEnumerator RoundPlaying()
    {

        for (int i = 1; i <= roundNr; i++)
        {
            bool endTurn = false;
            UpdateRoundTxt(i);

            while (!endTurn)
            {
                currentPlayer.EnableCards();
                opponentPlayer.EnableCards();

                // referee whistle 
                if(i>1) SoundManager.instance.Whistle();

                // first player select his card
                CURRENT_STATE = STATES.CHOOSE_OWN_CARD;
                Messages.instance.SelectYourCard(currentPlayer.name);
                while (!currentPlayer.isCardChoosen) yield return null;

                // first player select enemy card to attack
                CURRENT_STATE = STATES.CHOOSE_ENEMY_CARD;
                Messages.instance.SelectEnemyCard(currentPlayer.name);
                while (!opponentPlayer.isCardChoosen) yield return null;

                // first player block enemy stat
                CURRENT_STATE = STATES.BLOCK_STAT;
                Messages.instance.BlockEnemy(currentPlayer.name);
                while (!opponentPlayer.isOneStatBlocked) yield return null;

                // change Players
               yield return StartCoroutine(ChangePlayers());

                // second player block enemy stat
                Messages.instance.BlockEnemy(currentPlayer.name);
                while (!opponentPlayer.isOneStatBlocked) yield return null;

                // second player choose engagement
                CURRENT_STATE = STATES.CHOOSE_ENGAGEMENT;
                currentPlayer.ChoosenCard.ec.ShowEngagementPanel();
                Messages.instance.ChooseEngagement(currentPlayer.name);
                while (!currentPlayer.isEngagementChoosen) yield return null;

                // second player choose 2 Stats
                CURRENT_STATE = STATES.CHOOSE_STATS;
                Messages.instance.Choose2Stats(currentPlayer.name);
                while (!currentPlayer.isFirstStatChoosen || !currentPlayer.isSecondStatChoosen) yield return null;

                // unblock selected stats
                yield return new WaitForSeconds(0.3f);
                // disable selected card in black rectangle
                currentPlayer.EnableSelectedStats();

                // Show stats without Engagement 
                currentPlayer.ChoosenCard.ec.EnemyWithoutEngagement();
                // change Players
                yield return StartCoroutine(ChangePlayers());

                // first player choose engagement
                CURRENT_STATE = STATES.CHOOSE_ENGAGEMENT;
                currentPlayer.ChoosenCard.ec.ShowEngagementPanel();
                Messages.instance.ChooseEngagement(currentPlayer.name);
                while (!currentPlayer.isEngagementChoosen) yield return null;

                // first player choose 2 Stats
                CURRENT_STATE = STATES.CHOOSE_STATS;
                Messages.instance.Choose2Stats(currentPlayer.name);
                while (!currentPlayer.isFirstStatChoosen || !currentPlayer.isSecondStatChoosen) yield return null;


                // Show real oponnent's engagement
                opponentPlayer.ChoosenCard.ec.ShowEnemyRealEngagement();
                currentPlayer.ChoosenCard.ec.ShowEnemyRealEngagement();
                yield return new WaitForSeconds(0.3f);

                // Compare choosen stats by both player
                float currentResult = currentPlayer.FightResult;
                float opponentResult = opponentPlayer.FightResult;

                bool DidHeWin = currentResult > opponentResult;

                if (DidHeWin)
                {
                    // if player defeted GoalKeeper he get point
                    if (currentPlayer._ACTION == POS_LINE.GOALKEEPER)
                    {
                        SoundManager.instance.Goal();

                        currentPlayer.Score++;
                        currentPlayer._ACTION = POS_LINE.HELP;
                        endTurn = true;
                        yield return new WaitForSeconds(0.5f);
                    }
                    else  // if attacker player not defeted GoalKeeper yet he can attack next line
                    {
                        SoundManager.instance.Cheering();
                        currentPlayer._ACTION++;
                    }
                }
                else
                {
                    SoundManager.instance.Miss();
                    currentPlayer._ACTION = POS_LINE.HELP;
                    endTurn = true;
                }
                // time after battle to see the result
                yield return new WaitForSeconds(1f);

                // Anumation returned Cards
                currentPlayer.ChoosenCard.ReturnCard();
                opponentPlayer.ChoosenCard.ReturnCard();

                // time to return cards
                yield return new WaitForSeconds(1.0f);


                currentPlayer.DisableChoosenCard();
                opponentPlayer.DisableChoosenCard();
                yield return new WaitForSeconds(0.7f);
                currentPlayer.ResetPlayer();
                opponentPlayer.ResetPlayer();
            }
            if (i < roundNr) StartCoroutine(ChangePlayers());
        }
    }

    void GameEnd()
    {
        gameOverPanel.SetActive(true);
        CheckWinner();
        DisablePlayersCards();
        Messages.instance.ShowMessage = false;
    }

    IEnumerator ChangePlayers()
    {
        // change players sides
        Player safe = currentPlayer;
        currentPlayer = opponentPlayer;
        opponentPlayer = safe;

        currentPlayer.IsHisTurn = !currentPlayer.IsHisTurn;
        opponentPlayer.IsHisTurn = !opponentPlayer.IsHisTurn;

        if (isCurtain)
        {
            curtain.gameObject.SetActive(true);

            if (currentPlayer._player == PLAYER.P1)
                curtain.GetComponent<Image>().color = Color.red;
            else
                curtain.GetComponent<Image>().color = new Color32(0, 128, 255, 255);

            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(curtain.CheckCurtainDisable());
        }
    }

    public bool CheckIfPlayerChoseThisCard(int cardId)
    {
        if (!currentPlayer.isCardChoosen) return false;           // card hasn't selected yet
        if (currentPlayer.SelectedCardID() == cardId) return true;

        return false;
    }

    public bool CheckCurrentState(STATES _STATE)
    {
        if (CURRENT_STATE == _STATE) return true;
        return false;
    }

    void UpdateRoundTxt(int current)
    {
        roundTxt.text = "Round: " + current + "/" + roundNr;
    }

    void CheckWinner()
    {
        Text winerTxt = gameOverPanel.GetComponent<Text>();

        if (Player1.Score > Player2.Score)
            winerTxt.text = "Player 1 wins!";
        else if (Player1.Score < Player2.Score)
            winerTxt.text = "Player 2 wins!";

        else
            winerTxt.text = "Draw!";

        winerTxt.enabled = true;
    }

    void DisablePlayersCards()
    {
        currentPlayer.DisableAllCards();
        opponentPlayer.DisableAllCards();
    }
}