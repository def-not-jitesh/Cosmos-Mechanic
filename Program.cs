using System; 
using Raylib_cs; 
using static Raylib_cs.Raylib; 
using Game.Render; 
using Game.Utility; 

public class CosmosMechanic {
	public static void Main() {
		InitWindow(Utils.screenWidth, Utils.screenHeight, "cosmos mechanic"); 
		SetTargetFPS(60); 

		Utils.init(); 
		Map map = new Map(); 	
		map.init(); 
		map.LoadMap(Constants.Room.cockpit);

		while (!WindowShouldClose()) {
			BeginDrawing(); 
			ClearBackground(Color.Black); 

			map.update(); 

			EndDrawing(); 
		}

		CloseWindow(); 
	}
}
