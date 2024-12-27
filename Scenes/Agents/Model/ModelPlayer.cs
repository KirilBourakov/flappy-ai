using Godot;
using System;
using System.Linq;

public partial class ModelPlayer : Agent
{
	private const int RAY_NUM = 13;
	private const int INPUT_NUM = RAY_NUM+1;

	public float distance = 0;
	public bool dead {get; set;} = false;

	public NeuralNetwork neuralNetwork;
	private RayCast2D[] inputs = new RayCast2D[RAY_NUM];
	

	public ModelPlayer(){
		this.neuralNetwork = new NeuralNetwork(new int[] {INPUT_NUM, 12, 1});
	}

    public override void _Ready()
    {
    	var children = GetNode<Node2D>("Raycasts").GetChildren();
		int i = 0;
		foreach (RayCast2D child in children.Cast<RayCast2D>())
		{
			if (i < RAY_NUM){
				inputs[i] = child;
				i++;
			} else {
				break;
			}
		}
    }

    public override void _PhysicsProcess(double delta)
	{
		if (dead){
			return;
		}
		this.distance +=  this.Speed * (float)delta;
		
		Vector2 velocity = Velocity;
		velocity += GetGravity() * (float)delta;


		// run neural network
		float[] networkInputs = new float[INPUT_NUM];
		int i = 0;
		foreach (var input in this.inputs){
			if (input.IsColliding()){
				Vector2 collision = input.GetCollisionPoint();
				float distance = (float) Math.Sqrt(
					Math.Pow(collision.X - this.Position.X, 2) +
					Math.Pow(collision.Y - this.Position.Y, 2)
				);

				networkInputs[i] = distance;
			}
			i++;
		}
		networkInputs[INPUT_NUM-1] = this.Velocity.Y;
		bool activated = this.neuralNetwork.Evaluate(networkInputs)[0] == 1;

		// handle jump
		if(activated){
			velocity.Y = JumpVelocity;
		}	

		velocity.X = Speed;
		Velocity = velocity;
		MoveAndSlide();
	}

    public override void Kill()
    {
        this.dead = true;
    }
    public override void AddPoint()
    {
		if(!this.dead){
			this.points++;
		}
    }
	public int GetFitness(){
		return (int) Math.Ceiling(distance / 200f);
	}
}
