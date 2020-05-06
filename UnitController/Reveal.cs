using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reveal : MonoBehaviour
{
    /// <summary>
    /// This class reveals and hides dark tiles
    /// and sets the radius of the reveal trigger collider
    /// </summary>

    [SerializeField]
    private float revealRadius;
    public float RevealRadius
    {
        get { return revealRadius; }
        set
        {
            revealRadius = value;
            if (_sphereCollider != null)
                _sphereCollider.radius = revealRadius;
        }
    }
    [SerializeField]
    private List<TileController> revealedTiles;
    

    private SphereCollider _sphereCollider;
    
    void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        RevealRadius = revealRadius;
        StartCoroutine(ActivateRevealCollider());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Darkness")
        {
            //Debug.Log("Cleared Darkness");
            TileController _tile = other.transform.parent.gameObject.GetComponent<TileController>();
            if (_tile != null)
            {
                _tile.AddRevealer(this.transform);
                revealedTiles.Add(other.transform.parent.GetComponent<TileController>());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Darkness")
        {
            //Debug.Log("Darkness creeping in");
            TileController _tile = other.transform.parent.gameObject.GetComponent<TileController>();
            if (_tile != null)
            {
                _tile.RemoveRevealer(this.transform);
                revealedTiles.Remove(other.transform.parent.GetComponent<TileController>());
            }
        }
    }

    private IEnumerator ActivateRevealCollider()
    {
        // Activate the collider just after the map is created
        yield return new WaitForSeconds(0.1f);
        _sphereCollider.enabled = true;
    }

    private void OnDestroy()
    {
        // If revealer is destroyed, remove this Transform from all
        // reavler lists of the individual revealed tiles by this revealer
        Debug.Log("Revealer destroyed");
        foreach (var tile in revealedTiles)
        {
            tile.RemoveRevealer(this.transform);
        }
    }
}
