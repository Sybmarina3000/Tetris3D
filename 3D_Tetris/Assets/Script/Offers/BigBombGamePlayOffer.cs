using System;
using DG.Tweening;
using Script.CheckPlace;
using Script.GameLogic.Bomb;
using Script.GameLogic.TetrisElement;
using Script.PlayerProfile;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Script.Offers
{
    public class BigBombGamePlayOffer: MonoBehaviour
    {
        private PlaneMatrix _matrix;
        private HeightHandler _height;
        private Generator _generator;
        private ElementData _elementData;
        private ChangeNewElementToBomb _changeNewElementToBomb;
        
        [SerializeField] private RectTransform _bombIcon;
        [SerializeField] private CanvasGroup _offerButtonCanvas;
        [SerializeField] private Button _offerButton;
        
        [SerializeField] private CanvasGroup _offerExtraPanel;
        [SerializeField] private Button _extraPanelCloseButton;
        [SerializeField] private Button _extraPanelAdsButton;

        [SerializeField] private int needOutOfLimitAmount = 5;
        [SerializeField] private int maxLessOfLimit = 1;

        [SerializeField] private int _bombCost;
        [SerializeField] private TextMeshProUGUI[] _textCurrency;
        
        private bool _isShow = false;
        
        //Animation
        private Sequence _showBtn, _hideBtn;
        [SerializeField] private float _yMove = -100;
        [SerializeField] private float _time = 0.5f;
        private FastElementDrop _fastElementDrop;
        private CheckPlaceManager _checkPlaceManager;

        private void Start()
        {
            _matrix = RealizationBox.Instance.matrix;
            _height = RealizationBox.Instance.haightHandler;
            _generator = RealizationBox.Instance.generator;
            _elementData = ElementData.Instance;
            _changeNewElementToBomb = RealizationBox.Instance.changeNewElementToBomb;
            _fastElementDrop = RealizationBox.Instance.fastElementDrop;
            _checkPlaceManager = RealizationBox.Instance.checkPlaceManager;
            
        //    RealizationBox.Instance.FSM.AddListener(TetrisState.GenerateElement, CheckShowOfferBtn);
            ElementData.Instance.onNewElementUpdate += CheckShowOfferBtn;
            
            RealizationBox.Instance.FSM.AddListener(TetrisState.LoseGame, HideBtn);
            RealizationBox.Instance.FSM.AddListener(TetrisState.WinGame, HideBtn);

            var rectTransform = _offerButtonCanvas.GetComponent<RectTransform>();
            
            _showBtn = DOTween.Sequence().SetAutoKill(false).Pause();
            _showBtn.Append(_offerButtonCanvas.DOFade(1, _time / 2).From(0))
                .Join(rectTransform.DOAnchorPosY(0, _time / 2).From(Vector2.up * _yMove));

            _hideBtn = DOTween.Sequence().SetAutoKill(false).Pause();
            _hideBtn.Append(_offerButtonCanvas.DOFade(0, _time / 2).From(1))
                .Join(rectTransform.DOAnchorPosY(_yMove, _time / 2).From(Vector2.zero)).OnComplete( () =>
                {
                    _bombIcon.DOKill();
                    _offerButtonCanvas.gameObject.SetActive(false);
                    _hideBtn.Rewind();
                });
            
            Clear();

            _offerButton.onClick.AddListener(OnOfferButtonClick);
            
            _extraPanelCloseButton.onClick.AddListener(HideExtraPanel);
            _extraPanelAdsButton.onClick.AddListener(OnAdsButtonClick);
         //   HideExtraPanel();
            //  Show();

            foreach (var t in _textCurrency)
            {
                t.text = _bombCost.ToString();
            }
        }
        
        public void CheckShowOfferBtn()
        {
            if (!_checkPlaceManager.CheckAllPosition(ElementData.Instance.newElement))
            {
                ShowBtn();
                return;
            }
            
            int yLimit = _height.limitHeight - 3;
            int outOfLimitAmount = 0;
            int lessOfLimitAmount = 0;
            
            if (_height.currentHeight < yLimit)
            {
                if (_isShow)
                    HideBtn();
                return;
            }
            
            for (var x = 0; x < _matrix.wight; x++)
            for (var z = 0; z < _matrix.wight; z++)
            {
                int y = _matrix.MinHeightInCoordinates(x, z);

                if (y < yLimit) lessOfLimitAmount++;
                
                if (y < yLimit && lessOfLimitAmount > maxLessOfLimit && !_isShow)
                    return;
                if (y > yLimit)
                    outOfLimitAmount++;
            }

            if (outOfLimitAmount >= needOutOfLimitAmount && !_isShow)
                ShowBtn();
            else if(outOfLimitAmount < needOutOfLimitAmount && _isShow)
                HideBtn();
        }

        public void ShowBtn()
        {
            _isShow = true;
            _offerButtonCanvas.gameObject.SetActive(true);
            _showBtn.Rewind();
            _showBtn.Play();
            
            _bombIcon.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.8f).From(Vector3.one * 1.2f).SetLoops(-1,LoopType.Yoyo);
            RealizationBox.Instance.slowManager.SetOfferSlow(true);
        }

        public void HideBtn()
        {
            _isShow = false;
            _hideBtn.Rewind();
            _hideBtn.Play();

            RealizationBox.Instance.slowManager.SetOfferSlow(false);
        }
        
        private void ShowExtraPanel()
        {
            _offerExtraPanel.gameObject.SetActive(true);
            _offerExtraPanel.DOFade(1, _time / 2).From(0);
            RealizationBox.Instance.influenceManager.enabled = false;
        }

        public void HideExtraPanel()
        {
            _offerExtraPanel.DOFade(0, _time / 2).From(1)
                .OnComplete(()=>
                {
                    _offerExtraPanel.gameObject.SetActive(false);
                    RealizationBox.Instance.influenceManager.enabled = true;
                });
        }
        
        public void OnOfferButtonClick()
        {
            if (!PlayerSaveProfile.instance.ChangeCurrencyAmount(Currency.coin, -_bombCost)) // currency < 1000
                ShowExtraPanel();
            else
                MakeBigBomb();
        }

        public void OnAdsButtonClick()
        {
            AdsManager.instance.ShowRewarded((b) =>
            {
                HideExtraPanel();
                MakeBigBomb();
            });
        }
        
        public void MakeBigBomb()
        {
            HideBtn();
            
            _fastElementDrop.ResetFastSpeed();
            
            if(Equals(_elementData.newElement, null))
                _generator.SetNextAsBigBomb();
            else
            {
                RealizationBox.Instance.FSM.SetNewState(TetrisState.BigBombGenegation);
            //    _changeNewElementToBomb.ChangeToBigBomb(true);
            }
        }
        
        public void Clear()
        {
            _offerButtonCanvas.gameObject.SetActive(false);
            _isShow = false;
            
            _offerExtraPanel.gameObject.SetActive(false);
        }
    }
}