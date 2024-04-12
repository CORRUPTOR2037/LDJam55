using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSearcher : MonoBehaviour
{
    public float MaxDistance;
    public GameObject selectedObject;

    RaycastHit hitInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    /*void Update()
    {

        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, MaxDistance, LayerMask.GetMask("Selectable"))) {
            Select(hitInfo.collider.gameObject);
        } else {
            if (selectedObject != null)
                Unselect(selectedObject);
            selectedObject = null;
        }
    }

    void Select(GameObject obj) {
        if (selectedObject == obj) return;

        selectedObject = obj;
        Outline outline = obj.GetComponent<Outline>();
        if (outline == null) {
            outline = obj.AddComponent<Outline>();
            outline.OutlineColor = Color.white;
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineWidth = 10;
        }
        outline.enabled = true;
    }

    void Unselect(GameObject obj) {
        obj.GetComponent<Outline>().enabled = false;
    }*/
}
