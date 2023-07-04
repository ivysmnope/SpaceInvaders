using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SpaceInvaders.Entities;
using System;
using System.Diagnostics;

namespace SpaceInvaders {
	public enum GameState {
		Playing,
		Paused,
		Dying,
	}

	public class SpaceInvadersGame : Game {
		public static GameState GameState = GameState.Playing;

		public static readonly int ScalingFactor = 4;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private static Texture2D greenLineTexture;
		public static Vector2 greenLinePosition;

		private Player player;
		private Projectile projectile;
		private RedShip redShip;

		public static int PlayerScore = 0;

		private static SpriteFont font;

		private Color clearColor = Color.Black;

		public SpaceInvadersGame() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize() {
			// TODO: Add your initialization logic here

			graphics.PreferredBackBufferWidth = 256 * ScalingFactor;
			graphics.PreferredBackBufferHeight = 224 * ScalingFactor;

			//graphics.ToggleFullScreen();

			graphics.ApplyChanges();

			player = new();
			projectile = new(GraphicsDevice);
			redShip = new();

			base.Initialize();
		}

		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			font = Content.Load<SpriteFont>("Fonts/Default");

			player.InitialiseValues(graphics, Content);

			greenLineTexture = new(GraphicsDevice, graphics.PreferredBackBufferWidth, ScalingFactor);
			Color[] greenLineColorData = new Color[graphics.PreferredBackBufferWidth * ScalingFactor];
			for (int i = 0; i < greenLineColorData.Length; i++)
				greenLineColorData[i] = Color.Green;
			greenLineTexture.SetData(greenLineColorData);
			greenLinePosition = new Vector2(0f, graphics.PreferredBackBufferHeight - 16 * ScalingFactor - ScalingFactor);

			EnemyManager.Reset(Content, graphics);
			BarriersManager.Reset(Content, graphics);

			redShip.InitialiseValues(Content, graphics);
		}

		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (GameState != GameState.Playing)
				return;

			// TODO: Add your update logic here
			player.Update(gameTime, graphics.PreferredBackBufferWidth);
			projectile.Update(gameTime, player);

			EnemyManager.Update(gameTime, GraphicsDevice, projectile, player);
			BarriersManager.Update(gameTime, projectile);

			redShip.Update(gameTime, projectile, graphics);

			if (EnemyManager.EnemiesLeft == 0) {
				Continue();
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			if (GameState == GameState.Paused) {
				GraphicsDevice.Clear(clearColor);
				return;
			}

			GraphicsDevice.Clear(clearColor);

			spriteBatch.Begin();

			// TODO: Add your drawing code here
			player.Draw(spriteBatch);

			projectile.Draw(spriteBatch);

			EnemyManager.Draw(spriteBatch);
			BarriersManager.Draw(spriteBatch);

			DrawGreenLine(spriteBatch);

			redShip.Draw(spriteBatch);

			DrawLives(spriteBatch, player.Texture);
			DrawScore(spriteBatch);

			spriteBatch.End();
			base.Draw(gameTime);
		}

		private void DrawGreenLine(SpriteBatch spriteBatch) {
			spriteBatch.Draw(greenLineTexture, greenLinePosition, Color.White);
		}

		private void DrawLives(SpriteBatch spriteBatch, Texture2D lifeTexture) {
			Vector2 livesStartingPosition = new(8 * ScalingFactor, graphics.PreferredBackBufferHeight - lifeTexture.Height * 1.5f);
			spriteBatch.DrawString(font, $"{player.Stats.Lives}", livesStartingPosition.SetX(livesStartingPosition.X - 8), Color.White);
			for (int i = 0; i < player.Stats.Lives - 1; i++) {
				spriteBatch.Draw(lifeTexture, livesStartingPosition.SetX(livesStartingPosition.X + (i) * lifeTexture.Width), Color.White);
			}
		}

		private void DrawScore(SpriteBatch spriteBatch) {
			string scoreText = $"Score: {PlayerScore:000000000}";
			Vector2 scorePosition = new(graphics.PreferredBackBufferWidth - 64 * 3, graphics.PreferredBackBufferHeight - 32);
			spriteBatch.DrawString(font, scoreText, scorePosition, Color.White);
		}

		private void Continue() {
			GameState = GameState.Paused;

			System.Threading.Thread.Sleep(2000);

			EnemyManager.Reset(Content, graphics);
			redShip.InitialiseValues(Content, graphics);

			GameState = GameState.Playing;
		}
	}
}
