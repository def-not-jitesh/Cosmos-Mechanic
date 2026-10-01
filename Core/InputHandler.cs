using System; 
using Raylib_cs; 
using static Raylib_cs.Raylib; 

using Game.Utility; 
using Game.Character; 

namespace Game.Core; 

public static class InputHandler {
	
	public static event EventHandler<Player.PlayerActions> InputEvent; 

	public static void update() {
		if (IsKeyDown(KeyboardKey.A)) {
			InputEvent?.Invoke(null, Player.PlayerActions.FaceLeft); 
		}

		if (IsKeyDown(KeyboardKey.S)) {
			InputEvent?.Invoke(null, Player.PlayerActions.FaceRight); 
		}
		
		if (IsKeyDown(KeyboardKey.Space)) {
			InputEvent?.Invoke(null, Player.PlayerActions.Jump); 
		}

		if (IsKeyDown(KeyboardKey.Q)) {
			InputEvent?.Invoke(null, Player.PlayerActions.SwitchGravity); 
		}
	}
}
