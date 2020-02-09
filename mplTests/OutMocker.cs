using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mplTests
{
    class OutMocker : TextWriter
    {
        public List<string> Output = new List<string>();
        public override Encoding Encoding { get; }

        public override void Write(int value)
        {
            Write(value.ToString());
        }

        public override void Write(bool value)
        {
            Write(value.ToString());
        }

        public override void Write(string value)
        {
            Output.Add(value);
            base.Write(value);
        }
    }
}
