using System; 
using System.Numerics;
using System.Collections.Generic; 
using System.IO; 
using Raylib_cs; 
using static Raylib_cs.Raylib; 

using Game.Utility;
using Game.Core; 

namespace Game.Character; 

public interface IPlayerState {
	public void handleInput(Player player, Player.PlayerActions action); 
	public void update(Player player, float deltaTime); 
}

public class Idle : IPlayerState {
	public virtual void handleInput(Player player, Player.PlayerActions action) {
		switch (action) {
			case Player.PlayerActions.Run: 
				player.currentFrameCount= 0; 
				Logger.Log(GameSystems.Player, "switching to Run state"); 
				player.state = new Run(); 
				break; 
			case Player.PlayerActions.Jump:
				player.currentFrameCount = 0; 
				Logger.Log(GameSystems.Player, "switching to Jump state"); 
				player.state = new Jump(); 
				break; 
		}
	}

	public virtual void update(Player player, float deltaTime) {
		if (!player.onGround) {
			player.dy += Constants.gravity * deltaTime; 
			player.position.Y += player.dy * deltaTime * player.gravityDirection; 
		}

		DrawTexturePro(player.stateTexture[Player.PlayerActions.Idle], 
			new Rectangle(player.currentFrameCount * player.size.X, 0, player.direction * player.size.X, player.gravityDirection * player.size.Y),
			new Rectangle(player.position.X, player.position.Y, player.size.X * player.scalePlayer, player.size.Y * player.scalePlayer),
			new Vector2(0f, 0f), 0f, Color.White); 

		if (player.currentFrameCount >= player.stateFrameCount[Player.PlayerActions.Idle]) player.currentFrameCount = 0;
	}
}

public class Run : IPlayerState {
	public virtual void handleInput(Player player, Player.PlayerActions action) {
		switch (action) {
			case Player.PlayerActions.Jump:
				player.currentFrameCount = 0; 
				Logger.Log(GameSystems.Player, "switching to Jump state"); 
				player.state = new Jump(); 
				break; 
		}

	}

	public virtual void update(Player player, float deltaTime) {

		if (!player.onGround) {
			player.position.X += player.velocity * deltaTime * player.direction; 
			player.dy += Constants.gravity * deltaTime;
			player.position.Y += player.dy * deltaTime * player.gravityDirection; 

			DrawTexturePro(player.stateTexture[Player.PlayerActions.Jump], 
				new Rectangle(player.currentFrameCount * player.size.X, 0, player.direction * player.size.X, player.gravityDirection * player.size.Y), 
				new Rectangle(player.position.X, player.position.Y, player.size.X * player.scalePlayer, player.size.Y * player.scalePlayer), 
				new Vector2(0f, 0f), 0f, Color.White); 

			if (player.currentFrameCount >= player.stateFrameCount[Player.PlayerActions.Jump]) {
				player.currentFrameCount = 0;
				player.state = new Idle();
			}

			return; 
		}
		
		player.position.X += player.velocity * deltaTime * player.direction; 

		DrawTexturePro(player.stateTexture[Player.PlayerActions.Run], 
			new Rectangle(player.currentFrameCount * player.size.X, 0, player.direction * player.size.X, player.gravityDirection * player.size.Y), 
			new Rectangle(player.position.X, player.position.Y, player.size.X * player.scalePlayer, player.size.Y * player.scalePlayer), 
			new Vector2(0f, 0f), 0f, Color.White); 

		if (player.currentFrameCount >= player.stateFrameCount[Player.PlayerActions.Run]) {
			player.currentFrameCount = 0;
			player.state = new Idle(); 
		}
	}
}

public class Jump : IPlayerState {

	public bool jumpStarted = false; 

	public virtual void handleInput(Player player, Player.PlayerActions action) {
		if (action == Player.PlayerActions.Run) {
			player.currentFrameCount = 0; 
			Logger.Log(GameSystems.Player, "switching to Run state");
			player.state = new Run(); 
		}
	}

	public virtual void update(Player player, float deltaTime) {
		if (player.onGround && jumpStarted) {
			player.state = new Idle(); 
			return; 
		}

		if (player.onGround && !jumpStarted) {
			jumpStarted = true; 
			player.position.Y += player.jumpVelocity * deltaTime * player.gravityDirection; 
		} 

		player.dy += Constants.gravity * deltaTime; 
		player.position.Y += player.dy * deltaTime * player.gravityDirection;

		DrawTexturePro(player.stateTexture[Player.PlayerActions.Jump], 
			new Rectangle(player.currentFrameCount * player.size.X, 0, player.direction * player.size.X, player.gravityDirection * player.size.Y), 
			new Rectangle(player.position.X, player.position.Y, player.size.X * player.scalePlayer, player.size.Y * player.scalePlayer), 
			new Vector2(0f, 0f), 0f, Color.White); 

		if (player.currentFrameCount >= player.stateFrameCount[Player.PlayerActions.Jump]) {
			player.currentFrameCount = 0; 
			player.state = new Idle(); 
		}
	}
}

public class Death : IPlayerState {

	// maybe player can die due to something in the future 

	public virtual void handleInput(Player player, Player.PlayerActions action) {}

	public virtual void update(Player player, float deltaTime) {
		DrawTexturePro(player.stateTexture[Player.PlayerActions.Death], 
			new Rectangle(player.currentFrameCount * player.size.X, 0, player.direction * player.size.X, player.gravityDirection * player.size.Y), 
			new Rectangle(player.position.X, player.position.Y, player.size.X * player.scalePlayer, player.size.Y * player.scalePlayer), 
			new Vector2(0f, 0f), 0f, Color.White); 

		if (player.currentFrameCount >= player.stateFrameCount[Player.PlayerActions.Death]) {
			player.currentFrameCount = 0; 
			player.isDead = true; 
			Logger.Log(GameSystems.Player, "player is dead"); 
		}
	}
}

public class Player { 

	public enum PlayerActions { 
		Idle, 
		Run,  
		Jump, 
		Death, 
		FaceLeft, 
		FaceRight, 
		SwitchGravity
	}
	
	public IPlayerState state; 
	public bool isDead = false; 
	public int direction = 1; // to flip the image  
	public int gravityDirection = 1; // 1 means gravity, -1 means anti-gravity
	public int currentFrameCount; 
	public float scalePlayer; 
	public float velocity;
	public float jumpVelocity;
	public float dy; 
	public bool onGround; 
	public float frameTimeCount;
	public float gravitySwitchCount; 

	public Vector2 position; 
	public Vector2 size; 

	public Dictionary<PlayerActions, int> stateFrameCount; 
	public Dictionary<PlayerActions, Texture2D> stateTexture; 

	public Player() {
		state = new Idle(); 
		position.X = 100f;
		position.Y = 100f;
		size.X = 48f; 
		size.Y = 48f; 
		velocity = 10f; 
		onGround = true;
		jumpVelocity = 5f; 
		scalePlayer = 3f;  
		gravitySwitchCount = 0.3f; 

		stateFrameCount = new Dictionary<PlayerActions, int>(); 
		stateFrameCount.Add(PlayerActions.Idle, 4); 
		stateFrameCount.Add(PlayerActions.Run, 6); 
		stateFrameCount.Add(PlayerActions.Jump, 4); 
		stateFrameCount.Add(PlayerActions.Death, 6); 

		stateTexture = new Dictionary<PlayerActions, Texture2D>(); 
		Logger.Log(GameSystems.Player, Path.Combine(Utils.basePath, "Assets", "characters", "cyborg"));
		stateTexture.Add(PlayerActions.Idle, LoadTexture(Path.Combine(Utils.basePath, "Assets", "characters", "cyborg", "Cyborg_idle.png"))); 
		stateTexture.Add(PlayerActions.Run, LoadTexture(Path.Combine(Utils.basePath, "Assets", "characters", "cyborg", "Cyborg_run.png"))); 
		stateTexture.Add(PlayerActions.Jump, LoadTexture(Path.Combine(Utils.basePath, "Assets", "characters", "cyborg", "Cyborg_jump.png"))); 
		stateTexture.Add(PlayerActions.Death, LoadTexture(Path.Combine(Utils.basePath, "Assets", "characters", "cyborg", "Cyborg_death.png"))); 
		
		Logger.Log(GameSystems.Player, "player is initialized"); 
	}

	public void onInput(object sender, PlayerActions e) {
		switch (e) {
			case PlayerActions.SwitchGravity:
				if (gravitySwitchCount <= 0f) {
					gravityDirection = -gravityDirection; 
					gravitySwitchCount = 0.3f; 
					if (gravityDirection > 0) Logger.Log(GameSystems.Player, "normal gravity state");
					else Logger.Log(GameSystems.Player, "anti-gravity state"); 
				}
				// play the animation
				break; 
			case PlayerActions.FaceLeft:
				if (direction > 0) {
					direction = -direction; 
					Logger.Log(GameSystems.Player, "facing left");
				}

				// switching direction implicitly means run, in this case atleast 
				state.handleInput(this, PlayerActions.Run); 
				break; 
			case PlayerActions.FaceRight:
				if (direction < 0) {
					direction = -direction;
					Logger.Log(GameSystems.Player, "facing right"); 
				}

				state.handleInput(this, PlayerActions.Run);
				break; 
			default:
				state.handleInput(this, e); 
				break; 
		}
	}

	public void update(float deltaTime) {
		if (onGround) dy = 0f; 

		frameTimeCount += deltaTime; 
		gravitySwitchCount -= deltaTime; 
		if (frameTimeCount >= Constants.frameTime) {
			currentFrameCount++; 
			frameTimeCount = 0f;
		}

		state.update(this, deltaTime); 
	}
}