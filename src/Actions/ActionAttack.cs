using Godot;
using System;

[GlobalClass]
public partial class ActionAttack : ActionBase
{
    public override void OnActionSucceeded(Character ownerCharacter, Character targetCharacter)
    {
        targetCharacter.TakeDamage(DamageOnCounter);
    }
}
