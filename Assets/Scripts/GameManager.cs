using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerController _playerController;

    private void Awake()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (!_playerController.isPlayerAlive)
            SceneManager.LoadScene(0);
        else if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
    }
}
