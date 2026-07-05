using Godot;
using System;

public partial class ActionAttack : ActionBase
{
    public override void OnActionSucceeded(Character ownerCharacter, Character targetCharacter)
    {
        targetCharacter.TakeDamage(DamageOnCounter);
    }
}
