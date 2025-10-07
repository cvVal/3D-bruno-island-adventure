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
        private VisualElement _nextButton;
        private VisualElement _choicesGroup;
        private Story _currentStory;
        private PlayerInput _playerInputCmp;
        private bool _hasChoices;
        private NpcController _npcControllerCmp;
        private Vector3 _npcOriginalForward;

        public UIDialogueState(UIController ui) : base(ui)
        {
        }

        public override void EnterState()
        {
            _dialogueContainer = UIController.RootElement.Q<VisualElement>(Constants.UIClassContainer);
            _dialogueText = UIController.RootElement.Q<Label>(Constants.UIClassDialogueText);
            _nextButton = UIController.RootElement.Q<VisualElement>(Constants.UIClassDialogueNextButton);
            _choicesGroup = UIController.RootElement.Q<VisualElement>(Constants.UIClassChoicesGroup);

            _dialogueContainer.style.display = DisplayStyle.Flex;

            _playerInputCmp = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<PlayerInput>();

            _playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);

            UIController.canPause = false;
        }

        public override void SelectButton()
        {
            UpdateDialogue();
        }

        public void SetStory(TextAsset inkJson, GameObject npc)
        {
            _currentStory = new Story(inkJson.text);
            _currentStory.BindExternalFunction(Constants.InkStoryVerifyQuest, VerifyQuest);

            _npcControllerCmp = npc.GetComponent<NpcController>();

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
                _currentStory.ChoosePathString(Constants.InkStoryPostcompletion);
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

            _dialogueText.text = _currentStory.Continue();

            _hasChoices = _currentStory.currentChoices.Count > 0;

            if (_hasChoices)
            {
                HandleNewChoices(_currentStory.currentChoices);
            }
            else
            {
                _nextButton.style.display = DisplayStyle.Flex;
                _choicesGroup.style.display = DisplayStyle.None;
            }
        }

        private void HandleNewChoices(List<Choice> choices)
        {
            _nextButton.style.display = DisplayStyle.None;
            _choicesGroup.style.display = DisplayStyle.Flex;

            _choicesGroup.Clear();
            UIController.Buttons?.Clear();

            choices.ForEach(CreateNewChoiceButton);

            UIController.Buttons = _choicesGroup.Query<Button>().ToList();
            UIController.Buttons[0].AddToClassList(Constants.UIClassActive);

            UIController.currentSelection = 0;
        }

        private void CreateNewChoiceButton(Choice choice)
        {
            var choiceButton = new Button();

            choiceButton.AddToClassList(Constants.UIClassMenuButton);
            choiceButton.text = choice.text;
            choiceButton.style.marginRight = 20;

            _choicesGroup.Add(choiceButton);
        }

        private void ExitDialogue()
        {
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

        private void VerifyQuest()
        {
            _currentStory.variablesState[Constants.InkStoryQuestCompleted] =
                _npcControllerCmp.CheckPlayerForQuestItem();
        }
    }
}