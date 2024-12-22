using Godot;

abstract partial class Agent : CharacterBody2D
{
    private float Speed = 200.0f;
	private float JumpVelocity = -200.0f;
    private int points;

    abstract public void Kill();
    abstract public void AddPoint();

}