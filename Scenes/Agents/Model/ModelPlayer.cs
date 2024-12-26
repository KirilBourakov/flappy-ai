using Godot;
using System;
using System.Linq;

public partial class ModelPlayer : Agent
{
	public bool dead {get; set;} = false;

	public double[] weights;

	private const int INPUT_NUM = 13;
	private RayCast2D[] inputs = new RayCast2D[INPUT_NUM];
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
			if (i < INPUT_NUM){
				inputs[i] = child;
				i++;
			} else {
				break;
			}
		}
    }

	public void InitWeights(){
		Random random = new Random();
		this.weights = new double[INPUT_NUM];
		for (int i = 0; i < INPUT_NUM; i++){
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
				Vector2 collision = input.GetCollisionPoint();
				double distance = Math.Sqrt(
					Math.Pow(collision.X - this.Position.X, 2) +
					Math.Pow(collision.Y - this.Position.Y, 2)
				);

				total += distance * this.weights[i];
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
