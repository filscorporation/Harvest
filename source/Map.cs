using System;
using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.UIElements;
using Math = Steel.Math;
using Random = Steel.Random;

namespace SteelCustom
{
    public class Map : ScriptComponent
    {
        public const int SIZE = 4;
        private Hex[,,] _map;
        private Entity _tooltip;

        public override void OnUpdate()
        {
            Vector2 point = Camera.Main.ScreenToWorldPoint(Input.MousePosition);
            Coords coords = PositionToCoords(point);
            Hex hex = GetHex(coords);
            if (hex != null && hex.Type != HexType.Water && hex.Type != HexType.Wasteland)
            {
                _tooltip = UITooltip.ShowTooltip(UIResource.GetResourceDescription(HexTypeToResource(hex.Type)), 200);
            }
            else
            {
                if (_tooltip != null)
                {
                    UITooltip.HideTooltip(_tooltip);
                    _tooltip = null;
                }
            }
        }

        public void Generate()
        {
            const int fullSize = SIZE * 2 + 1;
            _map = new Hex[fullSize, fullSize, fullSize];
            
            for (int q = -SIZE; q <= SIZE; q++)
            {
                for (int r = -SIZE; r <= SIZE; r++)
                {
                    for (int s = -SIZE; s <= SIZE; s++)
                    {
                        Coords coords = new Coords(q, r, s);
                        if (!coords.IsValid())
                            continue;
                        
                        Entity entity = new Entity($"Hex ({coords})", Entity);
                        Hex hex = entity.AddComponent<Hex>();
                        HexType hexType = GetHexType(coords);
                        hex.Init(coords, hexType, GetNumber(coords, hexType));
                        _map[SIZE + q, SIZE + r, SIZE + s] = hex;
                    }
                }
            }
        }

        public void GenerateDecorative()
        {
            const int fullSize = SIZE * 2 + 1;
            _map = new Hex[fullSize, fullSize, fullSize];
            
            for (int q = -SIZE; q <= SIZE; q++)
            {
                for (int r = -SIZE; r <= SIZE; r++)
                {
                    for (int s = -SIZE; s <= SIZE; s++)
                    {
                        Coords coords = new Coords(q, r, s);
                        if (!coords.IsValid())
                            continue;
                        
                        Entity entity = new Entity($"Hex ({coords})", Entity);
                        Hex hex = entity.AddComponent<Hex>();
                        HexType hexType = (HexType)Random.NextInt(1, 6);
                        hex.Init(coords, hexType, -1);
                        _map[SIZE + q, SIZE + r, SIZE + s] = hex;
                    }
                }
            }
        }

        private HexType GetHexType(Coords coords)
        {
            if (IsBorderHex(coords))
                return HexType.Water;

            return (HexType)Random.NextInt(1, 6);
        }

        private int GetNumber(Coords coords, HexType hexType)
        {
            if (hexType == HexType.Water || hexType == HexType.Wasteland)
                return -1;

            int number = Random.NextInt(1, 11);
            return number >= 7 ? number + 1 : number;
        }

        private IEnumerable<Hex> GetAllHex()
        {
            for (int q = -SIZE; q <= SIZE; q++)
            {
                for (int r = -SIZE; r <= SIZE; r++)
                {
                    for (int s = -SIZE; s <= SIZE; s++)
                    {
                        Coords coords = new Coords(q, r, s);
                        if (!coords.IsValid())
                            continue;
                        
                        yield return _map[SIZE + q, SIZE + r, SIZE + s];
                    }
                }
            }
        }

        public Hex GetHex(Coords coords)
        {
            if (coords.Q < -SIZE || coords.Q > SIZE
             || coords.R < -SIZE || coords.R > SIZE
             || coords.S < -SIZE || coords.S > SIZE
             || !coords.IsValid())
                return null;

            return _map[SIZE + coords.Q, SIZE + coords.R, SIZE + coords.S];
        }

        public static bool IsBorderHex(Coords coords)
        {
            return coords.Q == -SIZE || coords.Q == SIZE
                || coords.R == -SIZE || coords.R == SIZE
                || coords.S == -SIZE || coords.S == SIZE;
        }

        public static Vector2 CoordsToPosition(Coords coords)
        {
            return new Vector2(
                coords.Q * Math.Sqrt(3) * 0.5f + coords.R * 0 - coords.S * Math.Sqrt(3) * 0.5f,
                coords.Q * 0.5f - coords.R * 1 + coords.S * 0.5f
                ) * 0.5f;
        }

        public static Vector2 BuildingCoordsToPosition(BuildingCoords buildingCoords)
        {
            Vector2 result = Vector2.Zero;
            foreach (Coords coords in buildingCoords.AllCoords())
            {
                result += CoordsToPosition(coords);
            }

            return result / 3;
        }
        
        public static Coords PositionToCoords(Vector2 position)
        {
            float s = ((Math.Sqrt(3) / 3 * position.Y - position.X) / Math.Sqrt(3)) * 2.0f;
            float q = 2.0f / 3 * position.Y * 2.0f - s;
            float r = -q - s;

            return HexCoordsRound(q, r, s);
        }

        public BuildingCoords? PositionToBuildingCoords(Vector2 position)
        {
            Coords coords = PositionToCoords(position);
            List<Coords> neighbours = coords.Neighbours().ToList();
            neighbours.Add(neighbours.First()); // Loop

            float minDistance = float.MaxValue;
            BuildingCoords? closestBuildingCoords = null;
            for (int i = 0; i < neighbours.Count - 1; i++)
            {
                BuildingCoords buildingCoords = new BuildingCoords(coords, neighbours[i], neighbours[i + 1]);
                float distance = Vector2.Distance(position, BuildingCoordsToPosition(buildingCoords));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuildingCoords = buildingCoords;
                }
            }

            if (closestBuildingCoords.HasValue && !HasAllHexes(closestBuildingCoords.Value))
                return null;

            return closestBuildingCoords;
        }
        
        public static Coords HexCoordsRound(float fracQ, float fracR, float fracS)
        {
            int q = (int)Math.Round(fracQ);
            int r = (int)Math.Round(fracR);
            int s = (int)Math.Round(fracS);

            float qDiff = Math.Abs(q - fracQ);
            float rDiff = Math.Abs(r - fracR);
            float sDiff = Math.Abs(s - fracS);

            if (qDiff > rDiff && qDiff > sDiff)
                q = -r - s;
            else if (rDiff > sDiff)
                r = -q - s;
            else
                s = -q - r;

            return new Coords(q, r, s);
        }

        public Building GetBuildingAtBuildingCoords(BuildingCoords buildingCoords)
        {
            // Here we check if there is a building with all equal coordinates
            List<Coords> allCoords = buildingCoords.AllCoords().ToList();
            foreach (Coords coords in allCoords)
            {
                Hex hex = GetHex(coords);
                foreach (Building building in hex.Buildings)
                {
                    bool allEqual = true;
                    List<Coords> allOtherCoords = building.BuildingCoords.AllCoords().ToList();
                    for (int i = 0; i < allOtherCoords.Count; i++)
                    {
                        if (!allCoords[i].Equals(allOtherCoords[i]))
                            allEqual = false;
                    }

                    if (allEqual)
                        return building;
                }
            }

            return null;
        }

        public float ClosestBuildingDistance(BuildingCoords buildingCoords)
        {
            Vector2 position = BuildingCoordsToPosition(buildingCoords);
            float minDistance = float.MaxValue;
            foreach (Coords coords in buildingCoords.AllCoords())
            {
                Hex hex = GetHex(coords);
                if (hex == null)
                    continue;

                foreach (Building building in hex.Buildings)
                {
                    float distance = Vector2.Distance(position, building.Transformation.Position);
                    if (distance < minDistance)
                        minDistance = distance;
                }
            }

            return minDistance;
        }

        public bool HasAllHexes(BuildingCoords buildingCoords)
        {
            return buildingCoords.AllCoords().All(c => GetHex(c) != null);
        }

        public List<Hex> BuildingCoordsToResourceHexes(BuildingCoords buildingCoords)
        {
            List<Hex> result = new List<Hex>();
            foreach (Coords coords in buildingCoords.AllCoords())
            {
                Hex hex = GetHex(coords);
                if (hex.Type == HexType.Wasteland || hex.Type == HexType.Water)
                    continue;
                
                result.Add(hex);
            }

            return result;
        }

        public IEnumerable<Hex> GetHexesWithNumber(int number)
        {
            return GetAllHex().Where(h => h.Number == number);
        }

        public static ResourceType HexTypeToResource(HexType hexType)
        {
            if (hexType == HexType.Wasteland || hexType == HexType.Water)
                throw new ArgumentException(nameof(hexType));
            
            return (ResourceType)((int)hexType - 2);
        }
    }
}