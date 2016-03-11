using Fireworks.UI;
using System.Collections.Generic;

namespace Fireworks
{
	/// <summary>
	/// The Builder in charge of creating all the fireworks launchers.
	/// Once you click on a thumbnail button, this takes over and lets you select where to place it.
	/// </summary>
	public class FireworkLauncherBuilder : DecoBuilder
	{
		public static List<FireworkLauncher> allLaunchers = new List<FireworkLauncher>();

		protected override void Awake()
		{
			ghostMaterial = FireworksUIBuilder.ghostMat;
			ghostIntersectMaterial = FireworksUIBuilder.ghostIntersectMat;
			ghostCantBuildMaterial = FireworksUIBuilder.ghostCantBuildMat;
			base.Awake();
		}
	}
}
