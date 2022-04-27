﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Helper.Patterns;
using IntegerExtension;
using Script.GameLogic.TetrisElement;

public class Projection : MonoBehaviour
{
    private TetrisFSM _fsm;
    private PlaneMatrix _matrix;
    private ElementData _elementData;
    
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _heightProjection = 0.1f;

    private List<GameObject> _projectionsList = new List<GameObject>();
    private Pool<GameObject> _pool;
    
    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        _fsm = RealizationBox.Instance.FSM;
        _elementData = ElementData.Instance;
        
        _pool = new Pool<GameObject>(_prefab, transform);
        Invoke(nameof(LastStart), 1f);
    }

    private void LastStart()
    {
        _elementData.onNewElementUpdate += CreateProjection;
        
        _fsm.AddListener(TetrisState.MergeElement, () => Destroy());
    }

    public void CreateProjection()
    {
        var obj = _elementData.newElement;

        Destroy();

        var positionProjection = obj.blocks.Select(b => b.xz).Distinct();

        foreach (var item in positionProjection)
        {
            float y = _matrix.MinHeightInCoordinates(item.x.ToIndex(), item.z.ToIndex());

            var posProjection = new Vector3(item.x, y + _heightProjection, item.z);

            var o = _pool.Pop(true);
            o.transform.localPosition = posProjection;
            _projectionsList.Add(o);
        }
    }

    public void Destroy()
    {
        foreach (var item in _projectionsList) _pool.Push(item);
        _projectionsList.Clear();
    }

    private void OnDestroy()
    {
        _elementData.onNewElementUpdate -= CreateProjection;
    }
}