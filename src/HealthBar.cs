using Godot;
using System;

public partial class HealthBar : Node
{
    [Export] public float FullWidthLiteral { get; set; }
    [Export] public float FullWidthScale { get; set; }
    [Export] public float Height { get; set; }
    [Export] public Color FullColour { get; set; }
    [Export] public Color EmptyColour { get; set; } 
    
    private ColorRect ColourRect;
    private bool bFacingLeft;

    public override void _Ready()
    {
        base._Ready();
        ColourRect = GetNode<ColorRect>("ColourRect");
    }

    public void SetFacingLeft(bool bFacingLeft_)
    {
        bFacingLeft = bFacingLeft_;
        Initialise();
    }
    
    public float GetDirectionMod() { return bFacingLeft ? -1 : 1; }
    
    private void Initialise()
    {
        ColourRect.Color = FullColour;
        ColourRect.PivotOffset = new Vector2(FullWidthLiteral / 2f, 0f);
        ColourRect.Position = new Vector2(-FullWidthLiteral / 2f, 0f);
        ColourRect.Size = new Vector2(FullWidthLiteral, Height);
        ColourRect.Position = new Vector2(-FullWidthLiteral / 2f, -Height / 2f);
    }
    
    public void SetPercent(float percent)
    {
        ColourRect.Scale = new Vector2(Mathf.Lerp(FullWidthScale, 0f, 1f - percent), 1f);
        ColourRect.Color = FullColour.Lerp(EmptyColour, 1f - percent);
    }
}
