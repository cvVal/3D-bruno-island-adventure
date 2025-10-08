using System;
using System.Collections.Generic;
using System.Globalization;
using RPG.Core;
using RPG.Quest;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIRewardState : UIBaseState
    {
        private VisualElement _rewardContainer;
        private Label _rewardTitleLabel;
        private Label _rewardDetailsLabel;
        private Button _continueButton;
        private PlayerInput _playerInputCmp;
        private RewardSo _currentReward;
        private Action _onConfirmed;

        public UIRewardState(UIController ui) : base(ui)
        {
        }

        public void Configure(
            RewardSo reward,
            Action onConfirmed
        )
        {
            _currentReward = reward;
            _onConfirmed = onConfirmed;
        }

        public override void EnterState()
        {
            _playerInputCmp = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<PlayerInput>();

            _playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);

            _rewardContainer ??= UIController.RootElement.Q<VisualElement>(Constants.UIClassRewardContainer);

            if (_rewardContainer == null)
            {
                Debug.LogWarning("Reward container not found in UI document.");

                _onConfirmed?.Invoke();

                EventManager.RaiseToggleUI(false);

                UIController.RestoreStateAfterReward();

                return;
            }

            _rewardTitleLabel ??= _rewardContainer.Q<Label>(Constants.UIClassRewardTitleLabel);
            _rewardDetailsLabel ??= _rewardContainer.Q<Label>(Constants.UIClassRewardDetailsLabel);
            _continueButton ??= _rewardContainer.Q<Button>(Constants.UIClassRewardButton);

            PopulateRewardDetails();

            _rewardContainer.style.display = DisplayStyle.Flex;

            UIController.Buttons?.ForEach(button => button.RemoveFromClassList(Constants.UIClassActive));
            UIController.Buttons = new List<Button>();

            if (_continueButton != null)
            {
                _continueButton.RemoveFromClassList(Constants.UIClassActive);
                UIController.Buttons.Add(_continueButton);
                UIController.currentSelection = 0;
                _continueButton.AddToClassList(Constants.UIClassActive);
            }

            EventManager.RaiseToggleUI(true);
            UIController.canPause = false;
        }

        private void PopulateRewardDetails()
        {
            // No reward configured. Show a generic message and exit early.
            if (!_currentReward)
            {
                if (_rewardTitleLabel != null)
                    _rewardTitleLabel.text = "Reward Earned!";

                if (_rewardDetailsLabel != null)
                    _rewardDetailsLabel.text = "You received a mysterious gift.";

                return;
            }

            // Standard reward flow: keep the celebratory title.
            if (_rewardTitleLabel != null)
                _rewardTitleLabel.text = "Reward Earned!";

            // No details label means nothing else can be displayed.
            if (_rewardDetailsLabel == null) return;

            var lines = new List<string>();

            if (_currentReward.bonusHealth > 0)
                lines.Add($"+{FormatNumber(_currentReward.bonusHealth)} {Constants.PlayerPrefsHealth}");

            if (_currentReward.bonusDamage > 0)
                lines.Add($"+{FormatNumber(_currentReward.bonusDamage)} {Constants.PlayerPrefsDamage}");

            if (_currentReward.bonusPotion > 0)
            {
                var potionLabel = _currentReward.bonusPotion == 1 ? "Potion" : Constants.PlayerPrefsPotions;
                lines.Add($"+{_currentReward.bonusPotion} {potionLabel}");
            }

            if (_currentReward.forceWeaponSwap)
                lines.Add($"New Weapon: {_currentReward.weapon}");

            if (lines.Count == 0)
                lines.Add("You've earned a special boon!");

            _rewardDetailsLabel.text = string.Join("\n", lines);
        }

        private static string FormatNumber(float value)
        {
            return Mathf.Approximately(value % 1f, 0f)
                ? value.ToString("0", CultureInfo.InvariantCulture)
                : value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        public override void SelectButton()
        {
            if (_rewardContainer != null)
                _rewardContainer.style.display = DisplayStyle.None;

            if (UIController.Buttons != null)
            {
                foreach (var button in UIController.Buttons)
                {
                    button.RemoveFromClassList(Constants.UIClassActive);
                }

                UIController.Buttons.Clear();
            }

            _onConfirmed?.Invoke();

            EventManager.RaiseToggleUI(false);

            UIController.RestoreStateAfterReward();

            _currentReward = null;
            _onConfirmed = null;
        }
    }
}