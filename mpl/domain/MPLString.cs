using System;

namespace mpl.domain
{
    public class MplString : IValue
    {
        public readonly string Val;

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
            return (Val != null ? Val.GetHashCode() : 0);
        }

        public new Type GetType() => Type.String;

        public MplString(string val)
        {
            Val = val;
        }

        public static MplString operator +(MplString a, MplString b) => new MplString(a.Val + b.Val);
        public static MplBoolean operator ==(MplString a, MplString b) => new MplBoolean(a.Val == b.Val);
        public static MplBoolean operator !=(MplString a, MplString b) => new MplBoolean(a.Val != b.Val);
        public static MplBoolean operator <(MplString a, MplString b) => new MplBoolean(string.Compare(a.Val, b.Val, StringComparison.Ordinal) < 0);
        public static MplBoolean operator >(MplString a, MplString b) => new MplBoolean(string.Compare(a.Val, b.Val, StringComparison.Ordinal) > 0);
    }
}
