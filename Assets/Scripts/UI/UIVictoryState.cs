using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIVictoryState : UIBaseState
    {
        public UIVictoryState(UIController uiController) : base(uiController)
        {
        }

        public override void EnterState()
        {
            var playerInputCmp = GameObject.FindGameObjectWithTag(Constants.GameManagerTag).GetComponent<PlayerInput>();
            var victoryContainer = UIController.RootElement.Q<VisualElement>(Constants.UIVictoryContainer);

            playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);

            victoryContainer.style.display = DisplayStyle.Flex;
        }

        public override void SelectButton()
        {
            PlayerPrefs.DeleteAll();
            SceneTransition.Initiate(0);
        }
    }
}