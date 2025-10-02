using System.Collections.Generic;
using System.Globalization;
using RPG.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIController : MonoBehaviour
    {
        private UIDocument _uiDocumentCmp;
        private UIBaseState _currentState;
        private UIMainMenuState _mainMenuState;
        private UIDialogueState _dialogueState;
        private VisualElement _playerHUDContainer;
        private Label _healthLabel;
        private Label _potionsLabel;

        public VisualElement RootElement;
        public List<Button> Buttons;
        public VisualElement MainMenuContainer;
        public int currentSelection;

        private void Awake()
        {
            _mainMenuState = new UIMainMenuState(this);
            _dialogueState = new UIDialogueState(this);

            _uiDocumentCmp = GetComponent<UIDocument>();
            RootElement = _uiDocumentCmp.rootVisualElement;

            Buttons = new List<Button>();

            MainMenuContainer = RootElement.Q<VisualElement>("main-menu-container");
            _playerHUDContainer = RootElement.Q<VisualElement>("player-info-container");
            _healthLabel = _playerHUDContainer.Q<Label>("health-label");
            _potionsLabel = _playerHUDContainer.Q<Label>("potion-label");
        }

        private void Start()
        {
            var sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (sceneIndex == 0)
            {
                _currentState = _mainMenuState;
                _currentState.EnterState();
            }
            else
            {
                _playerHUDContainer.style.display = DisplayStyle.Flex;
            }
        }

        private void OnEnable()
        {
            EventManager.OnChangePlayerHealth += HandleChangePlayerHealth;
            EventManager.OnChangePlayerPotions += HandleChangePlayerPotions;
            EventManager.OnInitiateDialogue += HandleInitiateDialogue;
        }

        private void OnDisable()
        {
            EventManager.OnChangePlayerHealth -= HandleChangePlayerHealth;
            EventManager.OnChangePlayerPotions -= HandleChangePlayerPotions;
            EventManager.OnInitiateDialogue -= HandleInitiateDialogue;
        }

        public void HandleInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            _currentState.SelectButton();
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

        private void HandleChangePlayerHealth(float newHealthPoints)
        {
            _healthLabel.text = newHealthPoints.ToString(CultureInfo.InvariantCulture);
        }

        private void HandleChangePlayerPotions(int newPotionCount)
        {
            _potionsLabel.text = newPotionCount.ToString(CultureInfo.InvariantCulture);
        }

        private void HandleInitiateDialogue(TextAsset inkJson)
        {
            _currentState = _dialogueState;
            _currentState.EnterState();

            (_currentState as UIDialogueState)?.SetStory(inkJson);
        }
    }
}