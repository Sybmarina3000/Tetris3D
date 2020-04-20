﻿using System.Collections;
using Helper.Patterns.Messenger;
using UnityEngine;

public enum turn
{
    left,
    right
}

public class Turning : MonoBehaviour
{
    // for camera
    private Vector3 _offset; // начальное положение между камерой и площадкой
    private float _rotY; // поворот камеры
    [SerializeField] private GameObject _Camera;

    private GameCamera _gameCamera;

//    [SerializeField] StateMachine _StateMachine;
    [SerializeField] private GameObject _ObjectLook;

    private PlaneMatrix _matrix;

    private void Awake()
    {
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.NotActive,ResetTurn);
    }

    private void OnDestroy()
    {
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NotActive,ResetTurn);
    }

    private void Start()
    {
        _offset = Vector3.zero - _Camera.transform.position; // сохраняем расстояние между камерой и полем
        _matrix = PlaneMatrix.Instance;
        _gameCamera = _Camera.GetComponent<GameCamera>();
    }

    public bool Action(Element element, turn direction, float time)
    {
        if (CheckOpportunity(direction, element))
        {
//            if (!_StateMachine.ChangeState(EMachineState.Turn, false))
//                return false;

            Logic(direction, element);
            Vizual(direction, element, time);
            return true;
        }

        return false;
    }

    private bool CheckOpportunity(turn direction, Element element)
    {
        int x, z;
        if (direction == turn.left)
            foreach (var item in element.MyBlocks)
            {
                // по правилу поворота
                x = item.Coordinates.z;
                z = -item.Coordinates.x;

//                if (_matrix._matrix[x + 1, item.Coordinates.y, z + 1] != null)
                    return false;
            }
        else
            foreach (var item in element.MyBlocks)
            {
                // по правилу поворота
                x = -item.Coordinates.z;
                z = item.Coordinates.x;

//                if (_matrix._matrix[x + 1, item.Coordinates.y, z + 1] != null)
                    return false;
            }

        return true;
    }

    private void Logic(turn direction, Element element)
    {
        if (direction == turn.left)
            foreach (var item in element.MyBlocks)
            {
                var temp = item.Coordinates.x;
                item.SetCoordinates(item.Coordinates.z, item.Coordinates.y, -item.Coordinates.x);
            }
        else
            foreach (var item in element.MyBlocks)
            {
                var temp = item.Coordinates.x;
                item.SetCoordinates(-item.Coordinates.z, item.Coordinates.y, item.Coordinates.x);
            }
    }

    private void Vizual(turn direction, Element element, float time)
    {
        int angle;
        if (direction == turn.left)
            angle = 90;
        else
            angle = -90;

        StartCoroutine(TurnElement(element, angle, time, _ObjectLook));
        StartCoroutine(TurnCamera(direction, time));

        Messenger<Element>.Broadcast(GameEvent.TURN_ELEMENT.ToString(), element);
    }

    private IEnumerator TurnCamera(turn direction, float time)
    {
        int angle;
        if (direction == turn.left)
            angle = 90;
        else
            angle = -90;

        // начальный и конечный поворот
        var rotationStart = Quaternion.Euler(0, _rotY, 0);
        _rotY += angle;
        var rotationEnd = Quaternion.Euler(0, _rotY, 0);

        float countTime = 0;
        while (countTime < time)
        {
            if (countTime + Time.deltaTime < time)
                countTime += Time.deltaTime;
            else
                countTime = time;

            _Camera.transform.position = Vector3.zero -
                                         Quaternion.LerpUnclamped(rotationStart, rotationEnd, countTime / time) *
                                         _offset;

            _Camera.transform.LookAt(_ObjectLook.transform.position);
            yield return new WaitForEndOfFrame();
        }

        if (_rotY >= 360 || _rotY <= -360)
            _rotY = 0;

//        _gameCamera.Rotation = _rotY;
//        _StateMachine.ChangeState(EMachineState.EndInfluence);
    }

    private IEnumerator TurnElement(Element element, int angle, float time, GameObject target)
    {
        float deltaAngle;
        float countAngle = 0;

        do
        {
            deltaAngle = angle * (Time.deltaTime / time);
            if (angle > 0 && countAngle + deltaAngle > angle || angle < 0 && countAngle + deltaAngle < angle
            ) // если мы уже достаточно повернули и в ту и в другую сторону
            {
                deltaAngle = angle - countAngle; // узнаем сколько нам не хватает на самом деле  
                countAngle = angle;
            }
            else
            {
                countAngle += deltaAngle;
            }

            element.MyTransform.Rotate(target.transform.position, deltaAngle);

            yield return null;
        } while (angle > 0 && countAngle < angle || angle < 0 && countAngle > angle);
    }

    private void ResetTurn()
    {
        turn need;
        if (_rotY < 0)
            need = turn.left;
        else
            need = turn.right;

        var rotationEnd = Quaternion.Euler(0, 0, 0);
        _Camera.transform.position = Vector3.zero - rotationEnd * _offset;
        _Camera.transform.LookAt(_ObjectLook.transform.position);

//        _gameCamera.Rotation = 0;
        _rotY = 0;

      //  _gameCamera.ResetSettings();
    }
}