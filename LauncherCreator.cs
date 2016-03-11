using UnityEngine;

namespace Fireworks
{
	public class LauncherCreator
	{
		public static void CreateLaunchers()
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			FireworkLauncher launcher = cube.AddComponent<FireworkLauncher>();
			launcher.dontSerialize = true;
			launcher.isPreview = true;
			FireworkLauncherBuilder.allLaunchers.Add(launcher);
		}
	}
}
