using UnityEngine;

public class FollowPlayerShadow : MonoBehaviour
{
    [SerializeField] Transform Target;
    void LateUpdate()
    {
        Vector3 newPos = Target.position;

        if (!Target.GetComponent<PlayerMovementCharController>().Grounded )
        {
            newPos.y = transform.position.y;
        }

        transform.position = newPos;
    }
}
