﻿using Script.GameLogic.GameItems;
using Script.Influence;
using UnityEngine;

namespace Script.GameLogic.TetrisElement
{
    public class ElementDropper : MonoBehaviour
    {
        private TetrisFSM _fsm;
        
        private PlaneMatrix _matrix;

        private InfluenceManager _influence;

        private int _dropElementCount;

        private ElementCleaner _cleaner;

        private void Start()
        {
            _matrix = RealizationBox.Instance.matrix;
            _fsm = RealizationBox.Instance.FSM;
            _influence = RealizationBox.Instance.influenceManager;
            _cleaner = RealizationBox.Instance.elementCleaner;
        }

        #region  функции падения нового эл-та ( и его слияние)

        public void StartDropElement()
        {
            ElementData.newElement.LogicDrop();
            _influence.AddDrop(ElementData.newElement.myTransform, Vector3.down, global::Speed.timeDrop, CallDrop);
            
            var pickableBlocks = _matrix.GetPickableBlocksForElement(ElementData.newElement);
            foreach (var pBlock in pickableBlocks)
            {
                pBlock.Pick(ElementData.newElement);
                _matrix.UnbindBlock(pBlock);
                _cleaner.DeletePickableBlock((PickableBlock)pBlock);
            }
        }

        private void CallDrop()
        {
            if (!ElementData.newElement)
                return;
            if(_fsm.GetCurrentState() != TetrisState.WaitInfluence)
                InfluenceData.delayedDrop = true;
            else
                _fsm.SetNewState(TetrisState.Drop);
        }

        #endregion

        #region  функции падения всех эл-тов ( после уничтожения слоев)
        public void StartDropAllElements()
        {
            var countDropElements = DropAllElements();
            if (countDropElements > 0)
                return;

            _fsm.SetNewState(TetrisState.Collection);
        }

        private int DropAllElements()
        {
            _dropElementCount = 0;
            foreach (var item in ElementData.mergerElements)
            {
                var empty = _matrix.CheckEmptyPlaсe(item, new Vector3Int(0, -1, 0));
                if (empty) //если коллизии нет, элемент может падать вниз
                {
                    if (item._isBind)
                        _matrix.UnbindToMatrix(item);

                    _dropElementCount++;
                    item.LogicDrop();

                    _influence.AddDrop(item.myTransform, Vector3.down, global::Speed.timeDropAfterDestroy,
                        DecrementDropElementsCount);
                    
                    var pickableBlocks = _matrix.GetPickableBlocksForElement(item);
                    foreach (var pBlock in pickableBlocks)
                    {
                        pBlock.Pick(item);
                        _matrix.UnbindBlock(pBlock);
                        _cleaner.DeletePickableBlock((PickableBlock)pBlock);
                    }
                }
                else
                {
                    if (!item._isBind)
                        _matrix.BindToMatrix(item);
                }
            }

            return _dropElementCount;
        }

        private void DecrementDropElementsCount()
        {
            _dropElementCount--;
            if (_dropElementCount == 0)
                StartDropAllElements();
        }

        #endregion
    }
}