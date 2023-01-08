using System;
using Steel;

namespace SteelCustom.UIElements
{
    public class UIBuildButton : ScriptComponent
    {
        public BuildingType BuildingType { get; set; }
        
        private Entity _tooltip;
        private UIText _text;
        private UIText _text2;

        public override void OnCreate()
        {
            _text = UI.CreateUIText("0", "Price", Entity);
            _text.RectTransform.AnchorMin = Vector2.Zero;
            _text.RectTransform.AnchorMax = Vector2.One;
            _text.RectTransform.OffsetMin = new Vector2(2, 4);
            _text.RectTransform.OffsetMax = new Vector2(14, 2);
            _text.Color = Color.Black;
            _text.TextSize = 64;
            _text.TextAlignment = AlignmentType.BottomRight;

            _text2 = UI.CreateUIText("0", "Price2", Entity);
            _text2.RectTransform.AnchorMin = Vector2.Zero;
            _text2.RectTransform.AnchorMax = Vector2.One;
            _text2.RectTransform.OffsetMin = new Vector2(2, 4);
            _text2.RectTransform.OffsetMax = new Vector2(33 * 4, 2);
            _text2.Color = Color.Black;
            _text2.TextSize = 64;
            _text2.TextAlignment = AlignmentType.BottomRight;
        }

        public override void OnUpdate()
        {
            UpdateState();
        }

        public override void OnMouseOverUI()
        {
            if (Input.IsMouseJustPressed(MouseCodes.ButtonLeft))
                OnMouseEnterUI(); // Refresh tooltip
        }

        private void UpdateState()
        {
            Player player = GameController.Instance.Player;
            
            if (BuildingType == BuildingType.Storage && !player.Storage.CanUpgrade)
            {
                Entity.Destroy();
                return;
            }
            
            GetComponent<UIButton>().Interactable = player.CanBuyBuilding(BuildingType);
            (int Wood, int Corn, int Gold) price = Builder.GetBuildingPrice(BuildingType);

            switch (BuildingType)
            {
                case BuildingType.Ranch:
                case BuildingType.Town:
                    _text.Text = price.Gold.ToString();
                    _text2.Text = price.Corn.ToString();
                    break;
                case BuildingType.Storage:
                    _text.Text = price.Wood.ToString();
                    _text2.Entity.IsActiveSelf = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnMouseEnterUI()
        {
            _tooltip = UITooltip.ShowTooltip(GetTooltipText(), 130);
        }

        private string GetTooltipText()
        {
            switch (BuildingType)
            {
                case BuildingType.Ranch:
                    return "Ranch is the good start for your settlement.\n" +
                           "Harvest resources from adjacent tiles, when the dice rolls it's number.\n" +
                           "Can't place it next to the other buildings.";
                case BuildingType.Town:
                    return "As the time goes - your settlement starts growing.\n" +
                           "Upgrading your ranch to a town will double it's yield.";
                case BuildingType.Storage:
                    Storage storage = GameController.Instance.Player.Storage;
                    if (!storage.CanUpgrade)
                        return "Your storage is fully upgraded.";
                    return $"Sometimes you need to store your goods, before the merchant ship arrives.\n" +
                           $"Upgrade your storage to increase it's capacity from {storage.Capacity} to {storage.GetCapacityForLevel(storage.Level + 1)}.";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnMouseExitUI()
        {
            UITooltip.HideTooltip(_tooltip);
        }
    }
}