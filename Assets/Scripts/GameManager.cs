using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerCombatController controller;

    private void Awake()
    {
        controller = FindObjectOfType<PlayerCombatController>();
    }

    private void Update()
    {
        if (!controller.isPlayerAlive)
            SceneManager.LoadScene(0);
        else if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
    }
}
