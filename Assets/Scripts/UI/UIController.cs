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

        public UIBaseState CurrentState;
        public UIMainMenuState MainMenuState;
        public VisualElement RootElement;
        public List<Button> Buttons;
        public VisualElement MainMenuContainer;
        public VisualElement PlayerHUDContainer;
        public Label HealthLabel;
        public Label PotionsLabel;

        public int currentSelection;

        private void Awake()
        {
            MainMenuState = new UIMainMenuState(this);

            _uiDocumentCmp = GetComponent<UIDocument>();
            RootElement = _uiDocumentCmp.rootVisualElement;

            Buttons = new List<Button>();

            MainMenuContainer = RootElement.Q<VisualElement>("main-menu-container");
            PlayerHUDContainer = RootElement.Q<VisualElement>("player-info-container");
            HealthLabel = PlayerHUDContainer.Q<Label>("health-label");
            PotionsLabel = PlayerHUDContainer.Q<Label>("potion-label");
        }

        private void Start()
        {
            var sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (sceneIndex == 0)
            {
                CurrentState = MainMenuState;
                CurrentState.EnterState();
            }
            else
            {
                PlayerHUDContainer.style.display = DisplayStyle.Flex;
            }
        }

        private void OnEnable()
        {
            EventManager.OnChangePlayerHealth += HandleChangePlayerHealth;
            EventManager.OnChangePlayerPotions += HandleChangePlayerPotions;
        }

        private void OnDisable()
        {
            EventManager.OnChangePlayerHealth -= HandleChangePlayerHealth;
            EventManager.OnChangePlayerPotions -= HandleChangePlayerPotions;
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

        private void HandleChangePlayerHealth(float newHealthPoints)
        {
            HealthLabel.text = newHealthPoints.ToString(CultureInfo.InvariantCulture);
        }

        private void HandleChangePlayerPotions(int newPotionCount)
        {
            PotionsLabel.text = newPotionCount.ToString(CultureInfo.InvariantCulture);
        }
    }
}