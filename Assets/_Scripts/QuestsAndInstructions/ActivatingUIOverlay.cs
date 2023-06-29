using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatingUIOverlay : MonoBehaviour
{
    [SerializeField] private Camera rcamer;
    [SerializeField] private Camera lcamer;
    // Start is called before the first frame update
    void Start()
    {
        rcamer.enabled =  true;
        lcamer.enabled = true;
    }
}
