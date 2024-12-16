using Godot;
using System;

public partial class PointGiver : Area2D
{
    public void OnPlayerExit(Node2D body){
        Character c = (Character)body;
        c.addPoint();
    }
}
