using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeophysicsApp
{
    public class Sector
    {
        public int IdSector { get; set; }
        public int SquareSector { get; set; }
        public List<Tuple<int, int>> Coordinates { get; set; }
        public List<Profile> Profiles { get; set; }

        public Sector(int idSector, int squareSector)
        {
            IdSector = idSector;
            SquareSector = squareSector;
            Coordinates = new List<Tuple<int, int>>();
            Profiles = new List<Profile>();
        }
    }
}
