using Rewired;
using UnityEngine;

namespace Domains.Gameplay.DirigibleFlight
{
    [RequireComponent(typeof(DirigibleMovementController))]
    public class DirigibleInput : MonoBehaviour
    {
        // Rewired action indices (Action IDs)
        private const int TurnActionId = 0;
        private const int ThrustActionId = 1;

        private const int ChangeHeightActionId = 2;

        // There is no 3
        private const int ApplyEffectActionId = 4;
        private const int ChangeEffectActionId = 5;
        private const int LookYActionId = 6;
        private const int LookXActionId = 7;
        private const int ZoomActionId = 8;
        public int airshipPlayerId;

        private Rewired.Player airshipPlayer;
        private DirigibleCameraController dirigibleCameraController;

        private DirigibleEffectController dirigibleEffectController;
        private DirigibleMovementController dirigibleMovementController;


        private void Awake()
        {
            airshipPlayer = ReInput.players.GetPlayer(airshipPlayerId);
            dirigibleMovementController = gameObject.GetComponent<DirigibleMovementController>();
            dirigibleCameraController = gameObject.GetComponent<DirigibleCameraController>();
            dirigibleEffectController = GetComponent<DirigibleEffectController>();
        }

        // Update is called once per frame
        private void Update()
        {
            GetInput();
        }

        // Get All Input Values from Rewired Actions
        private void GetInput()
        {
            dirigibleMovementController.turnValue = airshipPlayer.GetAxis(TurnActionId);
            dirigibleMovementController.thrustValue = airshipPlayer.GetAxis(ThrustActionId);
            dirigibleMovementController.changeHeightValue = airshipPlayer.GetAxis(ChangeHeightActionId);

            dirigibleEffectController.applyEffect = airshipPlayer.GetButton(ApplyEffectActionId);
            dirigibleEffectController.changeEffect = airshipPlayer.GetAxis(ChangeEffectActionId);

            dirigibleCameraController.lookYValue = airshipPlayer.GetAxis(LookYActionId);
            dirigibleCameraController.lookXValue = airshipPlayer.GetAxis(LookXActionId);
            dirigibleCameraController.zoomValue = airshipPlayer.GetAxis(ZoomActionId);
        }
    }
}