using Godot;
using System;
using System.Collections.Generic;

public partial class InputListener : Node
{
	[Export] public Godot.Collections.Dictionary<string, ActionBase> ActionMap;
	
	private GameManager gameManager;

	public override void _Ready()
	{
		gameManager = GetParent<GameManager>();
	}
	
	public override void _Input(InputEvent inputEvent)
	{
		base._Input(inputEvent);
		
		foreach (KeyValuePair<string, ActionBase> action in ActionMap)
		{
			if (inputEvent.IsActionPressed(action.Key))
			{
				GD.Print("Passing action: " + action.Value.ActionName + " to player: " + inputEvent.Device);
				gameManager.GetPlayer(inputEvent.Device).pendingAction = action.Value;
			}
		}

		if (inputEvent.IsActionPressed("Restart"))
		{
			gameManager.Restart();
		}
		if (inputEvent.IsActionPressed("help"))
		{
			gameManager.SetExplainerVisibility(true);
		}
		if (inputEvent.IsActionReleased("help"))
		{
			gameManager.SetExplainerVisibility(false);
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Input.IsJoyButtonPressed(0, JoyButton.Start) && Input.IsJoyButtonPressed(1, JoyButton.Start))
		{
			GD.Print("Both players inputting start, beginning game");
			gameManager.BeginGame();
		}
	}
	
	private void DebugPrintActionMap()
	{
		GD.Print("DebugPrintActionMap");
		foreach (KeyValuePair<string, ActionBase> action in ActionMap)
		{
			GD.Print($" String: {action.Key}");
			GD.Print($"  ActionBase: {action.Value}");
			//GD.Print($"  ActionBase: {action.Value.ActionName}");
		}
	}
}
