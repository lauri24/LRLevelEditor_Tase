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
      public delegate void topBarMenuAction(Texture2D texture,string pathToTexture);

    
           public class TopBarMenuComponent {
               public event topBarMenuAction topBarMenuEventHandler;
               private  MapMenuComponent mapMenuComponent;
               Game1 game;
            public TopBarMenuComponent(MapMenuComponent mapMenuComponentIn,Game1 gameIn){
               this.mapMenuComponent=mapMenuComponentIn;
               this.game=gameIn;
            }
            public void BuildUITopBar()
		    {
              if (Desktop.ContextMenu != null)
            {
                // Dont show if it's already shown
                return;
            }

            var container = new HorizontalStackPanel
            {
                Spacing = 4
            };
            

            var menuItem1 = new MenuItem();
            menuItem1.Text = "Resize grid";
            menuItem1.Selected += (s, a) =>
            {
                // "Start New Game" selected
            mapMenuComponent.ShowGridResizingMenu();

                
            };

              var menuItem2 = new MenuItem();
            menuItem2.Text = "Menu";
            menuItem2.Selected += (s, a) =>
            {
                // "Start New Game" selected
                
               game.isMenuEnabled=true;
                
            };

            var menuItem3= new MenuItem();
            menuItem3.Text = "Texture";
            menuItem3.Selected += (s, a) =>
            {
                // "Start New Game" selected
             
                game.isTextureMenuEnabled=true;
            };
        
        
        

            var _menuFile = new MenuItem();
			_menuFile.Id = "_menuFile";
			_menuFile.Text = "&File";
            _menuFile.Items.Add(menuItem1);
            _menuFile.Items.Add(menuItem2);
            _menuFile.Items.Add(menuItem3);
            var _menuEdit = new MenuItem();
			_menuEdit.Id = "_menuEdit";
			_menuEdit.Text = "&Edit";

            var _menuHelp = new MenuItem();
			_menuHelp.Id = "_menuHelp";
			_menuHelp.Text = "&Help";
			//_menuHelp.Items.Add(_menuItemAbout);

			var _mainMenu = new HorizontalMenu();
			_mainMenu.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
			_mainMenu.Id = "_mainMenu";
			_mainMenu.Items.Add(_menuFile);
			_mainMenu.Items.Add(_menuEdit);
			_mainMenu.Items.Add(_menuHelp);
            Desktop.Widgets.Add(_mainMenu);
            Desktop.ShowContextMenu(container,new Point(0,0));
           // Desktop.Widgets.Add(horizontalStackPanel1);
			
		}

    }
}
        
	

