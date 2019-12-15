using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Xml;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using ScreenManager;
using System.Collections.Generic;
using Myra;
using Myra.Graphics2D.UI;
using System.Linq;
using Myra.Graphics2D.TextureAtlases;

namespace monoGameCP
{

    public class TileObject{

        private Rectangle rectangle;
        public bool isGreen;
        public string label_text;
        public string texturePath;
        public bool isCollidable;
        public bool isTextureAdded;
         [JsonIgnore]
        public Texture2D texture;
        public Rectangle Rectangle { get => rectangle; set => rectangle = value; }

        public TileObject()
        {
            
        }

   
        
    }


    public class Game1 : Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D pixel;
        SpriteFont verdana36;
        SpriteFont mainMenuFont;
        XmlTextWriter textWriter; 
        string selectedTexturePath;
        LevelStore store;
        Camera2d camera;
        MenuComponent menuComponent;
        TextureMenuComponent textureMenuComponent;
        MapMenuComponent mapMenuComponent; 
        KeyboardState previousState;
        bool isMenuEnabled;
        Texture2D selectedTexture;
        bool isTextureMenuEnabled;
        
        
        public System.Collections.Generic.List<TileObject> barriersList = new System.Collections.Generic.List<TileObject>();
      
        //private Dictionary<int,Rectangle> tilesDictionary = new Dictionary<int,Rectangle>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            changeWindowSize(800,600);
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        private void changeWindowSize(int x,int y){
            graphics.PreferredBackBufferHeight=y;
            graphics.PreferredBackBufferWidth=x;
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            store=new LevelStore();
            camera=new Camera2d();
            camera.Pos=new Vector2(0.0f,200.0f);
            isMenuEnabled=false;
            previousState = Keyboard.GetState();
            base.Initialize();
        }
        /* All the game’s logic should go in the Update() method while everything concerning what is showing on the screen should go in the Draw() method. So a game doesn’t really have an implementation for menus and pausing, it’s up to the developer to create something that will take care of that.*/
        //http://www.spikie.be/building-a-main-menu-and-loading-screens-in-xna.html
        //https://www.gamedev.net/forums/topic/604607-menus-in-xna/

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White }); // so that we can draw whatever color we want on top of it
            // TODO: use this.Content to load your game content here
             verdana36 = Content.Load<SpriteFont>("File");
             mainMenuFont=Content.Load<SpriteFont>("MainMenu");
            drawGridSystem(10,10,50);

            string[] menuItems={"Save Level","Load Level","Import Texture","Level Layout Settings","Quit"};
            mapMenuComponent=new MapMenuComponent(this);
            menuComponent=new MenuComponent(this,spriteBatch,mainMenuFont,menuItems);
            textureMenuComponent= new TextureMenuComponent(this,spriteBatch,mainMenuFont,menuItems);
            menuComponent.LevelLoadedFromJson += new levelLoadedEventHandler(onLevelLoaded);
            menuComponent.LevelSavedToJson +=new levelSavedEventHandler(onLevelSaved);
            textureMenuComponent.textureLoadedToUse +=new loadedTextureToUseHandler(onUseTexture);
            mapMenuComponent.changedGridMapToUse+=new gridMapDimensionsChangeHandler(onGridMapDimensionChange);
            Components.Add(menuComponent);
            Components.Add(textureMenuComponent);
            Components.Add(mapMenuComponent);
            MyraEnvironment.Game = this;

             BuildUITopBar();
             Desktop.TouchDown +=(s,a)=>{
				if (Desktop.DownKeys.Contains(Keys.LeftControl) || Desktop.DownKeys.Contains(Keys.RightControl))
				{
					ShowContextMenu();
					
				}
                if (Desktop.DownKeys.Contains(Keys.Escape) )
				{
					if(mapMenuComponent.IsGridResizeMenuEnabled()){
                       mapMenuComponent.RemoveGridResizingMenu();
                    }
					
				}
			};
            // Desktop.TouchDown += (s, a) => ShowContextMenu();
        }

        public void onGridMapDimensionChange(int width,int height,int tileSize){
          drawGridSystem(width,height,tileSize);
        }
         public void onUseTexture(Texture2D texture,string pathToTexture){
           selectedTexture=texture;
           selectedTexturePath=pathToTexture;
        }
        public void onLevelSaved(){
            System.Console.WriteLine("Level Saved");
             store.storeLevelAsJSON(barriersList,menuComponent.pathToLevel);
             isMenuEnabled=false;
        }
       public void onLevelLoaded(List<TileObject> jsonLevel)
        {
            Console.WriteLine("Level loaded");
            barriersList=jsonLevel;
            isMenuEnabled=false;
        }

        public void drawGridSystem(int gridSizeX,int gridSizeY,int tileSize){
         spriteBatch.Begin();
            for (int i = 0; i < gridSizeX; ++i)
{
                    for (int j = 0; j < gridSizeY; ++j)
                    {
                           Vector2 transformedV=Vector2.Transform(new Vector2(j*tileSize,i*tileSize), Matrix.Invert(camera.get_transformation(graphics.GraphicsDevice)));
                           var tile=new Rectangle((int)transformedV.X,(int)transformedV.Y,tileSize,tileSize);
                           TileObject tile2=new TileObject();
                           tile2.Rectangle=tile;
                           tile2.isGreen=false;
                           barriersList.Add(tile2);
                           DrawBorder(tile,2,Color.Red);
                           var positionsLabel="("+tile2.Rectangle.X+":"+tile2.Rectangle.Y+")";
                                            spriteBatch.DrawString(verdana36,positionsLabel, new Vector2(tile2.Rectangle.X, tile2.Rectangle.Y), Color.White);
                    }
            }
            spriteBatch.End();
        }


          public void updateGridSystem(){
 //spriteBatch.Begin();
            foreach(TileObject rect in barriersList){

                         
                           if(rect.isGreen){
                               if(selectedTexture!=null){
                                   if(rect.texture!=null){
                                        spriteBatch.Draw(rect.texture, rect.Rectangle, Color.Green);
                                   }
                                          
                               }else{
                                Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                                         texture.SetData<Color>(new Color[] { Color.White });
                                            spriteBatch.Draw(texture, rect.Rectangle, Color.Green);
                                           Vector2 worldPos = Vector2.Transform(new Vector2(rect.Rectangle.X,rect.Rectangle.Y), Matrix.Invert(camera._transform));
                                              var positionsLabel="("+rect.Rectangle.X+":"+rect.Rectangle.Y+")\n("+worldPos.X+":"+worldPos.Y+")";
                                            spriteBatch.DrawString(verdana36,positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White);
                               }
                           }else{
                             DrawBorder(rect.Rectangle,2,Color.Red);
                                               Vector2 worldPos = Vector2.Transform(new Vector2(rect.Rectangle.X,rect.Rectangle.Y), Matrix.Invert(camera._transform));
                                              var positionsLabel="("+rect.Rectangle.X+":"+rect.Rectangle.Y+")\n("+worldPos.X+":"+worldPos.Y+")";
                                            spriteBatch.DrawString(verdana36,positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White);
                           }
                           
                          
                          
                         

            }
                           
                      
                    
            
           // spriteBatch.End();
        }
        public Vector2 ScreenToWorld(int x, int y)
{
            return new Vector2(camera.Pos.X - x,camera.Pos.Y - y);
        }

        bool  isTileClicked(int mouseX,int mouseY){
            bool didHitTile=false;
            Vector2 worldPosition = Vector2.Transform(new Vector2(mouseX,mouseY), Matrix.Invert(camera._transform));
            foreach(TileObject rect in barriersList){

                               
                                 if(rect.Rectangle.Contains(worldPosition)){
                                      
                                       //spriteBatch.Begin(SpriteSortMode.BackToFront,BlendState.AlphaBlend,null,null,null,null,camera.get_transformation(graphics.GraphicsDevice));
                                     //   spriteBatch.Begin();
                                         // System.Console.WriteLine("Hit rect at {0} {1}",mouseState.X,mouseState.Y);
                                      didHitTile=true;
                                        //    spriteBatch.End();
                                 }
                    }
                                
                return didHitTile;

        }
        public void checkForMouseClick(){
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    //Write code here
                   var mouseState = Mouse.GetState();
                   var mousePosition = new Point(mouseState.X, mouseState.Y);
                   Vector2 worldPosition = Vector2.Transform(new Vector2(mouseState.X,mouseState.Y), Matrix.Invert(camera._transform));
                   System.Console.WriteLine("Mouse position X:{0} Y:{1}",mouseState.X,mouseState.Y);
                   System.Console.WriteLine("World position X:{0} Y:{1}",worldPosition.X,worldPosition.Y);
                   //System.Console.WriteLine("Tere {0},{1}",mouseState.X,mouseState.Y);
                   //System.Console.WriteLine("Tere count {0}",barriersList.Count);
                    foreach(TileObject rect in barriersList){

                               
                                 if(rect.Rectangle.Contains(worldPosition)){
                                      
                                       //spriteBatch.Begin(SpriteSortMode.BackToFront,BlendState.AlphaBlend,null,null,null,null,camera.get_transformation(graphics.GraphicsDevice));
                                     //   spriteBatch.Begin();
                                         // System.Console.WriteLine("Hit rect at {0} {1}",mouseState.X,mouseState.Y);
                                         System.Console.WriteLine("Hit");
                                         Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                                         texture.SetData<Color>(new Color[] { Color.White });
                                         rect.texture=selectedTexture;
                                         rect.texturePath=selectedTexturePath;
                                           rect.isGreen=true;
                                            spriteBatch.Draw(texture, rect.Rectangle, Color.Green);
                                        //    spriteBatch.End();
                                 }
                    }
                                

                }

        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)){
                 if(mapMenuComponent.IsGridResizeMenuEnabled()){
                    mapMenuComponent.RemoveGridResizingMenu();
                 }else{
                       // Exit();
                 }
                 
            }
               
                KeyboardState ks = Keyboard.GetState();
          
            // TODO: Add your update logic here
                if(!isTextureMenuEnabled){
                if(ks.IsKeyDown(Keys.Left) & !previousState.IsKeyDown(
                Keys.Left)){
                    camera.Move(new Vector2(-100.0f,0));
                }
                if(ks.IsKeyDown(Keys.Right) & !previousState.IsKeyDown(
                Keys.Right)){
                    camera.Move(new Vector2(100.0f,0));
                }
                if(ks.IsKeyDown(Keys.Z) & !previousState.IsKeyDown(
                Keys.Z)){
                    camera.Zoom=0.5f;
                }
                if(ks.IsKeyDown(Keys.T) & !previousState.IsKeyDown(
                Keys.T)){
                    if(isTextureMenuEnabled){
                        isTextureMenuEnabled=false;
                    }else{
                        isTextureMenuEnabled=true;
                    }
                  
                }
                if (ks.IsKeyDown(Keys.LeftControl) && ks.IsKeyDown(Keys.S)) {
                // Move backward
                   store.storeLevelAsJSON(barriersList,menuComponent.pathToLevel);
                }
                if(ks.IsKeyDown(Keys.M) & !previousState.IsKeyDown(
                Keys.M)){
                    System.Console.WriteLine("Menu");
                    if(isMenuEnabled){
                        isMenuEnabled=false;
                    }else{
                        isMenuEnabled=true;
                    }
                }
                }else{
                     if(ks.IsKeyDown(Keys.T) & !previousState.IsKeyDown(
                Keys.T)){
                    if(isTextureMenuEnabled){
                        isTextureMenuEnabled=false;
                    }else{
                        isTextureMenuEnabled=true;
                    }
                  
                }
                }
               previousState = ks;
            base.Update(gameTime);
            
        }
         private void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);
            
            // Draw left line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
           
            // TODO: Add your drawing code here
           // drawGridSystem(10,10);
            
           
//Vector2 worldPosition = Vector2.Transform(mousePosition, Matrix.Invert(viewMatrix));
            spriteBatch.Begin(SpriteSortMode.BackToFront,BlendState.AlphaBlend,null,null,null,null,camera.get_transformation(graphics.GraphicsDevice));
            //      spriteBatch.End();
              //  camera.Zoom = 0.5f;
            // camera.Move(new Vector2(500.0f,200.0f));
            if(isTextureMenuEnabled){
                textureMenuComponent.setMenuPosition(new Vector2(camera._pos.X-120,camera._pos.Y/2));
                textureMenuComponent.DrawMenu();
            }else if(isMenuEnabled){
                menuComponent.setMenuPosition(new Vector2(camera._pos.X-120,camera._pos.Y/2));
                menuComponent.DrawMenu();
            // Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
              //                           texture.SetData<Color>(new Color[] { Color.Gray });

                                           
                //                            spriteBatch.Draw(texture,new Rectangle(0,0,500,200), Color.Green);
                                           
            }else{
                updateGridSystem();
                checkForMouseClick();
            }
            
            
            base.Draw(gameTime);
           
            spriteBatch.End();
            Desktop.Render();
            
        }
    
     private void ShowContextMenu()
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
        menuItem1.Text = "Action grid";
        menuItem1.Selected += (s, a) =>
        {
            // "Start New Game" selected
      //  mapMenuComponent.ShowGridResizingMenu();

            
        };
        
        var menuItem2 = new MenuItem();
        menuItem2.Text = "Options";

        var menuItem3 = new MenuItem();
        menuItem3.Text = "Quit";

        var verticalMenu = new VerticalMenu();
        if(isTileClicked(Desktop.TouchPosition.X,Desktop.TouchPosition.Y)){
            verticalMenu.Items.Add(menuItem1);
            verticalMenu.Items.Add(menuItem2);
            verticalMenu.Items.Add(menuItem3);
        }else{
            verticalMenu.Items.Add(menuItem3);
        }
        container.Widgets.Add(verticalMenu);

        var mouseState = Mouse.GetState();
        var mousePosition = new Point(mouseState.X, mouseState.Y);
        //Vector2 worldPosition = Vector2.Transform(new Vector2(mouseState.X,mouseState.Y), Matrix.Invert(camera._transform));
        Vector2 worldPosition=Vector2.Transform(new Vector2(mouseState.X,mouseState.Y), Matrix.Invert(camera.get_transformation(graphics.GraphicsDevice)));
        Desktop.ShowContextMenu(container, Desktop.TouchPosition);
}
 private void BuildUITopBar()
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
        

            var _menuFile = new MenuItem();
			_menuFile.Id = "_menuFile";
			_menuFile.Text = "&File";
            _menuFile.Items.Add(menuItem1);

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