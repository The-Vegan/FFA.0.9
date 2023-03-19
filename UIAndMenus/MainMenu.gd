extends Control
#DEPRECIATED : REWRITEN IN C#

var camera : Camera2D

var gameMode: int
var playerCharacter : int = 0
var team : int = 1
var chosenTeam :int = 0
var numberOfPlayer : int = 1

var numberOfEntities : int = 12
var numberOfPlayers : int = 1

#Camera positions for menus
var back :Array = [Vector2(0,0)]
const MAINMENU = Vector2(0,0)
const SOLO = Vector2(0,-576)
const CHARSELECT = Vector2(-1024,0)
const LEVELSELECT = Vector2(-2048,0)




func _ready():
	camera = $Camera2D
	pass # Replace with function body.

func MoveCameraTo(var destination : int):
	
	if (destination == -1):#Back
		
		camera.position = back.pop_back()
		
		if (back.size() == 0):
			back.push_back(MAINMENU)
		
		return
	
	back.push_back(camera.position)
	
	match(destination):
		0:#main menu
			camera.position = MAINMENU
		1:#solo
			camera.position = SOLO
		2:#character selection
			camera.position = CHARSELECT
		3:#level selection
			camera.position = LEVELSELECT

func SetGame(mode : int):
	
	match(mode):
		0:#Classic
			print("Classic")
		1:#Team
			print("Team")
		2:#CTF
			print("CTF")
		3:#Siege
			print("Siege")
	
	if(mode != -1):
		gameMode = mode
	
	pass

func setCharacter(character : int):
	playerCharacter = character;
	
	match(playerCharacter):
		0:#None
			print("Random")
		1:#Pirate
			print("Pirate")
		2:#Blahaj
			print("Blahaj")
		3:#Monstropis
			print("Monstropis")
		_:
			print("Err -> Random, ID is : " + str(playerCharacter))



func DisplayErr():
	var tween = $Error/Tween
	
	$Error.modulate.a = 1
	yield(get_tree().create_timer(2),"timeout")
	tween.interpolate_property($Error.modulate,"a",1,0,3,Tween.TRANS_SINE,Tween.EASE_IN)
	
	
	


