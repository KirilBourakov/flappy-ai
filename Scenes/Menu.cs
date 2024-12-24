using Godot;
using System;

public partial class Menu : Control
{
	public void PlayPressed(){
		GetTree().ChangeSceneToFile("res://Scenes/Play.tscn");
	}
	
	public void TrainPressed(){
		GetTree().ChangeSceneToFile("res://Scenes/Train.tscn");
	}
	
	public void ExitPressed(){
		GetTree().Quit();
	}
}
