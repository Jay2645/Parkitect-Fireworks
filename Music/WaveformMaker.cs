using UnityEngine;
using UnityEngine.UI;

namespace Fireworks.Music
{
	[RequireComponent(typeof(AudioSource))]
	[RequireComponent(typeof(Image))]
	public class WaveformMaker : MonoBehaviour
	{
		public Color backgroundColor = Color.clear;
		public Color waveformColor = Color.black;
		public Slider slider;

		private float songPercentComplete = 0.0f;

		private AudioSource source;

		private void Start()
		{
			source = gameObject.GetComponent<AudioSource>();
			Image image = gameObject.GetComponent<Image>();
			image.color = Color.white;
			image.sprite = GetMusicTimeline(source.clip, waveformColor, backgroundColor);

			slider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(ChangeClipTime));
		}

		private void Update()
		{
			if (source.isPlaying)
			{
				if (Time.timeScale > 0.0f)
				{
					source.time *= Time.timeScale;
				}
				songPercentComplete = source.time / source.clip.length;
				slider.value = songPercentComplete;
			}
		}

		public void ChangeClipTime(float newTime)
		{
			if (!source.isPlaying)
			{
				source.Play();
			}
			songPercentComplete = newTime;
			source.time = songPercentComplete * source.clip.length;
		}

		public static Sprite GetMusicTimeline(AudioClip audioClip, Color waveformColor, Color backgroundColor)
		{
			float[] dataClip;
			Texture2D texture;
			int[] dataFinal;

			dataClip = new float[audioClip.samples];
			audioClip.GetData(dataClip, 0);

			texture = new Texture2D(500, 400, TextureFormat.ARGB32, false);

			dataFinal = new int[texture.width];

			for (int x = 0; x < texture.width; x++)
			{
				int indexData = (int)MapTruncate(x, 0, texture.width, 0, dataClip.Length - 1);

				int valueOnImage = (int)(MapTruncate(dataClip[indexData], 0, 1f, 0, texture.height / 2 - 1));

				dataFinal[x] = valueOnImage;
			}

			for (int i = 0; i < dataFinal.Length; ++i)
			{
				if (dataFinal[i] < 2)
				{
					dataFinal[i] = 2;
				}
				int average = 1;
				//dataFinal[i] += dataFinal[i - 4];
				//dataFinal[i] += dataFinal[i - 3];
				//dataFinal[i] += dataFinal[i - 2];
				if (i >= 1)
				{
					dataFinal[i] += dataFinal[i - 1];
					average++;
				}
				if (i < dataFinal.Length - 1)
				{
					dataFinal[i] += dataFinal[i + 1];
					average++;
				}
				//dataFinal[i] += dataFinal[i + 2];
				//dataFinal[i] += dataFinal[i + 3];
				//dataFinal[i] += dataFinal[i + 4];
				dataFinal[i] /= average;
			}

			for (int i = 0; i < dataFinal.Length; i++)
			{
				dataFinal[i] *= 3;
			}

			for (int x = 0; x < texture.width; x++)
			{

				for (int y = texture.height / 2 - 1; y < dataFinal[x] + texture.height / 2 - 1; y++)
					texture.SetPixel(x, y, waveformColor);
				for (int y = dataFinal[x] + texture.height / 2 - 1; y < texture.height; y++)
					texture.SetPixel(x, y, backgroundColor);

				for (int y = texture.height / 2 - 1; y > texture.height / 2 - 1 - dataFinal[x]; y--)
					texture.SetPixel(x, y, waveformColor);
				for (int y = texture.height / 2 - 1 - dataFinal[x]; y >= 0; y--)
					texture.SetPixel(x, y, backgroundColor);
			}

			texture.Apply();

			Rect rec = new Rect(0, 0, texture.width, texture.height);

			return Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
		}

		public static float MapTruncate(float value, float from1, float to1, float from2, float to2)
		{
			return Mathf.Clamp((value - from1) / (to1 - from1) * (to2 - from2) + from2, from2, to2);
		}
	}
}