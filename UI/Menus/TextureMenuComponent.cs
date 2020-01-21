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

namespace ScreenManager
{
    public delegate void loadedTextureToUseHandler(Texture2D texture, string pathToTexture);

    public class TextureMenuComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public event loadedTextureToUseHandler textureLoadedToUse;
        IEnumerable<string> texturePaths;
        string[] menuItems;
        int selectedIndex;
        public string pathToContent;
        Color normal = Color.White;
        Color hilite = Color.Yellow;

        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        Vector2 position;
        float width = 0f;
        float height = 0f;
		int start=0;
		int maxItems=20;
		int end=20;
        Game1 game;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                if (selectedIndex <= 0)
                    selectedIndex = 0;
                if (selectedIndex >= textures.Count)
                    selectedIndex = textures.Count - 1;
            }
        }
        public void setMenuPosition(Vector2 posIn)
        {
            position = posIn;
        }
        public TextureMenuComponent(Game1 game,
            SpriteBatch spriteBatch,
            SpriteFont spriteFont,
            string[] menuItems)
            :base(game)
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
            this.menuItems = menuItems;
            this.game=game;
            MeasureMenu();
            selectedIndex=0;
            pathToContent = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Content\\Texture";

            texturePaths = Directory.EnumerateFiles(pathToContent, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".png") || s.EndsWith(".PNG"));
            loadTextures(150);



        }

        private void loadTextures(int count){
          
            foreach (string dir in texturePaths)
            {
                if (count > 0)
                {
                    Texture2D text = loadNotBuiltTextureFile(dir);
                    if(!textures.ContainsKey(dir)){
                         textures.Add(dir, text);
                    }
                   
                    System.Console.WriteLine(dir);
                    count--;
                }
            }
        }

        public static void SavePicture(string Filename, Texture2D TextureToSave)
        {
            FileStream setStream = File.Open(Filename, FileMode.Create);
            StreamWriter writer = new StreamWriter(setStream);
            TextureToSave.SaveAsPng(setStream, TextureToSave.Width, TextureToSave.Height);
            setStream.Dispose();
        }
        /*public static Texture2D LoadTexture(string name)
        {
            Texture2D texture;
            if (textures.TryGetValue(name, out texture) == true)
            {
                return texture;
            }
            else
            {
            
                texture = Content.Load<Texture2D>(@"Textures\" + name);
                textures.Add(name, texture);
                return texture;
            }
        }*/
        private Texture2D loadNotBuiltTextureFile(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            Texture2D spriteAtlas = Texture2D.FromStream(GraphicsDevice, fileStream);
            fileStream.Dispose();
            return spriteAtlas;
        }

        private void MeasureMenu()
        {
            height = 0;
            width = 0;
            foreach (string item in menuItems)
            {
                Vector2 size = spriteFont.MeasureString(item);
                if (size.X > width)
                    width = size.X;
                height += spriteFont.LineSpacing + 5;
            }

            position = new Vector2(

                (Game.Window.ClientBounds.Width - width) / 2,
                (Game.Window.ClientBounds.Height - height) / 2);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        private bool CheckKey(Keys theKey)
        {
            return keyboardState.IsKeyUp(theKey) &&
                oldKeyboardState.IsKeyDown(theKey);
        }

        public override void Update(GameTime gameTime)
        {
            if(game.editorMenuState==EditorMenuState.TextureMenu){
          
            keyboardState = Keyboard.GetState();

            if (CheckKey(Keys.Right))
            {
                selectedIndex++;
                if (selectedIndex == textures.Count())
                    selectedIndex = 0;
            }
            if (CheckKey(Keys.Left))
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = textures.Count() - 1;
            }
            if (CheckKey(Keys.Escape))
            {
                System.Console.WriteLine("ESCAPE");
                game.editorMenuState=EditorMenuState.Editor;
            }
            if (CheckKey(Keys.Down))
            {
               
                if (start + 20 <= textures.Count-20){
                    selectedIndex=selectedIndex+20;
                    start=start+20;
                }
                   

                if (end + 20 <= textures.Count)
                    end=end+20;
                    
            }

            if (CheckKey(Keys.Up))
            {
               
                if (start - 20 >= 0){
                         selectedIndex=selectedIndex-20;
                         start=start-20;
                    }
                   

                if (end - 20 >= maxItems)
                    if(end-20>0){
                        end=end-20;
                    }
                   
            }

            if (CheckKey(Keys.Enter))
            {
                textureLoadedToUse(textures.ElementAt(selectedIndex).Value, textures.ElementAt(selectedIndex).Key);

            }
            }
            base.Update(gameTime);

            oldKeyboardState = keyboardState;

        }

        private void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White }); // so that we can draw whatever color we want on top of it

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
        public void DrawMenu()
        {
            Vector2 location = position;
            Color tint;
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.Gray });


            spriteBatch.Draw(texture, new Rectangle((int)location.X, (int)location.Y, 300, 250), Color.LightSlateGray);
            int currentTileX = (int)location.X;
            int currentTileY = (int)location.Y;

			for(int i=start;i<end;i++){
				KeyValuePair<string, Texture2D> entry=textures.ElementAt(i);
			    spriteBatch.Draw(entry.Value, new Rectangle(currentTileX, currentTileY, 50, 50), Color.White);
                currentTileX = currentTileX + 50;
                if (currentTileX > location.X + 250)
                {
                    currentTileX = (int)location.X;
                    currentTileY = currentTileY + 50;
                }
                if (textures.ElementAt(selectedIndex).Key == entry.Key)
                {
                    DrawBorder(new Rectangle(currentTileX, currentTileY, 50, 50), 2, Color.Red);
                }

			}

           /* foreach (KeyValuePair<string, Texture2D> entry in textures)
            {

                spriteBatch.Draw(entry.Value, new Rectangle(currentTileX, currentTileY, 50, 50), Color.White);
                currentTileX = currentTileX + 50;
                if (currentTileX > location.X + 250)
                {
                    currentTileX = (int)location.X;
                    currentTileY = currentTileY + 50;
                }
                if (textures.ElementAt(selectedIndex).Key == entry.Key)
                {
                    DrawBorder(new Rectangle(currentTileX, currentTileY, 50, 50), 2, Color.Red);
                }

            }*/

        }
    }
}
