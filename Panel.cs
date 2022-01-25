using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
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

			panel.Left.Set(560, 0);
			panel.Top.Set(32, 0);
			panel.Width.Set(60, 0);
			panel.Height.Set(60, 0);

			panel.OnClick += new MouseEvent(ButtonClicked);

			//panel.
			//	Main.mouseItem
			Append(panel);
		}

		private void ButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.mouseItem != null && Main.mouseItem.active)
			{
				if (Main.LocalPlayer.itemAnimation == 0)
				{
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
					
				}
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			bool hoveringOverReforgeButton = panel.Left.Pixels <= Main.mouseX && Main.mouseX <= panel.Left.Pixels+panel.Width.Pixels &&
				panel.Top.Pixels <= Main.mouseY && Main.mouseY <= panel.Top.Pixels + panel.Height.Pixels && !PlayerInput.IgnoreMouseInterface;
			if (hoveringOverReforgeButton)
			{
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
