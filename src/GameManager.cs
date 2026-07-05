using Godot;
using System;

public partial class GameManager : Node
{
    public enum GamePhase
    {
        Pregame,
        Input,
        //Animating,  // Not yet in use
        Executing,
        Finished
    }
    
    [Export] public Character Player0;
    [Export] public Character Player1;

    [Export] public float InputDuration;
    [Export] public float ExecutionDuration;
    [Export] public ActionBase DefaultAction;
    
    [Export] public bool MusicEnabled = true;
    [Export] public AudioStream MusicTrack;
    
    private InputListener inputListener;
    private InputTimer inputTimer;
    private AudioStreamPlayer audioStreamPlayer;
    private HealthBar HealthBar0;
    private HealthBar HealthBar1;
    
    private GamePhase gamePhase;
    private double phaseTimeRemaining;
    private int executionCounter = 0;


    public override void _Ready()
    {
        gamePhase = GamePhase.Pregame;
        inputListener = GetNode<InputListener>("InputListener");
        inputTimer = GetNode<InputTimer>("InputTimer");
        audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        HealthBar0 = GetNode<HealthBar>("HealthBar0");
        HealthBar1 = GetNode<HealthBar>("HealthBar1");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        switch (gamePhase)
        {
            case GamePhase.Pregame:
                break;
            case GamePhase.Input:
                HandleInputProcess(delta);
                break;
            case GamePhase.Executing:
                HandleExecutingProcess(delta);
                break;
        }
    }

    private void HandleInputProcess(double delta)
    {
        phaseTimeRemaining -= delta;
        inputTimer.SetRemainingTimePercent(MathF.Max(0f, (float)phaseTimeRemaining / InputDuration));
        if (phaseTimeRemaining <= 0f)
            StartExecutingPhase();
    }

    private void HandleExecutingProcess(double delta)
    {
        
        phaseTimeRemaining -= delta;
        if (phaseTimeRemaining <= 0f)
            StartInputPhase();          // may want to change this later
    }

    private void StartInputPhase()
    {
        phaseTimeRemaining = InputDuration;
        gamePhase = GamePhase.Input;
        Player0.pendingAction = DefaultAction;
        Player0.AnimateAction();
        Player1.pendingAction = DefaultAction;
        Player1.AnimateAction();
    }

    private void StartExecutingPhase()
    {
        phaseTimeRemaining = ExecutionDuration;
        gamePhase = GamePhase.Executing;
        Player0.AnimateAction();
        Player1.AnimateAction();

        ActionBase player0Action = Player0.pendingAction;
        ActionBase player1Action = Player1.pendingAction;
        GD.Print($"Executing {executionCounter}: p0: {player0Action.ActionName},\tp1: {player1Action.ActionName}");
        executionCounter++;

        bool actionResolved = false;
        if (player0Action.CountersAction(player1Action))
        {
            // p0 executes, p1 fails
            Player1.TakeDamage(player0Action.DamageOnCounter);
            player0Action.OnActionSucceeded(Player0, Player1);
            actionResolved = true;
            GD.Print($"  p0: {player0Action.ActionName} counters p1: {player1Action.ActionName}, dealing {player0Action.DamageOnCounter} damage");
        }
        if (player1Action.CountersAction(player0Action))
        {
            // p1 executes, p0 fails
            Player0.TakeDamage(player1Action.DamageOnCounter);
            player0Action.OnActionSucceeded(Player1, Player0);
            actionResolved = true;
            GD.Print($"  p1: {player1Action.ActionName} counters p0: {player0Action.ActionName}, dealing {player1Action.DamageOnCounter} damage");
        }

        if (!actionResolved)
        {
            Player0.TakeDamage(player1Action.DamageOnClash);
            Player1.TakeDamage(player0Action.DamageOnClash);
            GD.Print($"  clashing, taking damage p0: {player1Action.DamageOnClash},\tp1: {player0Action.DamageOnClash}");
        }
        
        if (Player0.IsDead() || Player1.IsDead())
            HandleFinishedPhase();
    }

    private void HandleFinishedPhase()
    {
        gamePhase = GamePhase.Finished;
        if (Player0.IsDead() && Player1.IsDead())
        {
            GD.Print("Draw");
        }
        else if (Player0.IsDead())
        {
            GD.Print("Player 1 Wins");
        }
        else if (Player1.IsDead())
        {
            GD.Print("Player 0 Wins");
        }
    }

    public void BeginGame()
    {
        Player0.Restart();
        Player1.Restart();
        StartMusic();
        StartInputPhase();
    }

    private void StartMusic()
    {
        if (MusicEnabled && !audioStreamPlayer.IsPlaying())
        {
            audioStreamPlayer.SetStream(MusicTrack);
            audioStreamPlayer.Play();
        }
    }

    public Character GetPlayer(int index)
    {
        return index == 0 ? Player0 : Player1;
    }

    public HealthBar GetHealthBar(int index)
    {
        return index == 0 ? HealthBar0 : HealthBar1;
    }
}
