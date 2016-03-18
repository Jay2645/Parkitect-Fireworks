using UnityEngine;

namespace Fireworks
{
	public class LauncherCreator
	{
		public static void CreateLaunchers()
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			CreateLauncher(cube);
			Debug.Log("Created fireworks launchers.");
		}

		private static Mortar CreateLauncher(GameObject model)
		{
			Mortar launcher = model.AddComponent<Mortar>();
			launcher.dontSerialize = true;
			launcher.isPreview = true;
			ScriptableSingleton<AssetManager>.Instance.registerObject(launcher);
			MortarBuilder.prefabLaunchers.Add(launcher);
			return launcher;
		}

		public static void CleanUpLaunchers()
		{
			Debug.Log("Cleaning up fireworks launchers.");
			foreach (Mortar mortar in Mortar.builtLaunchers)
			{
				mortar.CleanUp();
			}
			Mortar.builtLaunchers.Clear();
			foreach (Mortar mortar in MortarBuilder.prefabLaunchers)
			{
				ScriptableSingleton<AssetManager>.Instance.unregisterObject(mortar);
				mortar.CleanUp();
			}
			MortarBuilder.prefabLaunchers.Clear();
		}
	}
}
