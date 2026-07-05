using Godot;
using System;

public partial class ActionBlock : ActionBase
{
    [Export] public float HealOnSuccess { get; protected set; }

    public override void OnActionSucceeded(Character ownerCharacter, Character targetCharacter)
    {
        ownerCharacter.Heal(HealOnSuccess);
    }

    public override void OnAnimationStarted(Character ownerCharacter)
    {
        base.OnAnimationStarted(ownerCharacter);
        ownerCharacter.SpriteNode.ZIndex = 0;
    }
}
