using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public List<GameObject> currentSelected = new List<GameObject>();
    
    private Selectable _selectableObject;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var clickedObj = GetClickedObject();
            Debug.Log("Clicked on " + clickedObj);

            // Check if Hit Box collider clicked
            if (clickedObj.name == "HitBox")
            {
                Select(clickedObj.transform.parent.gameObject);
            }

            // If left clicked on not selectable / ground / other things
            // deselect everything
            else
            {
                DeselectAll();
            }
        }
    }



    private void Select(GameObject obj)
    {
        _selectableObject = obj.GetComponent<Selectable>();
        _selectableObject.isSelected = true;
        // Add object to selection array if not selected yet
        if (!currentSelected.Contains(obj))
        {
            currentSelected.Add(obj);
        }        
        Debug.Log("Selected " + obj.name);
    }

    private void DeselectAll()
    {
        if (currentSelected.Count > 0)
        {
            foreach (GameObject obj in currentSelected)
            {
                _selectableObject = obj.GetComponent<Selectable>();
                _selectableObject.isSelected = false;                
            }
            Debug.Log("Deselected all current selected objects");
        }
        currentSelected.Clear();
    }

    public GameObject GetClickedObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    public Vector3 GetMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return new Vector3(hit.point.x, 0, hit.point.z);
        }
        return Vector3.zero;
    }
}
