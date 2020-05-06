using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    // This script handles the hex tile status
    // Is it revealed by light -> show the tiles features (grassland, tree...)
    // If it is not revealed activate the "darkness" tile

    [SerializeField]
    private bool isHidden = true;

    [SerializeField]
    private GameObject _darkness;
    [SerializeField]
    private GameObject _tile;
    [SerializeField]
    private List<Transform> revealedBy;



    private MeshRenderer _meshRenderer;
    void Start()
    {
        _darkness = this.transform.GetChild(0).gameObject;
        _tile = this.transform.GetChild(1).gameObject;
        _tile.SetActive(false);
        _meshRenderer = _darkness.GetComponent<MeshRenderer>();
    }

    public bool IsHidden
    {
        get { return isHidden; }
        set
        {
            isHidden = value;
            if (isHidden)
            {
                if (_darkness != null)
                    // Hide Tile Animation
                    _meshRenderer.enabled = true;
                    _tile.SetActive(false);
                //Debug.Log("Hide: " + _tile.name);
            }
            else
            {
                if (_darkness != null)
                    // Reveal Tile animation
                    _meshRenderer.enabled = false;
                    _tile.SetActive(true);

                //Debug.Log("Show: " + _tile.name);
            }
        }
    }

    public void AddRevealer(Transform revealerTransform)
    {
        if (!revealedBy.Contains(revealerTransform))
        {
            revealedBy.Add(revealerTransform);
            IsHidden = false;
        }            
    }
    public void RemoveRevealer(Transform revealerTransform)
    {
        if (revealedBy.Contains(revealerTransform))
        {
            revealedBy.Remove(revealerTransform);
        }
        if (GetRevealedByCount() < 1)
        {
            IsHidden = true;
        }
    }

    public int GetRevealedByCount()
    {
        return revealedBy.Count;
    }
}
