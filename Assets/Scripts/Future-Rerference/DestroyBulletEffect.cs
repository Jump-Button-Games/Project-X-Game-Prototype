using UnityEngine;

/*
 * 
 *  Keeping this script as it contains code which may be useful later
 *  This script currently contains code to get the length of an animation clip
 * 
 */

public class DestroyBulletEffect : MonoBehaviour {

    private float clipTime;

    private Animator animator;
    private AnimationClip[] clips;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start() {

       clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Bullet-Impact-Effect-Clip":
                    clipTime = clip.length;
                    break;
            }
        }
    }

    void Update()
    {
        //Destroy(gameObject, clipTime);
    }
}
