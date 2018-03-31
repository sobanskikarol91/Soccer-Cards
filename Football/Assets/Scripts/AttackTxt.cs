using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AttackTxt : MonoBehaviour
{
    Text txt;

    void Start()
    {
        txt = GetComponent<Text>();
        txt.enabled = false;
    }

    public void OtherColumnImpediment(Position attacker, Position defender)
    {
        txt.enabled = true;

        int result = Mathf.Abs((int)attacker._POS_COL - (int)defender._POS_COL);

        if (result == 0 || defender._POS_COL == POS_COLUMN.GOAL_KEEPER)
            SetTxtAndColor(0, Color.green);

        else if (result == 1)
        {
            SetTxtAndColor(EngagementValues.difference1.inRound, Color.yellow);
            GameManager.instance.currentPlayer.ChoosenCard.CardAttackOtherPosition(EENGAGMENT.DIFFERENCE_1);
        }
        else if (result == 2)
        {
            SetTxtAndColor(EngagementValues.difference2.inRound, new Color32(255, 164, 0, 255));
            GameManager.instance.currentPlayer.ChoosenCard.CardAttackOtherPosition(EENGAGMENT.DIFFERENCE_2);
        }
        else
        {
            SetTxtAndColor(EngagementValues.difference3.inRound, Color.red);
            GameManager.instance.currentPlayer.ChoosenCard.CardAttackOtherPosition(EENGAGMENT.DIFFERENCE_3);
        }
    }



    public void OtherLineImpediment(Position attacker, Card _card)
    {
        POS_LINE currentPlayerAction = GameManager.instance.currentPlayer._ACTION;


        if (attacker.NAME == POSITION_NAME.GK)
        {
            SetTxtAndColor(EngagementValues.difference3.inRound, Color.red);
            _card.CardAttackOtherPosition(EENGAGMENT.DIFFERENCE_3);
        }
        else if (currentPlayerAction == POS_LINE.GOALKEEPER)
        {
            // if you attack GOALKEEPER your forwarder 
            if (attacker._POS_LINE == POS_LINE.ATTACK)
                SetTxtAndColor(0, Color.green);
            // if you attack GOALKEEPER your Midfielder 
            else if (attacker._POS_LINE == POS_LINE.HELP)
            {
                SetTxtAndColor(EngagementValues.difference1.inRound, Color.yellow);
                _card.CardAttackOtherPosition(EENGAGMENT.DIFFERENCE_1);
            }
            else if (attacker._POS_LINE == POS_LINE.DEFENDER)
            {
                SetTxtAndColor(EngagementValues.difference2.inRound, new Color32(255, 164, 0, 255));
                _card.CardAttackOtherPosition(EENGAGMENT.DIFFERENCE_2);
            }

            // if this  action is not attack defenders by deffenders
        }
        else if (currentPlayerAction == POS_LINE.DEFENDER)
        {
            // if you attack GOALKEEPER your forwarder 
            if (attacker._POS_LINE != POS_LINE.DEFENDER)
                SetTxtAndColor(0, Color.green);
            // if you attack GOALKEEPER your Midfielder 
            else
            {
                SetTxtAndColor(EngagementValues.difference1.inRound, Color.yellow);
                _card.CardAttackOtherPosition(EENGAGMENT.DIFFERENCE_1);
            }
            // if this  action is not attack defenders by deffenders
        }
        else
            SetTxtAndColor(0, Color.green);

        txt.enabled = true;


    }

    void SetTxtAndColor(float value, Color32 color)
    {
        value *= 100;
        txt.color = color;
        txt.text = (int)value + "%\n your stats";
    }

    public void NotSelected()
    {
        if (txt.enabled == false) return;
        txt.enabled = false;
        GameManager.instance.currentPlayer.ChoosenCard.EnemyDidntChosecard();
    }

    public void NotSelectedHighlightedCard(Card card)
    {
        txt.enabled = false;
        card.EnemyDidntChosecard();
    }

    public void DisableTxt()
    {
        txt.enabled = false;
    }
}
