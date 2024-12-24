using Godot;
using System;

public partial class Menu : Control
{
	public void PlayPressed(){
		GetTree().ChangeSceneToFile("Play.tscn");
	}
	
	public void TrainPressed(){
		return;
		// TODO: create Train Scene
		GetTree().ChangeSceneToFile("Train.tscn");
	}
	
	public void ExitPressed(){
		GetTree().Quit();
	}
}
