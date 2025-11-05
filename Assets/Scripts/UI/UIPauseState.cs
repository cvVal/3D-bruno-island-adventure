using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIPauseState : UIBaseState
    {
        private readonly UIQuitGame _uiQuitGameCmp;

        public UIPauseState(UIController uiController) : base(uiController)
        {
            _uiQuitGameCmp = uiController.GetComponent<UIQuitGame>();
        }

        public override void EnterState()
        {
            var playerInputCmp = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<PlayerInput>();

            var pauseContainer = UIController.RootElement.Q<VisualElement>(Constants.UIClassPauseContainer);

            playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);
            pauseContainer.style.display = DisplayStyle.Flex;

            UIController.Buttons = pauseContainer
                .Query<Button>(null, Constants.UIClassMenuButton)
                .ToList();

            UIController.Buttons[0].AddToClassList(Constants.UIClassActive);

            // Duck the background music
            AudioManager.DuckMusic();

            Time.timeScale = 0;
        }

        public override void SelectButton()
        {
            if (UIController.Buttons == null || UIController.Buttons.Count == 0) return;

            var btn = UIController.Buttons[UIController.currentSelection];
            if (btn == null) return;

            var name = btn.name ?? string.Empty;
            switch (name)
            {
                case Constants.UIClassResumeButton:
                    UIController.UnpauseNow();
                    return;
                case Constants.UIClassQuitButton:
                    _uiQuitGameCmp?.QuitGame();
                    return;
            }
        }
    }
}
