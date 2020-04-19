﻿using Helper.Patterns;
using IntegerExtension;
using UnityEngine;

public class GameLogicPool : MonoBehaviour
{
    [SerializeField] private GameObject _elementPrefab;
    [SerializeField] private GameObject _blockPrefab;

    [SerializeField] private Transform _elementPoolParent;
    [SerializeField] private Transform _blockPoolParent;
    
    private Pool<Element> _elementPool;
    private Pool<Block>   _blockPool;

    public Element CreateEmptyElement()
    {
        return _elementPool.Pop(true);
    }

    public void CreateBlock(Vector3Int position, Element element, Material material)
    {
        var currBlock = _blockPool.Pop(true);
        currBlock.Mesh.material = material;
        currBlock.SetCoordinates(position.x.ToCoordinat(), position.y, position.z.ToCoordinat());

        currBlock.MyTransform.parent = element.MyTransform;
        SetBlockPosition(currBlock);
        element.AddBlock(currBlock);
    }

    private void SetBlockPosition(Block block)
    {
        var position = new Vector3(block.Coordinates.x, block.Coordinates.y, block.Coordinates.z);
        block.gameObject.transform.localPosition = position;
    }

    public void DeleteElement(Element element)
    {
        if (element.MyBlocks.Count > 0)
            foreach (var block in element.MyBlocks)
            {
                DeleteBlock(block);
            }

        element.gameObject.SetActive(false);
        _elementPool.Push(element);
    }

    public void DeleteBlock(Block block)
    {
        block.gameObject.SetActive(false);
        _blockPool.Push(block);
    }
    
    
    private void Start()
    {
        _elementPool = new Pool<Element>(_elementPrefab.GetComponent<Element>(), _elementPoolParent);
        _blockPool = new Pool<Block>(_blockPrefab.GetComponent<Block>(), _blockPoolParent);
    }
}