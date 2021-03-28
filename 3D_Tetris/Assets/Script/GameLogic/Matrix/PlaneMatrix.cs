﻿using System;
using UnityEngine;
using IntegerExtension;

public class PlaneMatrix : Singleton<PlaneMatrix>
{
    private Block[,,] _matrix;

    [Header("Size plane")] private int _limitHeight = 18;
    [SerializeField] private int _wight;
    [SerializeField] private int _height;

    public int wight => _wight;

    public int height // высота отсчитывается от 0
        => _height - 1;

    public int limitHeight => _limitHeight;

    public event Action<int> OnDestroyLayer;
    
    protected override void Init()
    {
        ExtensionMetodsForMatrix.SetSizePlane(_wight);
        _matrix = new Block[_wight, _height, _wight];

        for (var i = 0; i < _wight; i++)
        for (var j = 0; j < _height; j++)
        for (var k = 0; k < _wight; k++)
            _matrix[i, j, k] = null;
    }

    public void SetLimitHeight(int limit)
    {
        _limitHeight = limit;
    }

    public bool CheckEmptyPlaсe(Element element, Vector3Int direction, bool forPlayerMove = false)
    {
        if (!element)
            return false;
        if (element.blocks.Count == 0)
            return false;

        Vector3Int newCoordinat;
        foreach (var item in element.blocks)
            if (!item.isDestroy)
            {
                newCoordinat = new Vector3Int(item.coordinates.x, item.coordinates.y, item.coordinates.z) + direction;

                if (newCoordinat.OutOfCoordinatLimit())
                    return false;

                if (!ReferenceEquals(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y, newCoordinat.z.ToIndex()], null))
                {
                    if (!element._isBind)
                        return false;
                    if (!element.blocks.Contains(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y,
                        newCoordinat.z.ToIndex()]))
                        return false;
                }
                if (forPlayerMove && newCoordinat.y + 1 < _height)
                {
                    if (!ReferenceEquals(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y + 1, newCoordinat.z.ToIndex()], null))
                        return false;
                }
            }
        return true;
    }

    public bool CheckEmptyPlace(int x_index, int y_index, int z_index)
    {
        return ReferenceEquals(_matrix[x_index, y_index, z_index], null);
    }

    #region привязка/отвязка эл-та к матрице

    public void BindToMatrix(Element element)
    {
        int x, y, z;
        foreach (var item in element.blocks)
        {
            if (ReferenceEquals(item, null) || item.isDestroy)
                continue;
            x = item.coordinates.x;
            y = item.coordinates.y;
            z = item.coordinates.z;

            _matrix[x.ToIndex(), y, z.ToIndex()] = item;
        }

        element._isBind = true;
    }

    public void UnbindToMatrix(Element element)
    {
        int x, y, z;
        foreach (var item in element.blocks)
        {
            if (ReferenceEquals(item, null) || item.isDestroy)
                continue;
            x = item.coordinates.x;
            y = item.coordinates.y;
            z = item.coordinates.z;

            _matrix[x.ToIndex(), y, z.ToIndex()] = null;
        }

        element._isBind = false;
    }

    #endregion

    #region сбор коллекций в слоях матрицы

    public bool CollectLayers()
    {
        var flag = false;
        ;
        for (var y = 0; y < _limitHeight; y++)
            if (CheckCollectedInLayer(y))
            {
                DestroyLayer(y);
                flag = true;
            }

        return flag;
    }

    private bool CheckCollectedInLayer(int layer)
    {
        for (var x = 0; x < wight; x++)
        for (var z = 0; z < wight; z++)
            if (ReferenceEquals(_matrix[x, layer, z], null))
                return false;
        return true;
    }

    private void DestroyLayer(int layer)
    {
        OnDestroyLayer?.Invoke(layer);
        
        for (var x = 0; x < wight; x++)
        for (var z = 0; z < wight; z++)
        {
            _matrix[x, layer, z].isDestroy = true;
            _matrix[x, layer, z] = null;
        }
    }

    #endregion

    public Vector3Int FindLowerAccessiblePlace()
    {
        var min = height - 1;
        int curr_min;

        var min_point = new Vector3Int(0, min, 0);

        for (var x = 0; x < wight && min != 0; ++x)
        for (var z = 0; z < wight && min != 0; ++z)
        {
            curr_min = MinHeightInCoordinates(x, z);
            if (curr_min < min)
            {
                min = curr_min;
                min_point = new Vector3Int(x, min, z);
            }
        }

        return min_point;
    }

    public int MinHeightInCoordinates(int x_index, int z_index)
    {
        for (var y = _matrix.GetUpperBound(1) - 1; y >= 0; --y)
            if (!ReferenceEquals(_matrix[x_index, y, z_index], null))
                return y + 1;
        return 0;
    }

    public void Clear()
    {
        _matrix = new Block[_wight, _height, _wight];
    }
}