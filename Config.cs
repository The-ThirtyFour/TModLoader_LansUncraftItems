using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace LansUncraftItems
{
	class Config : ModConfig
	{
		// You MUST specify a ConfigScope.
		public override ConfigScope Mode => ConfigScope.ServerSide;

		
		[Label("Ratio of items to get back for an uncrafted item")]
		[Tooltip("Rounding is handled by a chance to get the resource back")]
		[Range(0f,1f)]
		[Increment(0.05f)]
		[DrawTicks]
		[DefaultValue(0.9f)]
		public float Ratio;

    }
}
