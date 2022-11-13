using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace onboard
{
    public class MenuCard
    {
        private static float moveTime = 0.15f; // Time it takes to finish scrolling anim
        //private float timeRemaining = 0;

        public float rotation = 0f; // Initial pos
        private static float rotation_amt = MathHelper.ToRadians(25f); // Amount the card moves when scrolling

        public int listPos; // Tracks the card's current position on the screen
        public string name; // Each game name will taken from gameTitles list in Menu.cs
        //public float layer = 0f; // This doesn't work idk why

        // Same as rotation variables, but for scale, color
        public float scale = 1f;
        private static float scale_amt = 0.05f;

        // So uhh, I gues for decimal values the color should actually take 0.1-1.0, not 0.0-255.0? So technically this isn't correct?
        public Color cardColor = new Color(125, 0, 0);
        private static float red_amt = 30.0f;
        private static float color_amt = 50.0f;

        // Constants that determine the rate at which the rotation, color, scale change.
        private static float rotationSpeed = rotation_amt / moveTime; 
        private static float scaleSpeed = scale_amt / moveTime;
        private static float colorSpeed = color_amt / moveTime;
        private static float redSpeed = red_amt / moveTime;
    
        public MenuCard(int initialPos, string theName)
        {
            this.listPos = initialPos;
            this.name = theName;

            while(initialPos > 0)
            {
                rotation -= rotation_amt;
                scale -= scale_amt;

                cardColor.R += (byte)red_amt;
                cardColor.G += (byte)color_amt;
                cardColor.B += (byte)color_amt;

                initialPos--;
            }
            while (initialPos < 0)
            {
                rotation += rotation_amt;
                scale -= scale_amt;

                cardColor.R += (byte)red_amt;
                cardColor.G += (byte)color_amt;
                cardColor.B += (byte)color_amt;

                initialPos++;
            }

        }

        public void moveUp(GameTime gameTime)
        {
            // The card scales down moving away from the center, otherwise it scales up as it approaches the center 
            if (listPos > 0)
            {
                scaleDown((float)gameTime.ElapsedGameTime.TotalSeconds);
                
            }
            else
            {
                scaleUp((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            rotation -= rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; // To rotate counter clockwise (aka up), decrease angle
        }

        public void moveDown(GameTime gameTime)
        {
            // The card scales down moving away from the center, otherwise it scales up as it approaches the center 
            if (listPos >= 0)
            {
                scaleUp((float)gameTime.ElapsedGameTime.TotalSeconds);

            }
            else
            {
                scaleDown((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            rotation += rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; // To rotate counter counterclockwise (aka down), decrease angle
        }

        private void scaleDown(float elapsed)
        {
            scale -= scaleSpeed * elapsed;
            cardColor.R += (byte)(redSpeed * elapsed);
            cardColor.G += (byte)(colorSpeed * elapsed);
            cardColor.B += (byte)(colorSpeed * elapsed);
        }

        private void scaleUp(float elapsed)
        {
            scale += scaleSpeed * elapsed;
            cardColor.R -= (byte)(redSpeed * elapsed);
            cardColor.G -= (byte)(colorSpeed * elapsed);
            cardColor.B -= (byte)(colorSpeed * elapsed);
        }

        public void DrawSelf(SpriteBatch _spriteBatch, Texture2D cardTexture, SpriteFont font, int _sHeight, int scalingAmount)
        {
            _spriteBatch.Draw(
                cardTexture,
                new Vector2(0, _sHeight / 2 + cardTexture.Height / (2*scalingAmount)),
                null,
                cardColor,
                rotation,
                new Vector2(0, cardTexture.Height / 2),
                scale/scalingAmount, 
                SpriteEffects.None,
                0f
                );

             _spriteBatch.DrawString(
                font,
                name,
                new Vector2(0, _sHeight / 2 + cardTexture.Height / 4),
                Color.Black,
                rotation,
                new Vector2(-cardTexture.Width / 3,0),
                1f,
                SpriteEffects.None,
                0f
                );
        }
    
    }
}
