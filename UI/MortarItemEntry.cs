using Parkitect.UI;

namespace Fireworks.UI
{
	/// <summary>
	/// The thumbnail and button that appears in the Builder tab.
	/// </summary>
	public class MortarItemEntry : UIItemEntry
	{
		public override void setItem(SerializedMonoBehaviour item)
		{
			if (item == null)
			{
				throw new System.NullReferenceException("No buildable object attached to" + this + "! Item was null.");
			}
			BuildableObject buildableObject = item.GetComponent<BuildableObject>();
			if (buildableObject == null)
			{
				throw new System.NullReferenceException("No buildable object attached to" + this + "! No BuildableObject.");
			}
			base.setItem(item);
		}
	}
}
