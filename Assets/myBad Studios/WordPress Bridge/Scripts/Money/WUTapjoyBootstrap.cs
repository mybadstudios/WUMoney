//WordPress For Unity Money Extension © 2024 by Ryunosuke Jansen is licensed under CC BY-ND 4.0.

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#if (UNITY_ANDROID || UNITY_IOS) && WUTJ
using TapjoyUnity; 
#endif 

namespace MBS {
	/// <summary>
	/// Add this class to a Bootstrap scene and run only once but keepactive theentire game.
	/// This will connect you to Tapjoy and uniquely identify the player so their currency will be correctly allocated to them from Tapjoy
	/// 
	/// This class can be skipped entirely if you arenot integrating with Tapjoy. Most of thefunctionality in this class is not compiled 
	/// into the project if the WUTJ compiler drective is not set (default). In order to use thisscript andto integrate with Tapjoyyou need
	/// to first install the Tapjoy SDK into your project and then add the WUTJ compiler directive to your Player Settings.
	/// </summary>
	public class WUTapjoyBootstrap : MonoBehaviour {

		public enum __eBSNextSceneLoad {None, Immediately, AfterIDWasSet}

		#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		static Coroutine _onTapjoyConnect = null;
		#endif 

		/// <summary>
		/// When should the Bootstrap scene load the next scene: Immediately or after the User has been uniquely identified?
		/// </summary>
		[Header("After ID was set")]		
		public __eBSNextSceneLoad load_new_scene = __eBSNextSceneLoad.AfterIDWasSet;

		/// <summary>
		/// The name of the scene to load after this Bootstrap scene
		/// </summary>
		public string next_scene = "MoneyDemo";

		/// <summary>
		/// Your secret key for the iOS platform as provided by Tapjoy
		/// </summary>
		[Header("Tapjoy Currency Keys")]
		public string ios_sdk_key = "Copy from Tapjoy dashboard";

		/// <summary>
		/// Your secret key for the Android platform as provided by Tapjoy
		/// </summary>
		public string android_sdk_key = "Copy from Tapjoy dashboard";

		/// <summary>
		/// Do you want to show errors on screen at runtime? Perhaps only during development?
		/// </summary>
		[Header("General Settings")]
		public bool show_errors_on_screen = true;

		void Start () {
			DontDestroyOnLoad(gameObject);
			WULogin.OnLoggedIn += InitializeTapjoy;
			WULogin.OnLoggedOut += ClearUserId;

			#if (UNITY_ANDROID || UNITY_IOS) && WUTJ && !UNITY_EDITOR
			Tapjoy.OnSetUserIDSuccess += UserIdWasSet;
			Tapjoy.OnConnectSuccess += ValidateUserId;
			#endif

			if (load_new_scene == __eBSNextSceneLoad.Immediately && SceneManager.GetActiveScene().name != next_scene)
				SceneManager.LoadScene(next_scene);
		}

		/// <summary>
		/// Everything about the self hosted server relies on having a WP based TapJoy id
		/// Only once we have that do we consider ourselves logged in
		/// At this point you can show any 'OnAppLaunched' placements as only now it knows who the player is
		/// Also, trigger the "Go to next scene" so probably a bad idea to show a placement on successful login and 
		/// at the start of the second scene...
		/// </summary>
		void UserIdWasSet()
		{
			WUMoney.current_id = WUMoney.TapJoyUserID;

			#if WUTJ
			WUTJPlacement.OnConnectSuccess();
			WUTJPlacement.ShowPlacementWhenReady(eTJPlacements.Logged_In);
			#endif

			if (load_new_scene == __eBSNextSceneLoad.AfterIDWasSet && SceneManager.GetActiveScene().name != next_scene)
				SceneManager.LoadScene(next_scene);
		}

		//when logging out of WordPress, clear the previous logged in user's TapJoy ID
		void ClearUserId(CML response) => WUMoney.current_id = string.Empty;

		/// <summary>
		/// called upon successful login to WordPress
		/// if already connected to TapJoy then set the user id based on WordPress user ID
		/// if not already connected, run a coroutine that starts the connection then waits for it to complete
		/// if we are in the editor, just continue 
		/// </summary>
		void InitializeTapjoy(CML response)
		{
			#if (UNITY_ANDROID || UNITY_IOS) && WUTJ && !UNITY_EDITOR
			if (Tapjoy.IsConnected) 
			{
			if (!WUMoney.TJUserIDIsSet)
			WUMoney.SetUserID();
			return;
			}

			if (null == _onTapjoyConnect)
			_onTapjoyConnect = StartCoroutine(OnTapjoyConnect());
			#else
			UserIdWasSet();
			#endif
		}

		//if not yet connected to TapJoy when WULogin login completes,
		//tell TapJoy to connect and then wait for it to happen
		//once you are logged into both, set the user id based on the WP user ID
		IEnumerator OnTapjoyConnect()
		{
			#if (UNITY_ANDROID || UNITY_IOS) && WUTJ && !UNITY_EDITOR
			if (!Tapjoy.IsConnected)
				#if UNITY_ANDROID
				Tapjoy.Connect(android_sdk_key);
				#elif UNITY_IOS
				Tapjoy.Connect(ios_sdk_key);
				#endif
			yield return new WaitUntil(() => Tapjoy.IsConnected);
			Tapjoy.SetUserID( WUMoney.TapJoyUserID );
			_onTapjoyConnect = null;
			#else
			yield return 0;
			#endif
		}

		void ValidateUserId()
		{
			#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			if (null == _onTapjoyConnect && !WUMoney.TJUserIDIsSet)
				WUMoney.SetUserID();
			#endif

			#if !WUTJ
			if (!(WULogin.IsLoggedIn && WUMoney.TJUserIDIsSet))
				WUMoney.SetUserID();
			#endif
		}

		#if (UNITY_ANDROID || UNITY_IOS) && WUTJ && !UNITY_EDITOR
		void OnEnable()
		{
			TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
			TJPlacement.OnRequestFailure += HandlePlacementRequestFailure;
			TJPlacement.OnContentDismiss += HandlePlacementContentDismiss;
			TJPlacement.OnVideoError 	 += HandleVideoError; //not sure if this one is needed but playing it safe
		}

		void OnDisable()
		{
			TJPlacement.OnRequestSuccess -= HandlePlacementRequestSuccess;
			TJPlacement.OnRequestFailure -= HandlePlacementRequestFailure;
			TJPlacement.OnContentDismiss -= HandlePlacementContentDismiss;
			TJPlacement.OnVideoError 	 -= HandleVideoError;
		}

		void HandlePlacementRequestSuccess(TJPlacement placement)
		{
			if (!placement.IsContentAvailable()) 
			{
				if (show_errors_on_screen)
					StatusMessage.Message = "No content available for placement: " + placement.GetName();
				else
					Debug.Log("WUMoney: No content available for " + placement.GetName());
			}
		}

		void HandlePlacementRequestFailure(TJPlacement placement, string error) 
		{
			string message = "WUMoney: Request for " + placement.GetName() + " has failed because: " + error;
			if (show_errors_on_screen)
				StatusMessage.Message = message;
			else
				Debug.Log(message);
		}

		void HandlePlacementContentDismiss(TJPlacement placement) 
		{
			//rebuffer content once it's been shown to make sure it is ready for later
			WUTJPlacement.OnContentDismissed(placement);
		}

		void HandleVideoError(TJPlacement placement, string msg) 
		{
			string message = "WUMoney: HandleVideoError for placement " + placement.GetName() + "with message: " + msg;
			if (show_errors_on_screen)
				StatusMessage.Message = message;
			else
				Debug.Log(message);
			//reload placement content
			WUTJPlacement.OnVideoCompleted(placement);
		}

		#endif
	}
}