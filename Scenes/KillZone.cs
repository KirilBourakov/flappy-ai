using Godot;
using System;

public partial class KillZone : Area2D
{
    private Timer timer;
    public override void _Ready(){
        timer = GetNode<Timer>("Timer");
    }

    public void Collide(Node2D body){
        this.timer.Start();
        Engine.TimeScale = .5;
        Character c = (Character) body;
        c.Kill();
        GD.Print("die");
    }

    public void DeathTimerComplete(){
        Engine.TimeScale = 1;
        GetTree().ReloadCurrentScene();
    }
}
