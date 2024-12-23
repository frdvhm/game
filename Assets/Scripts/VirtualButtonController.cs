using UnityEngine;

public class VirtualButtonController : MonoBehaviour
{
    public Player player; // Drag prefab Tokoh dengan script Player

    public void MoveRight(bool isPressed)
    {
        if (player != null)
        {
            player.MoveRight(isPressed);
        }
    }

    public void MoveLeft(bool isPressed)
    {
        if (player != null)
        {
            player.MoveLeft(isPressed);
        }
    }

    public void Jump(bool isPressed)
    {
        if (player != null)
        {
            player.Jump(isPressed);
        }
    }
}
