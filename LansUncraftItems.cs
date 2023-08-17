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
using LansUILib.ui;
using LansUILib;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Input;

namespace LansUncraftItems
{
	public struct UncraftResult
	{
		public bool success = true;
		public string errorReason = "";

        public UncraftResult(bool success, string errorReason = "")
        {
            this.success = success;
            this.errorReason = errorReason;
        }
    }

	public class LansUncraftItemsUI : ModSystem
	{
        public static LansUncraftItemsUI Instance;
        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }


        public bool ShowUncraftUI = false;
        bool showingUI = false;
		int CurrentItemType = 0;

        LansUILib.ui.LComponent panel;

		int[] panelSize = new int[] { 250, 250, 250, 300 };

        public override void PostSetupRecipes()
		{
			base.PostSetupRecipes();
			LansUncraftItems.Instance.PostSetup();
        }

		protected bool IsShift()
		{
            var shift = false;
            Keys[] pressedKeys = Main.keyState.GetPressedKeys();
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                if (pressedKeys[i] == Keys.LeftShift || pressedKeys[i] == Keys.RightShift)
                {
					return true;
                }
            }
			return false;
        }

		public void RecreatePanel()
		{
            panel = LansUILib.UIFactory.CreatePanel("Main Panel", true, false);

            panel.SetAnchor(LansUILib.ui.AnchorPosition.TopLeft);
			panel.SetSize(panelSize[0], panelSize[1], panelSize[2], panelSize[3]);
            //panel.SetLayout(new LansUILib.ui.LayoutFlow(new bool[] { false, false }, new bool[] { true, true }, LayoutFlowType.Vertical, 5, 5, 5 ,5, 10));
            //panel.SetSize(250, 250, 100, 100);
			
			var inner = new LComponent("Inner");
			inner.isMask = true;
            panel.Add(inner);
			//inner.SetAnchor(AnchorPosition.TopLeft);
			//inner.SetSize(100, 100, 200, 200);
			inner.SetMargins(15, 15, 15, 15);
            inner.SetLayout(new LansUILib.ui.LayoutFlow(new bool[] { false, false }, new bool[] { true, true }, LayoutFlowType.Vertical, 0, 0, 0, 0, 10));

            inner.Add(LansUILib.UIFactory.CreateText($"Uncraft item: {Main.mouseItem.Name}", true));
            inner.Add(LansUILib.UIFactory.CreateText($"Chance: {GetInstance<Config>().Ratio*100}%", true));
			

			var recipes = LansUncraftItems.Instance.FindUncraftRecipes(Main.mouseItem);

            var scrollpanel = UIFactory.CreateScrollPanel();
			scrollpanel.wrapper.GetLayout().Flex = 1;

            //var recipePanel = LansUILib.UIFactory.CreatePanel("Recipe Panel", false, false);
            var recipePanel = scrollpanel.contentPanel;
            recipePanel.SetLayout(new LansUILib.ui.LayoutFlow(new bool[] { true, true }, new bool[] { false, false }, LayoutFlowType.Vertical, 20, 0, 0, 0, 5));
            inner.Add(scrollpanel.wrapper);

            foreach (var r in recipes)
			{
                var recipePanelCurr = LansUILib.UIFactory.CreatePanel("Recipe Panel Current", false, false);
                recipePanelCurr.SetLayout(new LansUILib.ui.LayoutFlow(new bool[] { true, true }, new bool[] { false, true }, LayoutFlowType.Horizontal, 3, 3, 3, 3, 5));

                var buttonOne = LansUILib.UIFactory.CreateButton("x1");
				buttonOne.Panel.SetLayout(new LayoutSize(32, 32));
                buttonOne.OnClicked += delegate (LansUILib.ui.MouseState state)
				{
					if (Main.LocalPlayer.itemAnimation == 0)
					{
						var res = LansUncraftItems.Instance.UncraftItem(Main.mouseItem, r, false);
                        if (!res.success)
                        {
                            Main.NewText(res.errorReason, new Color(255, 0, 0));
                        }
                    }
                };

                var buttonAll = LansUILib.UIFactory.CreateButton("All");
                buttonAll.Panel.SetLayout(new LayoutSize(32, 32));
                buttonAll.OnClicked += delegate (LansUILib.ui.MouseState state)
                {
                    if (Main.LocalPlayer.itemAnimation == 0)
                    {
                        var res = LansUncraftItems.Instance.UncraftItem(Main.mouseItem, r, true);
                        if (!res.success)
                        {
                            Main.NewText(res.errorReason, new Color(255, 0, 0));
                        }
                    }
                };

				recipePanelCurr.Add(buttonOne.Panel);
				recipePanelCurr.Add(buttonAll.Panel);

                foreach (var required in r.requiredItem)
                {
                    var reqItem = required.Clone();
                    Main.instance.LoadItem(reqItem.type);



                    var itemImage = UIFactory.CreateImage(TextureAssets.Item[reqItem.type], Main.itemAnimations[reqItem.type], true);
					itemImage.image.FillMode = LansUILib.ui.components.ImageFillMode.Normal;
                    var itemCount = UIFactory.CreateText($"x{reqItem.stack}", true);
                    recipePanelCurr.Add(itemImage);
					recipePanelCurr.Add(itemCount);
                }
				recipePanel.Add(recipePanelCurr);
            }

            for (int k = 0; k < RecipeGroup.nextRecipeGroupIndex; k++)
            {
                RecipeGroup rec = RecipeGroup.recipeGroups[k];
                if(rec.ContainsItem(Main.mouseItem.type))
				{
                    foreach (var validItem in rec.ValidItems)
                    {
						if (validItem != Main.mouseItem.type)
						{
							var recipePanelCurr = LansUILib.UIFactory.CreatePanel("Recipe Panel Current", false, false);
							recipePanelCurr.SetLayout(new LansUILib.ui.LayoutFlow(new bool[] { true, true }, new bool[] { false, true }, LayoutFlowType.Horizontal, 3, 3, 3, 3, 5));

							var buttonOne = LansUILib.UIFactory.CreateButton("x1");
							buttonOne.Panel.SetLayout(new LayoutSize(32, 32));
							buttonOne.OnClicked += delegate (LansUILib.ui.MouseState state)
							{
								if (Main.LocalPlayer.itemAnimation == 0)
								{
									LansUncraftItems.Instance.SwapItemGroup(Main.mouseItem, validItem, false);
								}
							};

							var buttonAll = LansUILib.UIFactory.CreateButton("All");
							buttonAll.Panel.SetLayout(new LayoutSize(32, 32));
							buttonAll.OnClicked += delegate (LansUILib.ui.MouseState state)
							{
								if (Main.LocalPlayer.itemAnimation == 0)
								{
									LansUncraftItems.Instance.SwapItemGroup(Main.mouseItem, validItem, true);
								}
							};
							recipePanelCurr.Add(buttonOne.Panel);
							recipePanelCurr.Add(buttonAll.Panel);


							Main.instance.LoadItem(validItem);
							var itemImage = UIFactory.CreateImage(TextureAssets.Item[validItem], Main.itemAnimations[validItem], true);
							var itemCount = UIFactory.CreateText($"x1 (100%)", true);
                            itemImage.image.FillMode = LansUILib.ui.components.ImageFillMode.Normal;
                            recipePanelCurr.Add(itemImage);
							recipePanelCurr.Add(itemCount);
							recipePanel.Add(recipePanelCurr);
						}
                    }
                }
            }

			
        }

        public override void PostUpdatePlayers()
		{
			base.PostUpdatePlayers();

			if (Main.mouseItem == null || !Main.mouseItem.active || Main.mouseItem.IsAir || !Main.playerInventory)
			{
				ShowUncraftUI = false;
            }

            if (ShowUncraftUI != showingUI)
			{
				showingUI = ShowUncraftUI;
                if (showingUI)
                {
					CurrentItemType = Main.mouseItem.type;

                    RecreatePanel();
                    LansUILib.UISystem.Instance.Screen.Add(panel);
                    panel.Invalidate();
                    

                }
                else
                {
                    panelSize = panel.GetSize();
                    LansUILib.UISystem.Instance.Screen.Remove(panel);
                }
            }
			else
			{
                if (showingUI && CurrentItemType != Main.mouseItem.type)
                {

					panelSize = panel.GetSize();

                    CurrentItemType = Main.mouseItem.type;
                    LansUILib.UISystem.Instance.Screen.Remove(panel);

                    RecreatePanel();
                    LansUILib.UISystem.Instance.Screen.Add(panel);
                    panel.Invalidate();
                }
            }
		}
	}

	public class LansUncraftItems : Mod
	{

		PlayerInventoryWrapper inventoryWrapper = new PlayerInventoryWrapper();

		public static LansUncraftItems Instance;



        Random random = new Random();

		public List<RecipeBlock> blockedRecipes = new List<RecipeBlock>();

		protected bool[] cyclicRecipe;

		public LansUncraftItems()
		{
			Instance = this;
		}

		public override void Load()
		{
			
		}

		public override void Unload()
		{
			Instance = null;
		}

		public void BlockMods(ModBlockedItemDef[] modDefs)
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

		public bool IsRecipeCyclic(int recipeId)
		{
			var recipe = Main.recipe[recipeId];
            if (recipe != null) {
				var output = recipe.createItem;

				if(recipe.requiredItem.Count != 1)
				{
					return false;
				}

				var input = recipe.requiredItem[0];


                for (int i = 0; i < Main.recipe.Length; i++)
				{
					if (Main.recipe[i] != null)
					{
						if (Main.recipe[i].createItem.type == input.type && Main.recipe[i].createItem.stack == input.stack)
						{
                            if(Main.recipe[i].requiredItem.Count == 1) {
								if(Main.recipe[i].requiredItem[0].type == output.type && Main.recipe[i].requiredItem[0].stack == output.stack)
								{
									return true;
								}

                            }
                        }
					}
				}
			}

            return false;
		}

		public void PostSetup()
		{
            BlockMods(
                new ModBlockedItemDef[]
                {
					/*new ModBlockedItemDef(
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
					),*/
			});

            cyclicRecipe = new bool[Main.recipe.Length];
            for (int i = 0; i < cyclicRecipe.Length; i++)
            {
                cyclicRecipe[i] = IsRecipeCyclic(i);
            }
        }

        private UncraftResult uncraftItem(Item item, Recipe recipe)
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
						return new UncraftResult(false, "Not enough space in inventory to uncraft.");
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
				return new UncraftResult(true);
			}
			return new UncraftResult(false, "Not enough items in stack to uncraft."); ;
		}

		public UncraftResult UncraftItem(Item item, Recipe recipe, bool all) {
			if(all)
			{
				int count = 0;
				while(true)
				{
					var res = uncraftItem(item, recipe);
					if(!res.success)
					{
						if(count > 0)
						{
							return new UncraftResult(true);
                        }
                        return res;
                    }
                    count++;
				}
			}
			else
			{
				return uncraftItem(item, recipe);
			}
		}

        private bool SwapItemGroup(Item item, int newType)
        {
			if (item.stack >= 1)
			{
				inventoryWrapper.StartBatch();

				if (!inventoryWrapper.AddItem(new Item(newType)))
				{
					inventoryWrapper.Restore();
					return false;
				}

				if (item.stack > 1)
				{
					item.stack -= 1;
				}
				else
				{
					item.TurnToAir();
				}
				return true;
			}
			return false;
        }

        public bool SwapItemGroup(Item item, int newType, bool all)
        {
            if (all)
            {
                int count = 0;
                while (SwapItemGroup(item, newType))
                {
                    count++;
                }
                return count > 0;
            }
            else
            {
                return SwapItemGroup(item, newType);
            }
        }


        public List<Recipe> FindUncraftRecipes(Item item)
		{
            List<Recipe> foundRecipes = new List<Recipe>();
            List<Recipe> foundBlockedRecipes = new List<Recipe>();
            for (int i = 0; i < Main.recipe.Length; i++)
            {
				if(cyclicRecipe[i])
				{
					continue;
				}

                if (Main.recipe[i] != null)
                {
                    if (Main.recipe[i].createItem.type == item.type)
                    {
                        var inBlocked = false;
                        foreach (var b in blockedRecipes)
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
			return foundRecipes;
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

				var res = UncraftItem(item, foundRecipes[0], all);

				Recipe.FindRecipes();

				if(!res.success)
				{
					Main.NewText(res.errorReason, new Color(255, 0, 0));
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
}