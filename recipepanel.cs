using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria;
using System.Security.Cryptography.X509Certificates;
using log4net.Config;
using Terraria.ModLoader.UI.Elements;
using IL.Terraria.World.Generation;
using System.Deployment.Internal;
using System.Diagnostics;

namespace LansUncraftItems
{
    class recipepanel : UIState
    {
        public List<TextButton> indexes = new List<TextButton>();

        public static bool visible = false;

        //public UIElement area;
        public SizePanel sizeY;
        public SizePanel sizeX;
        public UIElement recipes;
        //public UIScrollbar scroll;
        public DragableUIPanel dragbar;
        public mainPanel panel;
        public UIGrid grid;

        public recipeGrid textpanel;

        public UIElement dragbarbar;

        //public bool sizeXdrag = false;
        //public bool sizeYdrag = false;

        //public UIText sizeYT;
        //public UIText sizeXT;

        public override void OnInitialize()
        {
            //area = new UIElement();
            //area.Left.Set(200, 0);
            //area.Top.Set(200, 0);
            //area.Width.Set(200, 0);
            //area.Height.Set(200, 0);

            dragbarbar = new UIPanel();
            dragbarbar.Height.Set(15, 0);

            dragbar = new DragableUIPanel(dragbarbar);
            dragbar.Left.Set(400, 0);
            dragbar.Top.Set(400, 0);
            dragbar.Width.Set(200, 0);
            dragbar.Height.Set(200, 0);

            panel = new mainPanel();
            panel.Left.Set(0, 0);
            panel.Top.Set(0, 0);
            panel.Width.Set(200, 0);
            panel.Height.Set(200, 0);

            grid = new UIGrid();
            grid.Left.Set(3, 0);
            grid.Top.Set(3, 0);
            grid.Width.Set(panel.Width.Pixels - 3, 0);
            grid.Height.Set(panel.Height.Pixels - 3, 0);

            //recipes = new UIText("a\naa\na\na\na\na\na\na\na\na\na", 1.0f);
            recipes = new UIElement();
            recipes.Left.Set(30, 0);
            recipes.Top.Set(5, 0);
            //recipes.Width.Set(190, 0);
            //recipes.Height.Set(190, 0);

            //scroll = new UIScrollbar();
            //scroll.Left.Set(grid.Left.Pixels - 5, 0);
            //scroll.Top.Set(5, 0);
            //scroll.Width.Set(10, 0);
            //scroll.Height.Set(grid.Height.Pixels - 10,0);

            sizeY = new SizePanel();
            sizeY.Width.Set(panel.Width.Pixels, 0);
            sizeY.Height.Set(15, 0);
            sizeX = new SizePanel();
            sizeX.Height.Set(panel.Height.Pixels, 0);
            sizeX.Width.Set(15, 0);

            sizeY.Left.Set(panel.Left.Pixels, 0);
            sizeX.Top.Set(panel.Top.Pixels, 0);
            sizeY.Top.Set(panel.Top.Pixels + panel.Height.Pixels, 0);
            sizeX.Left.Set(panel.Left.Pixels + panel.Width.Pixels, 0);

            textpanel = new recipeGrid();
            textpanel.OverflowHidden = true;

            panel.OverflowHidden = false;


            /*sizeYT = new UIText("awa", 1);
            sizeYT.Height.Set(50, 1);
            sizeYT.Width.Set(50, 1);
            sizeXT = new UIText("awa", 1);
            sizeXT.Height.Set(50, 1);
            sizeXT.Width.Set(50, 1);
            sizeY.Append(sizeYT);
            sizeX.Append(sizeXT);*/


            /*
            sizeY.OnClick += SizeY_OnMouseDown;
            sizeY.OnMouseOut += SizeY_OnMouseUp;
            sizeX.OnClick += SizeX_OnMouseDown;
            sizeX.OnMouseOut += SizeX_OnMouseUp;
            */


            //area.Append(panel);
            //grid.Append(scroll);
            Append(dragbarbar);
            textpanel.Append(recipes);
            textpanel.Height.Set(panel.Height.Pixels, 0);
            textpanel.Width.Set(panel.Width.Pixels, 0);
            panel.Append(textpanel);
            //panel.Append(grid);
            Append(sizeY);
            Append(sizeX);
            dragbar.Append(panel);
            Append(dragbar);
        }

        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //scroll.SetView(0,200);

            panel.maxScroll = recipes.GetDimensions().Height;

            float posit = panel.scroll;

            //recipes.SetText(posit.ToString() + "\n" + recipes.GetDimensions().Height + "\na\nb\nc\nd\ne\nf\ng\nh\ni\nj\nk\nl");

            recipes.Top.Set(posit * -1, 0);

            if (sizeY.dragging)
            {
                float size;
                if (Main.mouseY - (dragbar.Top.Pixels) - 1 >= 100)
                {
                    size = Main.mouseY - (dragbar.Top.Pixels) -1;
                }
                else
                {
                    size = 100;
                }
                panel.Height.Set(size, 0);
                dragbar.Height.Set(size, 0);
                //sizeYT.SetText(size.ToString());
                dragbar.Recalculate();
            }
            if (sizeX.dragging)
            {
                float size;
                if (Main.mouseX - (dragbar.Left.Pixels) - 1 >= 100)
                {
                    size = Main.mouseX - (dragbar.Left.Pixels) - 1;
                }
                else
                {
                    size = 100;
                }
                panel.Width.Set(size, 0);
                dragbar.Width.Set(size, 0);
                //sizeXT.SetText(size.ToString());
                dragbar.Recalculate();
            }

            //sizeYT.SetText(sizeY.dragging.ToString());
            //sizeXT.SetText(sizeX.dragging.ToString());
            textpanel.Height.Set(panel.Height.Pixels, 0);
            textpanel.Width.Set(panel.Width.Pixels, 0);
            sizeY.Left.Set(dragbar.GetDimensions().X, 0);
            sizeX.Top.Set(dragbar.GetDimensions().Y, 0);
            sizeY.Top.Set(dragbar.GetDimensions().Y + panel.Height.Pixels, 0);
            sizeX.Left.Set(dragbar.GetDimensions().X + panel.Width.Pixels, 0);
            sizeY.Width.Set(dragbar.Width.Pixels + sizeX.Width.Pixels, 0);
            sizeX.Height.Set(dragbar.Height.Pixels + sizeY.Height.Pixels, 0);
            Recalculate();
            
        }

        public void AddRecipes(List<Recipe> FoundRecipes)
        {
            recipes.RemoveAllChildren();
            int separator = 10;
            int index = 0;
            foreach (UIPanel indexx in indexes)
            {
                indexx.Remove();
            }
            foreach (Recipe Recipe in FoundRecipes)
            {
                UIPanel reci = new UIPanel();
                String formtext = "";
                foreach (Item item in Recipe.requiredItem)
                {
                    if (item.netID == 0)
                    {
                        continue;
                    }
                    formtext += "[I/s" + item.stack + ":" + item.netID + "] ";
                }
                UIText recitext = new UIText(formtext,1);
                UIPanel button = new UIPanel();

                button.Left.Set(-30, 0);
                button.Top.Set(0, 0);
                button.Height.Set(30, 0);
                button.Width.Set(30, 0);

                recitext.Top.Set(separator, 0);
                recitext.Height.Set(30, 0);
                recitext.Width.Set(30, 0);

                TextButton buttontext = new TextButton(index, button, panel, Recipe, Main.mouseItem);

                indexes.Add(buttontext);

                recitext.Append(button);
                recipes.Append(recitext);

                

                Append(buttontext);

                index++;
                separator += 30;
            }
            recipes.Height.Set(separator, 0);
        }

        public void Button(UIMouseEvent evt, UIText button)
        {
            
        }
    }
    public class SizePanel : UIElement
    {
        public bool dragging = false;

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            dragstart(evt);
        }
        public override void MouseUp(UIMouseEvent evt)
        {
            base.MouseUp(evt);
            dragend(evt);
        }

        private void dragstart(UIMouseEvent evt)
        {
            dragging = true;
        }

        private void dragend(UIMouseEvent evt)
        {
            dragging = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }

    public class TextButton : UIPanel
    {
        public UIElement parentPanel;

        public event ButtonEventHandler Uncraft;

        public mainPanel mainPanel;

        public Recipe recipe;

        public int index;

        public Item item;

        public UIText test;

        public delegate void ButtonEventHandler(UIMouseEvent evt, UIElement parent,TextButton obj, Recipe selfRecipe, Item item, EventArgs e);

        public TextButton(int index, UIElement parent, mainPanel mainPanel, Recipe selfRecipe, Item item) : base()
        {
            //this.SetText(text,textScale,large);
            parentPanel = parent;
            this.mainPanel = mainPanel;
            this.index = index;
            recipe = selfRecipe;
            this.item = item;
        }

        

        /*public override void OnInitialize()
        {
            base.OnInitialize();
            Height.Set(30, 0);
            Width.Set(30, 0);

            OnMouseDown += SelfMouseDown;
            OnMouseOut += SelfMouseOut;
            OnMouseOver += SelfMouseOver;

            test = new UIText(index.ToString());
            test.Height.Set(300, 0);
            test.Width.Set(300, 0);
            Append(test);

            Debug.WriteLine("Button " + index.ToString() + " added");
        }*/

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Height.Set(30, 0);
            Width.Set(30, 0);
            Top.Set(parentPanel.GetDimensions().Y, 0);
            Left.Set(parentPanel.GetDimensions().X - 50, 0);
        }

        

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            //Debug.WriteLine("MouseDown on button " + index.ToString());
            if (Main.mouseItem != null)
            {
                Uncraft(evt, parentPanel, this, recipe, Main.mouseItem, null);

            }
            else
            {
                Main.NewText("you need to be holding your item");
            }
        }
    }

    public class mainPanel : UIPanel
    {
        public float scroll = 0;
        public float maxScroll = 0;

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            if (evt.ScrollWheelValue > 0)
            {
                if (scroll + 5 > maxScroll)
                {

                    scroll = maxScroll;
                }
                else
                { 
                    scroll += 5;
                }
            }
            else
            {
                if (scroll - 5 < 0)
                {
                    scroll = 0;
                }
                else
                {
                    scroll -= 5;
                }
            }
        }
    }

    public class recipeGrid : UIPanel
    {
        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            return;
        }
    }
}
