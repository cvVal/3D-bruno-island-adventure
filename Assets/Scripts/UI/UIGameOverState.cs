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
            
            if (UIController.gameOverClip)
                UIController.AudioSourceCmp.PlayOneShot(UIController.gameOverClip);
        }

        public override void SelectButton()
        {
            PlayerPrefs.DeleteAll();

            UIController.StartCoroutine(
                SceneTransition.Initiate(0, UIController.AudioSourceCmp)
            );
        }
    }
}