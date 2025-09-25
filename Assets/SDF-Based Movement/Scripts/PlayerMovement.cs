using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    public float PlayerRadius;

    private void OnValidate()
    {
        transform.localScale = new (PlayerRadius, PlayerRadius, 1f);
    }

    private void FixedUpdate()
    {
        transform.position = SDFMovement.Instance.GetValidPositionBySDF(transform.position, InputManager.Movement, Speed, PlayerRadius);
    }
}
