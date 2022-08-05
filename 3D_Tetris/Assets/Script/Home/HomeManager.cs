using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.PlayerProfile
{
    public class HomeManager : MonoBehaviour
    {
        [SerializeField] string _lvlSceneName;
        [SerializeField] private TextMeshProUGUI _lvlText;

        [SerializeField] private bool isCheat;
        [SerializeField] private List<GameObject> _cheatObjects;
        
        private void Start()
        {
            PlayerSaveProfile.instance.CheckWin();
            int lvl = PlayerSaveProfile.instance._lvl;

            SetLvlText();
            CheatSet();
        }

        public void CheatSet()
        {
            foreach (var c in _cheatObjects)
            {
                c.SetActive(isCheat);
            }
            
        }
        public void StartLvl()
        {
            SelectLvlSettings(PlayerSaveProfile.instance.GetCurrentLvlData());
            SceneManager.LoadScene(_lvlSceneName);
        }

        private void SelectLvlSettings(LvlSettings lvl)
        { 
            DOTween.KillAll();
            LvlLoader.instance.Select(lvl);
        }

        public void IncrementLvlData()
        {
            PlayerSaveProfile.instance.IncrementLvl();
            SetLvlText();
        }

        public void DecrementLvlData()
        {
            PlayerSaveProfile.instance.DecrementLvl();
            SetLvlText();
        }

        public void SetLvlText()
        {
            if (PlayerSaveProfile.instance._lvl > PlayerSaveProfile.instance._lvlData && isCheat)
                _lvlText.text = "level " + (PlayerSaveProfile.instance._lvl+1).ToString() + " (" + (PlayerSaveProfile.instance._lvlData+1) + ")";
            else
                _lvlText.text = "level " + (PlayerSaveProfile.instance._lvl + 1).ToString();
        }
    }
}