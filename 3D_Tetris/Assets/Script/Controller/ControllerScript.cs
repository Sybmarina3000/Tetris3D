﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ControllerScript : MonoBehaviour
{
    [SerializeField] StateMachine _StateMachine;

    [SerializeField] Moving moving;
    [SerializeField] Turning turning;

    // таблица для перемещения блоков в зависимости от угла обзора.
    private move[] A = { move._z, move._x, move.z, move.x };
    private move[] S = { move.x, move._z, move._x, move.z };
    private move[] D = { move.z, move.x, move._z, move._x };
    private move[] W = { move._x, move.z, move.x, move._z };
    private int indexTable;

    private Vector3 _offset; // начальное положение между камерой и площадкой
    private float _rotY;  // поворот камеры

    static public bool MoveTutorial { get; set; } 
    static public bool TurnTutorial { get; set; } 

    private void Start()
    {
        Messenger<ETouсhSign>.AddListener( ETouchType.Swipe.ToString(), Move);
        Messenger<ETouсhSign>.AddListener( ETouchType.OneClick.ToString(), Turn);
    }

    // Update is called once per frame
    private void Update()
    {

        // поворот сцены влево
        if ( TouchControl.TouchEvent == ETouсhSign.LeftOneTouch && ElementManager.NewElement !=null)//(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if( turning.Action(ElementManager.NewElement, turn.left, Speed.TimeRotate))
                CorrectIndex(90);           
        }
        // поворот сцены вправо
        if (TouchControl.TouchEvent == ETouсhSign.RightOneTouch && ElementManager.NewElement != null)// (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(turning.Action(ElementManager.NewElement, turn.right, Speed.TimeRotate))
                CorrectIndex(-90);           
        }

        if(TouchControl.TouchEvent == ETouсhSign.Swipe_LeftUp)//(Input.GetKeyDown(KeyCode.A))
        {
            moving.Action(ElementManager.NewElement, A[indexTable], Speed.TimeMove);
            if (MoveTutorial)
                Messenger<move>.Broadcast("MOVE", A[0]);
        }
        if (TouchControl.TouchEvent == ETouсhSign.Swipe_LeftDown)// (Input.GetKeyDown(KeyCode.S))
        {
            moving.Action(ElementManager.NewElement, S[indexTable], Speed.TimeMove);
            if (MoveTutorial)
                Messenger<move>.Broadcast("MOVE", S[0]);
        }
        if (TouchControl.TouchEvent == ETouсhSign.Swipe_RightDown)//(Input.GetKeyDown(KeyCode.D))
        {
            moving.Action(ElementManager.NewElement, D[indexTable], Speed.TimeMove);
            if (MoveTutorial)
                Messenger<move>.Broadcast("MOVE", D[0]);
        }
        if (TouchControl.TouchEvent == ETouсhSign.Swipe_RightUp)//(Input.GetKeyDown(KeyCode.W))
        {
            moving.Action(ElementManager.NewElement, W[indexTable], Speed.TimeMove);
            if (MoveTutorial)
                Messenger<move>.Broadcast("MOVE", W[0]);
        }

        // обнуляем переменную поскольку мы уже все определили
        TouchControl.TouchEvent = ETouсhSign.empty;
    }


    public void Turn(ETouсhSign touch) {
    
        if ( TouchControl.TouchEvent == ETouсhSign.LeftOneTouch && ElementManager.NewElement !=null)
        {
            if( turning.Action(ElementManager.NewElement, turn.left, Speed.TimeRotate))
                CorrectIndex(90);           
        }

        if (TouchControl.TouchEvent == ETouсhSign.RightOneTouch && ElementManager.NewElement != null)
        {
            if(turning.Action(ElementManager.NewElement, turn.right, Speed.TimeRotate))
                CorrectIndex(-90);           
        }
    }

    public void Move( ETouсhSign touch) {
            
    }
    
    private void CorrectIndex( int degree) {
        _rotY += degree;
        if (_rotY == 360 || _rotY == -360)
            _rotY = 0;

        if (_rotY > -1)
            indexTable = (int)_rotY / 90;
        else
            indexTable = ((int)_rotY + 360) / 90;
    }

}