



using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft;
using Newtonsoft.Json;
using monoGameCP;
using System.IO;
using System.Reflection;
using Myra;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.TextureAtlases;


namespace ScreenManager
{
 


    public class ContextMenuComponent
    {

        Game1 game;
        Camera2d camera;
        GraphicsDevice graphicsDevice;
        public ContextMenuComponent(Game1 gameIn,GraphicsDevice gDeviceIn,Camera2d cameraIn)
        {
            this.graphicsDevice=gDeviceIn;
            this.camera=cameraIn;
            this.game = gameIn;
        }
        public void ShowContextMenu(TileObject tile)
        {
            if (Desktop.ContextMenu != null)
            {
                // Dont show if it's already shown
                return;
            }

            var container = new VerticalStackPanel
            {
                Spacing = 4
            };

            var titleContainer = new Panel
            {
                Background = DefaultAssets.UISpritesheet["button"],
                Border = DefaultAssets.UISpritesheet["border"]
            };

            var titleLabel = new Label
            {
                Text = "Choose Option",
                HorizontalAlignment = HorizontalAlignment.Center
            };

            titleContainer.Widgets.Add(titleLabel);
            container.Widgets.Add(titleContainer);

            var menuItem1 = new MenuItem();
            menuItem1.Text = "Change levelDepth";
            menuItem1.Selected += (s, a) =>
            {
                // "Start New Game" selected
                //  mapMenuComponent.ShowGridResizingMenu();
                

            };

            var rotateAction = new MenuItem();
           rotateAction.Text = "Rotate";
           rotateAction.Selected += (s, a) =>
            {
                // "Start New Game" selected
                //  mapMenuComponent.ShowGridResizingMenu();
                tile.isRotated=true;
               if( tile.rotationAngle==MathHelper.Pi*2){
                   tile.rotationAngle=0;
               }else{
                   tile.rotationAngle+=MathHelper.PiOver2;
               }

            };


            var menuItem2 = new MenuItem();
            menuItem2.Text = "Options";

            var menuItem3 = new MenuItem();
            menuItem3.Text = "Quit";

            var verticalMenu = new VerticalMenu();
            if (game.isTileClicked(Desktop.TouchPosition.X, Desktop.TouchPosition.Y))
            {
                verticalMenu.Items.Add(menuItem1);
                verticalMenu.Items.Add(menuItem2);
                verticalMenu.Items.Add(menuItem3);
                verticalMenu.Items.Add(rotateAction);
            }
            else
            {
                verticalMenu.Items.Add(menuItem3);
            }
            container.Widgets.Add(verticalMenu);

            var mouseState = Mouse.GetState();
            var mousePosition = new Point(mouseState.X, mouseState.Y);
            //Vector2 worldPosition = Vector2.Transform(new Vector2(mouseState.X,mouseState.Y), Matrix.Invert(camera._transform));
            Microsoft.Xna.Framework.Vector2 worldPosition = Microsoft.Xna.Framework.Vector2.Transform(new Microsoft.Xna.Framework.Vector2(mouseState.X, mouseState.Y), Matrix.Invert(camera.get_transformation(graphicsDevice)));
            Desktop.ShowContextMenu(container, Desktop.TouchPosition);
        }

    }
}






