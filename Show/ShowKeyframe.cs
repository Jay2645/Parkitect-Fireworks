using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fireworks.Show
{
	public class ShowKeyframe : MonoBehaviour
	{
		public Toggle keyframe;
		public string time;
		public ShowTrack track;

		public bool isOn
		{
			get
			{
				return keyframe.isOn;
			}
			set
			{
				keyframe.isOn = value;
			}
		}

		private void Start()
		{
			keyframe = gameObject.GetComponent<Toggle>();
			keyframe.onValueChanged.AddListener(new UnityAction<bool>(ChangeToggleState));
		}

		private void ChangeToggleState(bool newState)
		{
			if (track == null)
			{
				Debug.Log(this + " did not have a track set!");
				return;
			}
			track.ToggleKeyframe(this);
		}
	}
}
