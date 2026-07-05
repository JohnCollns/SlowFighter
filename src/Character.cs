using Godot;
using System;

public partial class Character : Node2D
{
    [Export] public bool bFacingLeft = true;
    [Export] public float MaxHealth = 100f;
    public float Health = 100f;
    [Export] public bool bShowHealthLabel = true;

    private Sprite2D SpriteNode;
    private Label HealthLabel;
    //private ColorRect HealthBar;
    private HealthBar HealthBar;

    public ActionBase pendingAction;

    public override void _Ready()
    {
        SpriteNode = GetNode<Sprite2D>("SpriteNode");
        SpriteNode.FlipH = !bFacingLeft;
        SpriteNode.Position = new Vector2(SpriteNode.Position.X * (bFacingLeft ? 1f : -1f), SpriteNode.Position.Y);
        
        HealthLabel = GetNode<Label>("HealthLabel");
        HealthLabel.Position = new Vector2(HealthLabel.Position.X * (bFacingLeft ? 1f : -1f), HealthLabel.Position.Y);
        HealthLabel.SetVisible(bShowHealthLabel);
        
        Health = MaxHealth;
        UpdateHealthLabel();
        HealthBar = GetNode<GameManager>("../GameManagerScene").GetHealthBar(bFacingLeft ? 0 : 1);
        HealthBar.SetFacingLeft(bFacingLeft);
    }

    public void Restart()
    {
        Health = MaxHealth;
        UpdateHealthLabel();
        HealthBar.SetPercent(Health / MaxHealth);
        // set to standing sprite
    }

    protected void UpdateHealthLabel()
    {
        HealthLabel.Text = Health.ToString();
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        HealthBar.SetPercent(Health / MaxHealth);
        UpdateHealthLabel();
        // What to do if health drops below zero?
        //GD.Print("Damage taken, resultant health: " + Health);
    }

    public void Heal(float amount)
    {
        Health += amount;
        if  (Health > MaxHealth)
            Health = MaxHealth;
        UpdateHealthLabel();
        HealthBar.SetPercent(Health / MaxHealth);
    }

    public void AnimateAction(ActionBase action)
    {
        SpriteNode.Texture = action.AnimationSprite;
    }
    
    public void AnimateAction()
    {
        SpriteNode.Texture = pendingAction.AnimationSprite;
    }
    
    public bool IsDead() { return Health <= 0f; }
}
