using Fireworks.Loaders;
using System.Collections.Generic;
using UnityEngine;

namespace Fireworks
{
	/// <summary>
	/// Fetches and manages all the firework particle effects.
	/// </summary>
	public static class Firework
	{
		private static Dictionary<string, ParticleSystem> loadedFireworks = new Dictionary<string, ParticleSystem>();

		/// <summary>
		/// How long we wait after the firework is launched before it is destroyed.
		/// </summary>
		private const float FIREWORK_DESTROY_TIME = 10.0f;

		public static ParticleSystem GetFirework(string name)
		{
			ParticleSystem firework;
			if (!loadedFireworks.TryGetValue(name, out firework))
			{
				firework = LoadFirework(name);
			}
			if (firework == null)
			{
				return null;
			}
			else
			{
				ParticleSystem fireworkInstance = Object.Instantiate(firework);
				fireworkInstance.loop = false;
				// Clean up the firework when it's done being launched
				Object.Destroy(fireworkInstance, FIREWORK_DESTROY_TIME);
				return fireworkInstance;
			}
		}

		private static ParticleSystem LoadFirework(string toLoad)
		{
			GameObject fireworkGO = AssetBundleLoader.LoadAsset("firework", toLoad);
			if (fireworkGO == null)
			{
				Debug.Log("No firework found with name: " + toLoad);
				return null;
			}
			ParticleSystem firework = fireworkGO.GetComponent<ParticleSystem>();
			firework.loop = false;
			firework.transform.position = new Vector3(0.0f, 999.0f, 0.0f);
			loadedFireworks[toLoad] = firework;
			Debug.Log("Loaded " + firework);
			return firework;
		}

		public static void CleanUp()
		{
			loadedFireworks.Clear();
		}
	}
}
