using System;
using System.Collections.Generic;
using System.Linq;
using Steel;

namespace SteelCustom
{
    public class Hex : ScriptComponent
    {
        public Coords Coords { get; private set; }
        public HexType Type { get; private set; }
        public int Number { get; private set; }
        public List<Building> Buildings { get; } = new List<Building>();

        private Entity _numberEntity;

        public void Init(Coords coords, HexType type, int number)
        {
            Coords = coords;
            Type = type;
            Number = number;

            Transformation.LocalPosition = Map.CoordsToPosition(coords);

            Sprite sprite = ResourcesManager.GetAsepriteData(HexTypeToSpritePath(Type)).Sprites.First();
            Entity.AddComponent<SpriteRenderer>().Sprite = sprite;

            if (number > 0)
            {
                _numberEntity = new Entity("Number", Entity);
                _numberEntity.Transformation.LocalPosition = new Vector3(0, 0, 1.1f);
                _numberEntity.AddComponent<SpriteRenderer>().Sprite = ResourcesManager.GetImage(NumberToSpritePath(Number));
            }
        }

        private static string HexTypeToSpritePath(HexType type)
        {
            switch (type)
            {
                case HexType.Water:
                    return "hex_water.aseprite";
                case HexType.Wasteland:
                    return "hex_wasteland.aseprite";
                case HexType.Wood:
                    return "hex_wood.aseprite";
                case HexType.Tobacco:
                    return "hex_tobacco.aseprite";
                case HexType.Corn:
                    return "hex_corn.aseprite";
                case HexType.Cotton:
                    return "hex_cotton.aseprite";
                case HexType.Spices:
                    return "hex_spices.aseprite";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static string NumberToSpritePath(int number)
        {
            return $"number_{number}.png";
        }

        public override string ToString()
        {
            return $"({Type} hex n. {Number} {Coords})";
        }

        public void BindToBuilding(Building building)
        {
            Buildings.Add(building);
        }
    }
}