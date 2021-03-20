using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject objectToFollow;

    public float speed = 2.0f;

    void Start()
    {
        Debug.Log("In The FollowPlayer Script");
    }

    void Update()
    {
        float interpolation = speed * Time.deltaTime;

        Vector3 position = this.transform.position;
        /*position.y = Mathf.Lerp(this.transform.position.y, 0 , interpolation);
        Debug.Log("Position Y: " + " Transform Position Y: " +  this.transform.position.y + "ObjectToFollow Transform Position Y: " + objectToFollow.transform.position.y + "Interpolation " + interpolation);*/
        position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, interpolation);

        this.transform.position = position;

        //Debug.Log("Camera Position: " + this.transform.position);
    }
}
