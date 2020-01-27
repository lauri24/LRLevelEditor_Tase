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
      public delegate void topBarMenuAction(Texture2D texture,string pathToTexture);
      public delegate void topBarMenuChangeMapType(String msg);
    
           public class TopBarMenuComponent {
               public event topBarMenuAction topBarMenuEventHandler;
               public event topBarMenuChangeMapType topBarMenuMapTypeEventHandler;
               private  MapMenuComponent mapMenuComponent;
               private GridMapManager gridMapManager;
               Game1 game;
               Camera2d camera;
            public TopBarMenuComponent(MapMenuComponent mapMenuComponentIn,GridMapManager gridMapManagerIn,Game1 gameIn,Camera2d cameraIn){
               this.mapMenuComponent=mapMenuComponentIn;
               this.game=gameIn;
               this.camera=cameraIn;
               this.gridMapManager=gridMapManagerIn;
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
                  game.editorMenuState=EditorMenuState.MainMenu;
               game.isMenuEnabled=true;
                
            };

            var menuItem3= new MenuItem();
            menuItem3.Text = "Texture";
            menuItem3.Selected += (s, a) =>
            {
                // "Start New Game" selected
                game.editorMenuState=EditorMenuState.TextureMenu;
                game.isTextureMenuEnabled=true;
            };
        
        
        
            var menuItem4 = new MenuItem();
            menuItem4.Text = "Resize Window";
            menuItem4.Selected += (s, a) =>
            {
                // "Start New Game" selected
                mapMenuComponent.ShowWindowResizingMenu();

                
            };
        

            var _menuFile = new MenuItem();
			_menuFile.Id = "_menuFile";
			_menuFile.Text = "&File";
            _menuFile.Items.Add(menuItem1);
            _menuFile.Items.Add(menuItem2);
            _menuFile.Items.Add(menuItem3);
            _menuFile.Items.Add(menuItem4);


             var layer1 = new MenuItem();
			layer1.Id = "0_layer";
			layer1.Text = "0";
            layer1.Selected += (s, a) =>
            {
                // "Start New Game" selected
                this.game.currentLayerDepthLevel=0.0f;
                
            };

              var layer2 = new MenuItem();
			layer2.Id = "1_layer";
			layer2.Text = "1 Layer";
            layer2.Selected += (s, a) =>
            {
                // "Start New Game" selected
                  this.game.currentLayerDepthLevel=0.1f;
                
            };
              var layer3 = new MenuItem();
			layer3.Id = "2_layer";
			layer3.Text = "2 Layer";
            layer3.Selected += (s, a) =>
            {
                // "Start New Game" selected
                  this.game.currentLayerDepthLevel=0.2f;
                
            };
              var layer4 = new MenuItem();
			layer4.Id = "3_layer";
			layer4.Text = "3 Layer";
            layer4.Selected += (s, a) =>
            {
                // "Start New Game" selected
                  this.game.currentLayerDepthLevel=0.3f;
                
                
            };




            var _menuFileLayer = new MenuItem();
			_menuFileLayer.Id = "_LayerDepth";
			_menuFileLayer.Text = "&Layer Depth";
            _menuFileLayer.Items.Add(layer1);
            _menuFileLayer.Items.Add(layer2);
            _menuFileLayer.Items.Add(layer3);
            _menuFileLayer.Items.Add(layer4);
    
    
    
    
            var _menuEdit = new MenuItem();
			_menuEdit.Id = "_menuEdit";
			_menuEdit.Text = "&Edit";

    

             var _menuMapType = new MenuItem();
			_menuMapType.Id = "_menuMapType";
			_menuMapType.Text = "&Map Type";

            var _menuHelp = new MenuItem();
			_menuHelp.Id = "_menuHelp";
			_menuHelp.Text = "&Help";
			//_menuHelp.Items.Add(_menuItemAbout);

             var menuMapIsometric = new MenuItem();
            menuMapIsometric.Text = "Isometric";
            menuMapIsometric.Selected += (s, a) =>
            {
                // "Start New Game" selected
               // mapMenuComponent.ShowWindowResizingMenu();
               gridMapManager.gridMapType=GridMapManager.GridMapType.Isometric;
                topBarMenuMapTypeEventHandler("Isometric");
                
            };

            var menuMapDefault = new MenuItem();
            menuMapDefault.Text = "Default/Platformer";
            menuMapDefault.Selected += (s, a) =>
            {
                // "Start New Game" selected
               // mapMenuComponent.ShowWindowResizingMenu();
                 gridMapManager.gridMapType=GridMapManager.GridMapType.Default;
                 
                topBarMenuMapTypeEventHandler("Default");
            };
        
            _menuMapType.Items.Add(menuMapIsometric);
            _menuMapType.Items.Add(menuMapDefault);
			//_menuHelp.Items.Add(_menu

            var zoom160 = new MenuItem();
			zoom160.Id = "160";
			zoom160.Text = "160";
            zoom160.Selected += (s, a) =>
            {
                // "Start New Game" selected
                camera.Zoom = 1.6f;
            };

            var zoom120 =  new MenuItem();
			zoom120.Id = "120";
			zoom120.Text = "120";
            zoom120.Selected += (s, a) =>
            {
                // "Start New Game" selected
                camera.Zoom = 1.2f;
            };

			var zoom100 = new MenuItem();
			zoom100.Id = "100";
			zoom100.Text = "100";
            zoom100.Selected += (s, a) =>
            {
                // "Start New Game" selected
                camera.Zoom = 1.0f;
            };

            var zoom80 = new MenuItem();
			zoom80.Id = "80";
			zoom80.Text = "80";
            zoom80.Selected += (s, a) =>
            {
                // "Start New Game" selected
                 camera.Zoom = 0.8f;
            };

            var zoom50 = new MenuItem();
			zoom50.Id = "50";
			zoom50.Text = "50";
            zoom50.Selected += (s, a) =>
            {
                // "Start New Game" selected
                camera.Zoom = 0.5f;
            };

            var zoom20 =  new MenuItem();
			zoom20.Id = "20";
			zoom20.Text = "20";
            zoom20.Selected += (s, a) =>
            {
                // "Start New Game" selected
                camera.Zoom = 0.2f;
            };
            var _menuZoom = new MenuItem();
			_menuZoom.Id = "_menuZoom";
			_menuZoom.Text = "&Zoom";
            _menuZoom.Items.Add(zoom20);
            _menuZoom.Items.Add(zoom50);
            _menuZoom.Items.Add(zoom80);
            _menuZoom.Items.Add(zoom120);
            _menuZoom.Items.Add(zoom100);
            _menuZoom.Items.Add(zoom160);
          
			var _mainMenu = new HorizontalMenu();
			_mainMenu.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
			_mainMenu.Id = "_mainMenu";
			_mainMenu.Items.Add(_menuFile);
			_mainMenu.Items.Add(_menuEdit);
			_mainMenu.Items.Add(_menuHelp);
            _mainMenu.Items.Add(_menuZoom);
            _mainMenu.Items.Add(_menuMapType);
            _mainMenu.Items.Add(_menuFileLayer);
            Desktop.Widgets.Add(_mainMenu);
            Desktop.ShowContextMenu(container,new Point(0,0));
           // Desktop.Widgets.Add(horizontalStackPanel1);
			
		}

    }
}
        
	

