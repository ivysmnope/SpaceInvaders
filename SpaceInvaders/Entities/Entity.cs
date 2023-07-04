using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders.Entities {
	internal abstract class Entity {
		protected Texture2D texture;
		protected Vector2 position;

		protected Rectangle rectangle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetupRectangle() {
			rectangle = new((int)position.X, (int)position.Y, texture.Width, texture.Height);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Collision(Entity other) {
			//return rectangle.Left < other.rectangle.Right &&
			//	rectangle.Right > other.rectangle.Left &&
			//	rectangle.Top < other.rectangle.Bottom &&
			//	rectangle.Bottom > other.rectangle.Top;
			return rectangle.Intersects(other.rectangle);
		}

		public bool PerPixelCollision(Entity other) {
			if (!Collision(other))
				return false;

			Rectangle collisionRect = Rectangle.Intersect(rectangle, other.rectangle);

			Color[] spritePixels = new Color[texture.Width * texture.Height];
			texture.GetData(spritePixels);

			Color[] otherSpritePixels = new Color[other.texture.Width * other.texture.Height];
			other.texture.GetData(otherSpritePixels);

			for (int y = collisionRect.Top; y < collisionRect.Bottom; y++) {
				for (int x = collisionRect.Left; x < collisionRect.Right; x++) {
					int spriteIndex = (x - rectangle.Left) + (y - rectangle.Top) * texture.Width;
					int otherSpriteIndex = (x - other.rectangle.Left) + (y - other.rectangle.Top) * other.texture.Width;

					if (spritePixels[spriteIndex].A > 0 && otherSpritePixels[otherSpriteIndex].A > 0) {
						return true;
					}
				}
			}
			return false;
		}
	}
}
