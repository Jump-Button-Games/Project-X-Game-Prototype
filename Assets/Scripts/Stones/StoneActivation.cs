using UnityEngine;

[HelpURL("https://learn.unity.com/tutorial/introduction-to-particle-systems#5cf7ca71edbc2a09d0290dcb")]
public class StoneActivation : MonoBehaviour
{

    [Header("Player Input Controls")]
    PlayerInputContoller playerInputController;

    [Header("Particle System Controls")]
    ParticleSystem particleSys;

    void Awake()
    {
        playerInputController = new PlayerInputContoller();

        particleSys = GetComponentInChildren<ParticleSystem>();
        
        // Press '1' is activate
        playerInputController.Player.ActivateStones.performed += _ => UseRuby();

    }

    void OnEnable()
    {
        playerInputController.Enable();
    }

    void OnDisable()
    {
        playerInputController.Disable();
    }

    void UseRuby()
    {

        if (StoneManager.stoneCounter > 0)
        {
            particleSys.Play();
        }

        StoneManager.DecrementStoneCounter();
        
    }
}
