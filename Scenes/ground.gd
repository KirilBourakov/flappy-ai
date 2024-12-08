extends StaticBody2D

@onready var ground: Node2D = $".."
@onready var static_body_2d: StaticBody2D = $"."

func add() -> void:
	print('enter')
	var copynode = static_body_2d.duplicate();
	copynode.position.x = static_body_2d.position.x + 330
	
	ground.add_child(copynode)
	
	
func remove() -> void:
	print('exit')
	static_body_2d.queue_free();
