/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/FX/FXRenderer.cs                                    */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 12:16:04 by UDA                                      */
/*   Updated: 2025/12/02 12:16:04 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using UnityEngine.UI;

public class FXRenderer : MonoBehaviour
{
	[Header("UI References")]
	public Image overlayImage;
	public Image colorTintImage;

	private GameObject activeParticles;

	public void ApplyProfile(FXProfile profile)
	{
		if (profile == null)
		{
			ClearFX();
			return;
		}

		// === Edge Overlay (лед, жар, влажность) ===
		if (overlayImage != null)
		{
			overlayImage.sprite = profile.edgeOverlay;
			overlayImage.color = new Color(1,1,1, profile.overlayAlpha);
			overlayImage.enabled = (profile.edgeOverlay != null);
		}

		// === Screen Tint ===
		if (colorTintImage != null)
		{
			colorTintImage.color = profile.colorTint * profile.tintStrength;
			colorTintImage.enabled = (profile.tintStrength > 0.01f);
		}

		// === Particles ===
		if (activeParticles != null)
			Destroy(activeParticles);

		if (profile.particlePrefab != null)
			activeParticles = Instantiate(profile.particlePrefab, transform);
	}

	public void ClearFX()
	{
		if (overlayImage != null)
			overlayImage.enabled = false;

		if (colorTintImage != null)
			colorTintImage.enabled = false;

		if (activeParticles != null)
			Destroy(activeParticles);
	}
}
