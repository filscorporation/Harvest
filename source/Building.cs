using System.Linq;
using Steel;

namespace SteelCustom
{
    public class Building : ScriptComponent
    {
        public BuildingType Type { get; private set; }
        public BuildingCoords BuildingCoords { get; private set; }

        public void Init(BuildingCoords buildingCoords)
        {
            Type = BuildingType.Ranch;
            BuildingCoords = buildingCoords;

            foreach (Coords coords in buildingCoords.AllCoords())
            {
                GameController.Instance.Map.GetHex(coords).BindToBuilding(this);
            }
        }

        public void UpgradeToTown()
        {
            Type = BuildingType.Town;

            GetComponent<SpriteRenderer>().Sprite = ResourcesManager.GetAsepriteData("town.aseprite").Sprites.First();
        }
    }
}