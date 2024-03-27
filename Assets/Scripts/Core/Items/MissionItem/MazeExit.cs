using UnityEngine;

public class MazeExit : MissionItem
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Interact();
        }
    }
}


