using Steel;

namespace SteelCustom
{
    public class GameManager
    {
        public static void EntryPoint()
        {
            Screen.Width = 1366;
            Screen.Height = 768;
            
            SceneManager.SetActiveScene("start_scene.scene");
            
            Log.LogInfo("EntryPoint completed");
        }
    }
}