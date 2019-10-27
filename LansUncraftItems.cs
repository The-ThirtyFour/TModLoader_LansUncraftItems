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

		private bool uncraftItem(Item item, Recipe recipe)
		{
			if(item.type == recipe.createItem.type && item.stack >= recipe.createItem.stack)
			{
				inventoryWrapper.StartBatch();

				//success = inventoryWrapper.RemoveItem(item.type, 1);
				foreach (var required in recipe.requiredItem)
				{
					var reqItem = required.DeepClone();
					var ratioBack = GetInstance<Config>().Ratio;
					int back = (int)(reqItem.stack * ratioBack);
					float chanceForLast = (reqItem.stack * ratioBack) - back;
					if (random.NextDouble() <= chanceForLast)
					{
						back++;
					}
					reqItem.stack = back;

					if (!inventoryWrapper.AddItem(reqItem))
					{
						inventoryWrapper.Restore();
						return false;
					}
				}

				if (item.stack > recipe.createItem.stack)
				{
					item.stack -= recipe.createItem.stack;
				}
				else
				{
					item.TurnToAir();
				}
				return true;
			}
			return false;
		}

		private bool uncraftItem(Item item, Recipe recipe, bool all) {
			if(all)
			{
				int count = 0;
				while(uncraftItem(item, recipe))
				{
					count++;
				}
				return count > 0;
			}
			else
			{
				return uncraftItem(item, recipe);
			}
		}

		public void uncraftItem(Item item, bool all)
		{
			List<Recipe> foundRecipes = new List<Recipe>();
			for(int i=0; i<Main.recipe.Length; i++)
			{
				if (Main.recipe[i] != null)
				{
					if(Main.recipe[i].createItem.type == item.type)
					{
						foundRecipes.Add(Main.recipe[i]);
					}
				}
			}

			if(foundRecipes.Count > 0)
			{
				if(foundRecipes.Count > 1)
				{
					Main.NewText("Multiple uncraft recipes found, first one found is used for now...", new Color(255, 0, 0));
				}

				bool success = uncraftItem(item, foundRecipes[0], all);

				Recipe.FindRecipes();

				if(!success)
				{
					Main.NewText("Not enough items in stack for this uncraft recipe.", new Color(255, 0, 0));
				}

			}
			else
			{
				Main.NewText("No uncraft recipe found for this item.", new Color(255, 0, 0));
			}
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