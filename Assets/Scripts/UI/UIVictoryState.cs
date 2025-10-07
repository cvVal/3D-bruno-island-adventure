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
            var playerInputCmp = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<PlayerInput>();

            var victoryContainer = UIController.RootElement.Q<VisualElement>(Constants.UIClassVictoryContainer);

            playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);

            victoryContainer.style.display = DisplayStyle.Flex;

            if (UIController.victoryClip)
                UIController.AudioSourceCmp.PlayOneShot(UIController.victoryClip);

            UIController.canPause = false;
            
            AudioManager.DuckMusic();
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