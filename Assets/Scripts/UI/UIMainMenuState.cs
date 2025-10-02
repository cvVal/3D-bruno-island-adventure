using RPG.Core;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class UIMainMenuState : UIBaseState
    {
        public UIMainMenuState(UIController ui) : base(ui)
        {
        }

        public override void EnterState()
        {
            UIController.MainMenuContainer.style.display = DisplayStyle.Flex;
            
            UIController.Buttons = UIController.MainMenuContainer.Query<Button>(null, "menu-button").ToList();

            UIController.Buttons[0].AddToClassList("active");
        }

        public override void SelectButton()
        {
            var btn = UIController.Buttons[UIController.currentSelection];

            if (btn.name == "start-button")
            {
                SceneTransition.Initiate(1);
            }
        }
    }
}