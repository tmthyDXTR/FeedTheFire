using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class BonfireController : MonoBehaviour
{
    /// <summary>
    /// This class handles the bonfire, and its strength level
    /// that gets boosted by burning logs
    /// </summary>

    private Reveal _reveal;

    #region Variables
    [SerializeField]
    private FireStrength fireStrength;
    public enum FireStrength
    {
        VeryWeak,
        Weak,
        Mid,
        Strong,
        VeryStrong,
    }
    [SerializeField]
    private int veryWeakRadius = 4;
    [SerializeField]
    private int weakRadius = 5;
    [SerializeField]
    private int midRadius = 6;
    [SerializeField]
    private int strongRadius = 7;
    [SerializeField]
    private int veryStrongRadius = 8;

    #endregion
    void Start()
    {
        _reveal = this.transform.Find("RevealRange").GetComponent<Reveal>();

        fireStrength = FireStrength.Mid;
    }

    void Update()
    {
        switch (fireStrength)
        {
            #region FireStrength VeryWeak
            case FireStrength.VeryWeak:
                //Debug.Log("Fire Strength: " + fireStrength);
                if(_reveal.RevealRadius != veryWeakRadius)
                {
                    _reveal.RevealRadius = veryWeakRadius;
                }

                break;
            #endregion


            #region FireStrength Weak
            case FireStrength.Weak:
                //Debug.Log("Fire Strength: " + fireStrength);
                if (_reveal.RevealRadius != weakRadius)
                {
                    _reveal.RevealRadius = weakRadius;
                }
                break;
            #endregion


            #region FireStrength Mid
            case FireStrength.Mid:
                //Debug.Log("Fire Strength: " + fireStrength);
                if (_reveal.RevealRadius != midRadius)
                {
                    _reveal.RevealRadius = midRadius;
                }
                break;
            #endregion


            #region FireStrength Strong
            case FireStrength.Strong:
                //Debug.Log("Fire Strength: " + fireStrength);
                if (_reveal.RevealRadius != strongRadius)
                {
                    _reveal.RevealRadius = strongRadius;
                }
                break;
            #endregion


            #region FireStrength VeryStrong
            case FireStrength.VeryStrong:
                //Debug.Log("Fire Strength: " + fireStrength);
                if (_reveal.RevealRadius != veryStrongRadius)
                {
                    _reveal.RevealRadius = veryStrongRadius;
                }
                break;
            #endregion
        }
    }
}