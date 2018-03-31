using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngagmentController : MonoBehaviour
{
    public GameObject engagment;

    float baseEngagmentValue;
    float currentEngagementValue;
    float previewEngagementValue;

    public float BaseEngagmentValue { get { return baseEngagmentValue; } set { baseEngagmentValue = value; CurrentEngagementValue = value; } }
    float CurrentEngagementValue { get { return currentEngagementValue; } set { currentEngagementValue = value; PreviewEngagementValue = value; } }
    public float PreviewEngagementValue { get { return previewEngagementValue; } set { previewEngagementValue = value; UpdateEngagementLabel(); } }
        // engagemend'll display after change player
    float withoutEngagement;
    public Text cardNameTxt;
    Card _card;
    StatButton[] _statButtons;


    void Start()
    {
        _statButtons = GetComponentsInChildren<StatButton>();
        _card = GetComponent<Card>();
        BaseEngagmentValue = 100;
    }

    public void PreviewEngagment(EENGAGMENT _engagementType)  // When player highlighted any engagement button
    {
        Engagment newEngagement = EngagementValues.CheckEngagement(_engagementType);

        PreviewEngagementValue += newEngagement.afterRound * baseEngagmentValue;

        for (int i = 0; i < _statButtons.Length; i++)
            _statButtons[i].EngagementValue = newEngagement.inRound;
    }

    void UpdateEngagementLabel()
    {
        cardNameTxt.text = _card._position.NAME + " " + previewEngagementValue.ToString("F1") + "%";
    }

    public void AcceptEngagement()
    {
        withoutEngagement = currentEngagementValue;
        // preview value now assign to current
        currentEngagementValue = previewEngagementValue;
        GameManager.instance.ChooseEngagement = true;
    }

    // when player exit pointer from engagment Button
    public void StatWithoutEngagement()
    {
        // Reset Prewiev value to current
        PreviewEngagementValue = currentEngagementValue;

        foreach (var sb in _statButtons)
            sb.ShowEnemyStatWithoutEngagment();
    }

    public void CountStatAfterBattle()
    {
        foreach (StatButton sb in _statButtons)
            sb.CurrentStatValue = sb.BaseValue * currentEngagementValue / 100;
    }

    public void AcceptImpedientFromEnemyCard()
    {
        foreach (StatButton sb in _statButtons)
            sb.AcceptStats();
    }

    public void DisableEngagement()
    {
        engagment.SetActive(false);
    }

    public void Reset()
    {
        CountStatAfterBattle();
        engagment.SetActive(false);
    }
     
    public void ShowEngagementPanel()
    {
        // check if player seleced this card before to show engagement options
        if (!GameManager.instance.CheckIfPlayerChoseThisCard(_card.GetInstanceID())) return;     // if player chose another card
        if (!GameManager.instance.ChooseEngagement)
            engagment.SetActive(true);
    }

    public void EnemyWithoutEngagement()
    {
        cardNameTxt.text = _card._position.name + " " + withoutEngagement.ToString("F1") + "%";

        foreach (StatButton sb in _statButtons)
            sb.ShowEnemyStatWithoutEngagment();
    }

    public void ShowEnemyRealEngagement()
    {
        cardNameTxt.text = _card._position.name + " " + currentEngagementValue.ToString("F1") + "%";

        foreach (StatButton sb in _statButtons)
            sb.AcceptStats();
    }
}
