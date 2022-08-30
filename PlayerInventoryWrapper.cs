using Terraria;

namespace LansUncraftItems
{
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