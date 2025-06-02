using System;

namespace Domains.Player.Scripts
{
    [Serializable]
    public static class CharacterResourcePaths
    {
        public static string CharacterStatProfileFilePath =>
            // "CharacterStatsProfiles/MediumProfile";
            "CharacterStatsProfiles/AdminProfile";
        // "CharacterStatsProfiles/BasicProfile";

        public static string GameLevelStatProfileFilePath =>
            "GameLevelStatsProfiles/MainSceneLevelStats";
    }
}