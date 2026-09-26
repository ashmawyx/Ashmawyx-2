using System.Collections.Generic;

namespace AshmawyX
{
    public static class KeyRoutingRules
    {
        private static Dictionary<string, int> map = new Dictionary<string, int>
        {
            { "1", 0x31 },
            { "2", 0x32 },
            { "3", 0x33 },
            { "4", 0x34 },
            { "5", 0x35 },
            { "6", 0x36 },
            { "7", 0x37 },
            { "8", 0x38 },
            { "9", 0x39 },
            { "0", 0x30 },

            { "F", 0x46 },
            { "R", 0x52 },
            { "E", 0x45 },
            { "Q", 0x51 },

            { "SHIFT", 0x10 },
            { "CTRL", 0x11 },
            { "ALT", 0x12 }
        };

        public static int StringToVK(string key)
        {
            if (map.ContainsKey(key))
                return map[key];

            return -1;
        }
    }
}
