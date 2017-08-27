﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : MenuScreen
{

    [Header("UI_ELEMENTS")]
    public Button backButton;

    public void OnBackButtonPressed()
    {
        GameManager.Singleton.cameraController.audioSource.PlayOneShot(onTappedAudioClip, 1f);

        OnTearDown();       // Tear down this menu screen
        pauseMainMenu.gameObject.SetActive(true);
        pauseMainMenu.OnSetup();    // Transition back to pause main menu
    }
}
