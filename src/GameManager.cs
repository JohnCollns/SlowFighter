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

    public enum ExecutionFrame
    {
        Block,
        //MinorAttack,
        //MajorAttack,
        Attack,
        Reaction
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
    private ExecutionFrame executionFrame;
    //[Export] public int NumberOfExecutionFrames = 4;
    [Export] public float ExecutionFramesDuration;
    private float executionFrameTimeElapsed = 0f;
    private ActionBase player0ExecutingAction;
    private ActionBase player1ExecutingAction;
    private bool bWillClash = false;


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

        executionFrameTimeElapsed += (float)delta;
        if (executionFrameTimeElapsed >= ExecutionFramesDuration)
        {
            executionFrame += 1;
            StartExecutionFrame();
        }
    }

    private void StartInputPhase()
    {
        phaseTimeRemaining = InputDuration;
        gamePhase = GamePhase.Input;
        Player0.AnimateAction(DefaultAction);
        Player1.AnimateAction(DefaultAction);
        Player0.bTookDamageThisExecution = false;
        Player1.bTookDamageThisExecution = false;
    }

    private void StartExecutingPhase()
    {
        //phaseTimeRemaining = ExecutionDuration;
        phaseTimeRemaining = 4 * ExecutionFramesDuration;   // How can I make this ExecutionFrame number of entries?
        executionFrame = ExecutionFrame.Block;
        gamePhase = GamePhase.Executing;
        // Player0.AnimateAction();
        // Player1.AnimateAction();
        bWillClash = false;

        player0ExecutingAction = Player0.pendingAction;
        player1ExecutingAction = Player1.pendingAction;
        Player0.pendingAction = DefaultAction;
        Player1.pendingAction = DefaultAction;
        GD.Print($"Executing {executionCounter}: p0: {player0ExecutingAction.ActionName},\tp1: {player1ExecutingAction.ActionName}");
        executionCounter++;

        // bool actionResolved = false;
        // if (player0ExecutingAction.CountersAction(player1ExecutingAction))
        // {
        //     // p0 executes, p1 fails
        //     Player1.TakeDamage(player0ExecutingAction.DamageOnCounter);
        //     player0ExecutingAction.OnActionSucceeded(Player0, Player1);
        //     actionResolved = true;
        //     GD.Print($"  p0: {player0ExecutingAction.ActionName} counters p1: {player1ExecutingAction.ActionName}, dealing {player0ExecutingAction.DamageOnCounter} damage");
        // }
        // if (player1ExecutingAction.CountersAction(player0ExecutingAction))
        // {
        //     // p1 executes, p0 fails
        //     Player0.TakeDamage(player1ExecutingAction.DamageOnCounter);
        //     player0ExecutingAction.OnActionSucceeded(Player1, Player0);
        //     actionResolved = true;
        //     GD.Print($"  p1: {player1ExecutingAction.ActionName} counters p0: {player0ExecutingAction.ActionName}, dealing {player1ExecutingAction.DamageOnCounter} damage");
        // }
        //
        // if (!actionResolved)
        // {
        //     Player0.TakeDamage(player1ExecutingAction.DamageOnClash);
        //     Player1.TakeDamage(player0ExecutingAction.DamageOnClash);
        //     GD.Print($"  clashing, taking damage p0: {player1ExecutingAction.DamageOnClash},\tp1: {player0ExecutingAction.DamageOnClash}");
        // }
        
        StartExecutionFrame();
        
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

    private void StartExecutionFrame()
    {
        executionFrameTimeElapsed = 0;
        GD.Print($"Starting execution frame: {executionFrame}");
        switch (executionFrame)
        {
            case ExecutionFrame.Block: HandleBlockFrame(); break;
            // case ExecutionFrame.MinorAttack: HandleMinorAttack(); break;
            // case ExecutionFrame.MajorAttack: HandleMajorAttack(); break;
            case ExecutionFrame.Attack: HandleAttack(); break;
            case ExecutionFrame.Reaction: HandleReaction(); break;
        }
    }

    private void HandleBlockFrame()
    {
        if (player0ExecutingAction.ActionType == ActionBase.ActionBaseType.Block)
        {
            Player0.AnimateAction(player0ExecutingAction);
        }
        if (player1ExecutingAction.ActionType == ActionBase.ActionBaseType.Block)
        {
            Player1.AnimateAction(player1ExecutingAction);
        }
    }

    private void HandleAttack()
    {
        bool actionResolved = false;
        if (player0ExecutingAction.CountersAction(player1ExecutingAction))
        {
            // p0 executes, p1 fails
            Player1.TakeDamage(player0ExecutingAction.DamageOnCounter);
            player0ExecutingAction.OnActionSucceeded(Player0, Player1);
            Player0.AnimateAction(player0ExecutingAction);
            Player1.AnimateAction(player1ExecutingAction);
            actionResolved = true;
            GD.Print($"  p0: {player0ExecutingAction.ActionName} counters p1: {player1ExecutingAction.ActionName}, dealing {player0ExecutingAction.DamageOnCounter} damage");
        }
        if (player1ExecutingAction.CountersAction(player0ExecutingAction))
        {
            // p1 executes, p0 fails
            Player0.TakeDamage(player1ExecutingAction.DamageOnCounter);
            player1ExecutingAction.OnActionSucceeded(Player1, Player0);
            Player0.AnimateAction(player0ExecutingAction);
            Player1.AnimateAction(player1ExecutingAction);
            actionResolved = true;
            GD.Print($"  p1: {player1ExecutingAction.ActionName} counters p0: {player0ExecutingAction.ActionName}, dealing {player1ExecutingAction.DamageOnCounter} damage");
        }

        if (!actionResolved)
        {
            Player0.TakeDamage(player1ExecutingAction.DamageOnClash);
            Player1.TakeDamage(player0ExecutingAction.DamageOnClash);
            Player0.AnimateAction(player0ExecutingAction);
            Player1.AnimateAction(player1ExecutingAction);
            GD.Print($"  clashing, taking damage p0: {player1ExecutingAction.DamageOnClash},\tp1: {player0ExecutingAction.DamageOnClash}");
        }
    }

    private void HandleReaction()
    {
        Player0.AnimateReaction();
        Player1.AnimateReaction();
    }

    // private void HandleMinorAttack()
    // {
    //     if (player0ExecutingAction.ActionType == ActionBase.ActionBaseType.MinorAttack)
    //     {
    //         Player0.AnimateAction(player0ExecutingAction);
    //     }
    //     if (player1ExecutingAction.ActionType == ActionBase.ActionBaseType.MinorAttack)
    //     {
    //         Player1.AnimateAction(player1ExecutingAction);
    //     }
    // }
    //
    // private void HandleMajorAttack()
    // {
    //     if (player0ExecutingAction.ActionType == ActionBase.ActionBaseType.MajorAttack)
    //     {
    //         Player0.AnimateAction(player0ExecutingAction);
    //     }
    //     if (player1ExecutingAction.ActionType == ActionBase.ActionBaseType.MajorAttack)
    //     {
    //         Player1.AnimateAction(player1ExecutingAction);
    //     }
    // }

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
