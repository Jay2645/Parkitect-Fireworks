using Fireworks.UI;
using System.Collections.Generic;

namespace Fireworks
{
	/// <summary>
	/// A firework launcher.
	/// Basically a piece of scenery which may shoot a firework at some point during a show.
	/// </summary>
	public class FireworkLauncher : Deco
	{
		public static List<FireworkLauncher> builtLaunchers = new List<FireworkLauncher>();

		protected override void Awake()
		{
			buildOnGrid = true;
			base.Awake();
		}

		public override void Start()
		{
			if (!dontSerialize && !isBlueprint && !isPreview)
			{
				AddLauncher(this);
			}
			base.Start();
		}

		public override void onDestructionClick()
		{
			RemoveLauncher(this);
			base.onDestructionClick();
		}

		private static void AddLauncher(FireworkLauncher launcher)
		{
			builtLaunchers.Add(launcher);
			ShowWindow.AddTrack(launcher);
		}

		private static void RemoveLauncher(FireworkLauncher launcher)
		{
			builtLaunchers.Remove(launcher);
		}
	}
}
