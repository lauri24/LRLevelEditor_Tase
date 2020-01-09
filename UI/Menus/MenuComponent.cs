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
    public delegate void levelLoadedEventHandler(List<TileObject> level);
    public delegate void levelSavedEventHandler();
    public class MenuComponent : Microsoft.Xna.Framework.DrawableGameComponent
	{
        
        public event levelLoadedEventHandler LevelLoadedFromJson;
        public event levelSavedEventHandler LevelSavedToJson;
		string[] menuItems;
		int selectedIndex;
        public string pathToLevel;
		Color normal = Color.White;
		Color hilite = Color.Yellow;

		KeyboardState keyboardState;
		KeyboardState oldKeyboardState;

		SpriteBatch spriteBatch;
		SpriteFont spriteFont;

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
				if (selectedIndex >= menuItems.Length)
					selectedIndex = menuItems.Length - 1;
			}
		}
        public void setMenuPosition(Vector2 posIn){
            position=posIn;
        }
		public MenuComponent(Game game, 
			SpriteBatch spriteBatch, 
			SpriteFont spriteFont, 
			string[] menuItems)
			: base(game)
		{
			this.spriteBatch = spriteBatch;
			this.spriteFont = spriteFont;
			this.menuItems = menuItems;
			MeasureMenu();
            pathToLevel=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)+"/Content/level1.json";
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

			if (CheckKey(Keys.Down))
			{
				selectedIndex++;
				if (selectedIndex == menuItems.Length)
					selectedIndex = 0;
			}
			if (CheckKey(Keys.Up))
			{
				selectedIndex--;
				if (selectedIndex < 0)
					selectedIndex = menuItems.Length - 1;
			}

            if(CheckKey(Keys.Enter)){
                if( menuItems[selectedIndex]=="Import Texture"){
                        System.Console.WriteLine("Import Texture");
                }
                if(menuItems[selectedIndex]=="Save Level"){
                        LevelSavedToJson();
                }
                if(menuItems[selectedIndex]=="Load Level"){
                   
                    System.Console.WriteLine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                    using (StreamReader r = new StreamReader(pathToLevel))
                    {
                        string json = r.ReadToEnd();
                        List<TileObject> levelObject = JsonConvert.DeserializeObject<List<TileObject>>(json);
						foreach(TileObject tile in levelObject){
							if(tile.texturePath!=null){
								 FileStream fileStream = new FileStream(tile.texturePath, FileMode.Open);
          						 tile.texture = Texture2D.FromStream(GraphicsDevice, fileStream);
          						 fileStream.Dispose(); 
							}


						}
                        LevelLoadedFromJson(levelObject);
                    }
                    

                  // string fullPath = process.MainModule.FileName;
                  
                }
                if(menuItems[selectedIndex]=="Level Layout Settings"){
                    
                }
                if(menuItems[selectedIndex]=="Quit"){
                     System.Console.WriteLine("Quit");
                     Game.Exit();
                }
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
        public void DrawMenu(){
            Vector2 location = position;
			Color tint;
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                                       texture.SetData<Color>(new Color[] { Color.Gray });

                                           
            spriteBatch.Draw(texture,new Rectangle((int)location.X-5,(int)location.Y,300,250), Color.LightSlateGray);
			for (int i = 0; i < menuItems.Length; i++)
			{
				if (i == selectedIndex)
					tint = hilite;
				else
					tint = normal;


               
				
                spriteBatch.DrawString(
					spriteFont,
					menuItems[i],
					new Vector2(location.X,location.Y),
					tint);
				location.Y += spriteFont.LineSpacing + 5;
			}
        }
	}
}