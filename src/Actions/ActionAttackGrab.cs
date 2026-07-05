using Godot;
using System;

public partial class ActionAttackGrab : ActionAttack
{
    [Export] public Texture2D ForegroundTexture { get; set; }

    public override void OnAnimationStarted(Character ownerCharacter)
    {
        base.OnAnimationStarted(ownerCharacter);
        ownerCharacter.ForegroundSpriteNode.Texture = ForegroundTexture;
        ownerCharacter.SpriteNode.ZIndex = 0;
    }

    public override void OnAnimationFinished(Character ownerCharacter)
    {
        ownerCharacter.ForegroundSpriteNode.Texture = null;
        ownerCharacter.SpriteNode.ZIndex = 1;
    }
}
