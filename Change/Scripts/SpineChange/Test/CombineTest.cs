using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DancingWord.SpineChange;

public class CombineTest : MonoBehaviour {
    public Texture2D tex;
    public Material material;
    private void Start () {
       tex = material.mainTexture as Texture2D;
    }
}