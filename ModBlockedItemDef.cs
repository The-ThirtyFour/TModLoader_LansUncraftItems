namespace LansUncraftItems
{
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
}