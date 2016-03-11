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
				defaultFirework.loop = false;
				defaultFirework.transform.position = new Vector3(0.0f, 999.0f, 0.0f);
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
				return launcher.name + "(" + fireworkParticle.name + ")";
			}
		}

		public void Launch()
		{
			if (fireworkParticle != null && launcher != null)
			{
				ParticleSystem firework = (ParticleSystem)Object.Instantiate(fireworkParticle, launcher.transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
				Debug.Log("Launching " + firework + " from " + launcher);
				firework.loop = false;
				Object.Destroy(firework.gameObject, 10.0f);
			}
		}
	}
}
