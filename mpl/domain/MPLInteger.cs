using mpl.Exceptions;

namespace mpl.domain
{
    public class MplInteger : IValue
    {
        public readonly long Val;
        public readonly int Line;
        public readonly int Position;

        public MplInteger(long val, int line, int position)
        {
            Val = val;
            Line = line;
            Position = position;
        }
        public override int GetHashCode()
        {
            return (int)Val;
        }

        public new Type GetType() => Type.Int;

        public bool Equals(MplInteger other)
        {
            return Val == other.Val;
        }

        public override bool Equals(object obj)
        {
            return obj is MplInteger other && Equals(other);
        }

        public static MplInteger operator +(MplInteger a, MplInteger b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplInteger(a.Val + b.Val, l, p);
        }

        public static MplInteger operator -(MplInteger a, MplInteger b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplInteger(a.Val - b.Val, l, p);
        }

        public static MplInteger operator *(MplInteger a, MplInteger b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplInteger(a.Val * b.Val, l, p);
        }

        public static MplInteger operator /(MplInteger a, MplInteger b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            if (b.Val == 0) 
                throw new MplDivideByZeroException("Attempted to divide by zero", b.Line, b.Position);
            return new MplInteger(a.Val / b.Val, l, p);
        }

        public static MplBoolean operator ==(MplInteger a, MplInteger b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(a.Val == b.Val, l, p);
        }

        public static MplBoolean operator !=(MplInteger a, MplInteger b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(a.Val != b.Val, l, p);
        }

        public static MplBoolean operator <(MplInteger a, MplInteger b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(a.Val < b.Val, l, p);
        }

        public static MplBoolean operator >(MplInteger a, MplInteger b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(a.Val > b.Val, l, p);
        }

        public int GetLine() => Line;

        public int GetPosition() => Position;
    }
}
