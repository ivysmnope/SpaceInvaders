using System;

namespace SpaceInvaders {
	internal class Program {
		[STAThread]
		private static void Main(string[] args) {
			using var game = new SpaceInvadersGame();
			game.Run();
		}
	}
}
