using Steel;

namespace SteelCustom.UIElements
{
    public class UIShop : ScriptComponent
    {
        private bool _initialized = false;
        private (UIButton BuyButton, UIText BuyPrice, UIButton SellButton, UIText SellPrice)[] _uiElements
            = new (UIButton BuyButton, UIText BuyPrice, UIButton SellButton, UIText SellPrice)[5];
        
        public void UpdateState(MerchantShip merchantShip, ShopState shopState)
        {
            if (!_initialized)
                Init(merchantShip);

            for (int i = 0; i < 5; i++)
            {
                _uiElements[i].BuyButton.Entity.IsActiveSelf = shopState.BuyPrice[i] >= 0;
                _uiElements[i].BuyButton.Interactable = merchantShip.CanBuyResource((ResourceType)i);
                _uiElements[i].BuyPrice.Text = $"-{shopState.BuyPrice[i]}";
                _uiElements[i].SellButton.Entity.IsActiveSelf = shopState.SellPrice[i] >= 0;
                _uiElements[i].SellButton.Interactable = merchantShip.CanSellResource((ResourceType)i);
                _uiElements[i].SellPrice.Text = $"+{shopState.SellPrice[i]}";
                _uiElements[i].SellPrice.Color = shopState.SellPrice[i] == ShopState.MAX_PRICE ? new Color(147, 63, 69) : Color.Black;
            }
        }

        private void Init(MerchantShip merchantShip)
        {
            _initialized = true;
            
            Sprite priceSprite = ResourcesManager.GetImage("ui_price.png");
            
            UIImage buyLabelImage = UI.CreateUIImage(ResourcesManager.GetImage("ui_buy.png"), "Buy label", Entity);
            buyLabelImage.RectTransform.AnchorMin = new Vector2(0, 1);
            buyLabelImage.RectTransform.AnchorMax = new Vector2(0, 1);
            buyLabelImage.RectTransform.Pivot = new Vector2(0, 1);
            buyLabelImage.RectTransform.Size = new Vector2(28 * 4, 16 * 4);
            buyLabelImage.RectTransform.AnchoredPosition = new Vector2(8, -8);
            
            UIImage sellLabelImage = UI.CreateUIImage(ResourcesManager.GetImage("ui_sell.png"), "Sell label", Entity);
            sellLabelImage.RectTransform.AnchorMin = new Vector2(0, 1);
            sellLabelImage.RectTransform.AnchorMax = new Vector2(0, 1);
            sellLabelImage.RectTransform.Pivot = new Vector2(0, 1);
            sellLabelImage.RectTransform.Size = new Vector2(28 * 4, 16 * 4);
            sellLabelImage.RectTransform.AnchoredPosition = new Vector2(8 + 28 * 4 + 4 + 16 * 4 + 4, -8);

            for (int i = 0; i < 5; i++)
            {
                ResourceType resourceType = (ResourceType)i;
                
                UIButton buyPriceButton = UI.CreateUIButton(priceSprite, "Buy", Entity);
                buyPriceButton.RectTransform.AnchorMin = new Vector2(0, 1);
                buyPriceButton.RectTransform.AnchorMax = new Vector2(0, 1);
                buyPriceButton.RectTransform.Pivot = new Vector2(0, 1);
                buyPriceButton.RectTransform.Size = new Vector2(28 * 4, 16 * 4);
                buyPriceButton.RectTransform.AnchoredPosition = new Vector2(8, -(8 + (16 + 1) * 4 * (i + 1)));
                buyPriceButton.OnClick.AddCallback(() => merchantShip.BuyResource(resourceType));

                UIText buyPriceText = UI.CreateUIText("0", "Buy price text", buyPriceButton.Entity);
                buyPriceText.RectTransform.AnchorMin = Vector2.Zero;
                buyPriceText.RectTransform.AnchorMax = Vector2.One;
                buyPriceText.RectTransform.OffsetMin = new Vector2(2, 2);
                buyPriceText.RectTransform.OffsetMax = new Vector2(2, 2);
                buyPriceText.Color = Color.Black;
                buyPriceText.TextSize = 64;
                buyPriceText.TextAlignment = AlignmentType.CenterRight;
                
                UIImage resourceIcon = UI.CreateUIImage(ResourcesManager.GetImage(UIManager.ResourceIndexToSpritePath(i)), "Icon", Entity);
                resourceIcon.RectTransform.AnchorMin = new Vector2(0, 1);
                resourceIcon.RectTransform.AnchorMax = new Vector2(0, 1);
                resourceIcon.RectTransform.Pivot = new Vector2(0, 1);
                resourceIcon.RectTransform.Size = new Vector2(16 * 4, 16 * 4);
                resourceIcon.RectTransform.AnchoredPosition = new Vector2(8 + 28 * 4 + 4, -(8 + (16 + 1) * 4 * (i + 1)));
                
                UIButton sellPriceButton = UI.CreateUIButton(priceSprite, "Sell", Entity);
                sellPriceButton.RectTransform.AnchorMin = new Vector2(0, 1);
                sellPriceButton.RectTransform.AnchorMax = new Vector2(0, 1);
                sellPriceButton.RectTransform.Pivot = new Vector2(0, 1);
                sellPriceButton.RectTransform.Size = new Vector2(28 * 4, 16 * 4);
                sellPriceButton.RectTransform.AnchoredPosition = new Vector2(8 + 28 * 4 + 4 + 16 * 4 + 4, -(8 + (16 + 1) * 4 * (i + 1)));
                sellPriceButton.OnClick.AddCallback(() => merchantShip.SellResource(resourceType));

                UIText sellPriceText = UI.CreateUIText("0", "Sell price text", sellPriceButton.Entity);
                sellPriceText.RectTransform.AnchorMin = Vector2.Zero;
                sellPriceText.RectTransform.AnchorMax = Vector2.One;
                sellPriceText.RectTransform.OffsetMin = new Vector2(2, 2);
                sellPriceText.RectTransform.OffsetMax = new Vector2(2, 2);
                sellPriceText.Color = Color.Black;
                sellPriceText.TextSize = 64;
                sellPriceText.TextAlignment = AlignmentType.CenterRight;

                _uiElements[i].BuyButton = buyPriceButton;
                _uiElements[i].BuyPrice = buyPriceText;
                _uiElements[i].SellButton = sellPriceButton;
                _uiElements[i].SellPrice = sellPriceText;
            }
        }
    }
}