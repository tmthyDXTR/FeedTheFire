﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    /// <summary>
    /// This class handles the selection through input
    /// </summary>
    #region Variables
    [SerializeField]
    public bool isActive = true;
    public List<GameObject> currentSelected = new List<GameObject>();
    
    private Selectable _selectableObject;

    #endregion


    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var clickedObj = GetClickedObject();
                Debug.Log("Clicked on " + clickedObj.transform.parent.gameObject.name);

                // Check if Hit Box collider clicked
                Selectable _selectable = clickedObj.transform.parent.gameObject.GetComponent<Selectable>();
                if (_selectable != null)
                {
                    DeselectAll();
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
    }

    public void SetSelectManagerActive(bool value)
    {
        isActive = value;
        if (isActive)
            Debug.Log("Selection Manager activated");
        else
            Debug.Log("Selection Manager deactivated");
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
        GameObject clickedObj = null;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                clickedObj = hit.collider.gameObject;
            }
        }
        return clickedObj;
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
