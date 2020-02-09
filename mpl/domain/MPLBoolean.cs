using System;
using System.Collections.Generic;
using System.Text;

namespace mpl.domain
{
    public class MplBoolean : IValue
    {
        
        public readonly bool Val;

        public MplBoolean(bool val)
        {
            Val = val;
        }

        protected bool Equals(MplBoolean other)
        {
            return Val == other.Val;
        }

        public override int GetHashCode()
        {
            return Val.GetHashCode();
        }

        public new Type GetType() => Type.Bool;

        public override bool Equals(object obj)
        {
            return obj is MplBoolean other && Equals(other);
        }

        public static MplBoolean operator ==(MplBoolean a, MplBoolean b) => new MplBoolean(a.Val == b.Val);
        public static MplBoolean operator !=(MplBoolean a, MplBoolean b) => new MplBoolean(a.Val != b.Val);
        public static MplBoolean operator <(MplBoolean a, MplBoolean b) => new MplBoolean(!a.Val && b.Val);
        public static MplBoolean operator >(MplBoolean a, MplBoolean b) => new MplBoolean(a.Val && !b.Val);
        public static MplBoolean operator &(MplBoolean a, MplBoolean b) => new MplBoolean(a.Val && b.Val);
        public static MplBoolean operator !(MplBoolean a) => new MplBoolean(!a.Val);
    }
}
