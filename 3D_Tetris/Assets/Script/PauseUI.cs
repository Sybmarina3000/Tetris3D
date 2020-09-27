﻿using Script.Controller;
using Script.Influence;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private BottomPanelAnimation _pausePanel;
    
    [SerializeField] private BottomPanelAnimation _gamePanel;

    private InfluenceManager _influenceManager;
    private MovementJoystick _joystick;
    private TapsEvents _tapsEvents;
    
    private void Awake()
    {
        _influenceManager = RealizationBox.Instance.influenceManager;
        _joystick = RealizationBox.Instance.joystick;
        _tapsEvents = RealizationBox.Instance.tapsEvents;
    }

    public void SetPauseGame(bool isPause)
    {
        _influenceManager.enabled = !isPause;
        
        if (isPause)
        {
            _pausePanel.Show();
            _gamePanel.Hide();
            
            _joystick.Hide();
            _joystick.enabled = false;
            _tapsEvents.enabled = false;
        }
        else
        {
            _gamePanel.Show();
            _pausePanel.Hide();
            
            _joystick.enabled = true;
            _tapsEvents.enabled = true;
        }
        
    }
}