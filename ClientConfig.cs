using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace LansUncraftItems
{
	class ClientConfig : ModConfig
	{
		// You MUST specify a ConfigScope.
		public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Offset Icon By X Pixels To The Right")]
        [Range(-10000, 10000)]
        [Increment(1)]
        [DefaultValue(0)]
        public int OffsetX;

        [Label("Offset Icon By Y Pixels To The Bottom")]
        [Range(-10000, 10000)]
        [Increment(1)]
        [DefaultValue(0)]
        public int OffsetY;

    }
}
