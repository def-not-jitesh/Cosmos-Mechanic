using System; 
using Raylib_cs; 
using static Raylib_cs.Raylib; 

using Game.Utility; 

namespace Game.Core; 

public class InputHandler {
	
	public event EventHandler<Utility.GameActions> InputEvent; 

	public void update() {
		if (IsKeyDown(KeyboardKey.A)) {
			if (IsKeyDown(KeyboardKey.LeftShift) || IsKeyDown(KeyboardKey.RightShift)) InputEvent?.Invoke(this, Utility.GameActions.RunRight); 
			else InputEvent?.Invoke(this, Utility.GameActions.moveRight); 
		}

		if (IsKeyDown(KeyboardKey.S)) {
			if (IsKeyDown(KeyboardKey.LeftShift) || IsKeyDown(KeyboardKey.RightShift)) InputEvent?.Invoke(this, Utility.GameActions.RunLeft); 
			else InputEvent?.Invoke(this, Utility.GameActions.moveLeft); 
		}
		
		if (IsMouseButtonPressed(MouseButton.Right)) {
			InputEvent?.Invoke(this, Utility.GameActions.Attack); 
		}
		
		if (IsKeyDown(KeyboardKey.Space)) {
			InputEvent?.Invoke(this, Utility.GameActions.Jump); 
		}

		if (IsKeyDown(KeyboardKey.Q)) {
			InputEvent?.Invoke(this, Utility.GameActions.AntiGravity); 
		}
	}
}
