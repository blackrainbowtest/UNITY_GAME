/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/UI/EventSystemManager.cs                 */
/*                                                        /\_/\               */
/*                                                       ( вЂў.вЂў )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/04                                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Ensures only one EventSystem exists in the scene.
/// If another EventSystem is already present, this one will be destroyed.
/// </summary>
public class EventSystemManager : MonoBehaviour
{
	private void Awake()
	{
		// Check if there's already an EventSystem in the scene
		EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
		
		if (eventSystems.Length > 1)
		{
			// There's already an EventSystem, destroy this GameObject
			UDADebug.Log("[EventSystemManager] Duplicate EventSystem found. Destroying this one.");
			Destroy(gameObject);
		}
	}
}

