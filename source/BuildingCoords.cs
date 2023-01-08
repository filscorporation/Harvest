using System.Collections.Generic;

namespace SteelCustom
{
    public struct BuildingCoords
    {
        public Coords Bottom { get; }
        public Coords TopLeft { get; }
        public Coords TopRight { get; }
        
        public BuildingCoords(Coords coords1, Coords coords2, Coords coords3)
        {
            Coords bottom = coords1, topLeft = coords2, topRight = coords3;
            
            if (coords1.R > coords2.R && coords1.R > coords3.R) bottom = coords1;
            if (coords2.R > coords1.R && coords2.R > coords3.R) bottom = coords2;
            if (coords3.R > coords1.R && coords3.R > coords2.R) bottom = coords3;
            
            if (coords1.S > coords2.S && coords1.S > coords3.S) topLeft = coords1;
            if (coords2.S > coords1.S && coords2.S > coords3.S) topLeft = coords2;
            if (coords3.S > coords2.S && coords3.S > coords1.S) topLeft = coords3;
            
            if (coords1.Q > coords2.Q && coords1.Q > coords3.Q) topRight = coords1;
            if (coords2.Q > coords1.Q && coords2.Q > coords3.Q) topRight = coords2;
            if (coords3.Q > coords2.Q && coords3.Q > coords1.Q) topRight = coords3;

            Bottom = bottom;
            TopLeft = topLeft;
            TopRight = topRight;
        }

        public IEnumerable<Coords> AllCoords()
        {
            yield return Bottom;
            yield return TopLeft;
            yield return TopRight;
        }
    }
}