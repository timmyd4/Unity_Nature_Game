using UnityEngine;

public class StaffAnimationController : MonoBehaviour
{
    public Animator staffAnimator; // Animator for staff
    

    public void PlayAnimationAndParticles()
    {
        // Trigger the shooting animation
        if (staffAnimator != null)
        {
            staffAnimator.SetTrigger("PlayStaffAnimation");
        }

        
    }
}
