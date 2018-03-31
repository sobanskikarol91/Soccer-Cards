using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Messages : MonoBehaviour
{
    public static Messages instance;

    public GameObject _messageGO;
    public GameObject messageCanvas;
    public Text txt;
    [HideInInspector]
    bool showMessage;
    public bool ShowMessage { set { showMessage = value; _messageGO.SetActive(value); } }


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        messageCanvas.SetActive(true);

    }
    public void SelectYourCard(string player)
    {
        if (showMessage)
            txt.text = (player + " wybierz swoja karte aby zaatakowac");
    }

    public void SelectEnemyCard(string player)
    {
        if (showMessage)
            txt.text = (player + " wybierz karte przeciwnika");
    }

    public void BlockEnemy(string player)
    {
        if (showMessage)
            txt.text = (player + " zablokuj jedna z 6 statystyk przeciwnika");
    }

    public void ChooseEngagement(string player)
    {
        if (showMessage)
            txt.text = (player + " wybierz zaangazowanie (S M L)");
    }

    public void Choose2Stats(string player)
    {
        if (showMessage)
            txt.text = (player + " wybierz po jednej ze swoich statystyk z obu kolumn ");
    }
}
