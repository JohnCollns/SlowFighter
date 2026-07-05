using Godot;
using System;

public partial class HealthBar : Node2D
{
    [Export] public float FullWidthLiteral { get; set; }
    [Export] public float FullWidthScale { get; set; }
    [Export] public float Height { get; set; }
    [Export] public Color FullColour { get; set; }
    [Export] public Color EmptyColour { get; set; }
    [Export] public float EasingFactor = 3f;
    
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
        // ColourRect.PivotOffset = new Vector2(FullWidthLiteral * -GetDirectionMod(), 0f);
        // ColourRect.Position = new Vector2(-FullWidthLiteral * GetDirectionMod(), 0f);
        // ColourRect.Size = new Vector2(FullWidthLiteral * -GetDirectionMod(), Height);
        // ColourRect.Position = new Vector2(-FullWidthLiteral * GetDirectionMod(), ColourRect.Position.Y);
        
        // ColourRect.PivotOffset = new Vector2(FullWidthLiteral, 0f);
        // ColourRect.Position = new Vector2(-FullWidthLiteral, 0f);
        // ColourRect.Size = new Vector2(FullWidthLiteral, Height);
        // ColourRect.Position = new Vector2(-FullWidthLiteral, ColourRect.Position.Y);
        if (bFacingLeft)
        {
            ColourRect.PivotOffset = new Vector2(FullWidthLiteral, 0f);
            ColourRect.Size = new Vector2(FullWidthLiteral, Height);
            ColourRect.Position = new Vector2(-FullWidthLiteral, ColourRect.Position.Y);
        }
        else
        {
            ColourRect.PivotOffset = new Vector2(0f, 0f);
            ColourRect.Position = new Vector2(0f, ColourRect.Position.Y);
            ColourRect.Size = new Vector2(FullWidthLiteral, Height);
            //ColourRect.Position = new Vector2(0, ColourRect.Position.Y);
        }
        SetPercent(1f);
        GD.Print($"HealthBar Init. Left: {bFacingLeft}, Pos: {ColourRect.Position}");
    }
    
    public void SetPercent(float percent)
    {
        percent = Mathf.Max(percent, 0f);
        //percent = PerformEasing(percent);
        // Do some logarithmic adjustment
        ColourRect.Scale = new Vector2(Mathf.Lerp(FullWidthScale, 0f, 1f - percent), 1f);
        ColourRect.Color = FullColour.Lerp(EmptyColour, 1f - percent);
    }

    private float PerformEasing(float percent)
    {
        percent = Mathf.Max(percent, 0f);
        percent = Mathf.Pow(percent, EasingFactor);
        return percent;
    }
}
