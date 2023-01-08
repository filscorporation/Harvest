using Steel;

namespace SteelCustom.UIElements
{
    public class UIStorage : ScriptComponent
    {
        private UIText _text;
        private UIImage _fullImage;
        private Entity _tooltip;

        public override void OnCreate()
        {
            _text = UI.CreateUIText("0/0", "Capacity", Entity);
            _text.RectTransform.AnchorMin = Vector2.Zero;
            _text.RectTransform.AnchorMax = Vector2.One;
            _text.RectTransform.OffsetMin = new Vector2(2, 2);
            _text.RectTransform.OffsetMax = new Vector2(2, 2);
            _text.Color = Color.Black;
            _text.TextSize = 64;
            _text.TextAlignment = AlignmentType.BottomRight;
            
            _fullImage = UI.CreateUIImage(ResourcesManager.GetImage("ui_storage_full.png"), "Storage full", Entity);
            _fullImage.RectTransform.AnchorMin = new Vector2(0, 0);
            _fullImage.RectTransform.AnchorMax = new Vector2(0, 0);
            _fullImage.RectTransform.Pivot = new Vector2(0, 1);
            _fullImage.RectTransform.Size = new Vector2(54 * 4, 16 * 4);
            _fullImage.RectTransform.AnchoredPosition = new Vector2(0, -4);

            _fullImage.Entity.IsActiveSelf = false;
        }

        public override void OnMouseEnterUI()
        {
            _tooltip = UITooltip.ShowTooltip("When storage is full, you can not gain new resources.\nYou can upgrade capacity in build menu", 100);
        }

        public override void OnMouseExitUI()
        {
            UITooltip.HideTooltip(_tooltip);
        }
        
        public override void OnUpdate()
        {
            Player player = GameController.Instance.Player;
            _text.Text = $"{player.ResourcesAmount}/{player.Storage.Capacity}";
            
            _fullImage.Entity.IsActiveSelf = player.IsStorageFull;
        }
    }
}