//WordPress For Unity Money Extension © 2024 by Ryunosuke Jansen is licensed under CC BY-ND 4.0.

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#if WUTJ
using TapjoyUnity;
#endif

namespace MBS {

	#if WUTJ
	//suggested placements
	public enum eTJPlacements {Logged_In, App_Resume, App_Close, Main_Menu, Pause, Stage_Open, Stage_Complete, Stage_Failed, Level_Up, 
		Store_Open, In_App_Purchase, Abandon_In_App_Purchase, Virtual_Good_Purchased, Sign_Up_Complete, User_High_Score, 
		Out_Of_Goods, Out_Of_Energy, Finished_Tutorial, OfferWall_Request, Interstitial_Request, Video_Request }

	public class WUTJPlacement : MonoBehaviour
	{
		TJPlacement placement = null;

		static public bool viewIsShowing = false;

		static public Action<eTJPlacements> 
		_showPlacement,
		_showPlacementDelayed;

		static public Action<TJPlacement> 
		_onVideoDidComplete,
		_onContentDismissed;

		static public Action 
		_onConnectSuccess;

		public eTJPlacements _placement;

		public string PlacementName	 { get { return _placement.ToString().Replace("_"," "); } }
		public bool IsReady			 { get { return null == placement ? false : placement.IsContentReady(); } }

		void Awake()
		{
			#if (UNITY_ANDROID || UNITY_IOS) && WUTJ && !UNITY_EDITOR
			_onConnectSuccess += RequestContent;
			_showPlacement += __PlacementCheck;
			_onContentDismissed += __OnContentDismissed;
			_showPlacementDelayed += __ShowWhenReady;
			_onVideoDidComplete += __VideoCompleted;
			#endif
		}

		static public void OnConnectSuccess()
		{
			if (null != _onConnectSuccess)
				_onConnectSuccess();
		}

		static public void OnContentDismissed(TJPlacement placed)
		{
			viewIsShowing = false;
			if (null != _onContentDismissed)
				_onContentDismissed(placed);
		}

		static public void OnVideoCompleted(TJPlacement placed)
		{
			if (null != _onVideoDidComplete)
				_onVideoDidComplete(placed);
		}

		static public void ShowPlacement(eTJPlacements name)
		{
			if (null != _showPlacement)
				_showPlacement(name);
		}

		static public void ShowPlacementWhenReady(eTJPlacements name)
		{
			if (null != _showPlacementDelayed)
				_showPlacementDelayed(name);
		}

		void __PlacementCheck(eTJPlacements name)
		{
			if (name != _placement) return;

			if ( viewIsShowing || !Tapjoy.IsConnected ) 
				return; 

			ShowContent();
		}

		void __ShowWhenReady(eTJPlacements name)
		{
			if (name != _placement) return;
			if ( viewIsShowing || !Tapjoy.IsConnected ) 
				return;

			StartCoroutine(_showWhenReady());
		}

		void __OnContentDismissed(TJPlacement placed)
		{
			if (placed.GetName() != placement.GetName()) return;
			if (null != placement && !placement.IsContentReady())
			{
				RequestContent();
				WUMoney.GetCurrencyBalance();
			}
		}

		void __VideoCompleted(TJPlacement placed)
		{
			if (placed.GetName() != placement.GetName()) return;
			if (null != placement && !placement.IsContentReady())
				RequestContent();
		}

		IEnumerator _showWhenReady()
		{
			yield return new WaitUntil( () => null != placement );
			yield return new WaitUntil( () => placement.IsContentReady());
			ShowContent();
		}

		public void ShowContent()
		{
			if (!WUMoney.TJUserIDIsSet)
			{
				WUMoney.SetUserID();
				Debug.LogError("WUMoney: User ID was not set at the time of showing the placement");
				return;
			}
			if (IsReady)
				placement.ShowContent();
			else
			{
				RequestContent();
				Debug.LogWarning("Only now requesting content");
			}
		}

		public void RequestContent()
		{
			if (!WUMoney.TJUserIDIsSet)
			{
				//most likely you have auto login turned on in Windows->Tapjoy
				Invoke("RequestContent",0.5f);
				Debug.LogError("USER ID WAS NOT SET WHEN REQUESTING CONTENT");
				return;
			}
			if (placement == null) 
				placement = TJPlacement.CreatePlacement( PlacementName );
			placement.RequestContent();
		}

	}
	#endif
}