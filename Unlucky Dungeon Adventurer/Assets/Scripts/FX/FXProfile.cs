/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/FX/FXProfile.cs                                     */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 12:15:30 by UDA                                      */
/*   Updated: 2025/12/02 12:15:30 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

[CreateAssetMenu(fileName = "FXProfile", menuName = "UDA/FX/Biome FX Profile")]
public class FXProfile : ScriptableObject
{
	[Header("General")]
	public string biomeId;

	[Header("Screen Overlays")]
	public Sprite edgeOverlay;       // ice, heat, haze around the edges
	[Range(0,1)] public float overlayAlpha = 0.4f;

	[Header("Color Tint")]
	public Color colorTint = Color.white;   // light screen tinting
	[Range(0,1)] public float tintStrength = 0.3f;

	[Header("Particles")]
	public GameObject particlePrefab; // snow, leaves, sand
}
