using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIMainMenuState : UIBaseState
    {
        private int _sceneIndex;

        public UIMainMenuState(UIController ui) : base(ui)
        {
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
            else
            {
                UIController.StartCoroutine(
                    SceneTransition.Initiate(_sceneIndex, UIController.AudioSourceCmp)
                );
            }
        }

        private void AddButton()
        {
            var continueButton = new Button();
            continueButton.AddToClassList(Constants.UIClassMenuButton);
            continueButton.text = "Continue";

            var mainMenuButtons = UIController.MainMenuContainer.Q<VisualElement>(Constants.UIClassButtons);
            mainMenuButtons.Add(continueButton);
        }
    }
}