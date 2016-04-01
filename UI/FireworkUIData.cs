using Parkitect.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fireworks.UI.Tracks
{
	/// <summary>
	/// This is the visible firework "item" that the user will drag around on the screen.
	/// </summary>
	public class FireworkUIData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public string Firework
		{
			get
			{
				return _firework;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					HasFirework = false;
					gameObject.name = "No Firework";
				}
				else
				{
					HasFirework = true;
					gameObject.name = value;
				}
				_firework = value;
				ChangeImageState();
			}
		}
		private string _firework;
		public bool HasFirework
		{
			get;
			private set;
		}

		private bool mouseOver = false;
		private bool beingDragged = false;

		public string Time
		{
			get
			{
				return _time;
			}
			set
			{
				_time = value;
			}
		}
		private string _time;

		public FireworkUITrack parentTrack;

		public Color loadedColor;
		public Color emptyColor;

		private Vector2 offset;

		private void Start()
		{
			emptyColor = Color.clear;
			loadedColor.r = Random.value;
			loadedColor.b = Random.value;
			loadedColor.g = Random.value;
			loadedColor.a = 1.0f;
			ChangeImageState();
		}

		public void ChangeImageState()
		{
			Image image = gameObject.GetComponent<Image>();

			float alpha = 1.0f;
			if (!parentTrack.Interactable)
			{
				alpha = FireworkUITrack.UI_NONINTERACTABLE_ALPHA;
			}
			loadedColor.a = alpha;

			if (HasFirework)
			{
				image.color = loadedColor;
			}
			else
			{
				image.color = emptyColor;
			}
		}

		/// <summary>
		/// This will recalculate our onscreen position to align us with the slot we currently reside in.
		/// </summary>
		public void RecalculatePosition()
		{
			FireworkUISlot slot = parentTrack.GetSlotAtTime(Time);
			transform.SetParent(slot.transform, true);
			transform.localPosition = Vector3.zero;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!HasFirework || !parentTrack.Interactable)
			{
				return;
			}
			beingDragged = true;
			gameObject.GetComponentInChildren<CanvasGroup>().blocksRaycasts = false;
			transform.SetParent(transform.parent.parent.parent, true);
			OnDrag(eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!HasFirework || !parentTrack.Interactable)
			{
				return;
			}
			transform.position = eventData.position - offset;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (!HasFirework)
			{
				return;
			}
			gameObject.GetComponentInChildren<CanvasGroup>().blocksRaycasts = true;
			RecalculatePosition();

			beingDragged = false;
			if (!mouseOver && !beingDragged)
			{
				UITooltipController.Instance.hideTooltip();
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!parentTrack.Interactable)
			{
				return;
			}
			if (HasFirework)
			{
				if (eventData.button == PointerEventData.InputButton.Right)
				{
					parentTrack.RemoveFirework(Time);
				}
				else if (eventData.button == PointerEventData.InputButton.Left)
				{
					offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
				}
			}
			else
			{
				Dropdown.OptionData fireworkOption = ShowWindow.fireworkDropdown.options[ShowWindow.fireworkDropdown.value];
				parentTrack.AddFirework(Time, fireworkOption.text);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			mouseOver = true;
			UITooltipController.Instance.showTooltip(Firework, true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			mouseOver = false;
			if (!mouseOver && !beingDragged)
			{
				UITooltipController.Instance.hideTooltip();
			}
		}
	}
}