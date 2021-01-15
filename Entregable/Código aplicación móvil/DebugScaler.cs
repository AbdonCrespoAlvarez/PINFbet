using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Used to facilitate academic evaluation via Windows and Linux versions of PINFbet's
mobile application.
Module used in order to escalate correctly screen resolutions in Windows and Linux versions
of PINFbet's mobile application.
*/

public class DebugScaler : MonoBehaviour
{
    [SerializeField]
    private RectTransform canvRect;
    private RectTransform myRect;
	private void Start()
	{
        myRect = GetComponent<RectTransform>();
	}
	void Update()
    {
        float diff = Math.Abs(canvRect.rect.height - myRect.rect.height);
        float ratio = (myRect.rect.height - diff) / myRect.rect.height;
        float value = ratio;

        myRect.localScale = new Vector3(value, value, 1);
    }
}
