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
   
     public delegate void gridMapDimensionsChangeHandler(int width,int height,int tileSize);

  
    public class MapMenuComponent : Microsoft.Xna.Framework.DrawableGameComponent
	{
        Grid grid;
        Panel panel;
        public event gridMapDimensionsChangeHandler changedGridMapToUse;

		public MapMenuComponent(Game game)
			: base(game)
		{
			
              
            
		}

	
        public bool IsGridResizeMenuEnabled(){
            if(panel==null){
                return false;
            }
            return true;
        }
		public override void Initialize()
		{
			base.Initialize();
		}

	
      
		public override void Update(GameTime gameTime)
		{
			

			base.Update(gameTime);

			
		}
        public void RemoveGridResizingMenu(){
            if(panel!=null){
                 Desktop.Widgets.Remove(panel);
                 panel=null;
            }
           
        }

        /*public void  ShowGridResizingMenu(){
           
           
           
                var panel = new Panel();
                var paddedCenteredButton = new TextButton();
                paddedCenteredButton.Text = "Padded Centered Button";
                paddedCenteredButton.PaddingLeft = 8;
                paddedCenteredButton.PaddingRight = 8;
                paddedCenteredButton.PaddingTop = 8;
                paddedCenteredButton.PaddingBottom = 8;
                paddedCenteredButton.HorizontalAlignment = HorizontalAlignment.Center;
                paddedCenteredButton.VerticalAlignment = VerticalAlignment.Center;


            panel.Widgets.Add(paddedCenteredButton);
            Desktop.Widgets.Add(panel);

        }*/
        public void ShowGridResizingMenu(){
      
         if(panel==null){
            panel = new Panel();

                

        var helloWorld = new Label
        {
        Id = "label",
        Text = "Hello, World!"
        };

        helloWorld.PaddingLeft = 8;
        helloWorld.PaddingRight = 8;
        helloWorld.PaddingTop = 8;
        helloWorld.PaddingBottom = 8;
        helloWorld.HorizontalAlignment = HorizontalAlignment.Center;
        helloWorld.VerticalAlignment = VerticalAlignment.Center;



        panel.Widgets.Add(helloWorld);
   
      ;
        // ComboBox
        /*var combo = new ComboBox
        {
        GridColumn = 2,
        GridRow = 0
        };
        
        combo.Items.Add(new ListItem("Red", Color.Red));
        combo.Items.Add(new ListItem("Green", Color.Green));
        combo.Items.Add(new ListItem("Blue", Color.Blue));
        grid.Widgets.Add(combo);*/

        var gridWidthButton = new SpinButton
        {
        GridColumn = 1,
        GridRow = 0,
        Width = 100,
        Nullable = true
        };
        var gridHeightButton = new SpinButton
        {
        GridColumn = 1,
        GridRow = 1,
        Width = 100,
        Nullable = true
        };
         var tileSizeButton = new SpinButton
        {
        GridColumn = 1,
        GridRow = 2,
        Width = 100,
        Nullable = true
        };


         gridWidthButton.PaddingLeft = 3;
        gridWidthButton.PaddingRight = 3;
        gridWidthButton.PaddingTop = 3;
        gridWidthButton.PaddingBottom = 3;
        gridWidthButton.Left = -30;
        gridWidthButton.Top = -100;
        gridWidthButton.HorizontalAlignment = HorizontalAlignment.Center;
        gridWidthButton.VerticalAlignment = VerticalAlignment.Center;

         gridHeightButton.PaddingLeft = 3;
         gridHeightButton.PaddingRight = 3;
         gridHeightButton.PaddingTop = 3;
         gridHeightButton.PaddingBottom = 3;
         gridHeightButton.Left = -150;
         gridHeightButton.Top = -20;
         gridHeightButton.HorizontalAlignment = HorizontalAlignment.Center;
         gridHeightButton.VerticalAlignment = VerticalAlignment.Center;

         tileSizeButton.PaddingLeft = 3;
        tileSizeButton.PaddingRight = 3;
        tileSizeButton.PaddingTop = 3;
        tileSizeButton.PaddingBottom = 3;
        tileSizeButton.Left = -200;
        tileSizeButton.Top = -100;
        tileSizeButton.HorizontalAlignment = HorizontalAlignment.Center;
        tileSizeButton.VerticalAlignment = VerticalAlignment.Center;


        panel.Widgets.Add(tileSizeButton);
        panel.Widgets.Add(gridHeightButton);
        panel.Widgets.Add(gridWidthButton);

        // Button
        var button = new TextButton
        {
        GridColumn = 0,
        GridRow = 3,
        Text = "Change Grid Size"
        };
        

        button.PaddingLeft = 3;
        button.PaddingRight = 3;
        button.PaddingTop = 3;
        button.PaddingBottom = 3;
        button.Left = -30;
        button.Top = -20;
        button.HorizontalAlignment = HorizontalAlignment.Center;
        button.VerticalAlignment = VerticalAlignment.Center;


        button.Click += (s, a) =>
        {
            var width=gridWidthButton.Value;//x
            var height=gridHeightButton.Value;//y
            var tileSize=tileSizeButton.Value;//tile
            changedGridMapToUse((int)width,(int)height,(int)tileSize);
           // drawGridSystem((int)width,(int)height,(int)tileSize);
      
        };
        panel.Widgets.Add(button);

       
         //var panel = new Panel();
         //panel.Widgets.Add(grid);

           Desktop.Widgets.Add(panel);
         }
        
    }
        
	}
}
