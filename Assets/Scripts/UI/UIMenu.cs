using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public GameObject uiMenu;

    private void Start()
    {
        uiMenu.SetActive(false);
        CharacterManager.Instance.Player.controller.onMenuAction += OnMenu;
    }

    public void OnMenu()
    {
        ToggleMenu();
    }

    private void ToggleMenu()
    {
        if (uiMenu.activeInHierarchy)
        {
            uiMenu.SetActive(false);
        }
        else
        {
            uiMenu.SetActive(true);
        }
    }
}
