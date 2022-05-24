using DG.Tweening;
using Helper.Patterns.FSM;
using Script.PlayerProfile;
using UnityEngine;

namespace Script.StateMachine.States
{
    public class WinState : AbstractState<TetrisState>
    {
        private PauseUI _pauseUI;

        public WinState()
        {
            _pauseUI = RealizationBox.Instance.pauseUI;
        }
        
        public override void Enter(TetrisState last)
        {
            base.Enter(last);
            
            if(PlayerSaveProfile.instance._lvl < RealizationBox.Instance.gameManager.currentLvl)
                PlayerSaveProfile.instance.SetLvl(RealizationBox.Instance.gameManager.currentLvl);
            
            if (_pauseUI.isPause)
            {
                _pauseUI.onPauseStateChange += WaitPause;
            }
            else
            {
                RealizationBox.Instance.gameManager.ShowWinPanel();
                RealizationBox.Instance.gameManager.HideGamePanels();
            }
        }

        public void WaitPause(bool pauseState)
        {
            if (!pauseState)
            {
                _pauseUI.onPauseStateChange -= WaitPause;
                _pauseUI.DOKill();
                RealizationBox.Instance.gameManager.ShowWinPanel();
                RealizationBox.Instance.gameManager.HideGamePanels();
            }
        }
        
        public override void Exit(TetrisState last)
        {
        }
    }
}