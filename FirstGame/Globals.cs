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
            DEFAULT_CONTACT_FILTER = 0x0001, //00000000 00000001
            PLAYER_CONTACT_FILTER = 0x0008, //00000000 00001000
            PLAYER_INTERACT_CONTACT_FILTER = 0x0002, //00000000 00000010
            LIGHT_CONTACT_FILTER = short.MinValue, //10000000 00000000
            BULLET_CONTACT_FILTER = 0x0004, //00000000 00000100
            ZOMBIE_CONTACT_FILTER = 0x0010, //00000000 00010000
            ALL_CONTACT_FILTER = -1, //11111111 11111111
            NONE_CONTACT_FILTER = 0X0000,

            PLAYER_CONTACT_GROUP = 42,
            TRANSPARENT_GROUP = -10,

            TAG_FOR_FIXED_UPDATE = 1;
    }
}
