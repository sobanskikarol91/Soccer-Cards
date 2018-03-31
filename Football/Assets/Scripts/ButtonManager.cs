using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void F442()
    {
       GameManager.instance.currentPlayer._FORMATION = FORMATION.F442;
    }

    public void F343()
    {
        GameManager.instance.currentPlayer._FORMATION = FORMATION.F343;
    }

    public void F433()
    {
        GameManager.instance.currentPlayer._FORMATION = FORMATION.F433;
    }
}
