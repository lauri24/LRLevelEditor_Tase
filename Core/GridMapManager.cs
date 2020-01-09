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

//Change namespace
namespace monoGameCP
{

class GridMapManager: Microsoft.Xna.Framework.DrawableGameComponent{

    SpriteBatch spriteBatch;
    public System.Collections.Generic.List<TileObject> barriersList;
    SpriteFont verdana36;

    Texture2D pixel;

    public GridMapManager(Game game, 
			SpriteBatch spriteBatch, 
			System.Collections.Generic.List<TileObject> barriersList, 
			SpriteFont verdana36,
            Texture2D pixel)
			: base(game)
		{
			this.spriteBatch = spriteBatch;
			this.barriersList = barriersList;
            this.verdana36 = verdana36;
            this.pixel=pixel;
		}


     public void drawGridSystem(int gridSizeX, int gridSizeY, int tileSize,Camera2d camera,  GraphicsDeviceManager graphics)
        {
            spriteBatch.Begin();
            for (int i = 0; i < gridSizeX; ++i)
            {
                for (int j = 0; j < gridSizeY; ++j)
                {
                    Vector2 transformedV = Vector2.Transform(new Vector2(j * tileSize, i * tileSize), Matrix.Invert(camera.get_transformation(graphics.GraphicsDevice)));
                    var tile = new Rectangle((int)transformedV.X, (int)transformedV.Y, tileSize, tileSize);
                    TileObject tile2 = new TileObject();
                    tile2.Rectangle = tile;
                    tile2.isGreen = false;
                    barriersList.Add(tile2);
                    DrawBorder(tile, 2, Color.Red);
                    var positionsLabel = "(" + tile2.Rectangle.X + ":" + tile2.Rectangle.Y + ")";
                    spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(tile2.Rectangle.X, tile2.Rectangle.Y), Color.White);
                }
            }
            spriteBatch.End();
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

        public void updateGridSystem(Texture2D selectedTexture,Camera2d camera)
        {
            //spriteBatch.Begin();

            foreach (TileObject rect in barriersList)
            {


                if (rect.isGreen)
                {
                    if (selectedTexture != null)
                    {
                        if (rect.texture != null)
                        {
                            spriteBatch.Draw(rect.texture, rect.Rectangle, Color.Green);
                        }

                    }
                    else
                    {
                        Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                        texture.SetData<Color>(new Color[] { Color.White });
                        spriteBatch.Draw(texture, rect.Rectangle, Color.Green);
                        Vector2 worldPos = Vector2.Transform(new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Matrix.Invert(camera._transform));
                        var positionsLabel = "(" + rect.Rectangle.X + ":" + rect.Rectangle.Y + ")\n(" + worldPos.X + ":" + worldPos.Y + ")";
                        spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White);
                    }
                }
                else
                {
                    DrawBorder(rect.Rectangle, 2, Color.Red);
                    Vector2 worldPos = Vector2.Transform(new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Matrix.Invert(camera._transform));
                    var positionsLabel = "(" + rect.Rectangle.X + ":" + rect.Rectangle.Y + ")\n(" + worldPos.X + ":" + worldPos.Y + ")";
                    spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White);
                }





            }




            // spriteBatch.End();
        }
}
}