using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using SharpDX.MediaFoundation.DirectX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SpaceInvaders.Entities {
	internal static class EnemyManager {
		private static int EnemyUpdateTime = 1000;
		private static readonly int MinEnemyUpdateTime = 350;

		private static readonly int enemyPerRow = 11;
		private static readonly (int One, int Two, int Three) enemiesRowCount = (1, 2, 2);
		private static int totalEnemiesCount;
		public static int EnemiesLeft => enemies.Count;

		private static List<Enemy> enemies;

		private static readonly Timer animationTimer;

		private static int maxWidth;
		private static bool moveLeft = true;
		private static bool moveDown = false;

		private static Texture2D explosionTexture;
		private static List<Vector2> explosionPositions = new();

		private static readonly (int One, int Two, int Three) enemyScores = (10, 20, 40);

		static EnemyManager() {
			animationTimer = new(EnemyUpdateTime);
			animationTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
				if (SpaceInvadersGame.GameState != GameState.Playing)
					return;
				bool leftBounds = enemies.Where(e => e.Position.X + e.Texture.Width >= maxWidth).Any();
				bool rightBound = enemies.Where(e => e.Position.X - e.Texture.Width / (2 * SpaceInvadersGame.ScalingFactor) < 0).Any();
				moveLeft = (!leftBounds && moveLeft) || (rightBound && !moveLeft);
				/*
				 *	left	right	moveL	moveL'
				 *		0		0		0		0
				 *		0		0		1		1
				 *		
				 *		0		1		0		1
				 *		0		1		1		1
				 *		
				 *		1		0		0		0
				 *		1		0		1		0
				 *		
				 *		1		1		0		1
				 *		1		1		1		0
				 *		
				 */

				Vector2 movement = new(enemies.First().Texture.Width / (2 * SpaceInvadersGame.ScalingFactor), 0);
				if (!moveLeft)
					movement *= -1;

				moveDown = (leftBounds && !moveDown) || (rightBound && !moveDown);
				/*
				 *	left	right	moveD	moveD'
				 *		0		0		0		0
				 *		0		0		1		0
				 *		
				 *		0		1		0		1
				 *		0		1		1		0
				 *		
				 *		1		0		0		1
				 *		1		0		1		0
				 *		
				 *		1		1		0		1
				 *		1		1		1		0
				 */

				if (moveDown)
					movement = new(0, enemies.First().Texture.Height / (1 * SpaceInvadersGame.ScalingFactor));

				foreach (Enemy enemy in enemies) {
					enemy.Position += movement;
					enemy.UpdateTexture();
				}
			};
		}

		public static void Update(GameTime gameTime, GraphicsDevice graphcisDevice, Projectile playerProjectile, Player player) {
			playerProjectile.SetupRectangle();
			player.SetupRectangle();
			foreach (Enemy enemy in enemies) {
				enemy.SetupRectangle();
				enemy.Update(gameTime, player);
				if (enemy.PerPixelCollision(playerProjectile)) {
					playerProjectile.Destroy();
					EnemyUpdateTime = Math.Max(MinEnemyUpdateTime, (EnemyUpdateTime * enemies.Count) / totalEnemiesCount + 100);
					animationTimer.Interval = EnemyUpdateTime;
					explosionPositions.Add(enemy.Position);
					new Task(() => {
						System.Threading.Thread.Sleep(1000);
						explosionPositions.Remove(explosionPositions.First());
					}).Start();
					enemies.Remove(enemy);
					SpaceInvadersGame.PlayerScore += enemy.Score;
					break;
				} else if (enemy.PerPixelCollision(player)) {
					player.Die();
				}
				foreach (Projectile projectile in enemy.Projectiles)
					BarriersManager.Update(gameTime, projectile);
			}

			if (new Random().Next(enemies.Count * 100) is int generatedNumber && enemies.Count > generatedNumber) {
				enemies.ElementAt(generatedNumber).Shoot(graphcisDevice);
			}
		}

		public static void Reset(ContentManager content, GraphicsDeviceManager graphics) {
			enemies = new();

			int scaledSize = 8 * SpaceInvadersGame.ScalingFactor;
			int row = 0;
			int xOffset = graphics.PreferredBackBufferWidth / 2 - scaledSize * enemyPerRow - scaledSize;
			int yOffset = 32 * SpaceInvadersGame.ScalingFactor;

			Action<int, string, int> spawnEnemies = (int enemiesRowCount, string texturePath, int score) => {
				for (int r = 0; r < enemiesRowCount; r++) {
					for (int i = 0; i < enemyPerRow; i++) {
						Enemy alien = new();
						alien.Score = score;
						alien.InitialiseValues(content, texturePath, new(0, 0));
						alien.Position = new(alien.Texture.Width * i + xOffset, alien.Texture.Height * row + (8 * row) + yOffset);
						alien.SetupRectangle();
						enemies.Add(alien);
					}
					row++;
				}
			};

			spawnEnemies.Invoke(enemiesRowCount.One, "Textures/Alien 0", enemyScores.One);
			spawnEnemies.Invoke(enemiesRowCount.Two, "Textures/Alien 1", enemyScores.Two);
			spawnEnemies.Invoke(enemiesRowCount.Three, "Textures/Alien 2", enemyScores.Three);

			totalEnemiesCount = enemies.Count;

			maxWidth = graphics.PreferredBackBufferWidth;

			if (animationTimer.Enabled)
				animationTimer.Stop();
			animationTimer.Start();

			explosionTexture = content.Load<Texture2D>("Textures/Explosion");
		}

		public static void Draw(SpriteBatch spriteBatch) {
			foreach (Enemy enemy in enemies)
				enemy.Draw(spriteBatch);

			foreach (Vector2 position in explosionPositions)
				spriteBatch.Draw(explosionTexture, position, Microsoft.Xna.Framework.Color.White);
		}

		internal static float Lerp(float firstFloat, float secondFloat, float by) {
			return (1f - by) * firstFloat + by * secondFloat;
		}
	}
}
