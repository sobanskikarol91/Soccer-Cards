using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class EngagmentButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    public EngagmentController ec;
    public AudioClip highlightedClip;
    public AudioClip pressClip;
    public EENGAGMENT _eEngagment;
    public Button[] otherButtons;
    AudioSource _audioSource;

    Button _button;
    bool isPressed = false;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void Deactivation()
    {
        if (isPressed) return;
        _button.interactable = false;
    }

   public  void ResetEngagement()
    {
        isPressed = false;
        _button.interactable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // if button is pressed don't change value
        if (isPressed == true) return;
        if (_button.interactable == false) return;
        _audioSource.clip = highlightedClip;
        SoundManager.instance.RandomClipSettings(_audioSource, 1f, 1f);

        ec.PreviewEngagment(_eEngagment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // if button is pressed don't change value
        if (isPressed == true) return;
        if (_button.interactable == false) return;

        ec.StatWithoutEngagement();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isPressed == true) return;

        _audioSource.clip = pressClip;
        SoundManager.instance.RandomClipSettings(_audioSource, 0.65f, 1.45f);

        isPressed = true;
        foreach (Button b in otherButtons)
            b.GetComponent<EngagmentButton>().Deactivation();

        ec.AcceptEngagement();
    }
}
