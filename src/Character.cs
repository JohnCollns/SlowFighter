using Godot;
using System;

public partial class Character : Node2D
{
    [Export] public bool bFacingLeft = true;
    [Export] public float MaxHealth = 100f;
    public float Health = 100f;
    [Export] public bool bShowHealthLabel = true;
    public bool bTookDamageThisExecution = false;

    public Sprite2D SpriteNode { get; private set; }
    public Sprite2D ForegroundSpriteNode { get; private set; }
    [Export] public Texture2D HurtSprite;
    private Label HealthLabel;
    //private ColorRect HealthBar;
    private HealthBar HealthBar;

    public ActionBase pendingAction;

    public override void _Ready()
    {
        SpriteNode = GetNode<Sprite2D>("SpriteNode");
        SpriteNode.FlipH = !bFacingLeft;
        SpriteNode.Position = new Vector2(SpriteNode.Position.X * (bFacingLeft ? 1f : -1f), SpriteNode.Position.Y);
        ForegroundSpriteNode = GetNode<Sprite2D>("ForegroundSpriteNode");
        ForegroundSpriteNode.FlipH = !bFacingLeft;
        ForegroundSpriteNode.Position = SpriteNode.Position;
        
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
        if (amount > 0f)
            bTookDamageThisExecution = true;
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
        action.OnAnimationStarted(this);
    }
    
    public void AnimateAction()
    {
        pendingAction.OnAnimationStarted(this);
    }

    public void AnimateReaction()
    {
        if (bTookDamageThisExecution)
        {
            SpriteNode.Texture = HurtSprite;
        }
    }
    
    public bool IsDead() { return Health <= 0f; }
    
    public int GetID() { return bFacingLeft ? 0 : 1; }
}
