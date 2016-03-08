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
	}
}
