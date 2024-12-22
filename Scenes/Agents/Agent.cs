using Godot;

abstract public partial class Agent : CharacterBody2D
{
    protected float Speed = 200.0f;
	protected float JumpVelocity = -200.0f;
    protected int points = 0;

    abstract public void Kill();
    abstract public void AddPoint();
    abstract override public void _PhysicsProcess(double delta);

}