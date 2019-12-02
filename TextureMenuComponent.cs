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
    public delegate void loadedTextureToUseHandler(Texture2D texture);

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

		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				selectedIndex = value;
				if (selectedIndex < 0)
					selectedIndex = 0;
				if (selectedIndex >= textures.Count)
					selectedIndex = textures.Count - 1;
			}
		}
        public void setMenuPosition(Vector2 posIn){
            position=posIn;
        }
		public TextureMenuComponent(Game game, 
			SpriteBatch spriteBatch, 
			SpriteFont spriteFont, 
			string[] menuItems)
			: base(game)
		{
			this.spriteBatch = spriteBatch;
			this.spriteFont = spriteFont;
			this.menuItems = menuItems;
			MeasureMenu();
           
            pathToContent=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)+"\\Content\\Texture";

            texturePaths=Directory.EnumerateFiles(pathToContent, "*.*",SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".png") || s.EndsWith(".PNG"));
			int count=20;
            foreach (string dir in texturePaths) 
            {
				if(count>0){
                 Texture2D text=loadNotBuiltTextureFile(dir);
                 textures.Add(dir,text);
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
        private Texture2D loadNotBuiltTextureFile(string path){
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
			keyboardState = Keyboard.GetState();

			if (CheckKey(Keys.Right))
			{
				selectedIndex++;
				if (selectedIndex ==  texturePaths.Count())
					selectedIndex = 0;
			}
			if (CheckKey(Keys.Left))
			{
				selectedIndex--;
				if (selectedIndex < 0)
					selectedIndex = texturePaths.Count() - 1;
			}
            if(CheckKey(Keys.Q)){
                System.Console.WriteLine("ESCAPE");
            }

            if(CheckKey(Keys.Enter)){
               textureLoadedToUse(textures.ElementAt(selectedIndex).Value);
                 
            }

			base.Update(gameTime);

			oldKeyboardState = keyboardState;
		}

		/*public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			Vector2 location = position;
			Color tint;

			for (int i = 0; i < menuItems.Length; i++)
			{
				if (i == selectedIndex)
					tint = hilite;
				else
					tint = normal;
				spriteBatch.DrawString(
					spriteFont,
					menuItems[i],
					location,
					tint);
				location.Y += spriteFont.LineSpacing + 5;
			}
		}*/
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
        public void DrawMenu(){
            Vector2 location = position;
			Color tint;
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                                       texture.SetData<Color>(new Color[] { Color.Gray });

                                           
            spriteBatch.Draw(texture,new Rectangle((int)location.X,(int)location.Y,300,250), Color.LightSlateGray);
            int currentTileX=(int)location.X;
            int currentTileY=(int)location.Y;
            foreach (KeyValuePair<string, Texture2D> entry in textures) 
            {
               
               spriteBatch.Draw(entry.Value,new Rectangle(currentTileX,currentTileY,50,50), Color.LightSlateGray);
               currentTileX=currentTileX+50;
               if(currentTileX>location.X+250){
                   currentTileX=(int)location.X;
                   currentTileY=currentTileY+50;
               }
               if(textures.ElementAt(selectedIndex).Key==entry.Key){
                    DrawBorder(new Rectangle(currentTileX,currentTileY,50,50),2,Color.Red);
               }

            }
        
        }
	}
}
