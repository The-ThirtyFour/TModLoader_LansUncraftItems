using Terraria;
using Terraria.ModLoader;
using MonoMod.Cil;
using static Mono.Cecil.Cil.OpCodes;
using System.Reflection;
using Mono.Cecil;
using Terraria.Localization;
using System;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;


namespace LansUncraftItems
{
	public class UnlimitedBuffLimit : Mod
	{

		PlayerInventoryWrapper inventoryWrapper = new PlayerInventoryWrapper();

		public static UnlimitedBuffLimit instance;

		internal Panel uncraftPanel;
		public UserInterface uncraftPanelInterface;

		Random random = new Random();

		public UnlimitedBuffLimit()
		{
			instance = this;
		}

		public override void Load()
		{
			// this makes sure that the UI doesn't get opened on the server
			// the server can't see UI, can it? it's just a command prompt
			if (!Main.dedServ)
			{
				uncraftPanel = new Panel();
				uncraftPanel.Initialize();
				uncraftPanelInterface = new UserInterface();
				uncraftPanelInterface.SetState(uncraftPanel);
			}
		}

		public override void Unload()
		{
			instance = null;
		}


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

		public bool uncraftItem(Item item)
		{
			for(int i=0; i<Main.recipe.Length; i++)
			{
				if (Main.recipe[i] != null)
				{
					if(Main.recipe[i].createItem.type == item.type)
					{
						inventoryWrapper.StartBatch();

						bool success = true;

						//success = inventoryWrapper.RemoveItem(item.type, 1);
						foreach(var required in Main.recipe[i].requiredItem)
						{
							var reqItem = required.DeepClone();
							var ratioBack = GetInstance<Config>().Ratio;
							int back = (int) (reqItem.stack * ratioBack);
							float chanceForLast = (reqItem.stack * ratioBack) - back;
							if(random.NextDouble()<= chanceForLast)
							{
								back++;
							}
							reqItem.stack = back;


							success = success && inventoryWrapper.AddItem(reqItem);
						}
						//Main.NewText("Uncrafted item: "+ success, 155, 155, 155);
						if (!success)
						{
							inventoryWrapper.Restore();
						}
						return success;
					}
				}
			}
			return false;
		}

		
	}

	public class PlayerInventoryWrapper
	{
		Item[] backup = new Item[50];

		public void StartBatch()
		{
			for (int k = 0; k < 50; k++)
			{
				backup[k] = Main.LocalPlayer.inventory[k].DeepClone();
			}
		}

		public void Restore()
		{
			for (int k = 0; k < 50; k++)
			{
				Main.LocalPlayer.inventory[k] = backup[k];
			}
		}

		public bool RemoveItem(int type, int count = 1)
		{

			for (int k = 0; k < 50; k++)
			{
				if (Main.LocalPlayer.inventory[k].type == type)
				{
					if (Main.LocalPlayer.inventory[k].stack <= count)
					{
						count -= Main.LocalPlayer.inventory[k].stack;
						Main.LocalPlayer.inventory[k].TurnToAir();

					}
					else
					{
						Main.LocalPlayer.inventory[k].stack -= count;
						return true;
					}
				}
			}


			return false;
		}

		public bool RemoveItem(Item item)
		{
			return RemoveItem(item.type, item.stack);
		}

		public bool AddItem(Item item)
		{
			for (int k = 0; k < 50; k++)
			{
				if (!Main.LocalPlayer.inventory[k].IsAir && Main.LocalPlayer.inventory[k].active && Main.LocalPlayer.inventory[k].type == item.type)
				{
					if (Main.LocalPlayer.inventory[k].stack+item.stack <= item.maxStack)
					{
						Main.LocalPlayer.inventory[k].stack += item.stack;
						return true;
					}
					else
					{
						int diff = item.maxStack - Main.LocalPlayer.inventory[k].stack;
						Main.LocalPlayer.inventory[k].stack = item.maxStack;
						item.stack -= diff;
					}
				}
			}

			for (int k = 0; k < 50; k++)
			{
				if (Main.LocalPlayer.inventory[k].IsAir)
				{
					Main.LocalPlayer.inventory[k] = item;
					return true;
				}
			}


			return false;
		}
	}
}