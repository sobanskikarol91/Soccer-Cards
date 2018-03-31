using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    public POSITION_NAME NAME;  
    public POS_LINE _POS_LINE;
    public POS_COLUMN _POS_COL;
    public int nr;

    public void AssignLineAndColumn()
    {
        string _posName = NAME.ToString();

        if (_posName.Length == 3)
        {
            // Columns
            if (_posName[1] == 'R') _POS_COL = POS_COLUMN.CENTER_RIGHT;
            else _POS_COL = POS_COLUMN.CENTER_LEFT;


            // Line
            if (_posName[2] == 'F') _POS_LINE = POS_LINE.ATTACK;
            else if (_posName[2] == 'M') _POS_LINE = POS_LINE.HELP;
            else _POS_LINE = POS_LINE.DEFENDER;
        }
        else
        {
            // Columns
            if (_posName[0] == 'R') _POS_COL = POS_COLUMN.RIGHT;
            else if (_posName[0] == 'L') _POS_COL = POS_COLUMN.LEFT;
            else _POS_COL = POS_COLUMN.CENTER;

            // Line
            if (_posName[1] == 'F') _POS_LINE = POS_LINE.ATTACK;
            else if (_posName[1] == 'M') _POS_LINE = POS_LINE.HELP;
            else if (_posName[1] == 'D') _POS_LINE = POS_LINE.DEFENDER;
            else
            {
                _POS_LINE = POS_LINE.GOALKEEPER;
                _POS_COL = POS_COLUMN.GOAL_KEEPER;
            }
        }
    }
}
