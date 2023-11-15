using UnityEngine;
 
public static class HexMetrics {
 
    public const float outerRadius = 0.5f;
    public const float innerRadius = outerRadius * 0.866025404f;
	
	public const float solidFactor = 0.75f;
	public const float blendFactor = 1f - solidFactor;

	public static Vector2[] corners = {
		new Vector2(0f, outerRadius),
		new Vector2(innerRadius, 0.5f * outerRadius),
		new Vector2(innerRadius, -0.5f * outerRadius),
		new Vector2(0f, -outerRadius),
		new Vector2(-innerRadius, -0.5f * outerRadius),
		new Vector2(-innerRadius, 0.5f * outerRadius),
		new Vector2(0f, outerRadius)
	};

	public static Vector2 GetFirstCorner (HexDirection direction) {
		return corners[(int)direction];
	}
	
	public static Vector2 GetSecondCorner (HexDirection direction) {
		return corners[(int)direction + 1];
	}

	public static Vector2 GetFirstSolidCorner (HexDirection direction) {
		return corners[(int)direction] * solidFactor;
	}
	
	public static Vector2 GetSecondSolidCorner (HexDirection direction) {
		return corners[(int)direction + 1] * solidFactor;
	}

	// 边部的桥
	public static Vector2 GetBridge (HexDirection direction) {
		return (corners[(int)direction] + corners[(int)direction + 1]) *
			blendFactor;
	}

}