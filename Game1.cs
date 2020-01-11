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

    public class MapInfoObject
    {

        public string mapType;
        public int windowWidth;
        public int windowHeight;
        public int gridSizeX;
        public int gridSizeY;
        public int tileSize;
        public MapInfoObject()
        {

        }

    }
    public class TileObject
    {

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

        ContextMenuComponent tileContextMenu;
        MenuComponent menuComponent;
        TextureMenuComponent textureMenuComponent;
        MapMenuComponent mapMenuComponent;
        KeyboardState previousState;
        TopBarMenuComponent topBarMenuComponent;
        public bool isMenuEnabled;
        Texture2D selectedTexture;
        public bool isTextureMenuEnabled;
        public bool isMapMenuEnabled;
        GridMapManager gridMapManager;

        MapInfoObject mapInfoObject;
        public System.Collections.Generic.List<TileObject> barriersList = new System.Collections.Generic.List<TileObject>();

        //private Dictionary<int,Rectangle> tilesDictionary = new Dictionary<int,Rectangle>();

        public Game1()
        {
            mapInfoObject = new MapInfoObject();
            graphics = new GraphicsDeviceManager(this);
            changeWindowSize(800, 600);
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            mapInfoObject.windowHeight = 600;
            mapInfoObject.windowWidth = 800;

        }
        private void changeWindowSize(int x, int y)
        {
            graphics.PreferredBackBufferHeight = y;
            graphics.PreferredBackBufferWidth = x;
            mapInfoObject.windowWidth = x;
            mapInfoObject.windowHeight = y;
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            store = new LevelStore();
            camera = new Camera2d();
            camera.Pos = new Vector2(0.0f, 200.0f);
            isMenuEnabled = false;
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
            mainMenuFont = Content.Load<SpriteFont>("MainMenu");
            gridMapManager = new GridMapManager(this, spriteBatch, barriersList, verdana36, pixel);
            gridMapManager.gridMapType = GridMapManager.GridMapType.Default;
            mapInfoObject.gridSizeX = 10;
            mapInfoObject.gridSizeY = 10;
            mapInfoObject.tileSize = 50;
            gridMapManager.drawGridSystem(mapInfoObject, camera, graphics);
            mapInfoObject.mapType = "Default";
            string[] menuItems = { "Save Level", "Load Level", "Import Texture", "Level Layout Settings", "Quit" };
            mapMenuComponent = new MapMenuComponent(this, graphics);
            mapMenuComponent.setPositionOfMenu(new Vector2(camera._pos.X / 2, camera._pos.Y / 2));
            menuComponent = new MenuComponent(this, spriteBatch, mainMenuFont, menuItems);
            textureMenuComponent = new TextureMenuComponent(this, spriteBatch, mainMenuFont, menuItems);
            menuComponent.LevelLoadedFromJson += new levelLoadedEventHandler(onLevelLoaded);
            menuComponent.LevelSavedToJson += new levelSavedEventHandler(onLevelSaved);
            textureMenuComponent.textureLoadedToUse += new loadedTextureToUseHandler(onUseTexture);
            mapMenuComponent.changedGridMapToUse += new gridMapDimensionsChangeHandler(onGridMapDimensionChange);

            Components.Add(menuComponent);
            Components.Add(textureMenuComponent);
            Components.Add(mapMenuComponent);
            MyraEnvironment.Game = this;
            topBarMenuComponent = new TopBarMenuComponent(mapMenuComponent, gridMapManager, this, camera);
            topBarMenuComponent.BuildUITopBar();
            topBarMenuComponent.topBarMenuMapTypeEventHandler += new topBarMenuChangeMapType(onMapTypeChange);
            tileContextMenu = new ContextMenuComponent(this, graphics.GraphicsDevice, camera);
            Desktop.TouchDown += (s, a) =>
            {
                if (Desktop.DownKeys.Contains(Keys.LeftControl) || Desktop.DownKeys.Contains(Keys.RightControl))
                {
                    tileContextMenu.ShowContextMenu();

                }
                if (Desktop.DownKeys.Contains(Keys.Escape))
                {
                    if (mapMenuComponent.IsGridResizeMenuEnabled())
                    {
                        mapMenuComponent.RemoveGridResizingMenu();
                    }

                }
            };
            // Desktop.TouchDown += (s, a) => ShowContextMenu();
        }
        public void onMapTypeChange(String msg)
        {


            if (gridMapManager.gridMapType == GridMapManager.GridMapType.Default)
            {
                mapInfoObject.mapType = "Default";
                gridMapManager.drawGridSystem(mapInfoObject, camera, graphics);
            }
            if (gridMapManager.gridMapType == GridMapManager.GridMapType.Isometric)
            {
                mapInfoObject.mapType = "Isometric";
                gridMapManager.drawIsometricGridSystem(mapInfoObject, camera, graphics);
            }

        }
        public void onGridMapDimensionChange(int width, int height, int tileSize)
        {
            mapInfoObject.gridSizeX = height;
            mapInfoObject.gridSizeY = width;
            mapInfoObject.tileSize = tileSize;
            if (gridMapManager.gridMapType == GridMapManager.GridMapType.Default)
            {
                mapInfoObject.mapType = "Default";
                gridMapManager.drawGridSystem(mapInfoObject, camera, graphics);
            }
            if (gridMapManager.gridMapType == GridMapManager.GridMapType.Isometric)
            {
                mapInfoObject.mapType = "Isometric";
                gridMapManager.drawIsometricGridSystem(mapInfoObject, camera, graphics);
            }

            mapMenuComponent.RemoveGridResizingMenu();
        }
        public void onUseTexture(Texture2D texture, string pathToTexture)
        {
            selectedTexture = texture;
            selectedTexturePath = pathToTexture;
            isTextureMenuEnabled = false;
        }
        public void onLevelSaved()
        {
            System.Console.WriteLine("Level Saved");
            store.storeLevelAsJSON(barriersList, mapInfoObject, menuComponent.pathToLevel);
            isMenuEnabled = false;
        }
        public void onLevelLoaded(List<TileObject> jsonLevel, MapInfoObject mapInfo)
        {
            Console.WriteLine("Level loaded");
            mapInfoObject = mapInfo;
            gridMapManager.drawGridSystem(mapInfoObject, camera, graphics);
            barriersList = jsonLevel;
            gridMapManager.barriersList=barriersList;
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(graphics.GraphicsDevice));
            gridMapManager.updateGridSystem(selectedTexture, camera);
            spriteBatch.End();
            isMenuEnabled = false;
        }

        public Vector2 ScreenToWorld(int x, int y)
        {
            return new Vector2(camera.Pos.X - x, camera.Pos.Y - y);
        }

        public bool isTileClicked(int mouseX, int mouseY)
        {
            bool didHitTile = false;
            Vector2 worldPosition = Vector2.Transform(new Vector2(mouseX, mouseY), Matrix.Invert(camera._transform));
            foreach (TileObject rect in barriersList)
            {


                if (rect.Rectangle.Contains(worldPosition))
                {

                    //spriteBatch.Begin(SpriteSortMode.BackToFront,BlendState.AlphaBlend,null,null,null,null,camera.get_transformation(graphics.GraphicsDevice));
                    //   spriteBatch.Begin();
                    // System.Console.WriteLine("Hit rect at {0} {1}",mouseState.X,mouseState.Y);
                    didHitTile = true;
                    //    spriteBatch.End();
                }
            }

            return didHitTile;

        }
        public void checkForMouseClick()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                //Write code here
                var mouseState = Mouse.GetState();
                var mousePosition = new Point(mouseState.X, mouseState.Y);
                Vector2 worldPosition = Vector2.Transform(new Vector2(mouseState.X, mouseState.Y), Matrix.Invert(camera._transform));
                System.Console.WriteLine("Mouse position X:{0} Y:{1}", mouseState.X, mouseState.Y);
                System.Console.WriteLine("World position X:{0} Y:{1}", worldPosition.X, worldPosition.Y);
                //System.Console.WriteLine("Tere {0},{1}",mouseState.X,mouseState.Y);
                //System.Console.WriteLine("Tere count {0}",barriersList.Count);
                gridMapManager.checkAndHighlightSelectedTile(worldPosition, selectedTexture, selectedTexturePath);

            }

        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (mapMenuComponent.IsGridResizeMenuEnabled())
                {
                    mapMenuComponent.RemoveGridResizingMenu();
                }
                else
                {
                    // Exit();
                }

            }

            KeyboardState ks = Keyboard.GetState();

            // TODO: Add your update logic here
            if (!isTextureMenuEnabled)
            {
                if (ks.IsKeyDown(Keys.Left) & !previousState.IsKeyDown(
                Keys.Left))
                {
                    camera.Move(new Vector2(-100.0f, 0));
                }
                if (ks.IsKeyDown(Keys.Right) & !previousState.IsKeyDown(
                Keys.Right))
                {
                    camera.Move(new Vector2(100.0f, 0));
                }
                if (ks.IsKeyDown(Keys.Z) & !previousState.IsKeyDown(
                Keys.Z))
                {
                    camera.Zoom = 0.5f;
                }
                if (ks.IsKeyDown(Keys.T) & !previousState.IsKeyDown(
                Keys.T))
                {
                    if (isTextureMenuEnabled)
                    {
                        isTextureMenuEnabled = false;
                    }
                    else
                    {
                        isTextureMenuEnabled = true;
                    }

                }
                if (ks.IsKeyDown(Keys.LeftControl) && ks.IsKeyDown(Keys.S))
                {
                    // Move backward
                    store.storeLevelAsJSON(barriersList, mapInfoObject, menuComponent.pathToLevel);
                }
                if (ks.IsKeyDown(Keys.Escape) & !previousState.IsKeyDown(
                Keys.Escape))
                {
                    System.Console.WriteLine("Menu");
                    if (isMenuEnabled)
                    {
                        isMenuEnabled = false;
                    }
                    else
                    {
                        isMenuEnabled = true;
                    }
                }
            }
            else
            {
                if (isTextureMenuEnabled)
                {
                    if (ks.IsKeyDown(Keys.T) & !previousState.IsKeyDown(
                     Keys.T))
                    {
                        if (isTextureMenuEnabled)
                        {
                            isTextureMenuEnabled = false;
                        }
                        else
                        {
                            isTextureMenuEnabled = true;
                        }

                    }
                }
            }
            previousState = ks;
            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            // drawGridSystem(10,10);


            //Vector2 worldPosition = Vector2.Transform(mousePosition, Matrix.Invert(viewMatrix));
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(graphics.GraphicsDevice));
            //      spriteBatch.End();
            //  camera.Zoom = 0.5f;
            // camera.Move(new Vector2(500.0f,200.0f));
            if (isTextureMenuEnabled)
            {
                textureMenuComponent.setMenuPosition(new Vector2(camera._pos.X - 120, camera._pos.Y / 2));
                textureMenuComponent.DrawMenu();
            }
            else if (isMenuEnabled)
            {
                menuComponent.setMenuPosition(new Vector2(camera._pos.X - 120, camera._pos.Y / 2));
                menuComponent.DrawMenu();
                // Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                //                           texture.SetData<Color>(new Color[] { Color.Gray });


                //                            spriteBatch.Draw(texture,new Rectangle(0,0,500,200), Color.Green);

            }
            else if (mapMenuComponent.IsGridResizeMenuEnabled())
            {
                mapMenuComponent.DrawBackground();
            }
            else
            {
                gridMapManager.barriersList=barriersList;
                gridMapManager.updateGridSystem(selectedTexture, camera);
                checkForMouseClick();
            }


            base.Draw(gameTime);

            spriteBatch.End();
            Desktop.Render();

        }
    }
}