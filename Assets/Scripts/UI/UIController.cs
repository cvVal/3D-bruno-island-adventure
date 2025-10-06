using System;
using System.Collections.Generic;
using System.Globalization;
using RPG.Core;
using RPG.Quest;
using RPG.Utility;
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
        private UIQuestItemState _questItemState;
        private UIVictoryState _victoryState;
        private UIGameOverState _gameOverState;

        private VisualElement _playerHUDContainer;
        private Label _healthLabel;
        private Label _potionsLabel;
        private VisualElement _questItemIcon;

        public VisualElement RootElement;
        public List<Button> Buttons;
        public VisualElement MainMenuContainer;
        public int currentSelection;
        public AudioClip victoryClip;
        public AudioClip gameOverClip;
        
        [NonSerialized] public AudioSource AudioSourceCmp;

        private void Awake()
        {
            _mainMenuState = new UIMainMenuState(this);
            _dialogueState = new UIDialogueState(this);
            _questItemState = new UIQuestItemState(this);
            _victoryState = new UIVictoryState(this);
            _gameOverState = new UIGameOverState(this);

            _uiDocumentCmp = GetComponent<UIDocument>();
            RootElement = _uiDocumentCmp.rootVisualElement;
            
            AudioSourceCmp = GetComponent<AudioSource>();

            Buttons = new List<Button>();

            MainMenuContainer = RootElement.Q<VisualElement>(Constants.UIClassMainMenuContainer);
            _playerHUDContainer = RootElement.Q<VisualElement>(Constants.UIClassPlayerInfoContainer);
            _healthLabel = _playerHUDContainer.Q<Label>(Constants.UIClassHealthLabel);
            _potionsLabel = _playerHUDContainer.Q<Label>(Constants.UIClassPotionLabel);
            _questItemIcon = _playerHUDContainer.Q<VisualElement>(Constants.UIClassQuestItemIcon);
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
            EventManager.OnTreasureChestUnlocked += HandleTreasureChestUnlocked;
            EventManager.OnVictory += HandleVictory;
            EventManager.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            EventManager.OnChangePlayerHealth -= HandleChangePlayerHealth;
            EventManager.OnChangePlayerPotions -= HandleChangePlayerPotions;
            EventManager.OnInitiateDialogue -= HandleInitiateDialogue;
            EventManager.OnTreasureChestUnlocked -= HandleTreasureChestUnlocked;
            EventManager.OnVictory -= HandleVictory;
            EventManager.OnGameOver -= HandleGameOver;
        }

        public void HandleInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            _currentState.SelectButton();
        }

        public void HandleNavigate(InputAction.CallbackContext context)
        {
            if (!context.performed || Buttons == null || Buttons.Count == 0) return;

            Buttons[currentSelection].RemoveFromClassList(Constants.UIClassActive);

            var input = context.ReadValue<Vector2>();
            currentSelection += input.x > 0 ? 1 : -1;
            currentSelection = Mathf.Clamp(currentSelection, 0, Buttons.Count - 1);
            Buttons[currentSelection].AddToClassList(Constants.UIClassActive);
        }

        private void HandleChangePlayerHealth(float newHealthPoints)
        {
            _healthLabel.text = newHealthPoints.ToString(CultureInfo.InvariantCulture);
        }

        private void HandleChangePlayerPotions(int newPotionCount)
        {
            _potionsLabel.text = newPotionCount.ToString(CultureInfo.InvariantCulture);
        }

        private void HandleInitiateDialogue(TextAsset inkJson, GameObject npc)
        {
            _currentState = _dialogueState;
            _currentState.EnterState();

            (_currentState as UIDialogueState)?.SetStory(inkJson, npc);
        }

        private void HandleTreasureChestUnlocked(QuestItemSo itemSo, bool showUI)
        {
            _questItemIcon.style.display = DisplayStyle.Flex;

            if (!showUI) return;

            _currentState = _questItemState;
            _currentState.EnterState();

            (_currentState as UIQuestItemState)?.SetQuestItemLabel(itemSo.itemName);
        }

        private void HandleVictory()
        {
            _currentState = _victoryState;
            _currentState.EnterState();
        }

        private void HandleGameOver()
        {
            _currentState = _gameOverState;
            _currentState.EnterState();
        }
    }
}