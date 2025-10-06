using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIPauseState : UIBaseState
    {
        public UIPauseState(UIController uiController) : base(uiController)
        {
        }

        public override void EnterState()
        {
            var playerInputCmp = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<PlayerInput>();

            var pauseContainer = UIController.RootElement.Q<VisualElement>(Constants.UIClassPauseContainer);

            playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);
            pauseContainer.style.display = DisplayStyle.Flex;
            
            // Duck the background music
            AudioManager.DuckMusic();
            
            Time.timeScale = 0;
        }

        public override void SelectButton()
        {
        }
    }
}