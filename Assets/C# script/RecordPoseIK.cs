using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

namespace RootMotion.Demos
{

    /// <summary>
    /// Demo script that shows how BipedIK performs compared to the built-in Animator IK
    /// </summary>
    public class RecordPoseIK : MonoBehaviour
    {

        [LargeHeader("References")]
        public BipedIK bipedIK;

        // Look At
        [LargeHeader("Look At")]
        public Transform lookAtTargetBiped;
        [Range(0f, 1f)] public float lookAtWeight = 1f;
        [Range(0f, 1f)] public float lookAtBodyWeight = 1f;
        [Range(0f, 1f)] public float lookAtHeadWeight = 1f;
        [Range(0f, 1f)] public float lookAtEyesWeight = 1f;
        [Range(0f, 1f)] public float lookAtClampWeight = 0.5f;
        [Range(0f, 1f)] public float lookAtClampWeightHead = 0.5f;
        [Range(0f, 1f)] public float lookAtClampWeightEyes = 0.5f;

        // Foot
        [LargeHeader("Foot")]
        public Transform footTargetBiped;
        [Range(0f, 1f)] public float footPositionWeight = 0f;
        [Range(0f, 1f)] public float footRotationWeight = 0f;

        // Hand
        [LargeHeader("Hand")]
        public Transform handTargetBiped;
     
        [Range(0f, 1f)] public float handPositionWeight = 0f;
        [Range(0f, 1f)] public float handRotationWeight = 0f;

        void OnAnimatorIK(int layer)
        {
            
            // Look At
            

            bipedIK.SetLookAtPosition(lookAtTargetBiped.position);
            bipedIK.SetLookAtWeight(lookAtWeight, lookAtBodyWeight, lookAtHeadWeight, lookAtEyesWeight, lookAtClampWeight, lookAtClampWeightHead, lookAtClampWeightEyes);




            // Foot

            

            bipedIK.SetIKPosition(AvatarIKGoal.LeftFoot, footTargetBiped.position);
            bipedIK.SetIKRotation(AvatarIKGoal.LeftFoot, footTargetBiped.rotation);
            bipedIK.SetIKPositionWeight(AvatarIKGoal.LeftFoot, footPositionWeight);
            bipedIK.SetIKRotationWeight(AvatarIKGoal.LeftFoot, footRotationWeight);

//hand
            bipedIK.SetIKPosition(AvatarIKGoal.LeftHand, handTargetBiped.position);
            bipedIK.SetIKRotation(AvatarIKGoal.LeftHand, handTargetBiped.rotation);
            bipedIK.SetIKPositionWeight(AvatarIKGoal.LeftHand, handPositionWeight);
            bipedIK.SetIKRotationWeight(AvatarIKGoal.LeftHand, handRotationWeight);

            
        }
    }
}
