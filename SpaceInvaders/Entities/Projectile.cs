using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SpaceInvaders.Entities {
	internal class Projectile : Entity {
		public float MoveSpeed = 64f * SpaceInvadersGame.ScalingFactor;
		public bool Shot { get; set; } = false;

		private static int? lowerBound = null;

		public Vector2 Position {
			get => position;
			set => position = value;
		}

		public Rectangle Rectangle => rectangle;
		public Texture2D Texture => texture;

		public Projectile(GraphicsDevice graphicsDevice) {
			CreateTexture(graphicsDevice);
			SetupRectangle();

			lowerBound ??= graphicsDevice.Viewport.Bounds.Bottom;
		}

		public void Shoot(Player player) {
			Shot = true;
			position = player.Position;
			position = position.SetX(player.Position.X + player.Texture.Width / 2 - texture.Width / 2);
		}

		protected void CreateTexture(GraphicsDevice graphicsDevice) {
			texture = new Texture2D(graphicsDevice, 2 * SpaceInvadersGame.ScalingFactor, 4 * SpaceInvadersGame.ScalingFactor);
			Color[] colorData = new Color[texture.Width * texture.Height];
			for (int i = 0; i < colorData.Length; i++)
				colorData[i] = Color.White;
			texture.SetData(colorData);
		}

		public void Draw(SpriteBatch spriteBatch) {
			if (Shot)
				spriteBatch.Draw(texture, position, Color.White);
		}

		public void Update(GameTime gameTime, Player player) {
			if (!Shot && Keyboard.GetState().IsKeyDown(Keys.Space) && player is not null)
				Shoot(player);

			if (Shot) {
				position = position.SetY(position.Y - MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
				if (position.Y <= 0)
					Destroy();
			}
		}

		public void EnemyUpdate(GameTime gameTime, Player player) {
			if (!Shot)
				return;

			position = position.SetY(position.Y - MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
			if (position.Y >= lowerBound - (lowerBound - SpaceInvadersGame.greenLinePosition.Y) - texture.Height)
				Destroy();

			player.SetupRectangle();
			SetupRectangle();
			if (Collision(player)) {
				Destroy();
				player.Die();
			}
		}

		public void Destroy() {
			Shot = false;
		}
	}
}
