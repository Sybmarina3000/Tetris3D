﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DG.Tweening;
using Script.Controller;
using Script.GameLogic.TetrisElement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Tutor
{
    public class FirstTutorial : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _firstTutor;
        [SerializeField] private float _timeStop = 0.75f;

        [SerializeField] private CanvasGroup _secondTutor;
        [SerializeField] private CanvasGroup _secondDotFiveTutor;
        [SerializeField] private CanvasGroup _thirdTutor;
        [SerializeField] private CanvasGroup _fourthTutor;

        [SerializeField] private CanvasGroup _topPanel;
        [SerializeField] private CanvasGroup _bottomPanel;
        [SerializeField] private RectTransform _hand;
        [SerializeField] private GameObject _islandHighlight;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Transform _islandTransform;
        
        private int _amountSetElements = 0; 
        
        private Action OnMoveSuccess, OnMoveFail;
        private bool _generateNeedElement;

        private IEnumerable<CoordinatXZ> blocksXZ;
        private float usualSpeed;
        private bool _isSuccess = false;

        [SerializeField] private int countOfMoveCycle = 2;
        private int _currentCountOfMoveCycle = 0;
        
        private void Start()
        {
            RealizationBox.Instance.FSM.OnStart += StartGame;
            
            _generateNeedElement = RealizationBox.Instance.generator._generateNeedElement;
            RealizationBox.Instance.generator._generateNeedElement = true;
            
            _topPanel.alpha = 0;
            _bottomPanel.alpha = 0;
            
            _topPanel.gameObject.SetActive(false);
            _bottomPanel.gameObject.SetActive(false);
            
            RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.OnlySingleTap;
            RealizationBox.Instance.generator.fixedHightPosition = 8;
            RealizationBox.Instance.tapsEvents.enabled = false;

            RealizationBox.Instance.projectionLineManager.TurnOnOff(false);
        }

        void StartGame()
        { 
            //  global::Speed.SetTimeDrop(0.26f);
            RealizationBox.Instance.tapsEvents.enabled = false;
           // Invoke(nameof(FirstStep), _timeStop);
            
            _firstTutor.DOFade(0, 0.1f);
            _secondTutor.DOFade(0, 0.1f);
            _thirdTutor.DOFade(0, 0.1f);
            _fourthTutor.DOFade(0, 0.1f);
            
            _amountSetElements = 0;
            _topPanel.alpha = 0;
            _topPanel.interactable = false;
            _bottomPanel.alpha = 0;
            _bottomPanel.interactable = false;

         //   RealizationBox.Instance.nextElementUI.gameObject.SetActive(false);
            FirstStep();
        }
        
        void FirstStep() // open joystick
        {   
            Invoke(nameof(FirstStepPause),_timeStop);
            // text
            _firstTutor.DOFade(1, 1f).OnComplete(() =>
            {
                RealizationBox.Instance.tapsEvents.enabled = true;
            });

            // tap event
           // RealizationBox.Instance.tapsEvents.enabled = true;
            RealizationBox.Instance.tapsEvents.OnSingleTap += SecondStep;
        }

        void FirstStepPause()
        {
            //pause
            RealizationBox.Instance.slowManager.SetPauseSlow(true);
        }

        void SecondStep() // move element
        {
            RealizationBox.Instance.speedChanger.ResetSpeed();
            RealizationBox.Instance.tapsEvents.OnSingleTap -= SecondStep;
            
            _firstTutor.DOKill();
            _secondTutor.DOKill();
            
            _firstTutor.DOFade(0, 0.1f).SetDelay(0.1f).OnComplete(() => _secondTutor.DOFade(1, 0.2f));
            
            blocksXZ = ElementData.Instance.newElement.blocks.Select(b => b.xz);

         //   RealizationBox.Instance.FSM.onStateChange += FinishMove;

            RealizationBox.Instance.FSM.onStateChange += FinishMove;
          //  RealizationBox.Instance.joystick.onStateChange += FinishMove;
            RealizationBox.Instance.FSM.onStateChange += SecondDotFiveStep;
            RealizationBox.Instance.gameController.onMoveApply += OnSuccess;
         //   OnMoveSuccess += ThirdStep;

            RealizationBox.Instance.joystick.onStateChange += OnJoyStickStateChange;
        }

        void SecondBreak()
        {
            RealizationBox.Instance.tapsEvents.OnSingleTap -= SecondStep;
        }
        
        public void OnJoyStickStateChange(JoystickState state)
        {
            if (state == JoystickState.Hide)
            {
                RealizationBox.Instance.tapsEvents.OnSingleTap += SecondStep;
                
                RealizationBox.Instance.joystick.onStateChange -= OnJoyStickStateChange;
                RealizationBox.Instance.FSM.onStateChange -= FinishMove;
                RealizationBox.Instance.FSM.onStateChange -= SecondDotFiveStep;
                RealizationBox.Instance.gameController.onMoveApply -= OnSuccess;
                
                _firstTutor.DOKill();
                _secondTutor.DOKill();
                _secondTutor.DOFade(0, 0.1f)
                    .OnComplete(() =>_firstTutor.DOFade(1, 0.2f));
            }
        }
        
        void SecondDotFiveStep(TetrisState state)
        {
            if (state == TetrisState.EndInfluence && _isSuccess)
            {
                RealizationBox.Instance.FSM.onStateChange -= SecondDotFiveStep;
                ElementData.Instance.onNewElementUpdate += ThirdStep;
                RealizationBox.Instance.generator.fixedHightPosition = 9;
                RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.SingleAndDouble;
                
                _secondTutor.DOFade(0, 0.1f);
                if (RealizationBox.Instance.joystick.state == JoystickState.Show)
                {
                    _secondDotFiveTutor.DOFade(1, 0.2f);

                    RealizationBox.Instance.joystick.onStateChange += SecondDotFiveFinish;
                }
                else
                {
                    RealizationBox.Instance.slowManager.SetPauseSlow(false);
                }
            }
        }
        
        void SecondDotFiveFinish(JoystickState state)
        {
            if (state == JoystickState.Hide)
            {
                RealizationBox.Instance.joystick.onStateChange -= SecondDotFiveFinish;
                RealizationBox.Instance.slowManager.SetPauseSlow(false);
                
                _secondDotFiveTutor.DOFade(0, 0.2f);
            }
        }
        
        void ThirdStep() // double tap
        {
            ElementData.Instance.onNewElementUpdate -= ThirdStep;
            //  OnMoveSuccess -= ThirdStep;
       //     OnMoveFail += ReturnToSecondStep;
            _currentCountOfMoveCycle++;
            if(_currentCountOfMoveCycle < countOfMoveCycle)
            {
                FirstStep();
                return;
            }
            
            _thirdTutor.DOFade(1, 0.2f);
            usualSpeed = global::Speed.timeDrop;
            global::Speed.SetTimeDrop(2.5f);
           
           RealizationBox.Instance.tapsEvents.OnDoubleTap += FourthStep;
           ElementData.Instance.onNewElementUpdate += ThirdStepIgnore;
        }

        void ThirdStepIgnore()
        {
            ElementData.Instance.onNewElementUpdate -= ThirdStepIgnore;
            FourthStep();
        }
        
        void FourthStep() // continue placing elements 
        {
            global::Speed.SetTimeDrop(usualSpeed);
         //   OnMoveFail -= ReturnToSecondStep;
            ElementData.Instance.onNewElementUpdate -= ThirdStepIgnore;
            RealizationBox.Instance.tapsEvents.OnDoubleTap -= FourthStep;
            _thirdTutor.DOKill();
            _thirdTutor.DOFade(0, 0.3f);
            
            ElementData.Instance.onNewElementUpdate +=  SixthStep;
            RealizationBox.Instance.slowManager.SetPauseSlow(false);
        }
        
        private void SixthStep() // drag the island to turn
        {
            if (++_amountSetElements == 1)
            {
                RealizationBox.Instance.generator.fixedHightPosition = 12;
            }
            if (_amountSetElements > 1)
            {
                ElementData.Instance.onNewElementUpdate -=  SixthStep;
                
                RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.SingleAndDrag;
              //  RealizationBox.Instance.tapsEvents.enabled = false;
              Invoke(nameof(SeventhStep), _timeStop);
            }
        }

        private void SeventhStep()
        {
           // RealizationBox.Instance.tapsEvents.enabled = true;
            RealizationBox.Instance.slowManager.SetPauseSlow(true);
            
            _fourthTutor.DOFade(1, 0.3f);
            RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.SingleAndDrag;
            RealizationBox.Instance.tapsEvents.OnDragIceIsland += Finished;

            int w = Screen.width / 2;
            
            _islandHighlight.SetActive(true);
            _hand.anchoredPosition = WorldToCanvas(_islandTransform.position);
            _hand.DOMoveX(w - w/3, 1.0f).From(w + w/3).SetLoops(-1, LoopType.Yoyo);
            
        }
        
        private void Finished()
        {
            RealizationBox.Instance.slowManager.SetPauseSlow(false);
            RealizationBox.Instance.tapsEvents.OnDragIceIsland -= Finished;
            _fourthTutor.DOFade(0, 0.3f).OnComplete(() =>
            {
                _topPanel.DOFade(1, 0.6f);
                _bottomPanel.DOFade(1, 0.6f);
            });
            
            _topPanel.interactable = true;
            _bottomPanel.interactable = true;
            RealizationBox.Instance.FSM.OnStart -= StartGame;
            RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.None;
            _hand.DOKill();
            _islandHighlight.SetActive(false);
            
        //    RealizationBox.Instance.joystick.onStateChange -= FinishMove;
            RealizationBox.Instance.generator._generateNeedElement = _generateNeedElement;
           // RealizationBox.Instance.FSM.onStateChange -= FinishMove;
           
           RealizationBox.Instance.nextElementUI.gameObject.SetActive(true);
           RealizationBox.Instance.projectionLineManager.TurnOnOff(true);
        }
        
        private Vector2 WorldToCanvas(Vector3 world_position)
        {
            var viewport_position = Camera.main.WorldToViewportPoint(world_position);
            var canvas_rect = _canvas.GetComponent<RectTransform>();
        
            return new Vector2((viewport_position.x * canvas_rect.sizeDelta.x) - (canvas_rect.sizeDelta.x * 0.5f),
                (viewport_position.y * canvas_rect.sizeDelta.y) - (canvas_rect.sizeDelta.y * 0.5f));
        }

        void OnSuccess(bool isSuccess, move move)
        {
            if(isSuccess)
            {
                RealizationBox.Instance.gameController.onMoveApply -= OnSuccess;
                RealizationBox.Instance.joystick.onStateChange -= OnJoyStickStateChange;
            }
            _isSuccess = isSuccess;
        }
        
        void FinishMove(TetrisState state)
        {
            if (state == TetrisState.EndInfluence && _isSuccess)
            {
                OnMoveSuccess?.Invoke();
            }
        }
        
     
        //
        // void FinishMove(JoystickState state)//TetrisState obj)
        // {
        //     /*if (obj != TetrisState.EndInfluence)
        //         return;*/
        //     if (state == JoystickState.Show || ElementData.Instance.newElement == null)
        //         return;
        //     
        //     var blocksAnswerXZ = ElementData.Instance.newElement.blocks.Select(b => b.xz);
        //     var razn = blocksXZ.Except(blocksAnswerXZ);
        //     if (razn.Count() > 0)
        //     {
        //         OnMoveSuccess?.Invoke();
        //     }
        //     else
        //     {
        //         OnMoveFail?.Invoke();
        //     }
        // }

        
    }
}