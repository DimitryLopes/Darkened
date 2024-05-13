using System.Collections.Generic;
using UnityEngine;

public class ItemHUDKeyboardSelector : MonoBehaviour
{
    private List<UIItemView> views;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            views[0].Select();
        else if(Input.GetKeyDown(KeyCode.Alpha2))
            views[1].Select();
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            views[2].Select();
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            views[3].Select();
        //else if (Input.GetKeyDown(KeyCode.Alpha5))
        //    views[4].Select();
    }

    public void SetViews(List<UIItemView> views)
    {
        this.views = views;
    }
}
