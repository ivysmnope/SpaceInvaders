using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders.Entities {
	internal static class BarriersManager {
		public static List<Barrier> Barriers { get; private set; }
		public static int BarriersCount { get; private set; } = 4;


		public static void Reset(ContentManager content, GraphicsDeviceManager graphicsDeviceManager) {
			Barriers = new List<Barrier>();

			int scaledSize = 24 * SpaceInvadersGame.ScalingFactor;
			int xOffset = graphicsDeviceManager.PreferredBackBufferWidth / 2 - (scaledSize * BarriersCount + 96 * (BarriersCount - 1)) / 2;
			int yOffset = 48 * SpaceInvadersGame.ScalingFactor;

			for (int i = 0; i < BarriersCount; i++) {
				Barrier barrier = new Barrier();
				barrier.InitialiseValues(graphicsDeviceManager.GraphicsDevice, content, new Vector2(0, 0), i + 1);

				// Calculate the x position of the barrier
				int xPosition = xOffset + (scaledSize + 96) * i;
				// Calculate the y position of the barrier
				int yPosition = graphicsDeviceManager.PreferredBackBufferHeight - barrier.Texture.Height - yOffset;

				// Update the position of the barrier
				barrier.Position = new Vector2(xPosition, yPosition);

				Barriers.Add(barrier);
			}
		}

		public static void Draw(SpriteBatch spriteBatch) {
			foreach (Barrier barrier in Barriers)
				barrier.Draw(spriteBatch);
		}

		public static void Update(GameTime gameTime, Projectile projectile) {
			projectile.SetupRectangle();
			foreach (Barrier barrier in Barriers) {
				barrier.SetupRectangle();
				barrier.HandleCollision(projectile);
			}
		}
	}
}
