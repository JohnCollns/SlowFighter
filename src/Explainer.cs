using Godot;
using System;

public partial class Explainer : Node
{
	
	public void SetVisible(bool visible)
	{
		SetVisibilityRecurseive(this, visible);
	}

	protected void SetVisibilityRecurseive(Node node, bool visible)
	{
		foreach (var child in node.GetChildren())
		{
			if (child is CanvasItem canvasItem)
			{
				canvasItem.SetVisible(visible);
			}
			SetVisibilityRecurseive(child, visible);
		}
	}
}
