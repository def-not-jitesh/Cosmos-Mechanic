namespace Game.Utility; 

public static class Constants {
	public const int aspectRatioX = 16; 
	public const int aspectRatioY = 9; 

	public const int tileCountRow = 60; 
	public const int tileCountColumn = 34; 

	public const float gravity = 9.8f; 

	public enum Room {
		cockpit, 
		utility, 
		surveillance, 
		weapons, 
		storage, 
		astronaut
	}
}