using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PositionManager : MonoBehaviour
{
    public static PositionManager instance;

    public Transform p1Slot;
    public Transform p2Slot;

    [System.Serializable]
    public class Formation
    {
        public Transform[] p1CardsPositions;
        public Transform[] p2CardsPositions;
    }

    public Formation f442;
    public Formation f433;
    public Formation f343;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }


    public Transform ChangeCardSide(bool hisTurn, FORMATION _FORMATION, int nr)
    {

        if (_FORMATION == FORMATION.F442)
        {
            if (hisTurn) return f442.p1CardsPositions[nr];
            else return f442.p2CardsPositions[nr];
        }
        else if (_FORMATION == FORMATION.F433)
        {
            if (hisTurn) return f433.p1CardsPositions[nr];
            else return f433.p2CardsPositions[nr];
        }
        else
        {
            if (hisTurn) return f343.p1CardsPositions[nr];
            else return f343.p2CardsPositions[nr];
        }
    }
    public void AssignCardToPosition(PLAYER _PLAYER, FORMATION _FORMATION, Transform card, int nr)
    {
        Transform pointer;
        if (_FORMATION == FORMATION.F442)
        {
            if (_PLAYER == PLAYER.P1) pointer = f442.p1CardsPositions[nr];
            else pointer = f442.p2CardsPositions[nr];
        }
        else if (_FORMATION == FORMATION.F433)
        {
            if (_PLAYER == PLAYER.P1) pointer = f433.p1CardsPositions[nr];
            else pointer = f433.p2CardsPositions[nr];
        }
        else
        {
            if (_PLAYER == PLAYER.P1) pointer = f343.p1CardsPositions[nr];
            else pointer = f343.p2CardsPositions[nr];
        }

        Position pointerPosition = pointer.GetComponent<Position>();
        card.SetParent(pointer);
        pointerPosition.nr = nr;
        card.position = pointer.position;
        card.GetComponent<Card>()._position = pointerPosition;
    }
}
