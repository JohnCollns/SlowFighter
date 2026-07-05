using Godot;
using System;
using Godot.Collections;

//using System.Collections.Generic;

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
        //return ActionCounters.Contains(otherAction.ActionName);
    }

    public virtual void OnActionSucceeded(Character ownerCharacter, Character targetCharacter) { }

    public virtual void OnAnimationStarted(Character ownerCharacter)
    {
        GD.Print($"P{ownerCharacter.GetID()} Animating: {ActionName}");
        ownerCharacter.SpriteNode.Texture = AnimationSprite;
    }
}
