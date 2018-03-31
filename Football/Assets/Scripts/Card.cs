using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public Player _player;
    public EngagmentButton[] _engagementButtons;
    StatButton[] _statButtons;
    public EngagmentController ec;
    public Image shadow;            //  disable shasdow effect
    [HideInInspector]
    public Position _position;
    public Outline boundry;
    Button _cardButton;
    Animator _animator;
    Transform parent;
    // Audio
    AudioSource _audioSource;
    public AudioClip pressAudio;
    public AudioClip HighlightedAudio;
    public AudioClip slideAudio;

    public AttackTxt _attackTxt;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        ec = GetComponent<EngagmentController>();
        _statButtons = GetComponentsInChildren<StatButton>();
        _audioSource = GetComponent<AudioSource>();
        _cardButton = GetComponent<Button>();
        shadow.enabled = false;
    }


    void RandomStats()
    {
        for (int i = 0; i < _statButtons.Length; i++)
            _statButtons[i].BaseValue = UnityEngine.Random.Range(85, 101);
    }

    public void Initialize() // Team
    {
        RandomStats();
        _position.AssignLineAndColumn();
        boundry.effectColor = CardColor.AssignColorToFootballer(_position._POS_LINE);
        // remember start position because card'll move to center in game
        parent = transform.parent;
    }

    public void SetCardPosiotionDependsOnSide(bool hisTurn)
    {
        int cardNr = _position.nr;
        ChangeStatsVisible();
        transform.SetParent(PositionManager.instance.ChangeCardSide(hisTurn, _player._FORMATION, cardNr), false);
        // set new parent after change, because player in next round will begun
        parent = transform.parent;
    }

    void PlayerCardChoosen()
    {
        // if there is state to choose enemy card return
        if (!GameManager.instance.CheckCurrentState(STATES.CHOOSE_OWN_CARD)) return;

        // // if player choos other card than attacker or helper
        //if (!CheckIfChooseAttackerOrHelper()) return;

        GameManager.instance.ChooseCard = this;
        GameManager.instance.currentPlayer.ChoosenCard.ec.AcceptImpedientFromEnemyCard();
        _attackTxt.DisableTxt();
        PressEffects();
        MoveCardToCenter();
    }

    void PressEffects()
    {
        _audioSource.clip = pressAudio;
        _audioSource.Play();
        _animator.SetBool("Pressed", true);
    }

    void EnemyCardChoosen()
    {
        // if player are in other stan than select enemy card
        if (!GameManager.instance.CheckCurrentState(STATES.CHOOSE_ENEMY_CARD)) return;
        // if player choose other card that he can do in this action;
        if (!ActionAndSelectedCard()) return;

        _attackTxt.DisableTxt();
        // Accept impedient if player attack other position than his
        GameManager.instance.ChooseEnemyCard = this;
        GameManager.instance.currentPlayer.ChoosenCard.ec.AcceptImpedientFromEnemyCard();
        PressEffects();

        MoveCardToCenter();
    }

    bool ActionAndSelectedCard()
    {
        // current player action
        POS_LINE playerAction = GameManager.instance.currentPlayer._ACTION;

        // return true when attacker atack helper or other side
        if ((playerAction == POS_LINE.HELP || playerAction == POS_LINE.ATTACK) && (_position._POS_LINE == POS_LINE.ATTACK || _position._POS_LINE == POS_LINE.HELP)) return true;
        return playerAction == _position._POS_LINE;
    }

    bool CheckIfChooseAttackerOrHelper()
    {
        return _position._POS_LINE == POS_LINE.HELP || _position._POS_LINE == POS_LINE.ATTACK;
    }

    public void ChangeStatsVisible()
    {
        if (_player.IsHisTurn)
            foreach (StatButton sb in _statButtons) sb.ShowStatsToCurrentPlayer();
        else
            foreach (StatButton sb in _statButtons) sb.ShowDiscoveredStats();
    }

    public void DisableCard()
    {
        _animator.SetBool("Disable", true);
        shadow.enabled = true;
        _cardButton.interactable = false;
        shadow.GetComponent<Animator>().SetBool("Disable", true);
    }

    public void EnableCard()
    {
        _animator.SetBool("Disable", false);
        shadow.enabled = true;
        _cardButton.interactable = true;
        shadow.GetComponent<Animator>().SetBool("Disable", false);
        Invoke("HideShadow", 0.34f);
    }

    void HideShadow()
    {
        shadow.enabled = false;
    }

    public void EnableSelectedStats()
    {
        foreach (StatButton sb in _statButtons)
        {
            // if button isn't block active it
            if (sb.isBlocked != true)
                sb.Active();
        }
    }

    bool CheckIfChooseEnemyCard()
    {
        if (GameManager.instance.CheckCurrentState(STATES.CHOOSE_ENEMY_CARD)
            && GameManager.instance.opponentPlayer._player == _player._player)
            return true;
        return false;
    }

    bool CheckIfChooseOwnCard()
    {
        if (GameManager.instance.CheckCurrentState(STATES.CHOOSE_OWN_CARD)
    && GameManager.instance.currentPlayer._player == _player._player)
            return true;
        return false;
    }

    public void CardAttackOtherPosition(EENGAGMENT impediment)
    {
        ec.PreviewEngagment(impediment);
    }

    public void EnemyDidntChosecard()
    {
        ec.StatWithoutEngagement();
    }

    public IEnumerator IEMoveCardToDestination(Vector3 destination, bool parentToDestination)
    {
        _audioSource.clip = slideAudio;
        SoundManager.instance.RandomClipSettings(_audioSource, 0.9f, 1.1f);
        float difference = Mathf.Abs((destination - transform.position).magnitude);

        float time = 0;

        while (Mathf.Abs((destination - transform.position).magnitude) > 0.01f)
        {
            time += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destination, 1.15f * difference * Time.deltaTime);
            yield return null;
        }

        if (parentToDestination)
        {
            transform.SetParent(parent, false);
        }

        transform.position = destination;
    }

    public void ReturnCard()
    {
        StartCoroutine(IEMoveCardToDestination(parent.position, true));
        _animator.SetBool("Pressed", false);
    }

    public void MoveCardToCenter()
    {
        Transform destination;
        if (_player._player == PLAYER.P1)
            destination = PositionManager.instance.p1Slot;
        else
            destination = PositionManager.instance.p2Slot;

        gameObject.transform.SetParent(destination, true);
        StartCoroutine(IEMoveCardToDestination(destination.position, false));
    }


    public void DisableEngagement()
    {
        ec.DisableEngagement();
    }

    public void ResetCard()
    {
        _animator.SetTrigger("Reset");

        // Reset engagement Coun stats after battle
        ec.Reset();

        // Reset engagement Buttons
        foreach (EngagmentButton eb in _engagementButtons)
            eb.ResetEngagement();

        // Reset stat Buttons
        foreach (StatButton sb in _statButtons)
            sb.ResetButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_cardButton.interactable == false) return;
        // if player is in state select enemy card ant this is not his card
        if (CheckIfChooseEnemyCard())
        {
            Position playerPosition = GameManager.instance.currentPlayer.ChoosenCard._position;
            _attackTxt.OtherColumnImpediment(playerPosition, _position);
        }
        else if (CheckIfChooseOwnCard())
            _attackTxt.OtherLineImpediment(_position, this);

        _audioSource.clip = HighlightedAudio;
        _audioSource.Play();
        _animator.SetBool("Highlighted", true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if player selected his own card
        if (_player._player == GameManager.instance.currentPlayer._player)
            PlayerCardChoosen();
        else // player selected opponent card to attack
            EnemyCardChoosen();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //disable txt with stat percent
        if (CheckIfChooseEnemyCard())
            _attackTxt.NotSelected();
        else if (_player.IsHisTurn && _player.isCardChoosen == false)
            _attackTxt.NotSelectedHighlightedCard(this);

        
        EventSystem.current.SetSelectedGameObject(null);
        _animator.SetBool("Highlighted", false);
    }
}