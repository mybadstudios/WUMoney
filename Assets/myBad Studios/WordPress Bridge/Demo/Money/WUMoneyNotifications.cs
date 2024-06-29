using UnityEngine;
using MBS;

/// <summary>
/// This simple class merely tells this game object to wait around until WUMoney reports that it recieved 
/// some points and when that event occurs it spawns an on screen notification informing you thereof.
/// </summary>
public class WUMoneyNotifications : MonoBehaviour {

	/// <summary>
	/// The parent canvas to spawn the notification prefab under
	/// </summary>
	public Canvas canvas;

	/// <summary>
	/// The image to display in the notification header
	/// </summary>
	public Sprite icon;

	void Start () => WUMoney.OnEarnedPoints += ShowNotification;	
	void ShowNotification(MBSEvent data)
	{
		int points_received = data.details[0].Int();
		MBSNotification.SpawnInstance(
			canvas, 
			new Vector2(165f, -80f), 
			new Vector2(0f, -80f),
			"Points Earned",
			$"You received {points_received} Tapjoy points",
			icon);
	}
}
