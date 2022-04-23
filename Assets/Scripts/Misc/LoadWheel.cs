using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadWheel : MonoBehaviour
{
    bool load = false;
    public bool Load
    {
        get { return load; }
        set 
        { 
            load = value;

            spriteSheetAnimation.loop = value;
            spriteSheetAnimation.image.enabled = value;
        }
    }

    SpriteSheetAnimation spriteSheetAnimation;

    Canvas canvas;

    void Awake()
    {
        spriteSheetAnimation = GetComponent<SpriteSheetAnimation>();
        canvas = GetComponentInParent<Canvas>();
        Load = load;
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(1)) Load = true;
        //if (Input.GetMouseButtonUp(1)) Load = false;

        if (Load)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
            transform.position = canvas.transform.TransformPoint(pos);
        }
    }
}
