using Terraria;


namespace LansUncraftItems
{
    public class RecipeBlock
	{
		public Recipe recipe;

		public RecipeBlockCondition condition;

		public RecipeBlock(Recipe recipe)
		{
			this.recipe = recipe;
		}
	}
}