using Godot;
using System;

public partial class InputTimer : Node
{
    [Export] public float FullWidthLiteral { get; set; }
    [Export] public float FullWidthScale { get; set; }
    [Export] public float Height { get; set; }
    [Export] public Color FullColour { get; set; }
    [Export] public Color EmptyColour { get; set; }
    
    private ColorRect colorRect;

    public override void _EnterTree()
    {
        base._EnterTree();
        colorRect = GetNode<ColorRect>("ColorRect");
        Initialise();
    }

    public override void _Ready()
    {
        Initialise();
    }

    public void SetRemainingTimePercent(float percent)
    {
        // Lerp width between FullWidth and 0. 
        // Lerp colour between FullColour and EmptyColour
        //colorRect.Size = new Vector2(Mathf.Lerp(FullWidth, 0f, percent), Height);
        colorRect.Scale = new Vector2(Mathf.Lerp(FullWidthScale, 0f, 1f - percent), 1f);
        colorRect.Color = FullColour.Lerp(EmptyColour, 1f - percent);
        //GD.Print($"SetTimeRem: {percent}, Scale: {colorRect.Scale.X}");
    }

    private void Initialise()
    {
        colorRect.Color = FullColour;
        colorRect.PivotOffset = new Vector2(FullWidthLiteral / 2f, 0f);
        colorRect.Position = new Vector2(-FullWidthLiteral / 2f, 0f);
        colorRect.Size = new Vector2(FullWidthLiteral, Height);
        colorRect.Position = new Vector2(-FullWidthLiteral / 2f, -Height / 2f);
    }
}
