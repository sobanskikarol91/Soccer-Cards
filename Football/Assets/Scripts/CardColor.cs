using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardColor
{
    public static Color attacker = Color.red;
    public static Color helper = Color.cyan;
    public static Color defender = Color.yellow;
    public static Color goalKeeper = Color.green;
    public static Color opponentCard = Color.black;
    public static Color ownCard = Color.white;

    public static Color AssignColorToFootballer(POS_LINE p)
    {
        if (p == POS_LINE.ATTACK)
            return attacker;
        else if (p ==POS_LINE.HELP)
          return helper;
        else if (p == POS_LINE.DEFENDER)
            return defender;
        else  return goalKeeper;
    }
}


