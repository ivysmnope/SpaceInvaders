using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SpaceInvaders.Entities {
	internal class RedShip : Entity {
		public bool Shown { get; private set; } = false;

		private float moveSpeed = 96f;
		private Timer respawnTimer;

		private int rightBound;

		private int currentScore;
		private int[] scores = new int[] { 100, 50, 50, 100, 150, 100, 100, 50, 300, 100, 100, 100, 50, 150, 100, 50 };

		public RedShip() {
			respawnTimer = new(10000);
			respawnTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
				if (Shown)
					return;

				if (new Random().Next(0, 1000) <= 10)
					Reset();
			};
			respawnTimer.Start();
		}

		public void Reset() {
			Shown = true;
			currentScore = 0;

			if (new Random().Next(0, 2) == 0) {
				position = position.SetX(rightBound + texture.Width * 2);
				moveSpeed = MathF.Abs(moveSpeed) * -1;
			} else {
				position = position.SetX(0 - texture.Width * 2);
				moveSpeed = MathF.Abs(moveSpeed);
			}
		}

		public void InitialiseValues(ContentManager content, GraphicsDeviceManager graphics) {
			texture = content.Load<Texture2D>("Textures/Ufo");
			position = new(0f, 32f);
			Shown = false;
			rightBound = graphics.PreferredBackBufferWidth;
		}

		public void Update(GameTime gameTime, Projectile playerProjectile, GraphicsDeviceManager graphics) {
			if (!Shown)
				return;

			Shown = !((position.X + texture.Width >= graphics.PreferredBackBufferWidth + texture.Width * 3 && moveSpeed > 0)
				|| (position.X - texture.Width <= 0 - texture.Width * 3 && moveSpeed < 0));

			if (PerPixelCollision(playerProjectile)) {
				playerProjectile.Destroy();
				Destroy();
			}

			position += new Vector2(moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0f);
		}

		public void Draw(SpriteBatch spriteBatch) {
			if (!Shown)
				return;
			spriteBatch.Draw(texture, position, Color.White);
		}

		public void Destroy() {
			Shown = false;
			SpaceInvadersGame.PlayerScore += scores[currentScore++ % scores.Length];
		}
	}
}
