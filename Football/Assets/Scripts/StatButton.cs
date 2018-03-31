using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public enum STATTYPE { firstGroup, secondGroup }

    // which stat nr it is
    public int statNr;            // stat nr in footbal card
    // what group it is
    public STATTYPE _statType;
    // reference to button to disable them
    public StatButton[] otherButtons;
    // reference to card to know which card it is
    public Card _card;
    Button _button;

    public AudioClip highlightedClip;
    public AudioClip pressClip;
    public AudioClip blockClip;
    public bool isBlocked;
    // when opponent discovered state change to true
    bool isDiscovered = false;
    bool firstTimeDiscovered;
    float engagementValue;
    float currentStatValue;
    float baseValue;


    public float BaseValue { get { return baseValue; } set { baseValue = value; currentStatValue = value; engagementValue = value; HideStat(); } }
    public float CurrentStatValue { get { return currentStatValue; } set { currentStatValue = value; DisplayStat(currentStatValue); } }
    public float EngagementValue { get { return engagementValue; } set { engagementValue = currentStatValue + currentStatValue * value; DisplayStat(engagementValue); } }
    Text statTxt;
    AudioSource _audioSource;


    public void Active()
    {
        statTxt.color = StatTextColor.active;
        _button.interactable = true;
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        statTxt = GetComponentInChildren<Text>();
        _audioSource = GetComponent<AudioSource>();
    }

    void BlockStat()
    {
        // if opponent has already blocked stat
        if (GameManager.instance.opponentPlayer.isOneStatBlocked) return;
        // if it's not the same card that player selected before, for example enemy choosen card
        if (GameManager.instance.opponentPlayer.SelectedCardID() != _card.GetInstanceID()) return;


        AudioPressEffect();
        GameManager.instance.BlockOpponentStat = true;
        isBlocked = true;
        Deactivation();

        _audioSource.clip = blockClip;
        _audioSource.Play();
    }

    void ChooseStats()
    {
        // if it's not the same card that player selected before
        if (GameManager.instance.currentPlayer.SelectedCardID() != _card.GetInstanceID()) return;

        // if player chose before this groups
        if (_statType == STATTYPE.firstGroup && GameManager.instance.ChooseFirstStat) return;
        if (_statType == STATTYPE.secondGroup && GameManager.instance.ChooseSecondStat) return;

       // return if stat is already block
        if (isBlocked) return; //

        AudioPressEffect();
        foreach (StatButton b in otherButtons)
            b.Deactivation();

        if (_statType == STATTYPE.firstGroup)
            GameManager.instance.ChooseFirstStat = true;
        else if (_statType == STATTYPE.secondGroup)
            GameManager.instance.ChooseSecondStat = true;


        GameManager.instance.currentPlayer.AddChoosenStat(engagementValue);

        firstTimeDiscovered = true;

        // disable Engagement Panel 
        if (GameManager.instance.ChooseFirstStat && GameManager.instance.ChooseSecondStat)
            _card.DisableEngagement();
    }

    public void Deactivation()
    {
        _button.interactable = false;
        statTxt.color = StatTextColor.deactivate;
    }



    // In fight player display only discovered stats 
    void ShowDiscoveredStatInFight()
    {
        if (!isDiscovered && firstTimeDiscovered)
        {
            isDiscovered = true;
            statTxt.text = " " + currentStatValue.ToString("F1");

            // enable visible image
            foreach (Transform t in transform)
                if (t.name == "visible")
                    t.GetComponent<Image>().enabled = true;
        }
    }

    // Current Player'll see all his stats
    public void ShowStatsToCurrentPlayer()
    {
        statTxt.text = " " + currentStatValue.ToString("F1");
    }

    public void DisplayStat(float value)
    {
        statTxt.text = " " + value.ToString("F1");
    }

    public void ShowDiscoveredStats()  // card
    {
        // if opponent allready discovered stat do nothing
        if (isDiscovered)
            DisplayStat(currentStatValue);
        else
            HideStat();
    }

    void HideStat()
    {
        statTxt.text = "   ---";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // if there is state to choose own stats and cards belong to current player or
        // if there is state to block enemy stat and cards belong to enemy
        if (GameManager.instance.CURRENT_STATE == STATES.CHOOSE_STATS &&
            GameManager.instance.currentPlayer._player == _card._player._player ||
            GameManager.instance.CURRENT_STATE == STATES.BLOCK_STAT &&
            GameManager.instance.opponentPlayer._player == _card._player._player)
        {
            SoundManager.instance.RandomClipSettings(_audioSource, 1f, 1f);
            _audioSource.clip = highlightedClip;
            _audioSource.Play();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.CURRENT_STATE == STATES.BLOCK_STAT)
            BlockStat();
        else if (GameManager.instance.CURRENT_STATE == STATES.CHOOSE_STATS)
            ChooseStats();

        // disable highlight the button
        EventSystem.current.SetSelectedGameObject(null);
    }

    void AudioPressEffect()
    {
        _audioSource.clip = pressClip;
        SoundManager.instance.RandomClipSettings(_audioSource, 0.65f, 1.45f);
    }

    // when enemy has discovered opponent stat and opponent chose his engagement player can't see engagement on discovered stats
    public void ShowEnemyStatWithoutEngagment()
    {
        DisplayStat(currentStatValue);
    }

    public void AcceptStats()
    {
        currentStatValue = engagementValue;
        ShowDiscoveredStatInFight();
    }

    public void ResetButton()
    {
        engagementValue = currentStatValue;
        // if it isn't his turn show only discovered states after reuturned card
        if(!_card._player.IsHisTurn)
        ShowDiscoveredStats();
        isBlocked = false;
        Active();
    }
}
