using Godot;
using System;

public partial class PointGiver : Area2D
{
    public void OnPlayerExit(Node2D body){
        Agent agent = (Agent)body;
        agent.AddPoint();
    }
}
