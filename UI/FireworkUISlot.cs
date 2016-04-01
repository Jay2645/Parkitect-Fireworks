using UnityEngine;
using UnityEngine.EventSystems;

namespace Fireworks.UI.Tracks
{
	/// <summary>
	/// This is the "slot" that FireworkUIData objects fit in to.
	/// FireworkUIData objects can be dragged to any FireworkUISlot.
	/// When they are dropped onto a FireworkUISlot, any FireworkUIData object present in that slot will be "swapped" with the dropped object.
	/// </summary>
	public class FireworkUISlot : MonoBehaviour, IDropHandler
	{
		public string Time
		{
			get;
			set;
		}

		public FireworkUITrack parentTrack;

		/// <summary>
		/// This is called when something is dropped onto us.
		/// If it's a FireworkUIData, we want to swap its position with the FireworkUIData presently in this slot.
		/// </summary>
		/// <param name="eventData">The PointerEventData representing the current drag operation.</param>
		public void OnDrop(PointerEventData eventData)
		{
			FireworkUIData draggedFirework = eventData.pointerDrag.GetComponent<FireworkUIData>();
			if (draggedFirework == null || draggedFirework.parentTrack == parentTrack && draggedFirework.Time == Time || !draggedFirework.parentTrack.Interactable)
			{
				return;
			}

			string initialTime = draggedFirework.Time;
			FireworkUITrack initialParent = draggedFirework.parentTrack;

			draggedFirework.Time = Time;
			draggedFirework.parentTrack = parentTrack;

			FireworkUIData targetFirework = parentTrack.GetDataAtTime(Time);
			targetFirework.Time = initialTime;
			targetFirework.parentTrack = initialParent;

			parentTrack.SetDataAtTime(Time, draggedFirework);
			initialParent.SetDataAtTime(initialTime, targetFirework);

			Debug.Log("Moved " + draggedFirework.Firework + " from " + initialParent.mortarParent + " at " + initialTime + " to " + parentTrack.mortarParent + " at " + Time);

			targetFirework.RecalculatePosition();
		}
	}
}