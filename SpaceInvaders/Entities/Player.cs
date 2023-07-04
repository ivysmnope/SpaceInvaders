using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SharpDX.Direct3D9;
using SpaceInvaders.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders.Entities {
	internal class PlayerStats {
		public int Lives { get; set; } = 3;
		public int Score { get; set; } = 0;
	}

	internal class Player : Entity {
		private float moveSpeed = 50f * SpaceInvadersGame.ScalingFactor;

		public Vector2 Position => position;
		public Texture2D Texture => texture;

		private Texture2D textureDead;
		private Texture2D textureDead1;
		private Texture2D textureDead2;

		private bool dead = false;

		public PlayerStats Stats;

		public Player() {
			Stats = new();
		}

		public void InitialiseValues(GraphicsDeviceManager graphicsDeviceManager, ContentManager content) {
			InitialiseTexture(content);
			InitialisePosition(graphicsDeviceManager);
			SetupRectangle();
		}

		private void InitialiseTexture(ContentManager content) {
			texture = content.Load<Texture2D>("Textures/Cannon");
			textureDead = textureDead1 = content.Load<Texture2D>("Textures/Cannon Hit - 1");
			textureDead2 = content.Load<Texture2D>("Textures/Cannon Hit - 2");
		}

		private void InitialisePosition(GraphicsDeviceManager graphicsDeviceManager) {
			position = new(
				graphicsDeviceManager.PreferredBackBufferWidth / 2 - texture.Width / 2,
				graphicsDeviceManager.PreferredBackBufferHeight - (32 * SpaceInvadersGame.ScalingFactor) - texture.Height);
		}

		public void Draw(SpriteBatch spriteBatch) {
			spriteBatch.Draw(dead ? textureDead : texture, position, Color.White);
		}

		public void Update(GameTime gameTime, float upperBound) {
			KeyboardState keyboardState = Keyboard.GetState();

			upperBound -= texture.Width;

			float newX = position.X;

			if (keyboardState.IsKeyDown(Keys.A)) {
				newX -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
			} else if (keyboardState.IsKeyDown(Keys.D)) {
				newX += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}

			position = position.SetX(MathF.Min(MathF.Max(0, newX), upperBound));
		}

		public void Die() {
			SpaceInvadersGame.GameState = GameState.Dying;
			dead = true;
			new Task(() => {
				textureDead = textureDead1;
				System.Threading.Thread.Sleep(500);
				textureDead = textureDead2;
				System.Threading.Thread.Sleep(500);
				textureDead = textureDead1;
				System.Threading.Thread.Sleep(500);
				textureDead = textureDead2;
				System.Threading.Thread.Sleep(500);
				textureDead = textureDead1;
				SpaceInvadersGame.GameState = GameState.Playing;
				Stats.Lives--;
				if (Stats.Lives == 0)
					SpaceInvadersGame.GameState = GameState.Paused;
				dead = false;
			}).Start();
		}
	}
}
