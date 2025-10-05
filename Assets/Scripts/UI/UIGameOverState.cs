using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIGameOverState : UIBaseState
    {
        public UIGameOverState(UIController uiController) : base(uiController)
        {
        }

        public override void EnterState()
        {
            var playerInputCmp = GameObject.FindGameObjectWithTag(Constants.GameManagerTag).GetComponent<PlayerInput>();

            var gameOverContainer = UIController.RootElement.Q<VisualElement>(Constants.UIGameOverContainer);

            playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);

            gameOverContainer.style.display = DisplayStyle.Flex;
        }

        public override void SelectButton()
        {
            PlayerPrefs.DeleteAll();
            SceneTransition.Initiate(0);
        }
    }
}