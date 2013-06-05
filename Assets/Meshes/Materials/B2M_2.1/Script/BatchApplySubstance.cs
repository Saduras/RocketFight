using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BatchApplySubstance : EditorWindow {
	
	private ProceduralMaterial Mat;
	private GameObject obj;
	GameObject[] Objects;
	
	// Add menu item to the Window menu
	[MenuItem ("Window/BatchApplySubstance")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		EditorWindow.GetWindow<BatchApplySubstance> (false, "Batch-Apply Substance");
			
	}
	
	// Implement your own editor GUI here.
	void OnGUI () {
		
		Objects = Selection.gameObjects as GameObject[];
		
		EditorGUI.LabelField(new Rect(10,10,position.width - 20, 16),"Selected Objs :", (Objects.Length).ToString());
		Mat = EditorGUI.ObjectField(new Rect(10,40,position.width - 20, 16),"Substance Mat.",Mat,typeof(ProceduralMaterial)) as ProceduralMaterial;
	
		if (GUI.Button(new Rect(10, 70, 50, 30), "Apply!"))
		{
			foreach(GameObject Object in Objects)
			{
				if (Object.GetComponent<Renderer>() != null)
				{
					Object.renderer.sharedMaterial = Mat as ProceduralMaterial;
				}
			}
		}	
	}
	
}