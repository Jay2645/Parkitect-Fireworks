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
		public int fireworkIndex = -1;
		public FireworkLauncher launcher;

		private const string DEFAULT_FIREWORK_STRING = "Firework-1";

		private static ParticleSystem defaultFirework = null;

		public Firework(string fireworkParticleName, int index, FireworkLauncher launcher)
		{
			fireworkParticle = LoadFirework(fireworkParticleName);
			this.launcher = launcher;
		}

		private void SetDefaultFirework()
		{
			if (defaultFirework == null)
			{
				defaultFirework = LoadFirework(DEFAULT_FIREWORK_STRING);
			}
			fireworkParticle = defaultFirework;
		}

		private static ParticleSystem LoadFirework(string toLoad)
		{
			GameObject fireworkGO = AssetBundleLoader.LoadAsset("firework", toLoad);
			ParticleSystem firework = fireworkGO.GetComponent<ParticleSystem>();
			firework.loop = false;
			firework.transform.position = new Vector3(0.0f, 999.0f, 0.0f);
			return firework;
		}

		public override string ToString()
		{
			if (fireworkParticle == null)
			{
				return "None";
			}
			else
			{
				return launcher.name + "(" + fireworkParticle.name + ")";
			}
		}

		public void Launch()
		{
			if (fireworkParticle != null && launcher != null)
			{
				ParticleSystem firework = (ParticleSystem)Object.Instantiate(fireworkParticle, launcher.transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
				firework.loop = false;
				Object.Destroy(firework.gameObject, 10.0f);
			}
		}
	}
}
