using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace LansUncraftItems
{
	class Panel : UIState
	{
		public static bool visible = false;
		Asset<Texture2D> texture;

		bool shift = false;

		UIImageButton panel;

		public override void OnInitialize()
		{


			texture = ModContent.Request<Texture2D>("LansUncraftItems/uncraft");


			panel = new UIImageButton(texture);

			panel.Width.Set(60, 0);
			panel.Height.Set(60, 0);

			panel.OnLeftClick += new MouseEvent(ButtonClicked);

		}

		private void ButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.mouseItem != null && Main.mouseItem.active && !Main.mouseItem.IsAir)
			{
				//if (Main.LocalPlayer.itemAnimation == 0)
				//{
					LansUncraftItemsUI.Instance.ShowUncraftUI = !LansUncraftItemsUI.Instance.ShowUncraftUI;
                    /*
					shift = false;
					Keys[] pressedKeys = Main.keyState.GetPressedKeys();
					for (int i = 0; i < pressedKeys.Length; i++)
					{
						if (pressedKeys[i] == Keys.LeftShift || pressedKeys[i] == Keys.RightShift)
						{
							shift = true;
						}
					}

					bool all = false;

					if (shift)
					{
						all = true;
					}

					LansUncraftItems.instance.uncraftItem(Main.mouseItem, all);
					*/
                //}
			}
			else
			{
                Main.NewText("Click with an item on your mouse.", new Color(255, 0, 0));
            }
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var shouldBeVisible = true;
            if ((Main.LocalPlayer.chest != -1 || Main.npcShop > 0) && !Main.recBigList)
            {
				shouldBeVisible = false;
            }


            if (visible != shouldBeVisible)
			{
				visible = shouldBeVisible;
				if (visible)
				{
					Append(panel);
				}
				else
				{
					Append(panel);
				}
            }
            int num = 448 + 0 -70 + ModContent.GetInstance<ClientConfig>().OffsetX;
            int num2 = 258 + 0 + ModContent.GetInstance<ClientConfig>().OffsetY;
            num += Main.trashSlotOffset.X;
            num2 += Main.trashSlotOffset.Y;
            panel.Left.Set(num, 0);
            panel.Top.Set(num2, 0);
			Recalculate();


            bool hoveringOverReforgeButton = panel.Left.Pixels <= Main.mouseX && Main.mouseX <= panel.Left.Pixels+panel.Width.Pixels &&
				panel.Top.Pixels <= Main.mouseY && Main.mouseY <= panel.Top.Pixels + panel.Height.Pixels && !PlayerInput.IgnoreMouseInterface;
			if (hoveringOverReforgeButton)
			{
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
