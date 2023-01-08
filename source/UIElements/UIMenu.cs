using Steel;

namespace SteelCustom.UIElements
{
    public class UIMenu : ScriptComponent
    {
        private bool menuOpened;

        private Entity menu;
        private UIButton playButton;
        private UIButton continueButton;
        private UIButton soundButton;
        private bool lostMenuOpened = false;
        private Entity _decorationsEntity;

        public override void OnCreate()
        {
            GetComponent<RectTransformation>().AnchorMin = Vector2.Zero;
            GetComponent<RectTransformation>().AnchorMax = Vector2.One;
            
            menu = UI.CreateUIImage(ResourcesManager.GetImage("ui_dim.png"), "Menu", Entity).Entity;
            RectTransformation menuRT = menu.GetComponent<RectTransformation>();
            menuRT.AnchorMin = Vector2.Zero;
            menuRT.AnchorMax = Vector2.One;
            
            InitMenuDecorations();

            playButton = CreateMenuButton("Play", 140);
            playButton.OnClick.AddCallback(Play);
            continueButton = CreateMenuButton("Continue", 140);
            continueButton.OnClick.AddCallback(CloseMenu);
            continueButton.Entity.IsActiveSelf = false;
            CreateMenuButton("Exit", 0).OnClick.AddCallback(Exit);
            soundButton = CreateSoundButton();
            CreateAbout();
            
            {
                UIText uiText = UI.CreateUIText($"How to play:\n" +
                                                $"Place starting ranch, harvest resources by rolling the dice, sell goods to the merchant and earn as much gold as you can", "Text", menu);
                uiText.Color = Color.Black;
                uiText.TextSize = 32;
                uiText.TextAlignment = AlignmentType.CenterLeft;
                uiText.TextOverflowMode = OverflowMode.WrapByWords;

                uiText.RectTransform.AnchorMin = new Vector2(0.0f, 1);
                uiText.RectTransform.AnchorMax = new Vector2(0.0f, 1);
                uiText.RectTransform.Size = new Vector2(500, 200);
                uiText.RectTransform.Pivot = new Vector2(0.0f, 1);
                uiText.RectTransform.AnchoredPosition = new Vector2(10, -10);
            }
        }

        public override void OnUpdate()
        {
            if (Input.IsKeyJustPressed(KeyCode.Escape))
            {
                if (menuOpened)
                    CloseMenu();
                else if (GameController.Instance != null && GameController.Instance.Builder != null)
                {
                    if (!GameController.Instance.Builder.CancelPlaceBuilding())
                    {
                        OpenMenu();
                    }
                }
            }
        }

        private void InitMenuDecorations()
        {
            _decorationsEntity = new Entity("Decorations");
            Map map = _decorationsEntity.AddComponent<Map>();
            map.Transformation.Position = new Vector3(-4, -3, 0);
            map.GenerateDecorative();
            _decorationsEntity.RemoveComponent<Map>();
            
            Entity shipEntity = ResourcesManager.GetAsepriteData("ship.aseprite", true).CreateEntityFromAsepriteData();
            shipEntity.Parent = _decorationsEntity;
            shipEntity.Transformation.Position = new Vector3(4.8f, -2.0f, 0.5f);
            shipEntity.GetComponent<Animator>().Play("Idle");
        }

        public void OpenOnWinScreen()
        {
            lostMenuOpened = true;
            
            menu?.Destroy();
            
            menu = UI.CreateUIImage(ResourcesManager.GetImage("ui_dim.png"), "Menu", Entity).Entity;
            RectTransformation menuRT = menu.GetComponent<RectTransformation>();
            menuRT.AnchorMin = Vector2.Zero;
            menuRT.AnchorMax = Vector2.One;

            {
                UIImage image = UI.CreateUIImage(ResourcesManager.GetImage("ui_game_completed.png"), "Completed", menu);
                image.RectTransform.AnchorMin = new Vector2(0, 1);
                image.RectTransform.AnchorMax = new Vector2(0, 1);
                image.RectTransform.Size = new Vector2(105 * 4, 18 * 4);
                image.RectTransform.Pivot = new Vector2(0, 1);
                image.RectTransform.AnchoredPosition = new Vector2(50, -50);
            }
            {
                UIImage image = UI.CreateUIImage(ResourcesManager.GetImage("ui_earned.png"), "Earned", menu);
                image.RectTransform.AnchorMin = new Vector2(1, 1);
                image.RectTransform.AnchorMax = new Vector2(1, 1);
                image.RectTransform.Size = new Vector2(140 * 4, 22 * 4);
                image.RectTransform.Pivot = new Vector2(1, 1);
                image.RectTransform.AnchoredPosition = new Vector2(-50, -50);
                
                UIImage image2 = UI.CreateUIImage(ResourcesManager.GetImage("ui_earned_gold.png"), "Earned2", menu);
                image2.RectTransform.AnchorMin = new Vector2(1, 1);
                image2.RectTransform.AnchorMax = new Vector2(1, 1);
                image2.RectTransform.Size = new Vector2(86 * 4, 22 * 4);
                image2.RectTransform.Pivot = new Vector2(1, 1);
                image2.RectTransform.AnchoredPosition = new Vector2(-100, -50 - 22 * 4 - 10);
                
                UIText uiText = UI.CreateUIText($"{GameController.Instance.Player.GetGoldAmount()}", "Text", image2.Entity);
                uiText.RectTransform.AnchorMin = Vector2.Zero;
                uiText.RectTransform.AnchorMax = Vector2.One;
                uiText.RectTransform.OffsetMin = new Vector2(8, 8);
                uiText.RectTransform.OffsetMax = new Vector2(8, 8);
                
                uiText.Color = Color.Black;
                uiText.TextSize = 64;
                uiText.TextAlignment = AlignmentType.BottomMiddle;
            }
            {
                UIText text = UI.CreateUIText("Good result is above 100\nAmazing result is above 200", "Result info", menu);
                text.Color = Color.Black;
                text.TextSize = 32;
                text.TextAlignment = AlignmentType.BottomLeft;
                text.RectTransform.AnchorMin = new Vector2(1, 1);
                text.RectTransform.AnchorMax = new Vector2(1, 1);
                text.RectTransform.Pivot = new Vector2(1, 1);
                text.RectTransform.Size = new Vector2(400, 100);
                text.RectTransform.AnchoredPosition = new Vector2(0, -300);
            }

            CreateMenuButton("Restart", 140).OnClick.AddCallback(Restart);
            CreateMenuButton("Exit", 0).OnClick.AddCallback(Exit);
            CreateAbout();
        }

        private void OpenMenu()
        {
            if (lostMenuOpened)
                return;
            
            Time.TimeScale = 0.0f;
            
            menuOpened = true;
            menu.IsActiveSelf = true;
        }

        private void CloseMenu()
        {
            if (lostMenuOpened)
                return;
            
            Time.TimeScale = 1.0f;
            
            menuOpened = false;
            menu.IsActiveSelf = false;
            _decorationsEntity.IsActiveSelf = false;
        }

        private void Play()
        {
            continueButton.Entity.IsActiveSelf = true;
            playButton.Entity.IsActiveSelf = true;

            CloseMenu();
            
            GameController.Instance.StartGame();
        }

        private void Restart()
        {
            GameController.Instance.RestartGame();
        }

        private void Exit()
        {
            GameController.Instance.ExitGame();
        }

        private void ChangeSound()
        {
            if (GameController.SoundOn)
            {
                GameController.SoundOn = false;
                Camera.Main.Entity.AddComponent<AudioListener>().Volume = 0.0f;
                soundButton.TargetImage.Sprite = ResourcesManager.GetImage("ui_sound_off.png");
            }
            else
            {
                GameController.SoundOn = true;
                Camera.Main.Entity.AddComponent<AudioListener>().Volume = GameController.DEFAULT_VOLUME;
                soundButton.TargetImage.Sprite = ResourcesManager.GetImage("ui_sound_on.png");
            }
        }

        private UIButton CreateMenuButton(string text, float y)
        {
            Sprite sprite = ResourcesManager.GetImage("ui_frame.png");
            UIButton button = UI.CreateUIButton(sprite, "Menu button", menu);
            button.RectTransform.AnchorMin = new Vector2(0.5f, 0.5f);
            button.RectTransform.AnchorMax = new Vector2(0.5f, 0.5f);
            button.RectTransform.Size = new Vector2(360, 120);
            button.RectTransform.AnchoredPosition = new Vector2(0, y);

            UIText uiText = UI.CreateUIText(text, "Label", button.Entity);
            uiText.Color = Color.Black;
            uiText.TextSize = 96;
            uiText.TextAlignment = AlignmentType.CenterMiddle;
            uiText.RectTransform.AnchorMin = Vector2.Zero;
            uiText.RectTransform.AnchorMax = Vector2.One;

            return button;
        }

        private UIButton CreateSoundButton()
        {
            Sprite sprite = ResourcesManager.GetImage("ui_sound_on.png");
            UIButton button = UI.CreateUIButton(sprite, "Sound button", menu);
            button.RectTransform.AnchorMin = new Vector2(0.5f, 0.0f);
            button.RectTransform.AnchorMax = new Vector2(0.5f, 0.0f);
            button.RectTransform.Size = new Vector2(83 * 4, 18 * 4);
            button.RectTransform.AnchoredPosition = new Vector2(250, 100);
            
            button.OnClick.AddCallback(ChangeSound);

            return button;
        }

        private void CreateAbout()
        {
            UIText text = UI.CreateUIText("Created in 48 hours for LD52 using Steel Engine", "About", menu);
            text.Color = Color.Black;
            text.TextSize = 32;
            text.RectTransform.AnchorMin = new Vector2(0.5f, 0.0f);
            text.RectTransform.AnchorMax = new Vector2(1.0f, 0.0f);
            text.RectTransform.Pivot = new Vector2(0.0f, 0.0f);
            text.RectTransform.Size = new Vector2(0, 40);
        }
    }
}