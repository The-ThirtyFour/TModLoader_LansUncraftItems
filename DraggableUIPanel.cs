using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;


namespace LansUncraftItems
{
    // This DragableUIPanel class inherits from UIPanel. 
    // Inheriting is a great tool for UI design. By inheriting, we get the background drawing for free from UIPanel
    // We've added some code to allow the panel to be dragged around. 
    // We've also added some code to ensure that the panel will bounce back into bounds if it is dragged outside or the screen resizes.
    // UIPanel does not prevent the player from using items when the mouse is clicked, so we've added that as well.
    public class DragableUIPanel : UIElement
    {
        // Stores the offset from the top left of the UIPanel while dragging.
        private Vector2 offset;
        public bool dragging;

        UIElement dragbar;
        //public float scroll = 0;
        //public float maxScroll = 0;

        public bool hoveringbutton = false;

        public DragableUIPanel(UIElement dragbar)
        {
            this.dragbar = dragbar;
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            OverflowHidden = false;

            dragbar.OnMouseDown += dragbar_MouseDown;
            dragbar.OnMouseUp += dragbar_MouseUp;
        }
        public void dragbar_MouseDown(UIMouseEvent evt, UIElement listeningElement)
        {
            if (hoveringbutton)
            {
                return;
            }
            //dragbar.MouseDown(evt);
            base.MouseDown(evt);
            DragStart(evt);
        }

        public void dragbar_MouseUp(UIMouseEvent evt, UIElement listeningElement)
        {
            if (hoveringbutton)
            {
                return;
            }
            //dragbar.MouseDown(evt);
            base.MouseUp(evt);
            DragEnd(evt);
        }

        private void DragStart(UIMouseEvent evt)
        {
            offset = new Vector2(evt.MousePosition.X - dragbar.GetDimensions().X, evt.MousePosition.Y - dragbar.GetDimensions().Y);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt)
        {
            Vector2 end = evt.MousePosition;
            dragging = false;

            Left.Set(end.X - offset.X, 0f);
            Top.Set(end.Y - offset.Y, 0f);

            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.

            // Checking ContainsPoint and then setting mouseInterface to true is very common. This causes clicks on this UIElement to not cause the player to use current items. 
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f); // Main.MouseScreen.X and Main.mouseX are the same.
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            // Here we check if the DragableUIPanel is outside the Parent UIElement rectangle. 
            // (In our example, the parent would be ExampleUI, a UIState. This means that we are checking that the DragableUIPanel is outside the whole screen)
            // By doing this and some simple math, we can snap the panel back on screen if the user resizes his window or otherwise changes resolution.
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                // Recalculate forces the UI system to do the positioning math again.
                Recalculate();
            }
            dragbar.Left.Set(Left.Pixels,0);
            dragbar.Top.Set(Top.Pixels,0);
            dragbar.Width.Set(this.Width.Pixels, 0);
        }
        /*public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);

            if (evt.ScrollWheelValue < 0) {
                scroll += 5;
            } else
            {
                scroll -= 5;
            }
            if (scroll > maxScroll)
            {
                scroll = maxScroll;
            }
            else if (scroll < 0) { 
                scroll = 0;
            }
        }*/
    }
}

