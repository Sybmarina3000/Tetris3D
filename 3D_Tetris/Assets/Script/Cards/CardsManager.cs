using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Cards
{
    public class CardsManager : MonoBehaviour
    {
        [SerializeField] private List<CanvasGroup> _hideElements; // when unlockPanel opened
        [SerializeField] private float _hideTime;
        
        [SerializeField] private UnlockCardPanel _unlockCardPanel;
        [SerializeField] private FullCardPanel _fullCardPanel;
        
        [SerializeField] private CardsList _cardsData;
        private List<CardIcon> _cards;
        
        
        private int currentCard = 2;

        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private Transform _cardsIconListParent;
        
        private void Start()
        {
            _cards = new List<CardIcon>();
            Load();
            
            _unlockCardPanel.closeBtn.onClick.AddListener(CloseUnlockPanel);
            _fullCardPanel.closeBtn.onClick.AddListener(CloseFullCardPanel);
            
            _unlockCardPanel.gameObject.SetActive(false);
            _fullCardPanel.gameObject.SetActive(false);
        }

        public void Load()
        {
            CreateCardIconList();
            _unlockCardPanel.Load(new List<int>(), _cardsData.cards[currentCard]);
        }

        public void Open()
        {
            OpenUnlockPanel();
        }

        // UNLOCK PANEL
        public void OpenUnlockPanel()
        {
            foreach (var h in _hideElements)
            {
                h.DOFade(0, _hideTime).From(1);
            }
            _unlockCardPanel.gameObject.SetActive(true);
            _unlockCardPanel.HideAll();

            _unlockCardPanel.transform.DOScale(1, _hideTime * 1.5f).From(0.7f);
            _unlockCardPanel.canvasGroup.DOFade(1, _hideTime * 1.5f).From(0)
                .OnComplete(()=> _unlockCardPanel.OpenUnlocked());
        }

        public void CloseUnlockPanel()
        {
            foreach (var h in _hideElements)
            {
                h.DOFade(1, _hideTime* 1.5f).From(0);
            }
            
            _unlockCardPanel.HideAll();
            _unlockCardPanel.transform.DOScale(0.7f, _hideTime).From(1);
            _unlockCardPanel.canvasGroup.DOFade(0, _hideTime).From(1)
                .OnComplete(()=>_unlockCardPanel.gameObject.SetActive(false));
            
        }

        // FULL CARD PANEL
        public void OpenFullCardPanel(int index)
        {
            foreach (var h in _hideElements)
            {
                h.DOFade(0, _hideTime).From(1);
            }
            _fullCardPanel.gameObject.SetActive(true);
            _fullCardPanel.SetImage(_cardsData.cards[index]);
            
            _fullCardPanel.transform.DOScale(1, _hideTime * 1.5f).From(0.7f);
            _fullCardPanel.canvasGroup.DOFade(1, _hideTime * 1.5f).From(0);
            
        }
        
        public void CloseFullCardPanel()
        {
            foreach (var h in _hideElements)
            {
                h.DOFade(1, _hideTime* 1.5f).From(0);
            }
            
            _fullCardPanel.transform.DOScale(0.7f, _hideTime).From(1);
            _fullCardPanel.canvasGroup.DOFade(0, _hideTime).From(1)
                .OnComplete(()=>_fullCardPanel.gameObject.SetActive(false));
        }
        
        //CARD ICONS
        public void CreateCardIconList()
        {
            for (int i = 0; i < _cardsData.cards.Count; i++)
            {
                var ci = Instantiate(_cardPrefab, _cardsIconListParent).GetComponent<CardIcon>();
                
                ci.SetState(i < currentCard? CardState.unlocked: i == currentCard? CardState.current : CardState.locked);
                ci.SetPicture(_cardsData.cards[i]);
                ci.SetIndex(i);
                
                _cards.Add(ci);
                ci.OnButtonClick += OnCardIconClick;
            }
        }

        public void OnCardIconClick(int index)
        {
            if (index == currentCard)
                OpenUnlockPanel();
            else
                OpenFullCardPanel(index);
        }
        public void UpdateProgressCard()
        {
            _cards[currentCard].SetProgress("0 /6");
        }

     
    }
}