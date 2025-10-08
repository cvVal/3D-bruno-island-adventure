using System.Collections.Generic;
using Ink.Runtime;
using RPG.Character;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIDialogueState : UIBaseState
    {
        private VisualElement _dialogueContainer;
        private Label _dialogueText;
        private Label _npcNameText;
        private VisualElement _nextButton;
        private VisualElement _choicesGroup;
        private Story _currentStory;
        private PlayerInput _playerInputCmp;
        private bool _hasChoices;
        private NpcController _npcControllerCmp;

        private Vector3 _npcOriginalForward;

        // Baseline text height stabilization
        private bool _baselineTextHeightCaptured;
        private float _baselineTextHeight;

        // Typewriter effect
        private bool _isTyping;
        private string _currentLineFullText = string.Empty;
        private float _typingStartTime;
        private IVisualElementScheduledItem _typingSchedule;
        private const float CharactersPerSecond = 40f; // tuning knob
        private const float MinTypeIntervalMs = 16f; // ~60fps update cadence

        // Fade-in effect
        private IVisualElementScheduledItem _fadeSchedule;
        private float _fadeStartTime;
        private const float FadeDuration = 0.25f; // seconds
        private bool _fadeCompleted;
        private bool _isPausedForReward;

        public UIDialogueState(UIController ui) : base(ui)
        {
        }

        public override void EnterState()
        {
            // Reset transient state in case of re-entry
            _isTyping = false;
            _fadeCompleted = false;
            _isPausedForReward = false;
            _currentLineFullText = string.Empty;
            _typingSchedule?.Pause();
            _fadeSchedule?.Pause();

            _dialogueContainer = UIController.RootElement.Q<VisualElement>(Constants.UIClassContainer);
            _dialogueText = UIController.RootElement.Q<Label>(Constants.UIClassDialogueText);
            _npcNameText = UIController.RootElement.Q<Label>(Constants.UIClassDialogueNpcNameText);
            _nextButton = UIController.RootElement.Q<VisualElement>(Constants.UIClassDialogueNextButton);
            _choicesGroup = UIController.RootElement.Q<VisualElement>(Constants.UIClassChoicesGroup);

            _dialogueContainer.style.display = DisplayStyle.Flex;

            // Start hidden (fade-in)
            _dialogueContainer.style.opacity = 0f;
            BeginFadeIn();

            _playerInputCmp = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<PlayerInput>();

            _playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);

            UIController.canPause = false;
        }

        public override void SelectButton()
        {
            // If we are mid-typewriter, skip to full line instead of advancing story
            if (_isTyping)
            {
                CompleteTyping();
                return;
            }

            UpdateDialogue();
        }

        public void PauseForReward()
        {
            if (_dialogueContainer == null) return;

            if (_isTyping)
            {
                CompleteTyping();
            }

            _typingSchedule?.Pause();
            _dialogueContainer.style.display = DisplayStyle.None;
            _isPausedForReward = true;
        }

        public void ResumeAfterReward()
        {
            if (!_isPausedForReward) return;

            _dialogueContainer.style.display = DisplayStyle.Flex;
            _dialogueContainer.style.opacity = 1f;

            if (!_playerInputCmp)
            {
                _playerInputCmp = GameObject
                    .FindGameObjectWithTag(Constants.GameManagerTag)
                    .GetComponent<PlayerInput>();
            }

            _playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);

            UIController.canPause = false;
            _isPausedForReward = false;
        }

        public void SetStory(GameObject npc)
        {
            _npcControllerCmp = npc.GetComponent<NpcController>();
            _currentStory = _npcControllerCmp.GetOrCreateStory();

            _npcNameText.text = _npcControllerCmp.name;

            // Store NPC's original rotation and rotate both to face each other
            _npcOriginalForward = npc.transform.forward;

            var player = GameObject.FindGameObjectWithTag(Constants.PlayerTag);
            if (player)
            {
                var directionToPlayer = (player.transform.position - npc.transform.position).normalized;
                directionToPlayer.y = 0;

                var directionToNpc = (npc.transform.position - player.transform.position).normalized;
                directionToNpc.y = 0;

                // Instantly rotate both NPC and player to face each other
                if (directionToPlayer != Vector3.zero)
                    npc.transform.rotation = Quaternion.LookRotation(directionToPlayer);

                if (directionToNpc != Vector3.zero)
                    player.transform.rotation = Quaternion.LookRotation(directionToNpc);
            }

            if (_npcControllerCmp.hasQuestItem)
            {
                _currentStory.ChoosePathString(Constants.InkStoryPostCompletionKnot);
            }

            UpdateDialogue();
        }

        private void UpdateDialogue()
        {
            if (_hasChoices)
            {
                _currentStory.ChooseChoiceIndex(UIController.currentSelection);
            }

            if (!_currentStory.canContinue)
            {
                ExitDialogue();
                return;
            }

            // Hide buttons while new line types
            _nextButton.style.visibility = Visibility.Hidden;
            _choicesGroup.style.display = DisplayStyle.None;

            _currentLineFullText = _currentStory.Continue();

            // Enforce baseline min height to prevent visual shrinking when clearing text
            if (_baselineTextHeightCaptured)
            {
                _dialogueText.style.minHeight = _baselineTextHeight;
            }

            _dialogueText.text = string.Empty;

            // Evaluate choices AFTER typing finishes; store flag for later
            _hasChoices = _currentStory.currentChoices.Count > 0;

            StartTyping();
        }

        private void StartTyping()
        {
            _isTyping = true;
            _typingStartTime = Time.realtimeSinceStartup;

            // Cancel prior schedule if any
            _typingSchedule?.Pause();

            _typingSchedule = _dialogueContainer.schedule
                .Execute(TypingTick)
                .Every((long)MinTypeIntervalMs);

            // Immediate first tick to avoid perceived delay for very short lines
            TypingTick();
        }

        private void TypingTick()
        {
            if (!_isTyping)
                return;

            if (string.IsNullOrEmpty(_currentLineFullText))
            {
                CompleteTyping();
                return;
            }

            var elapsed = Time.realtimeSinceStartup - _typingStartTime;
            var targetVisible = Mathf.Clamp(Mathf.FloorToInt(elapsed * CharactersPerSecond), 0,
                _currentLineFullText.Length);
            if (targetVisible <= _dialogueText.text.Length)
                return; // wait for next interval

            _dialogueText.text = _currentLineFullText[..targetVisible];

            if (targetVisible >= _currentLineFullText.Length)
            {
                CompleteTyping();
            }
        }

        private void CompleteTyping()
        {
            if (!_isTyping) return;
            _isTyping = false;
            _dialogueText.text = _currentLineFullText; // ensure full text is shown
            _typingSchedule?.Pause();

            // Capture / update baseline text height after layout resolves
            UIController.RootElement.schedule.Execute(() =>
            {
                if (_dialogueText == null) return;
                var h = _dialogueText.resolvedStyle.height;

                if (!(h > 0)) return;

                if (_baselineTextHeightCaptured && !(h > _baselineTextHeight)) return;

                _baselineTextHeight = h;
                _baselineTextHeightCaptured = true;
                _dialogueText.style.minHeight = _baselineTextHeight;
            });

            // Now reveal appropriate navigation UI
            if (_hasChoices)
            {
                HandleNewChoices(_currentStory.currentChoices);
            }
            else
            {
                // Show next button without altering layout height
                _nextButton.style.visibility = Visibility.Visible;
            }
        }

        private void HandleNewChoices(List<Choice> choices)
        {
            // Hide next button but keep its reserved space (visibility instead of display)
            _nextButton.style.visibility = Visibility.Hidden;
            _choicesGroup.style.display = DisplayStyle.Flex;

            _choicesGroup.Clear();
            UIController.Buttons?.Clear();

            choices.ForEach(CreateNewChoiceButton);

            UIController.Buttons = _choicesGroup.Query<Button>().ToList();
            if (UIController.Buttons.Count > 0)
            {
                UIController.Buttons[0].AddToClassList(Constants.UIClassActive);
                UIController.currentSelection = 0;
            }

            NormalizeChoiceButtonWidths();
        }

        private void CreateNewChoiceButton(Choice choice)
        {
            var choiceButton = new Button();
            choiceButton.AddToClassList(Constants.UIClassMenuButton);
            choiceButton.text = choice.text;
            choiceButton.style.marginRight = 0;
            _choicesGroup.Add(choiceButton);
        }

        private void NormalizeChoiceButtonWidths()
        {
            var buttons = _choicesGroup.Query<Button>().ToList();

            if (buttons == null || buttons.Count == 0) return;

            var longest = 0;

            foreach (var b in buttons)
            {
                if (!string.IsNullOrEmpty(b.text) && b.text.Length > longest)
                    longest = b.text.Length;
            }

            if (longest == 0) longest = 4;

            const float estCharPx = 18f;
            const float paddingAllowance = 64f;
            var rawWidth = longest * estCharPx + paddingAllowance;
            var clamped = Mathf.Clamp(rawWidth, 200f, 400f);

            foreach (var b in buttons)
            {
                b.style.width = clamped;
                // Enable text wrapping for very long choices
                b.style.whiteSpace = WhiteSpace.Normal;
                // Ensure minimum height for wrapped text
                b.style.minHeight = 40f;
            }
        }

        private void ExitDialogue()
        {
            _typingSchedule?.Pause();
            _fadeSchedule?.Pause();
            _isTyping = false;
            _currentLineFullText = string.Empty;
            _baselineTextHeightCaptured = false; // reset for a fresh conversation
            _baselineTextHeight = 0f;

            _dialogueContainer.style.display = DisplayStyle.None;
            _playerInputCmp.SwitchCurrentActionMap(Constants.GameplayActionMap);

            // Restore NPC's original rotation instantly
            if (_npcOriginalForward != Vector3.zero)
            {
                var npc = _npcControllerCmp.gameObject;
                npc.transform.rotation = Quaternion.LookRotation(_npcOriginalForward);
            }

            UIController.canPause = true;
        }

        private void BeginFadeIn()
        {
            _fadeStartTime = Time.realtimeSinceStartup;
            _fadeCompleted = false;
            _fadeSchedule?.Pause();
            _fadeSchedule = _dialogueContainer.schedule.Execute(FadeTick).Every(16); // ~60fps
            FadeTick();
        }

        private void FadeTick()
        {
            if (_fadeCompleted || _dialogueContainer == null) return;

            var t = (Time.realtimeSinceStartup - _fadeStartTime) / FadeDuration;

            if (t >= 1f)
            {
                _dialogueContainer.style.opacity = 1f;
                _fadeCompleted = true;
                _fadeSchedule?.Pause();
            }
            else
            {
                _dialogueContainer.style.opacity = Mathf.SmoothStep(0f, 1f, t);
            }
        }
    }
}