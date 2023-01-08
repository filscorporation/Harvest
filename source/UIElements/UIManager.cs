using System.Collections;
using Steel;

namespace SteelCustom.UIElements
{
    public class UIManager : ScriptComponent
    {
        public UIMenu Menu { get; private set; }
        public Entity UIRoot { get; private set; }
        
        private UIButton _endTurnButton;
        private readonly UIText[] _resourceTexts = new UIText[5];
        private UIText _goldText;
        
        private UIButton[] _buildingButtons = new UIButton[3];

        private UIImage _dice1, _dice2;
        private bool _animateDice;

        public override void OnUpdate()
        {
            UpdateResourcesTexts();
            UpdateGoldTexts();

            if (_animateDice)
                StartCoroutine(AnimateDice());
        }

        public void CreateMenu()
        {
            UIRoot = UI.CreateUIElement();
            UIRoot.GetComponent<RectTransformation>().AnchorMin = Vector2.Zero;
            UIRoot.GetComponent<RectTransformation>().AnchorMax = Vector2.One;
            
            Menu = UI.CreateUIElement("Menu").AddComponent<UIMenu>();
        }

        public void CreateGameUI()
        {
            InitEndTurnUI();
            InitResourcesUI();
            InitGoldUI();
            InitDiceUI();
            InitTurnCounterUI();
            InitStorageUI();
        }

        public void CreateBuildUI()
        {
            InitBuildUI();
        }

        private void InitEndTurnUI()
        {
            _endTurnButton = UI.CreateUIButton(ResourcesManager.GetImage("ui_end_turn.png"), "End turn button", UIRoot);
            _endTurnButton.RectTransform.AnchorMin = new Vector2(1, 0);
            _endTurnButton.RectTransform.AnchorMax = new Vector2(1, 0);
            _endTurnButton.RectTransform.Pivot = new Vector2(1, 0);
            _endTurnButton.RectTransform.Size = new Vector2(216, 60);
            _endTurnButton.RectTransform.AnchoredPosition = new Vector2(-30, 30);
            
            _endTurnButton.OnClick.AddCallback(GameController.Instance.Player.EndTurn);

            DisableEndTurnButton();
        }
        
        private void InitResourcesUI()
        {
            for (int i = 0; i < 5; i++)
            {
                UIImage image = UI.CreateUIImage(ResourcesManager.GetImage(ResourceIndexToSpritePath(i)), "Resource icon", UIRoot);
                image.RectTransform.AnchorMin = new Vector2(0, 1);
                image.RectTransform.AnchorMax = new Vector2(0, 1);
                image.RectTransform.Pivot = new Vector2(0, 1);
                image.RectTransform.Size = new Vector2(64, 64);
                image.RectTransform.AnchoredPosition = new Vector2(15 + (64 + 12) * i, -15);
                image.Entity.AddComponent<UIResource>().ResourceType = (ResourceType)i;

                UIImage priceField = UI.CreateUIImage(ResourcesManager.GetImage("ui_price_field.png"), "Price field", UIRoot);
                priceField.RectTransform.AnchorMin = new Vector2(0, 1);
                priceField.RectTransform.AnchorMax = new Vector2(0, 1);
                priceField.RectTransform.Pivot = new Vector2(0, 1);
                priceField.RectTransform.Size = new Vector2(64, 36);
                priceField.RectTransform.AnchoredPosition = new Vector2(15 + (64 + 12) * i, -15 - 64 - 4);

                UIText text = UI.CreateUIText("0", "Resource text", priceField.Entity);
                text.RectTransform.AnchorMin = Vector2.Zero;
                text.RectTransform.AnchorMax = Vector2.One;
                text.RectTransform.OffsetMin = new Vector2(2, 2);
                text.RectTransform.OffsetMax = new Vector2(6, 2);

                text.TextSize = 32;
                text.Color = Color.Black;
                text.TextAlignment = AlignmentType.BottomRight;

                _resourceTexts[i] = text;
            }
        }

        public static string ResourceIndexToSpritePath(int i)
        {
            return $"ui_{((ResourceType)i).ToString().ToLower()}.png";
        }

        private void InitGoldUI()
        {
            UIImage image = UI.CreateUIImage(ResourcesManager.GetImage("ui_gold.png"), "Gold icon", UIRoot);
            image.RectTransform.AnchorMin = new Vector2(0, 1);
            image.RectTransform.AnchorMax = new Vector2(0, 1);
            image.RectTransform.Pivot = new Vector2(0, 1);
            image.RectTransform.Size = new Vector2(64, 64);
            image.RectTransform.AnchoredPosition = new Vector2(55 + (64 + 12) * 5, -15);
            image.Entity.AddComponent<UIResource>().IsGold = true;

            UIImage priceField = UI.CreateUIImage(ResourcesManager.GetImage("ui_price_field.png"), "Price field", UIRoot);
            priceField.RectTransform.AnchorMin = new Vector2(0, 1);
            priceField.RectTransform.AnchorMax = new Vector2(0, 1);
            priceField.RectTransform.Pivot = new Vector2(0, 1);
            priceField.RectTransform.Size = new Vector2(64, 36);
            priceField.RectTransform.AnchoredPosition = new Vector2(55 + (64 + 12) * 5, -15 - 64 - 4);

            UIText text = UI.CreateUIText("0", "Gold text", priceField.Entity);
            text.RectTransform.AnchorMin = Vector2.Zero;
            text.RectTransform.AnchorMax = Vector2.One;
            text.RectTransform.OffsetMin = new Vector2(2, 2);
            text.RectTransform.OffsetMax = new Vector2(6, 2);

            text.TextSize = 32;
            text.Color = Color.Black;
            text.TextAlignment = AlignmentType.BottomRight;

            _goldText = text;
        }

        private void UpdateResourcesTexts()
        {
            Player player = GameController.Instance.Player;
            if (player == null)
                return;

            for (int i = 0; i < 5; i++)
            {
                _resourceTexts[i].Text = $"{player.GetResourceAmount((ResourceType)i)}";
            }
        }

        private void UpdateGoldTexts()
        {
            Player player = GameController.Instance.Player;
            if (player == null)
                return;

            _goldText.Text = $"{player.GetGoldAmount()}";
        }

        private void InitDiceUI()
        {
            _dice1 = UI.CreateUIImage(ResourcesManager.GetImage(DiceNumberToSpritePath(1)), "Dice icon", UIRoot);
            _dice1.RectTransform.AnchorMin = new Vector2(1, 0);
            _dice1.RectTransform.AnchorMax = new Vector2(1, 0);
            _dice1.RectTransform.Pivot = new Vector2(1, 0);
            _dice1.RectTransform.Size = new Vector2(64, 64);
            _dice1.RectTransform.AnchoredPosition = new Vector2(-15 - 30 - 216 - 4, 30);
            _dice1.Entity.AddComponent<UIDice>();
            
            _dice2 = UI.CreateUIImage(ResourcesManager.GetImage(DiceNumberToSpritePath(1)), "Dice icon", UIRoot);
            _dice2.RectTransform.AnchorMin = new Vector2(1, 0);
            _dice2.RectTransform.AnchorMax = new Vector2(1, 0);
            _dice2.RectTransform.Pivot = new Vector2(1, 0);
            _dice2.RectTransform.Size = new Vector2(64, 64);
            _dice2.RectTransform.AnchoredPosition = new Vector2(-80 - 30 - 216 - 4, 30);
            _dice2.Entity.AddComponent<UIDice>();

            _dice1.Entity.IsActiveSelf = false;
            _dice2.Entity.IsActiveSelf = false;
        }

        public void UpdateDice()
        {
            _dice1.Entity.IsActiveSelf = true;
            _dice2.Entity.IsActiveSelf = true;

            _animateDice = true;
        }

        private IEnumerator AnimateDice()
        {
            _animateDice = false;
            
            float timer = 0.0f;
            float timerInner = 0.0f;

            Dice dice = GameController.Instance.Dice;
            while (timer < 1.0f)
            {
                if (timerInner > 0.03f)
                {
                    timerInner -= 0.05f;
                    _dice1.Sprite = ResourcesManager.GetImage(DiceNumberToSpritePath(Random.NextInt(1, 6)));
                    _dice2.Sprite = ResourcesManager.GetImage(DiceNumberToSpritePath(Random.NextInt(1, 6)));
                }

                timer += Time.DeltaTime;
                timerInner += Time.DeltaTime;

                yield return null;
            }
            
            _dice1.Sprite = ResourcesManager.GetImage(DiceNumberToSpritePath(dice.Value1));
            _dice2.Sprite = ResourcesManager.GetImage(DiceNumberToSpritePath(dice.Value2));
        }

        private string DiceNumberToSpritePath(int number)
        {
            return $"ui_dice_{number}.png";
        }

        private void InitTurnCounterUI()
        {
            UIImage turnCounterImage = UI.CreateUIImage(ResourcesManager.GetImage("ui_turn_counter.png"), "Turn counter", UIRoot);
            turnCounterImage.RectTransform.AnchorMin = new Vector2(0, 1);
            turnCounterImage.RectTransform.AnchorMax = new Vector2(0, 1);
            turnCounterImage.RectTransform.Pivot = new Vector2(0, 1);
            turnCounterImage.RectTransform.Size = new Vector2(48 * 4, 38 * 4);
            turnCounterImage.RectTransform.AnchoredPosition = new Vector2(15, -280);
            turnCounterImage.Entity.AddComponent<UITurnCounter>();
        }

        private void InitStorageUI()
        {
            UIImage turnCounterImage = UI.CreateUIImage(ResourcesManager.GetImage("ui_storage.png"), "Storage", UIRoot);
            turnCounterImage.RectTransform.AnchorMin = new Vector2(0, 1);
            turnCounterImage.RectTransform.AnchorMax = new Vector2(0, 1);
            turnCounterImage.RectTransform.Pivot = new Vector2(0, 1);
            turnCounterImage.RectTransform.Size = new Vector2(54 * 4, 16 * 4);
            turnCounterImage.RectTransform.AnchoredPosition = new Vector2(15, -130);
            turnCounterImage.Entity.AddComponent<UIStorage>();
        }

        private void InitBuildUI()
        {
            for (int i = 0; i < 3; i++)
            {
                BuildingType buildingType = (BuildingType)i;
                UIButton button = UI.CreateUIButton(ResourcesManager.GetImage($"ui_buy_{buildingType.ToString().ToLower()}.png"), "Build button", UIRoot);
                button.RectTransform.AnchorMin = Vector2.Zero;
                button.RectTransform.AnchorMax = Vector2.Zero;
                button.RectTransform.Pivot = Vector2.Zero;
                button.RectTransform.Size = new Vector2(102 * 4, 20 * 4);
                button.RectTransform.AnchoredPosition = new Vector2(15, 15 + (20 * 4 + 4) * (2 - i));
                button.Entity.AddComponent<UIBuildButton>().BuildingType = buildingType;
            
                button.OnClick.AddCallback(() => GameController.Instance.Player.TryBuyBuilding(buildingType));

                _buildingButtons[i] = button;
            }
        }

        public void EnableEndTurnButton()
        {
            _endTurnButton.Entity.IsActiveSelf = true;
        }

        public void DisableEndTurnButton()
        {
            _endTurnButton.Entity.IsActiveSelf = false;
        }
    }
}