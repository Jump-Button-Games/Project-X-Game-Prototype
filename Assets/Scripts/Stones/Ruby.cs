using UnityEngine;

public class Ruby : MonoBehaviour
{

    const string type = "Ruby";
    const string power = "Environmental";

    string rubyName;

    [SerializeField]
    string effect = "effect";

    void Start()
    {
        rubyName = effect + " " + type;
    }

    void OnCollisionEnter2D(Collision2D colliderInfo)
    {
        if (colliderInfo.gameObject.tag == "Player")
        {
            StoneManager.IncrementStoneCounter();
            StoneManager.stoneType = type;
            Destroy(this.gameObject);
        }
    }

}
