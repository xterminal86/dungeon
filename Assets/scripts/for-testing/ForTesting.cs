using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ForTesting : MonoBehaviour 
{
  float _velocity = 0.1f;
  float _valueToChange = 0.0f;	
	void Update () 
  {
    _valueToChange = Mathf.SmoothDamp(_valueToChange, 1.0f, ref _velocity, Mathf.Infinity, 0.1f);

    //Debug.Log(_valueToChange + " " + _velocity);	
	}
}
