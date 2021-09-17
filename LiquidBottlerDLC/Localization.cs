using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alesseon.LiquidBottler
{
    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class ALESSEON
                {
                    public class LIQUIDBOTTLER
                    {
                        public static LocString NAME = "Liquid Bottle Filler";
                        public static LocString DESC = "Allow Duplicants to fetch bottled liquids for delivery to buildings.";
                        public static LocString EFFECT = "Automatically stores piped <link=\"ELEMENTSLIQUID\">Liquid</link> into bottles for manual transport.";
                    }
                    public class LIQUIDBOTTLEEMPTIER
                    {
                        public static LocString NAME = "Liquid bottle emptier";
                        public static LocString DESC = "Allows emptying bottles directly to the pipe system.";
                        public static LocString EFFECT = "Automatically empties <link=\"ELEMENTSLIQUID\">Liquid</link> from bottles for pipe transport.";
                    }
                }
            }
        }
        public class UI
        {
            public class USERMENUACTIONS
            {
                public class AUTO_PUMP_DROP
                {
                    public class DENIED
                    {
                        public static LocString NAME = "Enable auto drop";
                        public static LocString TOOLTIP = "Drop the fluid";
                    }
                    public class ALLOWED
                    {
                        public static LocString NAME = "Disable auto drop";
                        public static LocString TOOLTIP = "Auto drop disabled";
                    }
                }
            }
        }
    }
}
