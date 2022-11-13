using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Devcade;


namespace onboard
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private SpriteFont _devcadeMenuBig;
		private SpriteFont _devcadeMenuTitle;

		private Menu _mainMenu;
		private DevcadeClient _client;

		private bool _loading = false;

		private string state = "launch";
		private float fadeColor = 0f;

		KeyboardState lastState;

		private Texture2D cardTexture;
		private Texture2D loadingSpin;
		private Texture2D BGgradient;
		private Texture2D icon;
		private Texture2D titleTexture;
		private Texture2D descriptionTexture;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			_mainMenu = new Menu(_graphics);
			_client = new DevcadeClient();
			Content.RootDirectory = "Content";
			IsMouseVisible = false;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			if (GraphicsDevice == null)
			{
				_graphics.ApplyChanges();
			}

			Input.Initialize();
			_mainMenu.updateDims(_graphics);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_devcadeMenuBig = Content.Load<SpriteFont>("devcade-menu-big");
			_devcadeMenuTitle = Content.Load<SpriteFont>("devcade-menu-title");

			cardTexture = Content.Load<Texture2D>("card");
			titleTexture = Content.Load<Texture2D>("tansparent-logo");

			descriptionTexture = Content.Load<Texture2D>("description");

			BGgradient = Content.Load<Texture2D>("OnboardBackgroundGradient");
			icon = Content.Load<Texture2D>("CSH");

			loadingSpin = Content.Load<Texture2D>("loadingSheet");

			// TODO: use this.Content to load your game content here
			_mainMenu.gameTitles = _client.GetGames();
			_mainMenu.setCards();
		}

		protected override void Update(GameTime gameTime)
		{
			// Update inputs
			KeyboardState myState = Keyboard.GetState();
			Input.Update(); // Controller update

			// Keyboard only to exit menu as it should never exit in prod
			if (Keyboard.GetState().IsKeyDown(Keys.Tab))
			{
				Exit();
			}

			if (lastState == null)
			{
				lastState = Keyboard.GetState(); // god i hate video games
			}

			// If the state is loading, it is still taking input as though it is in the input state..?
			switch (state)
			{
				// Fade in when the app launches
				case "launch":
					if (fadeColor < 1f)
					{
						fadeColor += (float)(gameTime.ElapsedGameTime.TotalSeconds);
					} else
					{
						// Once the animation completes, begin tracking input
						state = "input";
					}

					break;

				case "loading":
					// Check for process that matches last launched game and display loading screen if it's running 
					// This can be done easier by keeping a reference to the process spawned and .HasExited property...
					_loading = Util.IsProcessOpen(_mainMenu.gameSelected().name);

					if (fadeColor < 1f)
					{
						fadeColor += (float)(gameTime.ElapsedGameTime.TotalSeconds);
					}

					if (!_loading)
					{
						fadeColor = 0f;
						state = "launch";
					}
					break;

				// In this state, the user is able to scroll through the menu and launch games
				// TODO: Update _itemSelected  and top/bottom of list check to be a part of Menu.cs
				case "input":
					if (((myState.IsKeyDown(Keys.Down) && lastState.IsKeyUp(Keys.Down)) || // Keyboard down
						Input.GetButtonDown(1, Input.ArcadeButtons.StickDown) ||             // or joystick down
						Input.GetButtonDown(2, Input.ArcadeButtons.StickDown)) &&            // of either player
						_mainMenu.itemSelected < _mainMenu.gamesLen() - 1)                   // and not at bottom of list
					{
						_mainMenu.beginAnimUp();
					}

					if (((myState.IsKeyDown(Keys.Up) && lastState.IsKeyUp(Keys.Up)) || // Keyboard up
						Input.GetButtonDown(1, Input.ArcadeButtons.StickUp) ||					 // or joystick up
						Input.GetButtonDown(2, Input.ArcadeButtons.StickUp)) && 				 // of either player
						_mainMenu.itemSelected > 0)																			 // and not at top of list
					{
						_mainMenu.beginAnimDown();
					}

					if ((myState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter)) || // Keyboard Enter
						Input.GetButtonDown(1, Input.ArcadeButtons.Menu) ||                   // or menu button
						Input.GetButtonDown(2, Input.ArcadeButtons.Menu))                     // of either player
					{
						state = "description";
					}
					_mainMenu.animate(gameTime);
					break;

				case "description":
					if ((myState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter)) || // Keyboard Enter
						Input.GetButtonDown(1, Input.ArcadeButtons.Menu) ||                   // or menu button
						Input.GetButtonDown(2, Input.ArcadeButtons.Menu))                     // of either player
					{
						Console.WriteLine("Running game!!!");
						//_client.runGame(_mainMenu.gameSelected());
						fadeColor = 0f;
						_loading = true;
						state = "loading";
					} else if ((myState.IsKeyDown(Keys.RightShift) && lastState.IsKeyUp(Keys.RightShift)) || // Keyboard Rshift
						Input.GetButtonDown(1, Input.ArcadeButtons.B4) ||																			 // or B4 button
						Input.GetButtonDown(2, Input.ArcadeButtons.B4))																				 // of either player
					{
						state = "input";
					}

					break;
			}

			lastState = Keyboard.GetState();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			_spriteBatch.Begin();
			GraphicsDevice.Clear(Color.Black);

			switch (state)
			{
				case "launch":
				case "input":
					_mainMenu.drawBackground(_spriteBatch, BGgradient, icon, fadeColor, gameTime);
					_mainMenu.drawTitle(_spriteBatch, titleTexture, fadeColor);
					_mainMenu.drawCards(_spriteBatch, cardTexture, _devcadeMenuBig);
					break;

				case "loading":
					_mainMenu.drawLoading(_spriteBatch, loadingSpin, fadeColor);
					_mainMenu.drawTitle(_spriteBatch, titleTexture, fadeColor);
					break;

				case "description":
					_mainMenu.drawBackground(_spriteBatch, BGgradient, icon, fadeColor, gameTime);
					_mainMenu.drawTitle(_spriteBatch, titleTexture, fadeColor);
					_mainMenu.drawDescription(_spriteBatch, descriptionTexture, _devcadeMenuTitle, _devcadeMenuBig);
					break;
			}

			_spriteBatch.DrawString(_devcadeMenuBig, state, new Vector2(0, 0), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}

// TODO: Add error handling!!!
