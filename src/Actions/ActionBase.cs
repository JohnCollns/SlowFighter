using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class ActionBase : Resource
{
	public enum ActionBaseType
	{
		Stand,
		Block,
		MinorAttack,    // Jab, Grab
		MajorAttack,    // Overhead, Low Kick
	}
	[Export] public string ActionName { get; protected set; }
	[Export] public ActionBaseType ActionType { get; protected set; }

	// I wish I could use the Texture2DArray, but I couldn't find a way to make it work :(
	// Also couldn't find how to make a struct exposed on resource, so I have duplicate these
	// I'm sorry John for this horrible horrible code
	[Export] public Texture2D BlueBlockingLayer;
	[Export] public Texture2D BlueAttackedTopLayer;
	[Export] public Texture2D BlueFaceLayer;
	[Export] public Texture2D BlueAttackedBottomLayer;
	
	[Export] public Texture2D RedBlockingLayer;
	[Export] public Texture2D RedAttackedTopLayer;
	[Export] public Texture2D RedFaceLayer;
	[Export] public Texture2D RedAttackedBottomLayer;

	// [Export] public string[] a;
	// [Export] public Array<string> b;
	// animaions
	//      successful
	//      countered
	//      blocked ?
	[Export] public Texture2D AnimationSprite;
	
	// Set of actions that this action counters. 
	//[Export] public HashSet<ActionBase> ActionCounters { get; protected set; }
	//[Export] public Godot.Collections.Array<ActionBase> ActionCounters { get; protected set; }
	//[Export] public Godot.Collections.Array<string> ActionCounters { get; protected set; }
	//[Export] public Godot.Collections.Array<string> ActionCounters;
	[Export] public string[] ActionCounters;
	
	[Export] public float DamageOnCounter { get; protected set; }
	[Export] public float DamageOnClash { get; protected set; }

	public bool IsAttack()
	{
		return ActionType == ActionBaseType.MinorAttack || ActionType == ActionBaseType.MajorAttack;
	}

	public bool CountersAction(ActionBase otherAction)
	{
		if (ActionCounters == null) return false;
		foreach (string counter in ActionCounters)
		{
			if (counter == otherAction.ActionName)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void OnActionSucceeded(Character ownerCharacter, Character targetCharacter) { }

	public virtual void OnAnimationStarted(Character ownerCharacter)
	{
		GD.Print($"P{ownerCharacter.GetID()} Animating: {ActionName}");
		ownerCharacter.SpriteNode.Texture = AnimationSprite;
		
		var BlockingLayer 		= (Sprite2D)ownerCharacter.IconSprite.GetChild(0);
		var AttackedTopLayer 	= (Sprite2D)ownerCharacter.IconSprite.GetChild(1);
		var FaceLayer 			= (Sprite2D)ownerCharacter.IconSprite.GetChild(2);
		var AttackedBottomLayer = (Sprite2D)ownerCharacter.IconSprite.GetChild(3);
		
		if (FaceLayer == null) return;
		
		if (!ownerCharacter.bFacingLeft) {
			BlockingLayer.Texture 		= RedBlockingLayer;
			AttackedTopLayer.Texture 	= RedAttackedTopLayer;
			FaceLayer.Texture			= RedFaceLayer;
			AttackedBottomLayer.Texture = RedAttackedBottomLayer;
		}
		
		else {
			BlockingLayer.Texture 		= BlueBlockingLayer;
			AttackedTopLayer.Texture 	= BlueAttackedTopLayer;
			FaceLayer.Texture			= BlueFaceLayer;
			AttackedBottomLayer.Texture = BlueAttackedBottomLayer;
		}
	}
	
	public virtual void OnAnimationFinished(Character ownerCharacter) 
	{
		//ownerCharacter.IconSprite.Texture = null;
	}
}
