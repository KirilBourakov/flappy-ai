using Godot;
using System;

public partial class ground : StaticBody2D
{
	private Node2D _ground;
    private StaticBody2D _staticBody2D;

	public override void _Ready()
	{
		_ground = GetParent<Node2D>();
        _staticBody2D = this;
	}

	public void Add(){
		StaticBody2D copyNode = (StaticBody2D)_staticBody2D.Duplicate();
		copyNode.Position = new Vector2(_staticBody2D.Position.X + 330, _staticBody2D.Position.Y);

		_ground.AddChild(copyNode);
	}

	public void Remove()
    {
        _staticBody2D.QueueFree();
    }
}
