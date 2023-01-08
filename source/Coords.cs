using System.Collections.Generic;

namespace SteelCustom
{
    public struct Coords
    {
        public int Q { get; }
        public int R { get; }
        public int S { get; }

        public Coords(int q, int r, int s)
        {
            Q = q;
            R = r;
            S = s;
        }

        public bool IsValid()
        {
            return Q + R + S == 0;
        }

        public IEnumerable<Coords> Neighbours()
        {
            // Clockwise from top right
            yield return new Coords(Q + 1, R - 1, S);
            yield return new Coords(Q + 1, R, S - 1);
            yield return new Coords(Q, R + 1, S - 1);
            yield return new Coords(Q - 1, R + 1, S);
            yield return new Coords(Q - 1, R, S + 1);
            yield return new Coords(Q, R - 1, S + 1);
        }

        public override string ToString()
        {
            return $"({Q}, {R}, {S})";
        }

        public bool Equals(Coords other)
        {
            return Q == other.Q && R == other.R && S == other.S;
        }

        public override bool Equals(object obj)
        {
            return obj is Coords other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Q;
                hashCode = (hashCode * 397) ^ R;
                hashCode = (hashCode * 397) ^ S;
                return hashCode;
            }
        }
    }
}