using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeophysicsApp
{
    public class Profile
    {
        public int IdProfile { get; set; }
        public int IdSector { get; set; }
        public int IdEquipment { get; set; }
        public List<Tuple<int, int>> Coordinates { get; set; }
        public List<Picket> Pickets { get; set; }

        public Profile(int idProfile, int idSector, int idEquipment)
        {
            IdProfile = idProfile;
            IdSector = idSector;
            IdEquipment = idEquipment;
            Coordinates = new List<Tuple<int, int>>();
            Pickets = new List<Picket>();
        }
    }
}
