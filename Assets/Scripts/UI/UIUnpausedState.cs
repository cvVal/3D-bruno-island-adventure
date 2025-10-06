using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIUnpausedState : UIBaseState
    {
        public UIUnpausedState(UIController uiController) : base(uiController)
        {
        }

        public override void EnterState()
        {
            var playerInputCmp = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<PlayerInput>();

            var pauseContainer = UIController.RootElement.Q<VisualElement>(Constants.UIClassPauseContainer);

            playerInputCmp.SwitchCurrentActionMap(Constants.GameplayActionMap);

            pauseContainer.style.display = DisplayStyle.None;

            // Restore the background music volume
            AudioManager.RestoreMusic();

            Time.timeScale = 1;
        }

        public override void SelectButton()
        {
        }
    }
}