using Domains.Scene.Scripts;
using UnityEngine;

namespace Domains.Gameplay.Managers
{
    public abstract class Manager : MonoBehaviour
    {
        protected static string GetSaveFilePath()
        {
            return SaveManager.SaveProgressionFilePath;
        }

        protected abstract void LoadBooleanFlags();
    }
}