using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public PlayerController starterAssetsInputs;
        public CameraController cameraController;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            cameraController.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }

        public void VirtualCrouchInput(bool virtualCrouchState)
        {
            starterAssetsInputs.CrouchInput(virtualCrouchState);
        }

        public void VirtualSlideInput(bool virtualSlideState)
        {
            starterAssetsInputs.SlideInput(virtualSlideState);
        }

        public void VirtualTagInput(bool virtualTagState)
        {
            starterAssetsInputs.TagInput(virtualTagState);
        }

        public void VirtualPauseInput(bool virtualPauseState)
        {
            starterAssetsInputs.PauseInput(virtualPauseState);
        }

    }

}
