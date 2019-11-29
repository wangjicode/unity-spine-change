using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentObject : UnityEngine.Object {
	public TextAsset equipmentJsonAsset;
	public TextAsset equipmentTxtAsset;
    #if UNITY_ANDROID
    public Texture2D equipmentTexture_Alpha;
    public Texture2D equipmentTexture_RGB;
    #elif UNITY_IPHONE || UNITY_EDITOR
    public Texture2D equipmentTexture;
    #endif
	public void ClearData()
    {
        equipmentJsonAsset = null;
        equipmentTxtAsset = null;
        #if UNITY_ANDROID
        equipmentTexture_Alpha = null;
        equipmentTexture_RGB = null;
        #elif UNITY_IPHONE || UNITY_EDITOR
        equipmentTexture = null;
        #endif
    }
}
