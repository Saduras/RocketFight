using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {

	public static Texture2D GenerateOneColorTexture( Color color, int width, int height ) {	
		Texture2D rgb_texture = new Texture2D(width, height);
		
	    int i = 0;
	    int j = 0;
	    for(i = 0;i<width;i++)
	    {
	        for(j = 0;j<height;j++)
	        {
	            rgb_texture.SetPixel(i, j, color);
	        }
	    }
	    rgb_texture.Apply();
		
		return rgb_texture;
	}
}
