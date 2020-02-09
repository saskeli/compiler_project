using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mplTests
{
    public class InMocker : TextReader
    {
        public List<string> Inp;
        private int idx = 0;

        public InMocker(string[] inp)
        {
            this.Inp = new List<string>(inp);
        }

        public InMocker()
        {
            this.Inp = new List<string>();
        }

        public void SetInput(string[] inp)
        {
            this.Inp = new List<string>(inp);
        }

        public override string ReadLine()
        {
            return Inp[idx % Inp.Count];
        }


    }
}
