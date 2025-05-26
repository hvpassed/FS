using System;
using System.Collections;
using System.Collections.Generic;
using FS.Manager;
using TMPro;
using UnityEngine;

public class PlayerInfoView : MonoBehaviour
{
    // Start is called before the first frame update

    private GameManager _gameManager;
    private TextMeshProUGUI _playerInfo;
    private FSPlayerInputManager _fsPlayerInputManager;
    private void Awake()
    {
        _playerInfo = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _fsPlayerInputManager = FSPlayerInputManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        _playerInfo.text = $"Player Id: {_gameManager.PlayerId}\nLogic Frame:{_gameManager.FrameCount}\nInput Actions:{_fsPlayerInputManager.InputActionCount}";
    }
}
