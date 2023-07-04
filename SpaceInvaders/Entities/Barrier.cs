using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders.Entities {
	internal class Barrier : Entity {
		public Vector2 Position {
			get => position;
			set => position = value;
		}

		public Texture2D Texture {
			get => texture;
			set => texture = value;
		}

		private bool isDestroyed = false;

		public void InitialiseValues(GraphicsDevice graphicsDevice, ContentManager content, Vector2 position, int barrierIndex) {
			texture = content.Load<Texture2D>("Textures/Barrier " + barrierIndex);
			isDestroyed = false;
		}

		public void Draw(SpriteBatch spriteBatch) {
			if (!isDestroyed)
				spriteBatch.Draw(texture, position, Color.White);
		}

		public void HandleCollision(Projectile projectile) {
			if (isDestroyed || !PerPixelCollision(projectile))
				return;

			Rectangle collisionRect = Rectangle.Intersect(rectangle, projectile.Rectangle);

			Color[] textureData = new Color[texture.Width * texture.Height];
			texture.GetData(textureData);

			Color[] projectileTextureData = new Color[projectile.Texture.Width * projectile.Texture.Height];
			projectile.Texture.GetData(projectileTextureData);

			int centerX = -1, centerY = -1;
			for (int y = collisionRect.Top; y < collisionRect.Bottom; y++) {
				for (int x = collisionRect.Left; x < collisionRect.Right; x++) {
					int thisX = x - rectangle.Left;
					int thisY = (y - rectangle.Top) * texture.Width;
					int index = thisX + thisY;
					int projectileSpriteIndex = (x - projectile.Rectangle.Left) + (y - projectile.Rectangle.Top) * projectile.Texture.Width;

					if (textureData[index].A > 0 && projectileTextureData[projectileSpriteIndex].A > 0) {
						(centerX, centerY) = (thisX, thisY / texture.Width);
					}
				}
			}

			int radius = 3 * SpaceInvadersGame.ScalingFactor;

			if (centerX != -1 || centerY != -1) {
				textureData[centerX + centerY * texture.Width] = Color.Red;

				for (int y = 0; y < texture.Height; y++) {
					for (int x = 0; x < texture.Width; x++) {
						int index = x + y * texture.Width;

						int distanceX = Math.Abs(x - centerX);
						int distanceY = Math.Abs(y - centerY);
						float distance = MathF.Sqrt(distanceX * distanceX + distanceY * distanceY);

						if (distance <= radius)
							textureData[index] = Color.Transparent;
					}
				}
			}

			texture.SetData(textureData);

			isDestroyed = textureData.Count(c => c.A == 0) < textureData.Length * 0.15;

			projectile.Destroy();
		}
	}
}
