using System;

namespace mpl.domain
{
    public class Printer : Part
    {
        private readonly Part _parent;
        private readonly Assignment _ass;

        public Printer(Part parent, int line, int position)
        {
            _parent = parent;
            _ass = new Assignment(this, line, position);
        }

        public override void Run()
        {
            _ass.Run();
            switch (_ass.Value)
            {
                case MplBoolean boolean:
                    Console.Write(boolean.Val);
                    break;
                case MplInteger integer:
                    Console.Write(integer.Val);
                    break;
                default:
                    Console.Write(((MplString)_ass.Value).Val);
                    break;
            }
        }

        public override Part GetParent() => _parent;

        public override void Add(Token token)
        {
            _ass.Add(token);
        }

        public override bool Exit() => _ass.Exit();

        public override Definition GetDefinition(string name) => _parent.GetDefinition(name);
    }
}
