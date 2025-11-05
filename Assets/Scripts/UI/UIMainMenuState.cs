using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIMainMenuState : UIBaseState
    {
        private int _sceneIndex;
        private readonly UIQuitGame _uiQuitGameCmp;

        public UIMainMenuState(UIController ui) : base(ui)
        {
            _uiQuitGameCmp = ui.GetComponent<UIQuitGame>();
        }

        public override void EnterState()
        {
            if (PlayerPrefs.HasKey(Constants.PlayerPrefsSceneIndex))
            {
                _sceneIndex = PlayerPrefs.GetInt(Constants.PlayerPrefsSceneIndex);
                AddButton();
            }

            UIController.MainMenuContainer.style.display = DisplayStyle.Flex;

            UIController.Buttons = UIController.MainMenuContainer
                .Query<Button>(null, Constants.UIClassMenuButton)
                .ToList();

            UIController.Buttons[0].AddToClassList(Constants.UIClassActive);
        }

        public override void SelectButton()
        {
            var btn = UIController.Buttons[UIController.currentSelection];

            if (btn.name == Constants.UIClassStartButton)
            {
                PlayerPrefs.DeleteAll();

                UIController.StartCoroutine(
                    SceneTransition.Initiate(1, UIController.AudioSourceCmp)
                );
            }
            else if (btn.name == Constants.UIClassQuitButton)
            {
                _uiQuitGameCmp.QuitGame();
            }
            else
            {
                UIController.StartCoroutine(
                    SceneTransition.Initiate(_sceneIndex, UIController.AudioSourceCmp)
                );
            }
        }

        private void AddButton()
        {
            var mainMenuButtons = UIController.MainMenuContainer.Q<VisualElement>(Constants.UIClassButtons);

            var continueButton = new Button();
            continueButton.AddToClassList(Constants.UIClassMenuButton);
            continueButton.text = "Continue";

            mainMenuButtons.Add(continueButton);

            // Ensure the Quit button is always last. If a Quit button exists, remove it and re-add it so it becomes the last child.
            var quitButton = mainMenuButtons.Q<Button>(Constants.UIClassQuitButton);

            if (quitButton == null) return;

            mainMenuButtons.Remove(quitButton);
            mainMenuButtons.Add(quitButton);
        }
    }
}
