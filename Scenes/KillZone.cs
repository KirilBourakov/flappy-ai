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
        GD.Print("die");
    }

    public void DeathTimerComplete(){
        GetTree().ReloadCurrentScene();
    }
}
