using Steel;

namespace SteelCustom.UIElements
{
    public class UITurnCounter : ScriptComponent
    {
        private UIText _text;
        private Entity _tooltip;

        public override void OnCreate()
        {
            _text = UI.CreateUIText("0/0", "Counter", Entity);
            _text.RectTransform.AnchorMin = Vector2.Zero;
            _text.RectTransform.AnchorMax = Vector2.One;
            _text.RectTransform.OffsetMin = new Vector2(2, 2);
            _text.RectTransform.OffsetMax = new Vector2(2, 2);
            _text.Color = Color.Black;
            _text.TextSize = 64;
            _text.TextAlignment = AlignmentType.TopMiddle;
        }

        public override void OnMouseEnterUI()
        {
            _tooltip = UITooltip.ShowTooltip($"Game will end in {GameController.MAX_TURN - GameController.Instance.CurrentTurn} turns.\nGet as much gold as you can!\nYou will get guaranteed merchant at last turn.", 100);
        }

        public override void OnMouseExitUI()
        {
            UITooltip.HideTooltip(_tooltip);
        }
        
        public override void OnUpdate()
        {
            _text.Text = $"{GameController.Instance.CurrentTurn}/{GameController.MAX_TURN}";
            if (GameController.MAX_TURN - GameController.Instance.CurrentTurn <= 3)
                _text.Color = new Color(147, 63, 69);
        }
    }
}