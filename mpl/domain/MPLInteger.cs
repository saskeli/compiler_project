using mpl.Exceptions;

namespace mpl.domain
{
    public struct MplInteger : IValue
    {
        public readonly int Val;
        public readonly int Line;
        public readonly int Position;

        public MplInteger(int val, int line, int position)
        {
            Val = val;
            Line = line;
            Position = position;
        }
        public override int GetHashCode()
        {
            return Val;
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

        public static MplInteger operator +(MplInteger a, MplInteger b) => new MplInteger(a.Val + b.Val, a.Line, a.Position);
        public static MplInteger operator -(MplInteger a, MplInteger b) => new MplInteger(a.Val - b.Val, a.Line, a.Position);
        public static MplInteger operator *(MplInteger a, MplInteger b) => new MplInteger(a.Val * b.Val, a.Line, a.Position);

        public static MplInteger operator /(MplInteger a, MplInteger b)
        {
            if (b.Val == 0) 
                throw new MplDivideByZeroException("Attempted to divide by zero", b.Line, b.Position);
            return new MplInteger(a.Val / b.Val, a.Line, a.Position);
        }
        public static MplBoolean operator ==(MplInteger a, MplInteger b) => new MplBoolean(a.Val == b.Val);
        public static MplBoolean operator !=(MplInteger a, MplInteger b) => new MplBoolean(a.Val != b.Val);
        public static MplBoolean operator <(MplInteger a, MplInteger b) => new MplBoolean(a.Val < b.Val);
        public static MplBoolean operator >(MplInteger a, MplInteger b) => new MplBoolean(a.Val > b.Val);
        
    }
}
