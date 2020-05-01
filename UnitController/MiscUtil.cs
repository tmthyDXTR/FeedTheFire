using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscUtil : MonoBehaviour
{
    private SelectionManager _select;

    /// <summary>
    /// Variables for Line Draw
    /// </summary>
    [SerializeField]
    private bool drawSpellArrow = false;
    private GameObject arrowCap;
    private GameObject arrowBody;
    private float startScale;
    void Start()
    {
        _select = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
    }

    void Update()
    {
        if (drawSpellArrow)
        {
            DrawArrow(new Vector3(this.transform.position.x, 0.1f, this.transform.position.z), _select.GetMousePos());
        }
    }

    public void DrawArrow(Vector3 start, Vector3 end)
    {
        if (arrowCap == null)
        {            
            arrowCap = Instantiate(Resources.Load("arrowCap"),
                start,
                Quaternion.identity) as GameObject;
        }

        if (arrowBody == null)
        {
            arrowBody = Instantiate(Resources.Load("arrowBody"),
                start,
                Quaternion.identity) as GameObject;
            startScale = arrowBody.transform.GetChild(0).localScale.y;
        }
        else
        {
            var direction = (end - start).normalized;

            //var vect = this.transform.position + (direction / 2);
            arrowCap.transform.Rotate(direction);

            var dist = Vector3.Distance(start, end);
            var mid = dist / 2;
            arrowCap.transform.position = start + direction * dist;
            arrowCap.transform.LookAt(this.transform);
            arrowBody.transform.position = start + direction * mid;
            arrowBody.transform.LookAt(this.transform);
            arrowBody.transform.GetChild(0).localScale = new Vector3(0.15f, startScale * (dist - 0.25f), 0);
        }               
    }
}
