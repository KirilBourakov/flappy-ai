using Godot;
using System;
using System.Linq;

public partial class ModelPlayer : Agent
{
	public bool dead {get; set;} = false;

	public double[] weights;

	private RayCast2D[] inputs = new RayCast2D[9];
	public float distance = 0;


	public ModelPlayer(){
		this.InitWeights();
	}

    public override void _Ready()
    {
    	var children = GetNode<Node2D>("Raycasts").GetChildren();
		int i = 0;
		foreach (RayCast2D child in children.Cast<RayCast2D>())
		{
			if (i < 9){
				inputs[i] = child;
				i++;
			} else {
				break;
			}
		}
    }

	public void InitWeights(){
		Random random = new Random();
		this.weights = new double[9];
		for (int i = 0; i < 9; i++){
			this.weights[i] = (float)(random.NextDouble() * 20 - 10);
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


		// Handle Jump.
		int i = 0;
		double total = 0;
		foreach (var input in this.inputs){
			if (input.IsColliding()){
				total += this.weights[i];
			}
			i++;
		}
		bool activated = this.StepFunction(total);

		if(activated){
			velocity.Y = JumpVelocity;
		}	

		velocity.X = Speed;
		Velocity = velocity;
		MoveAndSlide();
	}

	private bool StepFunction(double inp){
		return inp > 0;
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
