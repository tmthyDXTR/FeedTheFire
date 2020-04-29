using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastRange : MonoBehaviour
{
    private CastUtil _cast;
    void Start()
    {
        _cast = this.transform.parent.gameObject.GetComponent<CastUtil>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9) // 9 = Enemy unit
        {
            if (!_cast.inCastRange.Contains(other.transform.parent.gameObject))
                _cast.inCastRange.Add(other.transform.parent.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9) // 9 = Enemy unit
        {
            if (_cast.inCastRange.Contains(other.transform.parent.gameObject))
                _cast.inCastRange.Remove(other.transform.parent.gameObject);
        }
    }
}
