using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeophysicsApp
{
    public class Measurement
    {
        public int IdPicket1 { get; set; }
        public int IdPicket2 { get; set; }
        public int Depth { get; set; }
        public int Difference { get; set; }

        public Measurement(int idPicket1, int idPicket2, int depth, int difference)
        {
            IdPicket1 = idPicket1;
            IdPicket2 = idPicket2;
            Depth = depth;
            Difference = difference;
        }
    }
}
