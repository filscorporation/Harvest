using System.Collections;
using Steel;
using SteelCustom.UIElements;

namespace SteelCustom
{
    public class MerchantShip : ScriptComponent
    {
        private Entity _shipEntity;
        private UIShop _uiShop;
        private RectTransformation _uiShopRect;
        private ShopState _shopState;

        private bool _isShopActive = false;
        private bool _startTrade = false;
        private bool _finishTrade = false;

        public override void OnCreate()
        {
            _shipEntity = ResourcesManager.GetAsepriteData("ship.aseprite", true).CreateEntityFromAsepriteData();
            _shipEntity.IsActiveSelf = false;

            Entity uiEntity = UI.CreateUIElement("UI shop", GameController.Instance.UIManager.UIRoot);
            _uiShop = uiEntity.AddComponent<UIShop>();
            uiEntity.AddComponent<UIImage>().Sprite = ResourcesManager.GetImage("ui_frame.png");
            _uiShopRect = uiEntity.GetComponent<RectTransformation>();
            _uiShopRect.AnchorMin = new Vector2(1.0f, 0.5f);
            _uiShopRect.AnchorMax = new Vector2(1.0f, 0.5f);
            _uiShopRect.Size = new Vector2(78 * 4, 105 * 4);
            _uiShopRect.Pivot = new Vector2(1.0f, 0.5f);
            _uiShopRect.AnchoredPosition = new Vector2(200, 110);
            uiEntity.IsActiveSelf = false;

            GameController.Instance.Player.OnResourcesChanged += OnPlayerResourcesChanged;
        }

        public override void OnDestroy()
        {
            GameController.Instance.Player.OnResourcesChanged -= OnPlayerResourcesChanged;
        }

        public override void OnUpdate()
        {
            if (_startTrade)
            {
                _startTrade = false;
                StartTradeInner();
            }
            if (_finishTrade)
            {
                _finishTrade = false;
                FinishTradeInner();
            }
        }

        private void OnPlayerResourcesChanged()
        {
            if (!_isShopActive)
                return;
            
            _uiShop.UpdateState(this, _shopState);
        }

        public void StartTrade()
        {
            _startTrade = true;
        }

        private void StartTradeInner()
        {
            _shopState = ShopState.GenerateRandom();
            _isShopActive = true;
            
            StopAllCoroutines();
            StartCoroutine(AnimateStartTrade());
        }

        private IEnumerator AnimateStartTrade()
        {
            _shipEntity.IsActiveSelf = true;
            _shipEntity.GetComponent<Animator>().Play("Idle");
            
            Vector3 shipStartPosition = new Vector3(8f, -2.0f, 0.5f);
            Vector3 shipEndPosition = new Vector3(4.8f, -2.0f, 0.5f);

            float progress = 0.0f;
            while (progress < 1.0f)
            {
                _shipEntity.Transformation.Position = Math.Lerp(shipStartPosition, shipEndPosition, progress);

                yield return null;
                progress += Time.DeltaTime / 1.5f;
            }
            
            _uiShop.Entity.IsActiveSelf = true;
            
            _uiShop.UpdateState(this, _shopState);

            Vector3 shopStartPosition = new Vector3(300, 100, 0);
            Vector3 shopEndPosition = new Vector3(-30, 100, 0);
            progress = 0.0f;
            while (progress < 1.0f)
            {
                _uiShopRect.AnchoredPosition = Math.Lerp(shopStartPosition, shopEndPosition, progress);

                yield return null;
                progress += Time.DeltaTime / 0.5f;
            }
        }

        public void FinishTrade()
        {
            _finishTrade = true;
        }

        private void FinishTradeInner()
        {
            _isShopActive = false;
            StopAllCoroutines();
            StartCoroutine(AnimateFinishTrade());
        }

        private IEnumerator AnimateFinishTrade()
        {
            Vector3 shipStartPosition = new Vector3(4.8f, -2.0f, 0.5f);
            Vector3 shipEndPosition = new Vector3(8f, -2.0f, 0.5f);
            Vector3 shopStartPosition = new Vector3(-30, 100, 0);
            Vector3 shopEndPosition = new Vector3(300, 100, 0);

            float progress = 0.0f;
            while (progress < 1.0f)
            {
                _shipEntity.Transformation.Position = Math.Lerp(shipStartPosition, shipEndPosition, progress);
                _uiShopRect.AnchoredPosition = Math.Lerp(shopStartPosition, shopEndPosition, progress);

                yield return null;
                progress += Time.DeltaTime / 0.5f;
            }
            
            _uiShop.Entity.IsActiveSelf = false;
            _shipEntity.IsActiveSelf = false;
        }

        public bool CanBuyResource(ResourceType resourceType)
        {
            Player player = GameController.Instance.Player;
            return !player.IsStorageFull && player.GetGoldAmount() >= _shopState.BuyPrice[(int)resourceType];
        }

        public bool CanSellResource(ResourceType resourceType)
        {
            return GameController.Instance.Player.GetResourceAmount(resourceType) > 0;
        }

        public void BuyResource(ResourceType resourceType)
        {
            if (!CanBuyResource(resourceType))
                return;
            
            GameController.Instance.Player.SpendGold(_shopState.BuyPrice[(int)resourceType]);
            GameController.Instance.Player.GainResource(resourceType);

            Entity.AddComponent<AudioSource>().Play(ResourcesManager.GetAudioTrack("buy_resource.wav"));
        }

        public void SellResource(ResourceType resourceType)
        {
            if (!CanSellResource(resourceType))
                return;

            GameController.Instance.Player.SpendResource(resourceType);
            GameController.Instance.Player.GainGold(_shopState.SellPrice[(int)resourceType]);

            Entity.AddComponent<AudioSource>().Play(ResourcesManager.GetAudioTrack("buy_resource.wav"));
        }
    }
}