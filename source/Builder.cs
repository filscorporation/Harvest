using System;
using System.Linq;
using Steel;

namespace SteelCustom
{
    public class Builder : ScriptComponent
    {
        private BuildingType? _currentPlacedBuilding;
        private BuildingCoords _placedCoords;
        private Entity _placedEntity;

        public override void OnUpdate()
        {
            UpdatePlaceBuilding();
        }

        private void UpdatePlaceBuilding()
        {
            if (!_currentPlacedBuilding.HasValue)
                return;

            Map map = GameController.Instance.Map;
            
            Vector2 point = Camera.Main.ScreenToWorldPoint(Input.MousePosition);
            BuildingCoords? buildingCoords = map.PositionToBuildingCoords(point);

            if (buildingCoords.HasValue && map.HasAllHexes(buildingCoords.Value))
            {
                Building building = map.GetBuildingAtBuildingCoords(buildingCoords.Value);

                switch (_currentPlacedBuilding.Value)
                {
                    case BuildingType.Ranch:
                        if (building == null)
                        {
                            if (map.ClosestBuildingDistance(buildingCoords.Value) > 0.5f)
                            {
                                _placedCoords = buildingCoords.Value;
                                _placedEntity.Transformation.Position = (Vector3)Map.BuildingCoordsToPosition(_placedCoords) + new Vector3(0, 0, 0.4f);
                            }
                        }
                        break;
                    case BuildingType.Town:
                        if (building != null)
                        {
                            // TODO: some bug here...
                            _placedCoords = buildingCoords.Value;
                            _placedEntity.Transformation.Position = (Vector3)Map.BuildingCoordsToPosition(_placedCoords) + new Vector3(0, 0, 0.7f);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (!UI.IsPointerOverUI() && Input.IsMouseJustPressed(MouseCodes.ButtonLeft))
            {
                FinishPlaceBuilding();
            }
        }

        public void StartPlaceBuilding(BuildingType buildingType)
        {
            if (_currentPlacedBuilding.HasValue)
            {
                _placedEntity.Destroy();
                _placedEntity = null;
                _currentPlacedBuilding = null;
            }

            _currentPlacedBuilding = buildingType;

            _placedEntity = new Entity($"Building {_currentPlacedBuilding}", Entity);
            _placedEntity.AddComponent<SpriteRenderer>().Sprite = ResourcesManager
                .GetAsepriteData(BuildingTypeToSpritePath(_currentPlacedBuilding.Value)).Sprites.First();
        }

        private void FinishPlaceBuilding()
        {
            if (!_currentPlacedBuilding.HasValue)
            {
                Log.LogWarning("No building to finish");
                return;
            }
            
            Map map = GameController.Instance.Map;
            
            if (map.HasAllHexes(_placedCoords))
            {
                Building existingBuilding = GameController.Instance.Map.GetBuildingAtBuildingCoords(_placedCoords);
                
                switch (_currentPlacedBuilding.Value)
                {
                    case BuildingType.Ranch:
                        if (existingBuilding == null && map.ClosestBuildingDistance(_placedCoords) > 0.5f)
                        {
                            Building building = _placedEntity.AddComponent<Building>();
                            building.Init(_placedCoords);
                            _placedEntity = null;

                            building.Entity.AddComponent<AudioSource>().Play(ResourcesManager.GetAudioTrack("place_building.wav"));

                            Player player = GameController.Instance.Player;
                            if (!player.FirstBuildingPlaced)
                            {
                                player.FirstBuildingPlaced = true;
                                player.GainResourcesFromBuilding(building);
                            }
                        }
                        break;
                    case BuildingType.Town:
                        if (existingBuilding != null)
                        {
                            existingBuilding.UpgradeToTown();
                            _placedEntity.Destroy();
                            _placedEntity = null;
                            
                            existingBuilding.Entity.AddComponent<AudioSource>().Play(ResourcesManager.GetAudioTrack("place_building.wav"));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (_placedEntity != null)
                {
                    _placedEntity.Destroy();
                    _placedEntity = null;
                }

                _currentPlacedBuilding = null;
            }
        }

        public bool CancelPlaceBuilding()
        {
            if (!_currentPlacedBuilding.HasValue || !GameController.Instance.Player.FirstBuildingPlaced)
            {
                return false;
            }

            _placedEntity.Destroy();
            _placedEntity = null;
            _currentPlacedBuilding = null;
            
            // TODO: return resources?

            return true;
        }

        private static string BuildingTypeToSpritePath(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.Ranch:
                    return "ranch.aseprite";
                case BuildingType.Town:
                    return "town.aseprite";
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
            }
        }

        public static (int Wood, int Corn, int Gold) GetBuildingPrice(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.Ranch:
                    return (0, 3, 5);
                case BuildingType.Town:
                    return (0, 7, 10);
                case BuildingType.Storage:
                    return (GameController.Instance.Player.Storage.GetUpgradePrice(), 0, 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
            }
        }
    }
}