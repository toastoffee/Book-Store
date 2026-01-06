using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public bool isChained = false;

    private void Start()
    {
        var bookPattern = GetComponent<BookPattern>();

        if (isChained)
        {
            GetComponent<RegularPolygon>().enabled = false;
        }
    }
}
