using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUIUtil : MonoBehaviour
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

    private GameObject targetArea;
    private GameObject rangeArea;
    void Start()
    {
        _select = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
    }

    public void RemoveUnitDraws()
    {
        RemoveTargetArea();
        RemoveArrow();
        RemoveCircleArea();
    }

    public void DrawUI(Spell spell, float range)
    {
        DrawCircleArea(this.transform.position, range);

        if (spell.form.aim == Form.Aim.Directional)
            DrawArrow(new Vector3(this.transform.position.x, 0.1f, this.transform.position.z), _select.GetMousePos());
        else if (spell.form.aim == Form.Aim.Point || spell.form.aim == Form.Aim.Auto)
            DrawTargetArea(_select.GetMousePos(), 0.5f);
    }

    public void DrawTargetArea(Vector3 position, float radius)
    {   // Draws a circular area at target position (mousPos)
        // indicating aim target
        if (targetArea == null)
        {
            targetArea = Instantiate(Resources.Load("targetArea"),
                position,
                Quaternion.identity) as GameObject;
        }
        else
        {
            targetArea.transform.position = new Vector3 (position.x, 0.1f, position.z);
            targetArea.transform.localScale = new Vector3(radius * 2, 0, radius * 2);
        }
    }

    private void RemoveTargetArea()
    {
        if (targetArea != null)
            GameObject.Destroy(targetArea);
    }

    public void DrawCircleArea(Vector3 position, float radius)
    {   // Draws a circular area at the the target position
        // indicating units cast range
        if (rangeArea == null)
        {
            rangeArea = Instantiate(Resources.Load("rangeArea"),
                position,
                Quaternion.identity) as GameObject;
        }
        else
        {
            rangeArea.transform.position = new Vector3(position.x, 0.05f, position.z);
            rangeArea.transform.localScale = new Vector3(radius*2, 0, radius*2);
        }
    }

    private void RemoveCircleArea()
    {
        if (rangeArea != null)
            GameObject.Destroy(rangeArea);
    }

    public void DrawArrow(Vector3 start, Vector3 end)
    {   // Draws an arrow, indicating cast direction
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

    private void RemoveArrow()
    {
        if (arrowBody != null)
            GameObject.Destroy(arrowBody);
        if (arrowCap != null)
            GameObject.Destroy(arrowCap);
    }
}
