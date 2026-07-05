using Godot;
using System;

public partial class ActionClearForeground : ActionBase
{
    public override void OnAnimationStarted(Character ownerCharacter)
    {
        base.OnAnimationStarted(ownerCharacter);
        ownerCharacter.ForegroundSpriteNode.Texture = null;
        ownerCharacter.SpriteNode.ZIndex = 1;
    }
}
