using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIController : MonoBehaviour
    {
        private UIDocument _uiDocumentCmp;

        public UIBaseState CurrentState;
        public UIMainMenuState MainMenuState;
        public VisualElement RootElement;
        public List<Button> Buttons;

        public int currentSelection;

        private void Awake()
        {
            MainMenuState = new UIMainMenuState(this);

            _uiDocumentCmp = GetComponent<UIDocument>();
            RootElement = _uiDocumentCmp.rootVisualElement;

            Buttons = new List<Button>();
        }

        private void Start()
        {
            CurrentState = MainMenuState;
            CurrentState.EnterState();
        }

        public void HandleInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            CurrentState.SelectButton();
        }

        public void HandleNavigate(InputAction.CallbackContext context)
        {
            if (!context.performed || Buttons == null || Buttons.Count == 0) return;

            Buttons[currentSelection].RemoveFromClassList("active");

            var input = context.ReadValue<Vector2>();
            currentSelection += input.x > 0 ? 1 : -1;
            currentSelection = Mathf.Clamp(currentSelection, 0, Buttons.Count - 1);
            Buttons[currentSelection].AddToClassList("active");
        }
    }
}