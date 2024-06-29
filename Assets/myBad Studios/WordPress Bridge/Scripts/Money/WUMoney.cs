//WordPress For Unity Money Extension © 2024 by Ryunosuke Jansen is licensed under CC BY-ND 4.0.

using UnityEngine;
using System;

namespace MBS {

	static public class WUMoney {

		#region private worker variables. Do not touch!!!
		enum WUTapJoyActions { GetPoints, SpendPoints, GivePoints }
		
		static readonly string money_filepath = "wub_money/unity_functions.php";
		static public readonly string ASSET = "MONEY";
		static public string TapJoyUserID => $"{WPServer.GameID}_{WULogin.UID}";
		static public string current_id  = "";
		static public bool TJUserIDIsSet => current_id == TapJoyUserID;
		#endregion

		#region event definitions

		//TapJoy points manually manipulated
		/// <summary>
		/// Triggered after you successfully manually added points to a currency
		/// </summary>
		static public Action<MBSEvent> OnAwardCurrencyResponse;

		/// <summary>
		/// Triggered after you successfully deducted points from a currency
		/// </summary>
		static public Action<MBSEvent> OnSpendCurrencyResponse;

		/// <summary>
		/// Triggered after you recieved the current balance of a specified currency
		/// </summary>
		static public Action<MBSEvent> OnGetCurrencyBalanceResponse;

		/// <summary>
		/// Triggered only if the server reported that Tapjoy has sent points to your server.
		/// The points will already have been added to the currency balance. 
		/// This event contains how many points was awarded as well as the balance from the server
		/// </summary>
		static public Action<MBSEvent> OnEarnedPoints;

		//when the above goes wrong...
		/// <summary>
		/// Triggered if the currency could not be updated. Usually this would mean a typo in the currency name
		/// </summary>
		static public Action<MBSEvent> OnAwardCurrencyResponseFailure;

		/// <summary>
		/// Triggered if the transaction failed. Usually this means either
		/// 1. a typo in the currency name 
		/// 2. the player didn't have enough currency to complete the transaction
		/// </summary>
		static public Action<MBSEvent> OnSpendCurrencyResponseFailure;

		/// <summary>
		/// Triggered if the server failed to find the currency to report it's balance
		/// </summary>
		static public Action<MBSEvent> OnGetCurrencyBalanceResponseFailure;

		#endregion

		#region public function implementations

		/// <summary>
		/// Identifies the player by Game and ID rather than by mobile device id.
		/// This Id makes it possible to recieve currency on any platform and spend it across devices / platforms
		/// </summary>
		/// <returns>The WUB Money unique ID for this player</returns>
		static public bool SetUserID()
		{
			if (WULogin.UID <= 0)
				return false;
	
			#if WUTJ
			TapjoyUnity.Tapjoy.SetUserID ( TapJoyUserID );
			#else
			current_id = TapJoyUserID;
			#endif

			return true;
		}

		/// <summary>
		/// Request the current balance of a specific currency from the server
		/// </summary>
		/// <param name="currency">The exact, case sensitive name of the currency balance you wish to poll</param>
		static public void GetCurrencyBalance(string currency = "points")
		{
			CMLData data = new CMLData();
			data.Set("currency", currency);
			WPServer.ContactServer(
				WUTapJoyActions.GetPoints.ToString(), 
				money_filepath, 
				ASSET, 
				data, 
				_onGetPointsBalanceResponse, 
				_onGetPointsBalanceResponseFailure);
		}

		/// <summary>
		/// Attempt to spend some currency
		/// </summary>
		/// <param name="amt">How much do you want to spend? Teh transaction will fail if your balance is too low</param>
		/// <param name="currency">The exact, case sensitive name of the currency you wish to spend from</param>
		/// <param name="meta">Feel free to ignore</param>
		static public void SpendCurrency(int amt, string currency = "points", string meta = "") =>	__award_and_spend(WUTapJoyActions.SpendPoints, amt, currency, _onSpendPointsResponse, _onSpendPointsResponseFailure, meta);

		/// <summary>
		/// Award points to a currency. If the currency does not exist it will be created and the points will become the new total balance
		/// </summary>
		/// <param name="amt">How many points do you want to add to this currency?</param>
		/// <param name="currency">The exact, case sensitive name of the currency you wish to award points to or the name of the currency you want to create</param>
		/// <param name="meta">Feel free to ignore</param>
		static public void AwardCurrency(int amt, string currency = "points", string meta = "") =>	__award_and_spend(WUTapJoyActions.GivePoints, amt, currency, _onAwardPointsResponse, _onAwardPointsResponseFailure, meta);		
		#endregion

		#region private worker functions

		static void __award_and_spend(WUTapJoyActions server_action, int amt, string currency, Action<CML> onSuccess, Action<CMLData> onFailed, string meta = "")
		{
			if (amt < 0)
			{
				Debug.LogError("Amounts cannot be negative");
				return;
			}

			CMLData data = new CMLData();
			data.Seti("amt", amt);
			data.Set("currency", currency);
			meta = meta.Trim();
			if (!string.IsNullOrEmpty(meta))
				data.Set("meta", meta);
			WPServer.ContactServer(server_action, money_filepath, ASSET, data, onSuccess, onFailed);
		}

		#endregion

		#region internal delegate event responders
		/// <summary>
		/// Tapjoy awards users with their points anywhere from immediately to 2 weeks later
		/// Since we never know WHEN a user gets new points from Tapjoy and since it might
		/// happen while the user is not playing, every time you contact the server it checks
		/// to see if any points were recieved from Tapjoy ince the last time it checked the balance.
		/// Thus, if the player had recieved points from Tapjoy on your server since the last time you
		/// checked, this function is called regardless of whether a server interaction you performed 
		/// returned as a success or failure
		/// 
		/// If no points were allocated then nothing happens. If points WERE allocated then this
		/// function will trigger the OnEarnedPoints event and tell you how many points were awarded.
		/// 
		/// You are free to ignore this event if you want but it DOES contain the amount of coins
		/// received as well as the updated total. It might be a good idea to update the TapJoy
		/// balance you store locally whenever the OnPointsAwarded event is triggered.
		/// 
		/// New points can be accessed via response.details[0].Int()
		/// Total balance can be accessed via response.details[0].Int("total");
		/// </summary>
		static public void TestForTapJoyAwards(CML response)
		{
			if (null == response || response.Count == 0)
				return;
			CMLData TapjoyAwarded = response.GetFirstNodeOfType("TapjoyAwarded");
			if (null == TapjoyAwarded) 
				return;
			CML award = new CML();
			award.CopyNode(TapjoyAwarded);

			if (null == OnEarnedPoints) return;
			OnEarnedPoints( new MBSEvent(award) );
		}

		static void _onAwardPointsResponse(CML response) => OnAwardCurrencyResponse?.Invoke(new MBSEvent(response));		
		static void _onSpendPointsResponse(CML response) => OnSpendCurrencyResponse?.Invoke( new MBSEvent(response) );
		static void _onGetPointsBalanceResponse(CML response) => OnGetCurrencyBalanceResponse?.Invoke( new MBSEvent(response) );

		static void _onAwardPointsResponseFailure(CMLData response)
		{
			if (null == OnAwardCurrencyResponseFailure) return;
			CML _r = new CML();
			_r.AddNode("Error");
			_r.CopyNode(response);
			OnAwardCurrencyResponseFailure( new MBSEvent(_r) );
		}

		static void _onSpendPointsResponseFailure(CMLData response)
		{
			if (null == OnSpendCurrencyResponseFailure) return;
			CML _r = new CML();
			_r.AddNode("Error");
			_r.CopyNode(response);
			OnSpendCurrencyResponseFailure( new MBSEvent(_r) );
		}

		static void _onGetPointsBalanceResponseFailure(CMLData response)
		{
			if (null == OnGetCurrencyBalanceResponseFailure) return;
			CML _r = new CML();
			_r.AddNode("Error");
			_r.CopyNode(response);
			OnGetCurrencyBalanceResponseFailure( new MBSEvent(_r) );
		}			
		#endregion

	}
}