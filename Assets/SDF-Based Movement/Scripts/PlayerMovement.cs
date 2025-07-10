using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    public float PlayerRadius;

    private void OnValidate()
    {
        transform.localScale = new Vector3(PlayerRadius, PlayerRadius, 1);
    }

    private void FixedUpdate()
    {
        transform.position = SDFMovement.instance.GetValidPositionBySDF(transform.position, InputManager.Movement, Speed, PlayerRadius);
    }
}
