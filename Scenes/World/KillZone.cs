using Godot;
using System;

public partial class KillZone : Area2D
{
    private Timer timer;
    public override void _Ready(){
        timer = GetNode<Timer>("Timer");
    }

    public void Collide(Node2D body){
        if (body is Character character)
        {
            this.timer.Start();
            Engine.TimeScale = .5;
            character.Kill();
        }
        else if (body is ModelPlayer modelPlayer){
            modelPlayer.Kill();
        }
        
    }

    public void DeathTimerComplete(){
        Engine.TimeScale = 1;
        GetTree().ReloadCurrentScene();
    }
}
