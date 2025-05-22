using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeophysicsApp
{
    public class Picket
    {
        public int IdPicket { get; set; }
        public int IdProfile { get; set; }
        public Tuple<int, int> Coordinates { get; set; }

        public Picket(int idPicket, int idProfile, Tuple<int, int> coordinates)
        {
            IdPicket = idPicket;
            IdProfile = idProfile;
            Coordinates = coordinates;
        }
    }
}
