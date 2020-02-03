using System.Numerics;
using System.Reflection.Metadata;
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
using RotatedRectangleCollisions;
using Vector2 = Microsoft.Xna.Framework.Vector2;
//Change namespace
namespace monoGameCP
{

    public class GridMapManager : Microsoft.Xna.Framework.DrawableGameComponent
    {

        SpriteBatch spriteBatch;
        public System.Collections.Generic.List<TileObject> barriersList;
        SpriteFont verdana36;
        Game1 game;
        Texture2D pixel;
        public enum GridMapType
        {
            Default,
            Isometric
        }

        public GridMapType gridMapType;
        public GridMapManager(Game1 game,
                SpriteBatch spriteBatch,
                System.Collections.Generic.List<TileObject> barriersList,
                SpriteFont verdana36,
                Texture2D pixel)
                : base(game)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.barriersList = barriersList;
            this.verdana36 = verdana36;
            this.pixel = pixel;

        }
        //http://clintbellanger.net/articles/isometric_math/
        //spriteBatch.Draw(texture, new Rectangle(400, 50, 100, 100), null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);



        public void drawIsometricGridSystem(MapInfoObject mapInfo, Camera2d camera, GraphicsDeviceManager graphics)
        {

            barriersList.Clear();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            for (int i = 0; i < mapInfo.gridSizeX; ++i)
            {
                for (int j = 0; j < mapInfo.gridSizeY; ++j)
                {

                    //  tempPt.x = pt.x - pt.y;
                    //tempPt.y = (pt.x + pt.y) / 2;
                    //var x=j*mapInfo.tileSize/2;
                    //var y=i*mapInfo.tileSize/2;

                    var x = j * mapInfo.tileSize;
                    var y = i * mapInfo.tileSize;

                    //var isotileX = x - y;
                    //var isotileY = (x+y) / 2;
                    //var isotileX = (x - y) * mapInfo.tileSize / 2;
                    //var isotileY = (x + y) * mapInfo.tileSize / 2;
                    Vector2 isoPt = TwoDToIso(new Vector2(x, y));
                    Vector2 transformedV = Vector2.Transform(new Vector2(isoPt.X, isoPt.Y), Matrix.Invert(camera.get_transformation(graphics.GraphicsDevice)));
                    var tile = new Rectangle((int)transformedV.X, (int)transformedV.Y, mapInfo.tileSize, mapInfo.tileSize);

                    TileObject tile2 = new TileObject();
                    tile2.Rectangle = tile;
                    tile2.isGreen = false;
                    barriersList.Add(tile2);
                    DrawBorder(tile, 2, Color.Red);
                    var positionsLabel = "(" + tile2.Rectangle.X + ":" + tile2.Rectangle.Y + ")";

                    spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(tile2.Rectangle.X, tile2.Rectangle.Y), Color.White, 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 0);


                }
            }
            spriteBatch.End();
        }

        public void drawGridSystem(MapInfoObject mapInfo, Camera2d camera, GraphicsDeviceManager graphics)
        {
            barriersList.Clear();
            spriteBatch.Begin(SpriteSortMode.FrontToBack,BlendState.AlphaBlend);
            for (int i = 0; i < mapInfo.gridSizeX; ++i)
            {
                for (int j = 0; j < mapInfo.gridSizeY; ++j)
                {
                    Vector2 transformedV = Vector2.Transform(new Vector2(j * mapInfo.tileSize, i * mapInfo.tileSize), Matrix.Invert(camera.get_transformation(graphics.GraphicsDevice)));
                    var tile = new Rectangle((int)transformedV.X, (int)transformedV.Y, mapInfo.tileSize, mapInfo.tileSize);
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
            //spriteBatch.Draw(texture, new Rectangle(400, 50, 100, 100), null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
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
                spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), null, borderColor, 0, Vector2.Zero, SpriteEffects.None, 0);

                // Draw left line
                spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), null, borderColor, 0, Vector2.Zero, SpriteEffects.None, 0);

                // Draw right line
                spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                                rectangleToDraw.Y,
                                                thicknessOfBorder,
                                                rectangleToDraw.Height), null, borderColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                // Draw bottom line
                spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
                                                rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                                rectangleToDraw.Width,
                                                thicknessOfBorder), null, borderColor, 0, Vector2.Zero, SpriteEffects.None, 0);

            }

        }


        public TileObject checkAndReturnHitTile(Vector2 worldPosition)
        {

            bool isHit = false;
            TileObject selectedTile = new TileObject();
            foreach (TileObject rect in barriersList)
            {

                if (gridMapType == GridMapType.Default)
                {
                    isHit = rect.Rectangle.Contains(worldPosition);
                }
                if (gridMapType == GridMapType.Isometric)
                {

                    isHit = rect.Rectangle.Contains(worldPosition);

                }
                if (isHit)
                {
                    selectedTile = rect;
                    break;
                }
            }
            if (isHit)
            {
                return selectedTile;
            }
            else
            {
                return null;
            }

        }
        public void checkAndHighlightSelectedTile(Vector2 worldPosition, Texture2D selectedTexture, string selectedTexturePath)
        {


            System.Collections.Generic.List<TileObject> copyOfBarriersList = new System.Collections.Generic.List<TileObject>(barriersList);
            foreach (TileObject rect in copyOfBarriersList)
            {
                bool isHit = false;
                if (gridMapType == GridMapType.Default)
                {
                    isHit = rect.Rectangle.Contains(worldPosition);
                }
                if (gridMapType == GridMapType.Isometric)
                {

                    isHit = rect.Rectangle.Contains(worldPosition);



                }
                if (isHit)
                {
                    if (game.currentLayerDepthLevel == rect.layerDepth)
                    {




                        //spriteBatch.Begin(SpriteSortMode.BackToFront,BlendState.AlphaBlend,null,null,null,null,camera.get_transformation(graphics.GraphicsDevice));
                        //   spriteBatch.Begin();
                        // System.Console.WriteLine("Hit rect at {0} {1}",mouseState.X,mouseState.Y);
                        Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                        texture.SetData<Color>(new Color[] { Color.White });


                        rect.layerDepth = game.currentLayerDepthLevel;
                        rect.texture = selectedTexture;
                        rect.texturePath = selectedTexturePath;
                        rect.isGreen = true;




                        if (gridMapType == GridMapType.Default)
                        {

                            spriteBatch.Draw(texture, rect.Rectangle, Color.Green);
                        }
                        if (gridMapType == GridMapType.Isometric)
                        {
                            spriteBatch.Draw(texture, rect.Rectangle, null, Color.Green, 0, Vector2.Zero, SpriteEffects.None, rect.layerDepth);
                        }
                    }
                    else
                    {
                        TileObject newTile=new TileObject(rect);
                        
                        Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                        texture.SetData<Color>(new Color[] { Color.White });


                        newTile.layerDepth = game.currentLayerDepthLevel;
                        newTile.texture = selectedTexture;
                        newTile.texturePath = selectedTexturePath;
                        newTile.isGreen = true;
                        barriersList.Add(newTile);
                    }
                    //    spriteBatch.End();
                }
            }

        }


        public Vector2 TwoDToIso(Vector2 pt)
        {
            var tempPt = new Vector2(0, 0);
            tempPt.X = (pt.X - pt.Y);
            tempPt.Y = (pt.X + pt.Y) / 2;
            return tempPt;

        }



        public void updateGridSystem(Texture2D selectedTexture, Camera2d camera)
        {
            //spriteBatch.Begin();

            foreach (TileObject rect in barriersList)
            {


                if (rect.isGreen)
                {
                        if (rect.texture != null)
                        {
                            if (gridMapType == GridMapType.Default)
                            {


                                spriteBatch.Draw(rect.texture, rect.Rectangle, Color.White);
                            }
                            if (gridMapType == GridMapType.Isometric)
                            {
                                if (rect.isRotated)
                                {

                                    Vector2 origin = new Vector2(rect.texture.Width / 2, rect.texture.Height / 2);
                                    Rectangle rectangle = new Rectangle();
                                    rectangle = rect.Rectangle;
                                    rectangle.X += rect.Rectangle.Width / 2;
                                    rectangle.Y += rect.Rectangle.Height / 2;

                                    spriteBatch.Draw(rect.texture, rectangle, null, Color.White, rect.rotationAngle, origin, SpriteEffects.None, rect.layerDepth);

                                }
                                else
                                {
                                    spriteBatch.Draw(rect.texture, rect.Rectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, rect.layerDepth);
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
                            spriteBatch.Draw(texture, rect.Rectangle, null, Color.Green, 0, Vector2.Zero, SpriteEffects.None, rect.layerDepth);
                        }
                        Vector2 worldPos = Vector2.Transform(new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Matrix.Invert(camera._transform));
                        var positionsLabel = "(" + rect.Rectangle.X + ":" + rect.Rectangle.Y + ")\n(" + worldPos.X + ":" + worldPos.Y + ")";

                        if (gridMapType == GridMapType.Default)
                        {
                            spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White);
                        }
                        if (gridMapType == GridMapType.Isometric)
                        {


                            spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White, 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 0);
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


                        spriteBatch.DrawString(verdana36, positionsLabel, new Vector2(rect.Rectangle.X, rect.Rectangle.Y), Color.White, 0, Vector2.Zero, Vector2.Zero, SpriteEffects.None, 0);
                    }
                }





            }




            // spriteBatch.End();
        }
    }
}