using System.Collections.Generic;
using System.IO;

namespace mplTests
{
    public class InMocker : TextReader
    {
        public List<string> Inp;
        private int _idx;

        public InMocker(IEnumerable<string> inp)
        {
            Inp = new List<string>(inp);
        }

        public InMocker()
        {
            Inp = new List<string>();
        }

        public void SetInput(string[] inp)
        {
            Inp = new List<string>(inp);
        }

        public override string ReadLine()
        {
            string s = Inp[_idx % Inp.Count];
            _idx++;
            return s;
        }


    }
}
