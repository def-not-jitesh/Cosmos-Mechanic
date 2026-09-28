using System; 
using Raylib_cs; 
using static Raylib_cs.Raylib; 

using Game.Core; 
using Game.Render; 
using Game.Utility; 

public class CosmosMechanic {
	public static void Main() {
		InitWindow(Utils.screenWidth, Utils.screenHeight, "cosmos mechanic"); 
		SetTargetFPS(60); 

		// initialize objects 
		Utils.init(); 
		InputHandler inputHandler = new InputHandler(); 
		Map map = new Map(); 	
		map.init(); 
		map.LoadMap(Constants.Room.cockpit);

		// subscribe to events 
		
		while (!WindowShouldClose()) {
			BeginDrawing(); 
			ClearBackground(Color.Black); 
			
			InputHandler.update(); 
			map.update(); 

			EndDrawing(); 
		}

		CloseWindow(); 
	}
}
