using UnityEngine;

namespace Domains.Gameplay.Mining.Scripts
{
    public interface IMinable
    {
        void MinableMineHit();
        void MinableFailHit(Vector3 hitPoint);

        int GetCurrentMinableHardness();
        void ShowMineablePrompt();
    }
}