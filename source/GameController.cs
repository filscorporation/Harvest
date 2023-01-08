using System;
using System.Collections;
using Steel;
using SteelCustom.UIElements;

namespace SteelCustom
{
    public class GameController : ScriptComponent
    {
        public static GameController Instance;
        
        public const float DEFAULT_VOLUME = 0.15f;
        public static bool SoundOn { get; set; } = true;
        
        public Map Map;
        public ResourceGainAnimator ResourceGainAnimator;
        public Player Player;
        public Dice Dice;
        public Builder Builder;
        public MerchantShip MerchantShip;
        
        public UIManager UIManager;
        
        public GameState GameState { get; private set; }
        private bool _changeState = false;
        private bool _startGame = false;
        private bool _endGame = false;

        public int CurrentTurn { get; private set; }
        public const int MAX_TURN = 60;
        
        public override void OnCreate()
        {
            Instance = this;
            
            Screen.Color = new Color(123, 153, 200);
            Camera.Main.Width = Map.SIZE * 2 + 5;

            StartCoroutine(IntroCoroutine());
        }

        public override void OnUpdate()
        {
            if (_changeState)
            {
                _changeState = false;
                
                switch (GameState)
                {
                    case GameState.Intro:
                        StartCoroutine(PlaceFirstBuildingCoroutine());
                        break;
                    case GameState.PlaceFirstBuilding:
                        StartCoroutine(ThrowDiceCoroutine());
                        break;
                    case GameState.ThrowDice:
                        StartCoroutine(PlayerTurnCoroutine());
                        break;
                    case GameState.PlayerTurn:
                        if (_endGame)
                            StartCoroutine(EndGameCoroutine());
                        else
                            StartCoroutine(ThrowDiceCoroutine());
                        break;
                    case GameState.EndGame:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void StartGame()
        {
            _startGame = true;
        }

        public void RestartGame()
        {
            SceneManager.SetActiveScene(SceneManager.GetActiveScene());
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private IEnumerator IntroCoroutine()
        {
            GameState = GameState.Intro;
            Log.LogInfo("Start Intro state");
            
            Camera.Main.Entity.GetComponent<AudioListener>().Volume = DEFAULT_VOLUME;
            Entity backgroundMusic = new Entity();
            AudioSource source = backgroundMusic.AddComponent<AudioSource>();
            source.Loop = true;
            source.Play(ResourcesManager.GetAudioTrack("background_music.wav"));
            source.Volume = 0.1f;
            
            UIManager = new Entity("UI manager").AddComponent<UIManager>();
            UIManager.CreateMenu();
            
            yield return new WaitWhile(() => !_startGame);

            Player = new Entity("Player").AddComponent<Player>();
            Dice = new Dice();
            Builder = new Entity("Builder").AddComponent<Builder>();
            MerchantShip = new Entity("Merchant ship").AddComponent<MerchantShip>();
            
            Map = new Entity("Map").AddComponent<Map>();
            Map.Generate();
            ResourceGainAnimator = new Entity("Resource gain animator").AddComponent<ResourceGainAnimator>();
            ResourceGainAnimator.Init(Map);

            UIManager.CreateGameUI();
            
            yield return new WaitForSeconds(0.1f);

            Log.LogInfo("End Intro state");
            _changeState = true;
        }

        private IEnumerator PlaceFirstBuildingCoroutine()
        {
            GameState = GameState.PlaceFirstBuilding;
            Log.LogInfo("Start PlaceFirstBuilding state");
            
            Builder.StartPlaceBuilding(BuildingType.Ranch);
            
            yield return new WaitWhile(() => !Player.FirstBuildingPlaced);
            
            yield return new WaitForSeconds(1.0f);
            
            UIManager.CreateBuildUI();

            Log.LogInfo("End PlaceFirstBuilding state");
            _changeState = true;
        }

        private IEnumerator ThrowDiceCoroutine()
        {
            GameState = GameState.ThrowDice;
            Log.LogInfo("Start ThrowDice state");

            Dice.Roll();
            UIManager.UpdateDice();
            
            Log.LogInfo($"Dice is {Dice.Value} ({Dice.Value1}, {Dice.Value2})");
            
            yield return new WaitForSeconds(1.0f);
            
            if (Dice.Value == 7)
                MerchantShip.StartTrade();
            else
                Player.GainResourcesFromNumber(Dice.Value);
            
            yield return new WaitForSeconds(1.0f);

            Log.LogInfo("End ThrowDice state");
            _changeState = true;
        }

        private IEnumerator PlayerTurnCoroutine()
        {
            GameState = GameState.PlayerTurn;
            Log.LogInfo("Start PlayerTurn state");

            CurrentTurn++;
            if (CurrentTurn >= MAX_TURN)
                _endGame = true;
            
            Player.StartTurn();
            UIManager.EnableEndTurnButton();
            
            yield return new WaitForSeconds(0.1f);
            
            yield return new WaitWhile(() => Player.IsTakingTurn);

            UIManager.DisableEndTurnButton();
            
            if (Dice.Value == 7)
            {
                MerchantShip.FinishTrade();
                yield return new WaitForSeconds(0.5f);
            }

            Log.LogInfo("End PlayerTurn state");
            _changeState = true;
        }

        private IEnumerator EndGameCoroutine()
        {
            GameState = GameState.EndGame;
            Log.LogInfo("Start EndGame state");
            
            yield return new WaitForSeconds(0.1f);
            
            Entity.AddComponent<AudioSource>().Play(ResourcesManager.GetAudioTrack("end_game.wav"));
            
            UIManager.Menu.OpenOnWinScreen();
        }
    }
}