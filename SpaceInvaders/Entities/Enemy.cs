using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Content;
using MonoGame.Extended.TextureAtlases;
using System.Diagnostics;
using System.Timers;
using MonoGame.Extended;
using MonoGame.Extended.Timers;

namespace SpaceInvaders.Entities {
	internal class Enemy : Entity {
		public Texture2D Texture {
			get => texture;
			set => texture = value;
		}
		public Texture2D Texture1 { get; set; }

		private bool firstTexture;

		public Vector2 Position {
			get => position;
			set => position = value;
		}

		public int Score { get; set; } = 0;

		public List<Projectile> Projectiles { get; private set; }
		public bool Dead { get; set; } = false;

		public void InitialiseValues(ContentManager content, string texturePath, Vector2 position) {
			InitialiseTexture(content, texturePath);
			Position = position;
			SetupRectangle();
			Projectiles = new();
		}

		private void InitialiseTexture(ContentManager content, string texturePath) {
			Texture = content.Load<Texture2D>($"{texturePath} - 1");
			Texture1 = content.Load<Texture2D>(texturePath + " - 2");
		}

		public void Draw(SpriteBatch spriteBatch) {
			spriteBatch.Draw(firstTexture ? Texture : Texture1, Position, Color.White);
			foreach (Projectile projectile in Projectiles)
				projectile.Draw(spriteBatch);
		}

		public void UpdateTexture() {
			firstTexture = !firstTexture;
		}

		public void Update(GameTime gameTime, Player player) {
			Projectiles = Projectiles.Where(p => p.Shot).ToList();

			foreach (Projectile projectile in Projectiles)
				projectile.EnemyUpdate(gameTime, player);
		}

		public void Shoot(GraphicsDevice graphcisDevice) {
			Projectile projectile = new Projectile(graphcisDevice);
			projectile.MoveSpeed *= -1;
			projectile.Position = new(position.X + texture.Width / 2 - projectile.Texture.Width / 2, position.Y + texture.Height);
			projectile.Shot = true;
			Projectiles.Add(projectile);
		}
	}
}
