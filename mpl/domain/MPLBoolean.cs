namespace mpl.domain
{
    public class MplBoolean : IValue
    {
        
        public readonly bool Val;
        public readonly int Line;
        public readonly int Position;

        public MplBoolean(bool val, int line, int position)
        {
            Val = val;
            Line = line;
            Position = position;
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

        public static MplBoolean operator ==(MplBoolean a, MplBoolean b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(a.Val == b.Val, l, p);
        }
        public static MplBoolean operator !=(MplBoolean a, MplBoolean b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(a.Val != b.Val, l, p);
        }
        public static MplBoolean operator <(MplBoolean a, MplBoolean b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(!a.Val && b.Val, l, p);
        }
        public static MplBoolean operator >(MplBoolean a, MplBoolean b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(a.Val && !b.Val, l, p);
        }
        public static MplBoolean operator &(MplBoolean a, MplBoolean b)
        {
            int l = a.Line > b.Line ? a.Line : b.Line;
            int p = a.Position > b.Position ? a.Position : b.Position;
            if (a.Line != b.Line) p = a.Line > b.Line ? a.Position : b.Position;
            return new MplBoolean(a.Val && b.Val, l, p);
        }
        public static MplBoolean operator !(MplBoolean a)
        {
            return new MplBoolean(!a.Val, a.Line, a.Position);
        }

        public int GetLine() => Line;

        public int GetPosition() => Position;
    }
}
