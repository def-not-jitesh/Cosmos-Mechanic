using System; 
using Raylib_cs; 
using static Raylib_cs.Raylib; 

using Game.Core; 
using Game.Render; 
using Game.Utility; 
using Game.Character; 

public class CosmosMechanic {

	private bool playerDead = false; 

	public static void Main() {
		InitWindow(Utils.screenWidth, Utils.screenHeight, "cosmos mechanic"); 
		SetTargetFPS(60); 

		// initialize objects 
		Utils.init(); 
		// Logger.Enable(GameSystems.Player); 
		Logger.Enable(GameSystems.Renderer);
		Player player = new Player(); 
		Map map = new Map(); 
		map.LoadMap(Constants.Room.cockpit);

		// subscribe to events 
		InputHandler.InputEvent += player.onInput; 
		
		while (!WindowShouldClose() && !player.isDead) {
			float deltaTime = GetFrameTime();
			BeginDrawing(); 
			ClearBackground(Color.Black); 
			
			InputHandler.update(); 
			map.update(); 
			player.update(deltaTime); 

			EndDrawing(); 
		}

		CloseWindow(); 
	}
}