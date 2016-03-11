using Fireworks.Loaders;
using UnityEngine;

namespace Fireworks
{
	/// <summary>
	/// A firework that will be shot out of a FireworkLauncher during a Show.
	/// </summary>
	public class Firework
	{
		public ParticleSystem fireworkParticle;
		public float showTime;
		public FireworkLauncher launcher;

		private static ParticleSystem defaultFirework = null;

		public Firework()
		{
			if (defaultFirework == null)
			{
				GameObject fireworkGO = AssetBundleLoader.LoadAsset("firework", "Firework-1");
				defaultFirework = fireworkGO.GetComponent<ParticleSystem>();
			}
			fireworkParticle = defaultFirework;
		}

		public override string ToString()
		{
			if (fireworkParticle == null)
			{
				return "None";
			}
			else
			{
				return fireworkParticle.name;
			}
		}

		public void Launch()
		{
			if (fireworkParticle != null)
			{
				Debug.Log("Launching " + fireworkParticle);
			}
		}
	}
}
