using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIQuestItemState : UIBaseState
    {
        private VisualElement _questItemContainer;
        private Label _questItemText;
        private PlayerInput _playerInputCmp;

        public UIQuestItemState(UIController ui) : base(ui)
        {
        }

        public override void EnterState()
        {
            _playerInputCmp = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<PlayerInput>();

            _playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);

            _questItemContainer = UIController.RootElement.Q<VisualElement>(Constants.UIClassItemContainer);
            _questItemText = _questItemContainer.Q<Label>(Constants.UIClassQuestItemLabel);

            _questItemContainer.style.display = DisplayStyle.Flex;

            EventManager.RaiseToggleUI(true);
        }

        public override void SelectButton()
        {
            _questItemContainer.style.display = DisplayStyle.None;
            _playerInputCmp.SwitchCurrentActionMap(Constants.GameplayActionMap);

            EventManager.RaiseToggleUI(false);
        }

        public void SetQuestItemLabel(string name)
        {
            _questItemText.text = name;
        }
    }
}