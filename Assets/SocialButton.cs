using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using joneyjs.EasyDebug;

public class SocialButton : MonoBehaviour
{
    float alpha = 1;
    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    void Update() {
        alpha = Mathf.Lerp(alpha, 1, .1f);
        image.color = new Color(1, 1, 1, alpha);
        
        if (MouseOver())
        {            
            alpha = .7f;

            if (Input.GetMouseButtonDown(0)) {      
                int cellX = cellXfromSocialButtonName( gameObject.name);
                int cellY = cellYfromSocialButtonName(gameObject.name);

                EasyDebugHelper.log("cellX: " + cellX + ", cellY:" + cellY);
                GameManager.managerInstance.OnClickInSocialCell(cellX, cellY);
            }
        }
    }

    private int cellXfromSocialButtonName(string socialButtonName) {
        return int.Parse(socialButtonName.Substring(7, 1));
    }

    private int cellYfromSocialButtonName(string socialButtonName){
        return int.Parse(socialButtonName.Substring(9, 1));
    }
    
    public bool MouseOver() {      
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (var go in raycastResults)
                {
                    if (go.gameObject.name == name)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    
}
