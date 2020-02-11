using System;

namespace mpl.domain
{
    public class MplString : IValue
    {
        public readonly string Val;
        public readonly int Line;
        public readonly int Position;

        public MplString(string val, int line, int position)
        {
            Val = val;
            Line = line;
            Position = position;
        }

        protected bool Equals(MplString other)
        {
            return Val == other.Val;
        }

        public override bool Equals(object obj)
        {
            return obj is MplString other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Val != null ? Val.GetHashCode() : 0;
        }

        public new Type GetType() => Type.String;

        public static MplString operator +(MplString a, MplString b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplString(a.Val + b.Val, l, p);
        }

        public static MplBoolean operator ==(MplString a, MplString b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(a.Val == b.Val, l, p);
        }

        public static MplBoolean operator !=(MplString a, MplString b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(a.Val != b.Val, l, p);
        }

        public static MplBoolean operator <(MplString a, MplString b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(string.Compare(a.Val, b.Val, StringComparison.Ordinal) < 0, l, p);
        }

        public static MplBoolean operator >(MplString a, MplString b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(string.Compare(a.Val, b.Val, StringComparison.Ordinal) > 0, l, p);
        }

        public int GetLine() => Line;

        public int GetPosition() => Position;
    }
}
