using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager;
using System;

namespace monoGameCP
{

    public class LRGUIComponents
    {
        private SpriteBatch spriteBatch;
        private MouseState mouse;
        private int hotItem = -1;
        private int activeItem = -1;
        GraphicsDeviceManager graphics;
        public LRGUIComponents(GraphicsDeviceManager graphics){
                this.graphics=graphics;
              spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
            mouse = Mouse.GetState();
            var mousePosition = new Point(mouse.X, mouse.Y);
            
        }

        public void Begin(Camera2d camera)
        {
            //Clear hot items before we begin.
            hotItem = -1;
            //priteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
          
           spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(graphics.GraphicsDevice));
        }

        public void End()
        {
            spriteBatch.End();
            //If the user isn't clicking anything anymore, unset the active item.
            if (mouse.LeftButton == ButtonState.Released)
            {
                activeItem = -1;
            }
        }

        public bool DoButton(int id, Rectangle rectangle, Texture2D texture,Camera2d camera)
        {
            //Determine if we're hot, and maybe even active
            if (MouseHit(rectangle,camera))
            {
                hotItem = id;
                if (activeItem == -1 && mouse.LeftButton == ButtonState.Pressed)
                    activeItem = id;
            }

            //Draw the button  
            Color drawColor;
            if (hotItem == id)
            {
                if (activeItem == id)
                {
                    drawColor = Color.White;
                }
                else
                {
                    drawColor = Color.LightGray;
                }
            }
            else
            {
                drawColor = Color.Gray;
            }
            spriteBatch.Draw(texture, rectangle, drawColor);

            //If we are hot and active but the mouse is no longer down, we where clicked
            //Line updated after comment by anonymous user, I feel so silly now!
           // return (mouse.LeftButton == ButtonState.Released && hotItem == id && activeItem == id );
           return true;
        }

          /// <summary>
        /// Draws a vertical or horizontal scrollbar at the specified position. If the grip was moved, returns the new value, else returns the inserted value
        /// </summary>        
        public float DoScrollbar(int id, Rectangle rectangle, Texture2D scrollbarTexture, Texture2D gripTexture, float max, float value, bool horizontal,Camera2d camera)
        {
            //No matter the input, value should be at least 0 and at most max
            value = Math.Min(Math.Max(value, 0), max);

            //Determine if we're hot, and maybe even active
            if (MouseHit(rectangle,camera)) //See previous part's code for this method
            {
                hotItem = id;
                if (activeItem == -1 && mouse.LeftButton == ButtonState.Pressed)
                    activeItem = id;
            }

            //Draw the scrollbar
            spriteBatch.Draw(scrollbarTexture, rectangle, Color.Red);

            //Position the grip relative on the scrollbar and make sure the grip stays inside the scrollbar
            //Note that the grip's width if vertical/height if horizontal is the scrollbar's smallest dimension.
            int gripPosition; Rectangle grip;
            if (horizontal)
            {
                gripPosition = rectangle.Left + (int)((rectangle.Width - rectangle.Height) * (value / max));
                grip = new Rectangle(gripPosition, rectangle.Top, rectangle.Height, rectangle.Height);
            }
            else
            {
                gripPosition = rectangle.Bottom - rectangle.Width - (int)((rectangle.Height - rectangle.Width) * (value / max));
                grip = new Rectangle(rectangle.Left, gripPosition, rectangle.Width, rectangle.Width);
            }
            
            //Draw the grip in the correct color
            if (activeItem == id || hotItem == id)
            {
                spriteBatch.Draw(gripTexture, grip, Color.Red);
            }
            else
            {
                spriteBatch.Draw(gripTexture, grip, Color.Blue);
            }

            //If we're active, calculate the new value and do some bookkeeping to make sure the mouse and grip are in sync            
            if (activeItem == id)
            {
                if (horizontal)
                {
                    //Because the grip's position is defined in the top left corner
                    //we need to shrink the movable domain slightly so our grip doesnt
                    //draw outside the scrollbar, while still getting a full range.                    
                    float mouseRelative = mouse.X - (rectangle.X + grip.Width / 2);
                    mouseRelative = Math.Min(mouseRelative, rectangle.Width - grip.Width);
                    mouseRelative = Math.Max(0, mouseRelative);
                    //We then calculate the relative mouse offset 0 if the mouse is at
                    //the left end or more to the left. 1 if the mouse is at the right end
                    //or more to the right. We then multiply this by max to get our new value.
                    value = (mouseRelative / (rectangle.Width - grip.Width)) * max;
                }
                else
                {
                    //same as horizontal bit in the end we inverse value,
                    //because we want the bottom to be 0 instead of the top
                    //while in y coordinates the top is 0.
                    float mouseRelative = mouse.Y - (rectangle.Y + grip.Height / 2);
                    mouseRelative = Math.Min(mouseRelative, rectangle.Height - grip.Height);
                    mouseRelative = Math.Max(0, mouseRelative);
                    value = max - (mouseRelative / (rectangle.Height - grip.Height)) * max;
                }                                               
            }
            return value;
        }
        //Small helper function that checks if the mouse is contained in the rectangle
        //Might even be unneeded but I like not having to manually write ‘new Point…’ every time.
        private bool MouseHit(Rectangle rectangle,Camera2d camera)
        {
            Vector2 worldPosition = Vector2.Transform(new Vector2(mouse.X, mouse.Y), Matrix.Invert(camera._transform));
            return rectangle.Contains(new Point((int)worldPosition.X,(int)worldPosition.Y));
        }

    }

}