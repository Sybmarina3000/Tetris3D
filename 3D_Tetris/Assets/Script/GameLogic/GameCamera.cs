﻿using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameCamera : MonoBehaviour
{
    public event Action onFirstAnimationEnd;
    public event Action onStabilizationEnd;

    public Transform lookAtPoint => _lookAtPoint;
    
    [SerializeField] private Vector3 _startPoint;
    
    [SerializeField] private Transform _maxDistance;
    [SerializeField] private Transform _minDistance;

    [SerializeField] private float _maxSize;
    [SerializeField] private float _minSize;

    [SerializeField] private Transform _maxLookAt;
    [SerializeField] private Transform _minLookAt;

    [SerializeField] private float _timeStabilization;

    [SerializeField] private Transform _objectLook;

    [SerializeField] private Transform _lookAtPoint; // for objects which lookAt camera;
    private Camera _camera;
   
    private Transform _myTransform;
    
    private HeightHandler _heightHandler;

    private int _currentHeight = 0;
    public int lastMaxCurrentHeight { get; private set; }

    [Space(15)] [Header("First Animation")] [SerializeField]
    private float _Time = 1;
    
    public void FirstAnimation()
    {
        //Debug.Log($"this rot {_camera.transform.localRotation}");
        _camera.transform.localRotation = Quaternion.identity;
        //Debug.Log($"this rot {_camera.transform.localRotation}");
        
        _camera.transform.DOLocalMove(Vector3.zero, _Time)
            .From(_startPoint).OnComplete(  () => onFirstAnimationEnd?.Invoke());
    }

    public void SetPositionWithoutAnimation()
    {
        int height = _heightHandler.currentHeight;
        int limit= _heightHandler.limitHeight; 
        
        if (_currentHeight == height)
            return;
        
        lastMaxCurrentHeight = _currentHeight < height ? height : _currentHeight;
        _currentHeight = height;
        
        var finishP = Vector3.Lerp(_minDistance.position, _maxDistance.position, height / (float) limit);
        var finishS = Mathf.Lerp(_minSize, _maxSize, height / (float) limit);
        var finishLookAt = Vector3.Lerp(_minLookAt.position, _maxLookAt.position, height / (float) limit);
        
        _myTransform.position = finishP;
        _camera.orthographicSize = finishS;
        _objectLook.position = finishLookAt;
        _myTransform.LookAt(_objectLook.position);
    }
    
    public bool SetStabilization()
    {
        return CheckStabilization(_heightHandler.limitHeight, _heightHandler.currentHeight);
    }
    
    private void Awake()
    {
        _myTransform = transform;
        _camera = GetComponent<Camera>();
    }

    // Use this for initialization
    private void Start()
    {
        //_camera.orthographicSize = _FirstOrthographicSize;
       // _myTransform.LookAt(_objectLook.position);
       
       _heightHandler = RealizationBox.Instance.haightHandler;
       
     //  _camera.transform.localRotation = Quaternion.identity;
     //  _camera.transform.position = _startPoint;
    }

    private bool CheckStabilization(int limit, int height)
    {
        if (_currentHeight == height)
        {
            onStabilizationEnd?.Invoke();
            return false;
        }
        
        _currentHeight = height;
        
        var finishP = Vector3.Lerp(_minDistance.position, _maxDistance.position, height / (float) limit);
        var finishS = Mathf.Lerp(_minSize, _maxSize, height / (float) limit);
        var finishLookAt = Vector3.Lerp(_minLookAt.position, _maxLookAt.position, height / (float) limit);
        
        _myTransform.DOMove(finishP, _timeStabilization).OnComplete( () => lastMaxCurrentHeight = _currentHeight);
        _camera.DOOrthoSize(finishS, _timeStabilization);
        _objectLook.DOMove(finishLookAt, _timeStabilization).OnUpdate(() => _myTransform.LookAt(_objectLook.position))
            .OnComplete(() => onStabilizationEnd?.Invoke());
        return true;
    }
}