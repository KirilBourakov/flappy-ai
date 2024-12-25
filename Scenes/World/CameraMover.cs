using Godot;
using System;

public partial class CameraMover : Node2D
{
	const int Y = -65;
	public Label infoLabel; 

	public override void _Ready(){
		infoLabel = GetNode<Label>("Camera2D/InfoLabel");
	}
	public override void _Process(double delta)
	{
		this.GlobalPosition = new Vector2(this.GlobalPosition.X + (float)(200*delta), Y);
		ModelState.Instance.currentGenerationDistancedTraveled += 200f * (float) delta;
		infoLabel.Text = $"Generation: {ModelState.Instance.generationNumber} \nFurthest Distance: {(int) ModelState.Instance.furthestDistanceTraveled}px \nCurrent Distance: {(int) ModelState.Instance.currentGenerationDistancedTraveled}px";
	}
}
