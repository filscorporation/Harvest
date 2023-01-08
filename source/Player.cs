using System;
using System.Linq;
using Steel;

namespace SteelCustom
{
    public class Player : ScriptComponent
    {
        public bool FirstBuildingPlaced { get; set; }
        public bool IsTakingTurn { get; set; }

        private readonly int[] _resources = new int[5];
        private int _gold = 0;

        public int ResourcesAmount => _resources.Sum();
        public Storage Storage { get; } = new Storage();
        public bool IsStorageFull => ResourcesAmount >= Storage.Capacity;

        public event Action OnResourcesChanged;

        public override void OnCreate()
        {
            //_resources[(int)ResourceType.Wood] += 50;// TODO: remove
            //_resources[(int)ResourceType.Corn] += 50;// TODO: remove
        }

        public void GainResource(ResourceType type)
        {
            if (IsStorageFull)
            {
                // Too much resources
                return;
            }
            
            _resources[(int)type] += 1;
            OnResourcesChanged?.Invoke();
        }

        public void SpendResource(ResourceType type)
        {
            if (_resources[(int)type] < 1)
                Log.LogError($"Spending more {type} resource than player has: {_resources[(int)type]} < {1}");
            
            _resources[(int)type] -= 1;
            OnResourcesChanged?.Invoke();
        }

        public int GetResourceAmount(ResourceType type)
        {
            return _resources[(int)type];
        }

        public void GainGold(int amount)
        {
            _gold += amount;
            OnResourcesChanged?.Invoke();
        }

        public void SpendGold(int amount)
        {
            if (_gold < amount)
                Log.LogError($"Spending more gold than player has: {_gold} < {amount}");
            
            _gold -= amount;
            OnResourcesChanged?.Invoke();
        }

        public int GetGoldAmount()
        {
            return _gold;
        }

        public void StartTurn()
        {
            IsTakingTurn = true;
        }

        public void EndTurn()
        {
            IsTakingTurn = false;
        }

        public void GainResourcesFromBuilding(Building building)
        {
            ResourceGainAnimator animator = GameController.Instance.ResourceGainAnimator;
            Map map = GameController.Instance.Map;
            foreach (Hex hex in map.BuildingCoordsToResourceHexes(building.BuildingCoords))
            {
                ResourceType resourceType = Map.HexTypeToResource(hex.Type);
                GainResource(resourceType);
                animator.Animate(hex, resourceType);
            }
        }

        public void GainResourcesFromNumber(int number)
        {
            ResourceGainAnimator animator = GameController.Instance.ResourceGainAnimator;
            foreach (Hex hex in GameController.Instance.Map.GetHexesWithNumber(number))
            {
                foreach (Building building in hex.Buildings)
                {
                    int resourcesCount = building.Type == BuildingType.Ranch ? 1 : 2;
                    for (int i = 0; i < resourcesCount; i++)
                    {
                        ResourceType resourceType = Map.HexTypeToResource(hex.Type);
                        GainResource(resourceType);
                        animator.Animate(hex, resourceType);
                    }
                }
            }
        }

        public bool CanBuyBuilding(BuildingType buildingType)
        {
            (int Wood, int Corn, int Gold) price = Builder.GetBuildingPrice(buildingType);
            
            return price.Wood <= GetResourceAmount(ResourceType.Wood)
                && price.Corn <= GetResourceAmount(ResourceType.Corn)
                && price.Gold <= GetGoldAmount();
        }

        public void TryBuyBuilding(BuildingType buildingType)
        {
            if (buildingType == BuildingType.Storage)
            {
                if (Storage.CanUpgrade && Storage.GetUpgradePrice() <= GetResourceAmount(ResourceType.Wood))
                {
                    for (int i = 0; i < Storage.GetUpgradePrice(); i++)
                    {
                        SpendResource(ResourceType.Wood);
                    }
                    Storage.Upgrade();
                    
                    GameController.Instance.Map.Entity.AddComponent<AudioSource>().Play(ResourcesManager.GetAudioTrack("storage_upgrade.wav"));
                }
                
                return;
            }

            Builder builder = GameController.Instance.Builder;
            (int Wood, int Corn, int Gold) price = Builder.GetBuildingPrice(buildingType);
            if (price.Corn <= GetResourceAmount(ResourceType.Corn) && price.Gold <= GetGoldAmount())
            {
                for (int i = 0; i < price.Corn; i++)
                {
                    SpendResource(ResourceType.Corn);
                }
                SpendGold(price.Gold);
                builder.StartPlaceBuilding(buildingType);
            }
        }
    }
}