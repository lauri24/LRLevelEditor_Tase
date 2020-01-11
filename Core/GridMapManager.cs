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

    public class GridMapManager : Microsoft.Xna.Framework.DrawableGameComponent
    {

        SpriteBatch spriteBatch;
        public System.Collections.Generic.List<TileObject> barriersList;
        SpriteFont verdana36;

        Texture2D pixel;
        public enum GridMapType
        {
            Default,
            Isometric
        }

        public GridMapType gridMapType;
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
            this.pixel = pixel;

        }
        //http://clintbellanger.net/articles/isometric_math/
        //spriteBatch.Draw(texture, new Rectangle(400, 50, 100, 100), null, Color.Red, MathHelper.PiOver4, Vector2.Zero, SpriteEffects.None, 0);
        public void drawIsometricGridSystem(MapInfoObject mapInfo, Camera2d camera, GraphicsDeviceManager graphics)
        {
            barriersList.Clear();
            spriteBatch.Begin();
            for (int i = 0; i < mapInfo.gridSizeX; ++i)
            {
                for (int j = 0; j < mapInfo.gridSizeY; ++j)
                {

                    var isotileX = (j - i) * mapInfo.tileSize / 2;
                    var isotileY = (j + i) * mapInfo.tileSize / 2;
                    Vector2 transformedV = Vector2.Transform(new Vector2(isotileX, isotileY), Matrix.Invert(camera.get_transformation(graphics.GraphicsDevice)));
                    var tile = new Rectangle((int)transformedV.X, (int)transformedV.Y, mapInfo.tileSize, mapInfo.tileSize);

                    TileObject tile2 = new TileObject();
                    tile2.Rectangle = tile;
                    tile2.isGreen = false;
                    barriersList.Add(tile2);
                    DrawBorder(tile, 2, Color.Red);
                    var positionsLabel = "(" + tile2.Rectangle.X + ":" + tile2.Rectangle.Y + ")";
                    if (gridMapType == GridMapType.Default)
                    {
                        spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(tile2.Rectangle.X, tile2.Rectangle.Y), Color.White);
                    }
                    if (gridMapType == GridMapType.Isometric)
                    {
                        //DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);

                        spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(tile2.Rectangle.X, tile2.Rectangle.Y), Color.White, MathHelper.PiOver4, Vector2.Zero, Vector2.Zero, SpriteEffects.None, 0);
                    }

                }
            }
            spriteBatch.End();
        }

        public void drawGridSystem(MapInfoObject mapInfo, Camera2d camera, GraphicsDeviceManager graphics)
        {
            barriersList.Clear();
            spriteBatch.Begin();
            for (int i = 0; i < mapInfo.gridSizeX; ++i)
            {
                for (int j = 0; j < mapInfo.gridSizeY; ++j)
                {
                    Vector2 transformedV = Vector2.Transform(new Vector2(j * mapInfo.tileSize, i * mapInfo.tileSize), Matrix.Invert(camera.get_transformation(graphics.GraphicsDevice)));
                    var tile = new Rectangle((int)transformedV.X, (int)transformedV.Y, mapInfo.tileSize,mapInfo.tileSize);
                    TileObject tile2 = new TileObject();
                    tile2.Rectangle = tile;
                    tile2.isGreen = false;
                    barriersList.Add(tile2);
                    DrawBorder(tile, 2, Color.Red);
                    var positionsLabel = "(" + tile2.Rectangle.X + ":" + tile2.Rectangle.Y + ")";
                    if (gridMapType == GridMapType.Default)
                    {
                        spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(tile2.Rectangle.X, tile2.Rectangle.Y), Color.White);
                    }
                    if (gridMapType == GridMapType.Isometric)
                    {
                        //DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);

                        spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(tile2.Rectangle.X, tile2.Rectangle.Y), Color.White, MathHelper.PiOver4, Vector2.Zero, Vector2.Zero, SpriteEffects.None, 0);
                    }

                }
            }
            spriteBatch.End();
        }

        private void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            //spriteBatch.Draw(texture, new Rectangle(400, 50, 100, 100), null, Color.Red, MathHelper.PiOver4, Vector2.Zero, SpriteEffects.None, 0);
            if (gridMapType == GridMapType.Default)
            {
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
            if (gridMapType == GridMapType.Isometric)
            {
                spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), null, borderColor, MathHelper.PiOver4, Vector2.Zero, SpriteEffects.None, 0);

                // Draw left line
                spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), null, borderColor, MathHelper.PiOver4, Vector2.Zero, SpriteEffects.None, 0);

                // Draw right line
                spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                                rectangleToDraw.Y,
                                                thicknessOfBorder,
                                                rectangleToDraw.Height), null, borderColor, MathHelper.PiOver4, Vector2.Zero, SpriteEffects.None, 0);
                // Draw bottom line
                spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
                                                rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                                rectangleToDraw.Width,
                                                thicknessOfBorder), null, borderColor, MathHelper.PiOver4, Vector2.Zero, SpriteEffects.None, 0);

            }

        }
        public void checkAndHighlightSelectedTile(Vector2 worldPosition, Texture2D selectedTexture, string selectedTexturePath)
        {
            foreach (TileObject rect in barriersList)
            {


                if (rect.Rectangle.Contains(worldPosition))
                {

                    //spriteBatch.Begin(SpriteSortMode.BackToFront,BlendState.AlphaBlend,null,null,null,null,camera.get_transformation(graphics.GraphicsDevice));
                    //   spriteBatch.Begin();
                    // System.Console.WriteLine("Hit rect at {0} {1}",mouseState.X,mouseState.Y);
                    System.Console.WriteLine("Hit");
                    Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                    texture.SetData<Color>(new Color[] { Color.White });
                    rect.texture = selectedTexture;
                    rect.texturePath = selectedTexturePath;
                    rect.isGreen = true;

                    if (gridMapType == GridMapType.Default)
                    {
                        spriteBatch.Draw(texture, rect.Rectangle, Color.Green);
                    }
                    if (gridMapType == GridMapType.Isometric)
                    {
                        spriteBatch.Draw(texture, rect.Rectangle, null, Color.Green, MathHelper.PiOver4, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    //    spriteBatch.End();
                }
            }

        }
        public void updateGridSystem(Texture2D selectedTexture, Camera2d camera)
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
                            if (gridMapType == GridMapType.Default)
                            {
                                spriteBatch.Draw(rect.texture, rect.Rectangle, Color.Green);
                            }
                            if (gridMapType == GridMapType.Isometric)
                            {
                                spriteBatch.Draw(rect.texture, rect.Rectangle, null, Color.Green, MathHelper.PiOver4, Vector2.Zero, SpriteEffects.None, 0);
                            }

                        }

                    }
                    else
                    {
                        Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                        texture.SetData<Color>(new Color[] { Color.White });
                        if (gridMapType == GridMapType.Default)
                        {
                            spriteBatch.Draw(texture, rect.Rectangle, Color.Green);
                        }
                        if (gridMapType == GridMapType.Isometric)
                        {
                            spriteBatch.Draw(texture, rect.Rectangle, null, Color.Green, MathHelper.PiOver4, Vector2.Zero, SpriteEffects.None, 0);
                        }
                        Vector2 worldPos = Vector2.Transform(new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Matrix.Invert(camera._transform));
                        var positionsLabel = "(" + rect.Rectangle.X + ":" + rect.Rectangle.Y + ")\n(" + worldPos.X + ":" + worldPos.Y + ")";

                        if (gridMapType == GridMapType.Default)
                        {
                            spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White);
                        }
                        if (gridMapType == GridMapType.Isometric)
                        {


                            spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White, MathHelper.PiOver4, Vector2.Zero, Vector2.Zero, SpriteEffects.None, 0);
                        }

                    }
                }
                else
                {
                    DrawBorder(rect.Rectangle, 2, Color.Red);
                    Vector2 worldPos = Vector2.Transform(new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Matrix.Invert(camera._transform));
                    var positionsLabel = "(" + rect.Rectangle.X + ":" + rect.Rectangle.Y + ")\n(" + worldPos.X + ":" + worldPos.Y + ")";

                    if (gridMapType == GridMapType.Default)
                    {
                        spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White);
                    }
                    if (gridMapType == GridMapType.Isometric)
                    {


                        spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White, MathHelper.PiOver4, Vector2.Zero, Vector2.Zero, SpriteEffects.None, 0);
                    }
                }





            }




            // spriteBatch.End();
        }
    }
}