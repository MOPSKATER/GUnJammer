using MelonLoader;

namespace GUnjammer
{
    public class Main : MelonMod
    {
        public override void OnApplicationLateStart()
        {
            GameDataManager.powerPrefs.dontUploadToLeaderboard = true;
            GS.ToggleSavingAllowed(false);
        }
    }
}