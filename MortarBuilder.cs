using Fireworks.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Fireworks
{
	/// <summary>
	/// The Builder in charge of creating all the fireworks launchers.
	/// Once you click on a thumbnail button, this takes over and lets you select where to place it.
	/// </summary>
	public class MortarBuilder : DecoBuilder
	{
		public static List<Mortar> prefabLaunchers = new List<Mortar>();

		protected override void Awake()
		{
			ghostMaterial = FireworksUIBuilder.ghostMat;
			ghostIntersectMaterial = FireworksUIBuilder.ghostIntersectMat;
			ghostCantBuildMaterial = FireworksUIBuilder.ghostCantBuildMat;
			base.Awake();
		}

		protected override float getHeightChangeDelta()
		{
			try
			{
				return base.getHeightChangeDelta();
			}
			catch (System.NullReferenceException)
			{
				// There's issues with getting null references sometimes
				// Everything works fine, it just clogs up the log
				return 0.0f;
			}
		}

		protected override Vector3 makeValidBuildPosition(Vector3 position)
		{
			try
			{
				return base.makeValidBuildPosition(position);
			}
			catch (System.NullReferenceException)
			{
				return Vector3.zero;
			}
		}

		protected override void Update()
		{
			try
			{
				base.Update();
			}
			catch (System.NullReferenceException)
			{

			}
		}
	}
}
