using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ForTesting : MonoBehaviour 
{
  float _velocity = 0.1f;
  float _valueToChange = 0.0f;	

  Int3 _r1 = new Int3();
  Int3 _r2 = new Int3();
  Int3 _r3 = new Int3();
  void Awake()
  {
    _r1.Set(1, 2, 3);

    _r2 = _r1;
    _r3 = _r1;

    Debug.Log(_r1 + " " + _r2 + " " + _r3);

    _r1.Set(5, 6, 7);

    Debug.Log(_r1 + " " + _r2 + " " + _r3);

    Int3 a = new Int3(1, 2, 3);
    Int3 b = new Int3(4, 5, 6);
    Int3 c = new Int3(1, 2, 3);

    Debug.Log((a == b) + " " + (b == c) + " " + (a == c));
  }

	void Update () 
  {
    _valueToChange = Mathf.SmoothDamp(_valueToChange, 1.0f, ref _velocity, Mathf.Infinity, 0.1f);

    //Debug.Log(_valueToChange + " " + _velocity);	
	}
}
