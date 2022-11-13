using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace onboard
{
    public class Menu
    {
        private GraphicsDeviceManager _device;

        // Synonymous with cards list from my project
        // Will have to convert this to a list of MenuCards
        public List<DevcadeGame> gameTitles { get; set; }
        private List<MenuCard> cards = new List<MenuCard>();
        public int itemSelected = 0;
        
        // Trying to make the lgo and other elements animate on startup
        //private const float moveTime = 0.15f; // Time it takes to make the logo to slide in
        //private float alpha = 0f;
        //private const float moveSpeed = 255.0f / fadeTime;

        private int loadingCol = 0;
        private int loadingRow = 0;
        private float offset = 0;

        private int _sWidth;
        private int _sHeight;
        private int scalingAmount = 0;

        private float moveTime = 0.15f; // This is the total time the scrolling animation takes
        private float timeRemaining = 0f;

        public bool movingUp;
        public bool movingDown;

        public Menu(GraphicsDeviceManager graph)
        {
            _device = graph;
        }

        public void updateDims(GraphicsDeviceManager _graphics) 
        {
            // This will be the apect ratio of the screen on the machine
            _sWidth = Int32.Parse(Environment.GetEnvironmentVariable("VIEW_WIDTH"));
            _sHeight = Int32.Parse(Environment.GetEnvironmentVariable("VIEW_HEIGHT"));

            scalingAmount = 1920/_sHeight; // This is the ratio of the optimal height to the current height, used to scale elements

            _graphics.PreferredBackBufferHeight = _sHeight;
            _graphics.PreferredBackBufferWidth = _sWidth;
            _graphics.ApplyChanges();
        }

        public void setCards()
        {
            for(int i=0; i<gameTitles.Count; i++)
            {
                cards.Add(new MenuCard(i*-1,gameTitles[i].name));
            }
        }

        public int gamesLen()
        {
            return gameTitles.Count;
        }

        public DevcadeGame gameSelected()
        {
            return gameTitles.ElementAt(itemSelected);
        }

        public void drawBackground(SpriteBatch _spriteBatch, Texture2D BGgradient, Texture2D icon, float col, GameTime gameTime)
        {
            _spriteBatch.Draw(
                BGgradient,
                new Vector2(0, 0),
                null,
                new Color(col,col,col),
                0f,
                new Vector2(0, 0),
                1f,
                SpriteEffects.None,
                0f
            );

            // Idea for scrolling icons: This surprisingly worked with no hassle
                // For row in range (# of rows)
                    // For column in range (# of cols)
                        // Draw a single icon. The location is based on it's row & column. Both of these values will incremement by 150 (150 is the size of the icon sprite)
                        // Added to the X and Y values will be it's offset, which is calculated by 150 * time elapsed. Making it move 150 px in one second
                        // Once offset reaches 150, it goes back to zero. 
            
            offset += (150 * (float)gameTime.ElapsedGameTime.TotalSeconds) / 2; // Divided by two to make the animation a little slower
            if (offset > 150)
            {
                offset = 0;
            }

            for(int row=-150; row <= 1800; row+=150) // Starts at -150 to draw an extra row above the screen
            {
                for(int column=0; column<=1200; column+=150)
                {
                    _spriteBatch.Draw(
                        icon,
                        new Vector2(column-offset, row+offset),
                        null,
                        new Color(col,col,col),
                        0f,
                        new Vector2(0, 0),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                } 
            }

        }

        public void drawTitle(SpriteBatch _spriteBatch, Texture2D titleTexture, float col)
        {

            _spriteBatch.Draw(
                titleTexture,
                new Rectangle(0,0, _sWidth, titleTexture.Height/scalingAmount),
                null,
                new Color(col,col,col),
                0f,
                new Vector2(0,0),
                SpriteEffects.None,
                0f
            );
            /*
            string welcome = "Welcome to Devcade";
            Vector2 welcomeSize = font.MeasureString(welcome);
            _spriteBatch.DrawString(font, welcome, new Vector2(_sWidth / 2 - welcomeSize.X / 2, _sHeight / 5 - welcomeSize.Y), Color.Black);
                         

            string wares = "Come enjoy our wares";
            Vector2 waresSize = font.MeasureString(wares);
            _spriteBatch.DrawString(font, wares, new Vector2(_sWidth / 2 + welcomeSize.X / 8, (_sHeight / 4.2f)), Color.Yellow, -0.3f, new Vector2(0, 0), new Vector2(0.5f, 0.5f), SpriteEffects.None, 1);
            */
        }

        public void drawLoading(SpriteBatch _spriteBatch, Texture2D loadingSpin, float col)
        { 
            if(loadingCol > 4)
            {
                loadingCol = 0;
                loadingRow++;
                if (loadingRow>4){
                    loadingRow = 0;
                }
            }

            // Creates a boundary to get the right spot on the spritesheet to be drawn
            Rectangle spriteBounds = new Rectangle(
                600 * loadingCol,
                600 * loadingRow,
                600,
                600
            );

            _spriteBatch.Draw(
                loadingSpin,
                new Vector2(_sWidth/2, _sHeight/2 + 150),
                spriteBounds,
                new Color(col, col, col),
                0f,
                new Vector2(300,300),
                1.5f,
                SpriteEffects.None,
                0f
            );

            loadingCol++;
            
        }

        // OLD
        public void drawSelection(SpriteBatch _spriteBatch, int menuItemSelected)
        {
            int rectLength = 300;
            int rectHeight = 40;
            RectangleSprite.DrawRectangle(_spriteBatch, new Rectangle(
                    _sWidth / 2 - rectLength/2,
                    ((_sHeight / 5) + (_sHeight / 10)) + ((_sHeight / 10) * menuItemSelected),
                    rectLength,
                    rectHeight
                ),
                Color.White, 3);
        }

        // OLD
        public void drawGameCount(SpriteFont font, SpriteBatch _spriteBatch, int itemSelected, int totalItems)
        {
            _spriteBatch.DrawString(font, itemSelected + " / " + totalItems, new Vector2(50, 50), Color.White);
        }

        // OLD
        public void drawGames(SpriteFont font, SpriteBatch _spriteBatch, int itemSelected, int maxItems)
        {
            int startPosition = (int)(Math.Floor(itemSelected / (double)maxItems) * maxItems);
            for (int i = 0; i < maxItems; i++)
            {
                if (startPosition + i > gameTitles.Count - 1)
                    break;
                string gameTitle = gameTitles.ElementAt(startPosition+i).name;
                Vector2 gameTitleSize = font.MeasureString(gameTitle);
                _spriteBatch.DrawString(font, gameTitle, new Vector2(_sWidth / 2 - gameTitleSize.X / 2, ((_sHeight / 5) + (_sHeight / 10)) + ((_sHeight / 10) * i)), Color.White);
            }
            /*
            int index = 0;
            foreach (String gameTitle in gameTitles)
            {
                Vector2 gameTitleSize = font.MeasureString(gameTitle);
                _spriteBatch.DrawString(font, gameTitle, new Vector2(_sWidth / 2 - gameTitleSize.X / 2, ((_sHeight / 5) + (_sHeight / 10)) + ((_sHeight / 10) * index)), Color.White);
                index++;
            }    */
        }

        public void drawDescription(SpriteBatch _spriteBatch, Texture2D descTexture, SpriteFont titleFont, SpriteFont descFont)
        {
            // If fonts on this page look blurry, then increase the font size in the .spritefont
            // TODO: Figure out how to format a large paragraph of text on the description
            //       Decide on a description page layout/style
            //       Add some cool animations
            
            Vector2 descPos = new Vector2(_sWidth/2, _sHeight/2 + descTexture.Height/(6*scalingAmount));

            _spriteBatch.Draw(descTexture, 
                descPos,
                null,
                Color.Purple,
                0f,
                new Vector2(descTexture.Width/2,descTexture.Height/2),
                1f/scalingAmount,
                SpriteEffects.None,
                0f
                );

            writeString( _spriteBatch,
                titleFont,
                gameSelected().name,
                new Vector2(descPos.X, descPos.Y - descTexture.Height/(3*scalingAmount)),
                1f
            );

            writeString(_spriteBatch,
                descFont,
                gameSelected().author,
                new Vector2(descPos.X, descPos.Y - descTexture.Height/(4*scalingAmount)),
                1.5f
            );
        }

        public void writeString(SpriteBatch _spriteBatch, SpriteFont font, string str, Vector2 pos, float scale)
        {
            Vector2 strSize = font.MeasureString(str);

            _spriteBatch.DrawString(font,
                str,
                pos,
                Color.White,
                0f,
                new Vector2(strSize.X/2,strSize.Y/2),
                scale/scalingAmount,
                SpriteEffects.None,
                0f
            );
        }

        public void drawCards(SpriteBatch _spriteBatch, Texture2D cardTexture, SpriteFont font)
        {
            // I still have no idea why the layerDepth does not work
            foreach(MenuCard card in cards)
            {
                if(Math.Abs(card.listPos) == 4)
                {
                   card.DrawSelf(_spriteBatch, cardTexture, font, _sHeight, scalingAmount);
                }
                
            }
            foreach(MenuCard card in cards)
            {
                if(Math.Abs(card.listPos) == 3)
                {
                   card.DrawSelf(_spriteBatch, cardTexture, font, _sHeight, scalingAmount);
                }
                
            }
            foreach(MenuCard card in cards)
            {
                if(Math.Abs(card.listPos) == 2)
                {
                   card.DrawSelf(_spriteBatch, cardTexture, font, _sHeight, scalingAmount);
                }
                
            }
            foreach(MenuCard card in cards)
            {
                if(Math.Abs(card.listPos) == 1)
                {
                   card.DrawSelf(_spriteBatch, cardTexture, font, _sHeight, scalingAmount);
                }
                
            }
            foreach(MenuCard card in cards)
            {
                if(Math.Abs(card.listPos) == 0)
                {
                   card.DrawSelf(_spriteBatch, cardTexture, font, _sHeight, scalingAmount);
                }
                
            }
        }

        public void beginAnimUp()
        {
            if (!(movingUp || movingDown))
                {
                    foreach(MenuCard card in cards)
                    {
                        card.listPos++;
                        //card.layer = (float)Math.Abs(card.listPos) / 4;
                    }
                    timeRemaining = moveTime; // Time remaining in the animation begins at the total expected move time
                    movingUp = true;
                    itemSelected++; // Update which game is currently selected, so the proper one will be launched
                }
        }

        public void beginAnimDown()
        {
            if (!(movingDown || movingUp))
                {
                    foreach (MenuCard card in cards)
                    {
                        card.listPos--;
                        //card.layer = (float)Math.Abs(card.listPos) / 4;
                    }
                    timeRemaining = moveTime; // Time remaining in the animation begins at the total expected move time
                    movingDown = true;
                    itemSelected--;
                }
        }

        public void animate(GameTime gameTime)
        {
            if (timeRemaining > 0) // Continues to execute the following code as long as the animation is playing AND max time isn't reached
            {
                if(movingUp)
                {
                    foreach(MenuCard card in cards)
                    {
                        card.moveUp(gameTime);
                    }
                }

                else if(movingDown)
                {
                    foreach(MenuCard card in cards)
                    {
                        card.moveDown(gameTime);
                    }
                }

                timeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds; // Decrement time until it reaches zero
            }

            else // Once timeleft reaches 0, finish anim. 
            {
                movingUp = false;
                movingDown = false;
            }
        }
    }
}
