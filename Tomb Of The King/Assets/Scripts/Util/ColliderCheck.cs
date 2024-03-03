using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;


public class ColliderCheck : MonoBehaviour{
	
	public delegate void EventCheck(Collider2D col);
	
	public event EventCheck OnEnterCall;
	public event EventCheck OnExitCall;
	
	
	private void OnTriggerEnter2D(Collider2D other)
    {
		print("entrou");
		OnEnterCall?.Invoke(other);
    }
	private void OnTriggerExit2D(Collider2D other)
    {
		print("saiu");
		OnExitCall?.Invoke(other);
    }
}