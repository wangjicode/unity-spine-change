using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestEvent : MonoBehaviour {

	public string id;
	private void Start () {
		GetComponent<Button>().onClick.AddListener(OnClickEvent);
	}
	private void OnClickEvent(){
		if(gameObject.name.Contains("CreateMiddle")){
			FindObjectOfType<ChangeTest>().CreateMiddle();
		}
		else{
			FindObjectOfType<ChangeTest>().Change(id);
		}
	}
}
