using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace LansUncraftItems
{
    public class UISystem : ModSystem
    {
		public UserInterface uncraftPanelInterface;
		internal Panel uncraftPanel;

		public override void UpdateUI(GameTime gameTime)
		{
			// it will only draw if the player is not on the main menu
			if (Main.playerInventory)
			{
				uncraftPanelInterface?.Update(gameTime);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("UncraftPanelLayer", DrawSomethingUI, InterfaceScaleType.UI));
			}
		}

		private bool DrawSomethingUI()
		{
			// it will only draw if the player is not on the main menu
			if (Main.playerInventory)
			{
				uncraftPanelInterface.Draw(Main.spriteBatch, new GameTime());
			}
			return true;
		}

        public override void OnModLoad()
        {
			// this makes sure that the UI doesn't get opened on the server
			// the server can't see UI, can it? it's just a command prompt
			if (!Main.dedServ)
			{
				uncraftPanel = new Panel();
				uncraftPanel.Initialize();
				this.
				uncraftPanelInterface = new UserInterface();
				uncraftPanelInterface.SetState(uncraftPanel);
			}
		}
    }
}
