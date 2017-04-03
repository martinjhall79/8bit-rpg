/// <summary>
/// Generic window.
/// The base class for all of the game windows
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenericWindow : MonoBehaviour
{
    // The default window selected when game starts up
    public GameObject firstSelected;
    // Default selected window
    protected EventSystem eventSystem
    {
        get { return GameObject.Find("EventSystem").GetComponent<EventSystem>(); }
    }

    // Default selected window
    public virtual void OnFocus()
    {
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    // Toggle whether window displays, trigger visibility when window opened or closed
    protected virtual void Display(bool value)
    {
        gameObject.SetActive(value);
    }

    public virtual void Open()
    {
        Display(true);
        OnFocus();
    }

    public virtual void Close()
    {
        Display(false);
    }

    protected virtual void Awake()
    {
        Close();
    }
}
