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
	public class RecipeBlockCondition
	{
		public virtual bool recipeCanBeUsed()
		{
			return false;
		}
	}

	public class RecipeBlock
	{
		public Recipe recipe;

		public RecipeBlockCondition condition;

		public RecipeBlock(Recipe recipe)
		{
			this.recipe = recipe;
		}
	}

	public class ModBlockedItemDef
    {
		public string ModName;
		public string[] ModItems;

        public ModBlockedItemDef(string modName, string[] modItems)
        {
            ModName = modName;
            ModItems = modItems;
        }
    }


	public class LansUncraftItems : Mod
	{

		PlayerInventoryWrapper inventoryWrapper = new PlayerInventoryWrapper();

		public static LansUncraftItems instance;

		

		Random random = new Random();

		public List<RecipeBlock> blockedRecipes = new List<RecipeBlock>();

		public LansUncraftItems()
		{
			instance = this;
		}

		public override void Load()
		{
			
		}

		

		public override void Unload()
		{
			instance = null;
		}

		private void BlockMods(ModBlockedItemDef[] modDefs)
        {
			foreach (var modDef in modDefs)
			{
				String modName = modDef.ModName;
				Mod externalMod = null;
				if (ModLoader.TryGetMod(modName, out externalMod))
				{
					string[] itemNames = modDef.ModItems;
					ModItem[] blockedCreateItems = new ModItem[itemNames.Length];

					for (int i = 0; i < itemNames.Length; i++)
					{
						if (!ModContent.TryFind(modName, "BasicEssence", out blockedCreateItems[i]))
						{
							throw new Exception($"{modName}'s items seems to have changed, LansUncraftItems can no longer support it. Please remove either mod.");
						}
					}

					for (int j = 0; j < blockedCreateItems.Length; j++)
					{
						if (blockedCreateItems[j] != null)
						{
							var item = blockedCreateItems[j].Item;
							for (int i = 0; i < Main.recipe.Length; i++)
							{
								if (Main.recipe[i] != null)
								{
									if (Main.recipe[i].createItem.type == item.type)
									{
										blockedRecipes.Add(new RecipeBlock(Main.recipe[i]));
									}
								}
							}
						}
					}
				}
			}
		}

		public override void PostAddRecipes()
		{
			base.PostAddRecipes();


			BlockMods(
				new ModBlockedItemDef[]
				{
					new ModBlockedItemDef(
						"TokenMod",
						new string[]
						{
							"BasicEssence",
							"Tier1Essence",
							"Tier2Essence",
							"Tier3Essence",
							"Tier4Essence",
							"Tier5Essence",
							"Tier6Essence",
							"Tier7Essence"
						}
					),
					new ModBlockedItemDef(
						"imkSushisMod",
						new string[]
						{
							"FishingHardmodeToken",
							"FishingStartToken",
							"LootGoblinsToken",
							"LootHardmodeToken",
							"LootMartiansToken",
							"LootMechToken",
							"LootPiratesToken",
							"LootPlanteraToken",
							"LootSkeletronToken",
							"LootStartToken",
							"SpacePurityHardmodeToken",
							"SpacePurityStartToken",
							"SurfaceDesertStartToken",
							"SurfacePurityEocToken",
							"SurfacePurityStartToken",
							"SwapToken",
							"TempleJunglePlanteraToken",
							"UndergroundCorruptionEocToken",
							"UndergroundCrimsonEocToken",
							"UndergroundDungeonSkeletronToken",
							"UndergroundJungleStartToken",
							"UndergroundPurityStartToken",
							"UndergroundSnowStartToken",
							"UnderwaterOceanStartToken",
							"UnderworldHellSkeletronToken"
						}
					),
			});
		}

		private bool uncraftItem(Item item, Recipe recipe)
		{
			if(item.type == recipe.createItem.type && item.stack >= recipe.createItem.stack)
			{
				inventoryWrapper.StartBatch();

				//success = inventoryWrapper.RemoveItem(item.type, 1);
				foreach (var required in recipe.requiredItem)
				{
					var reqItem = required.Clone();
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
			List<Recipe> foundBlockedRecipes = new List<Recipe>();
			for (int i=0; i<Main.recipe.Length; i++)
			{
				if (Main.recipe[i] != null)
				{
					if(Main.recipe[i].createItem.type == item.type)
					{
						var inBlocked = false;
						foreach(var b in blockedRecipes)
						{
							if (b.recipe == Main.recipe[i])
							{
								inBlocked = true;
							}
						}

						if (inBlocked)
						{
							foundBlockedRecipes.Add(Main.recipe[i]);
						}
						else
						{
							foundRecipes.Add(Main.recipe[i]);
						}
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
				if(foundBlockedRecipes.Count > 0)
				{
					Main.NewText("Uncraft recipe for this item has been disabled.", new Color(255, 0, 0));
				}
				else
				{
					Main.NewText("No uncraft recipe found for this item.", new Color(255, 0, 0));
				}
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
				backup[k] = Main.LocalPlayer.inventory[k].Clone();
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