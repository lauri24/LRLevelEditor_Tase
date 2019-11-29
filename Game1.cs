using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Xml;
using System.Text;
using Newtonsoft.Json;
using System.IO;
namespace monoGameCP
{

    public class TileObject{

        private Rectangle rectangle;
        public bool isGreen;
        public string label_text;
        public string texturePath;
        public bool isCollidable;
        

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
        XmlTextWriter textWriter; 
        LevelStore store;
        bool isMenuEnabled;
        public System.Collections.Generic.List<TileObject> barriersList = new System.Collections.Generic.List<TileObject>();
      
        //private Dictionary<int,Rectangle> tilesDictionary = new Dictionary<int,Rectangle>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            store=new LevelStore();
            isMenuEnabled=false;
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
           
            drawGridSystem(100,50);
        }


        public void drawGridSystem(int gridSize,int tileSize){
 spriteBatch.Begin();
            for (int i = 0; i < gridSize; ++i)
{
                    for (int j = 0; j < gridSize; ++j)
                    {
                           var tile=new Rectangle(j*tileSize,i*tileSize,tileSize,tileSize);
                           TileObject tile2=new TileObject();
                           tile2.Rectangle=tile;
                           tile2.isGreen=false;
                           barriersList.Add(tile2);
                           DrawBorder(tile,2,Color.Red);
                      
                    }
            }
            spriteBatch.End();
        }


          public void updateGridSystem(){
 spriteBatch.Begin();
            foreach(TileObject rect in barriersList){

                           
                           if(rect.isGreen){
                                Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                                         texture.SetData<Color>(new Color[] { Color.White });

                                           
                                            spriteBatch.Draw(texture, rect.Rectangle, Color.Green);
                           }else{
                             DrawBorder(rect.Rectangle,2,Color.Red);
                           }
                          
                          
                         

            }
                           
                      
                    
            
            spriteBatch.End();
        }

        public void checkForMouseClick(){
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    //Write code here
                   var mouseState = Mouse.GetState();
                   var mousePosition = new Point(mouseState.X, mouseState.Y);
                   //System.Console.WriteLine("Tere {0},{1}",mouseState.X,mouseState.Y);
                   //System.Console.WriteLine("Tere count {0}",barriersList.Count);
                    foreach(TileObject rect in barriersList){

                               
                                 if(rect.Rectangle.Contains(mousePosition)){
                                        spriteBatch.Begin();
                                         // System.Console.WriteLine("Hit rect at {0} {1}",mouseState.X,mouseState.Y);
                                        System.Console.WriteLine("Hit");
                                         Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                                         texture.SetData<Color>(new Color[] { Color.White });

                                           rect.isGreen=true;
                                            spriteBatch.Draw(texture, rect.Rectangle, Color.Green);
                                            spriteBatch.End();
                                 }
                    }
                                
                        // Rectangle area = someRectangle;

// Check if the mouse position is inside the rectangle
                      /*  if (area.Contains(mousePosition))
                        {
                            backgroundTexture = hoverTexture;
                        }
                        else
                        {
                            backgroundTexture = defaultTexture;
                        }*/
                }

        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // TODO: Add your update logic here
              KeyboardState ks = Keyboard.GetState();
               
                if (ks.IsKeyDown(Keys.LeftControl) && ks.IsKeyDown(Keys.S)) {
                // Move backward
                   store.storeLevelAsJSON(barriersList,"level1.json");
                }
                if(ks.IsKeyDown(Keys.M)){
                    System.Console.WriteLine("Menu");
                    if(isMenuEnabled){
                        isMenuEnabled=false;
                    }else{
                        isMenuEnabled=true;
                    }
                }
              
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
            
            updateGridSystem();
            checkForMouseClick();
            if(isMenuEnabled){
                spriteBatch.Begin();
             Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                                         texture.SetData<Color>(new Color[] { Color.Gray });

                                           
                                            spriteBatch.Draw(texture,new Rectangle(0,0,500,200), Color.Green);
                                            spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}