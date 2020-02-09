using System;
using System.Collections.Generic;
using System.Text;
using mpl.Exceptions;

namespace mpl.domain
{
    public struct MplInteger : IValue
    {
        public readonly int Val;

        public MplInteger(int val)
        {
            Val = val;
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

        public static MplInteger operator +(MplInteger a, MplInteger b) => new MplInteger(a.Val + b.Val);
        public static MplInteger operator -(MplInteger a, MplInteger b) => new MplInteger(a.Val - b.Val);
        public static MplInteger operator *(MplInteger a, MplInteger b) => new MplInteger(a.Val * b.Val);

        public static MplInteger operator /(MplInteger a, MplInteger b)
        {
            if (b.Val == 0) 
                throw new MplDivideByZeroException("Attempted to divide by zero", 0, 0);
            return new MplInteger(a.Val / b.Val);
        }
        public static MplBoolean operator ==(MplInteger a, MplInteger b) => new MplBoolean(a.Val == b.Val);
        public static MplBoolean operator !=(MplInteger a, MplInteger b) => new MplBoolean(a.Val != b.Val);
        public static MplBoolean operator <(MplInteger a, MplInteger b) => new MplBoolean(a.Val < b.Val);
        public static MplBoolean operator >(MplInteger a, MplInteger b) => new MplBoolean(a.Val > b.Val);
        
    }
}
