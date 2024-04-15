using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstGame
{
    public static class Globals
    {
        public const short
            //Contact filters
            DefaultCF = 0x0001,                 //00000000 00000001
            PlayerCF = 0x0008,                 //00000000 00001000
            PlayerInteractCF = 0x0002,                 //00000000 00000010
            LightCF = short.MinValue,        //10000000 00000000
            BulletCF = 0x0004,                 //00000000 00000100
            ZombieCF = 0x0010,                 //00000000 00010000
            AllCF = -1,                     //11111111 11111111
            //Contact groups
            PlayerCG = 42,
            LightCG = 10;

        static Dictionary<string, int> tiles = new Dictionary<string, int>();
    }
}
