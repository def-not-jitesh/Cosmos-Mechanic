using System; 
using System.Collections.Generic; 

namespace Game.Core;

public enum GameSystems {
    Player, 
    Renderer
}

public static class Logger {
    public static Dictionary<GameSystems, bool> logSystem = new Dictionary<GameSystems, bool> {
        {GameSystems.Renderer, false}, 
        {GameSystems.Player, false},  
    }; 

    public static void Log(GameSystems system, string message) {
        if (logSystem[system]) {
            Console.WriteLine($"[{system}]: {message}"); 
        }
    }

    public static void Enable(GameSystems system) { logSystem[system] = true; }
    public static void Disable(GameSystems system) { logSystem[system] = false; }
    public static bool CheckEnable(GameSystems system) { return logSystem[system]; }
}