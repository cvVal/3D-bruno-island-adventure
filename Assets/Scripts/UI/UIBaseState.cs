namespace RPG.UI
{
    public abstract class UIBaseState
    {
        protected readonly UIController UIController;

        protected UIBaseState(UIController ui)
        {
            UIController = ui;
        }

        public abstract void EnterState();

        public abstract void SelectButton();
    }
}