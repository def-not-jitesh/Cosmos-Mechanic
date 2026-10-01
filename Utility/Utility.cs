using System; 
using System.Collections.Generic; 

namespace Game.Utility; 

public static class Utils {

	public static string basePath = AppContext.BaseDirectory; 
	public static Dictionary<Constants.Room, string> roomMapSourceFile = new Dictionary<Constants.Room, string>(); 
	public static List<string> tilesetSourceFiles = new List<string>(); 

	public static int screenWidth = 1920;
       	public static int screenHeight = 1080; 	
	
	public static void init() {
		roomMapSourceFile.Add(Constants.Room.cockpit, "cockpit.tmx"); 

		tilesetSourceFiles.Add("tiles_cosmos_mechanic.tsx");
		tilesetSourceFiles.Add("objects_cosmos_mechanic.tsx");
	}
}	