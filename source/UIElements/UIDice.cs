using Steel;

namespace SteelCustom.UIElements
{
    public class UIDice : ScriptComponent
    {
        private Entity _tooltip;

        public override void OnMouseEnterUI()
        {
            _tooltip = UITooltip.ShowTooltip("Roll the dice each turn to gains resources near your town.\n" +
                                             "Roll 7 to get the merchant ship.", 120);
        }

        public override void OnMouseExitUI()
        {
            UITooltip.HideTooltip(_tooltip);
        }
    }
}