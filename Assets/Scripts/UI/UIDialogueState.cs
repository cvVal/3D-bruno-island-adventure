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

        public UIDialogueState(UIController ui) : base(ui)
        {
        }

        public override void EnterState()
        {
            _dialogueContainer = UIController.RootElement.Q<VisualElement>("dialogue-container");
            _dialogueText = UIController.RootElement.Q<Label>("dialogue-text");
            _nextButton = UIController.RootElement.Q<VisualElement>("dialogue-next-button");
            _choicesGroup = UIController.RootElement.Q<VisualElement>("choices-group");

            _dialogueContainer.style.display = DisplayStyle.Flex;

            _playerInputCmp = GameObject.FindGameObjectWithTag(Constants.GameManagerTag).GetComponent<PlayerInput>();
            _playerInputCmp.SwitchCurrentActionMap(Constants.UIActionMap);
        }

        public override void SelectButton()
        {
            UpdateDialogue();
        }

        public void SetStory(TextAsset inkJson, GameObject npc)
        {
            _currentStory = new Story(inkJson.text);
            _currentStory.BindExternalFunction("VerifyQuest", VerifyQuest);
            
            _npcControllerCmp = npc.GetComponent<NpcController>();

            if (_npcControllerCmp.hasQuestItem)
            {
                _currentStory.ChoosePathString("postCompletion");
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
            UIController.Buttons[0].AddToClassList("active");

            UIController.currentSelection = 0;
        }

        private void CreateNewChoiceButton(Choice choice)
        {
            var choiceButton = new Button();

            choiceButton.AddToClassList("menu-button");
            choiceButton.text = choice.text;
            choiceButton.style.marginRight = 20;

            _choicesGroup.Add(choiceButton);
        }

        private void ExitDialogue()
        {
            _dialogueContainer.style.display = DisplayStyle.None;
            _playerInputCmp.SwitchCurrentActionMap(Constants.GameplayActionMap);
        }

        private void VerifyQuest()
        {
            _currentStory.variablesState["questCompleted"] = _npcControllerCmp.CheckPlayerForQuestItem();
        }
    }
}