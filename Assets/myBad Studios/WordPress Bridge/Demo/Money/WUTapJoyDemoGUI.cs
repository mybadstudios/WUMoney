using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MBS;

public class WUTapJoyDemoGUI : MonoBehaviour {

	/// <summary>
	/// The text field to display the current Tapjoy currency balance onto
	/// </summary>
	public Text tapjoy_text;

	/// <summary>
	/// The text field to display the current "Credits" balance onto
	/// </summary>
	public Text credits_text;

	/// <summary>
	/// The text field to display the current "Pebbles" balance onto
	/// </summary>
	public Text pebbles_text;

	/// <summary>
	/// Holds a reference to the second panel which calls the Tapjoy placements (if configured)
	/// </summary>
	public GameObject tapjoy_panel;

	void Awake()
	{
		#if WUTJ
		tapjoy_panel.SetActive (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
		#else
		tapjoy_panel.SetActive( false );
		#endif

		WUMoney.OnAwardCurrencyResponse += UpdateTextField;
		WUMoney.OnSpendCurrencyResponse += UpdateTextField;
		WUMoney.OnGetCurrencyBalanceResponse += UpdateTextField;
		WUMoney.OnAwardCurrencyResponseFailure += UpdateTextFieldFailure;
		WUMoney.OnSpendCurrencyResponseFailure += UpdateTextFieldFailure;
		WUMoney.OnGetCurrencyBalanceResponseFailure += UpdateTextFieldFailure;
		WUMoney.OnEarnedPoints += OnPointsEarned;

		PopulateInitialBalances();
	}

	/// <summary>
	/// In the login prefab, you can specify the currencies to fetch during login 
	/// This way you can populate your game's currency data all at once and automatically
	/// without making separate calls to the server per currency to fetch it's balance.
	/// 
	/// This is optional, of course, but why NOT fetch all balances automatically as part of the login
	/// process rather than contacting the server for each individual currency one at a time afterwards?
	/// </summary>
	void PopulateInitialBalances() => StartCoroutine(WaitForConnection());//should have happened in the previous scene already but play it safe non the less	
	IEnumerator WaitForConnection()
	{
		yield return new WaitUntil(() => null != WULogin.fetched_info);
		string[] currencies = new string[]{WPServer.GameID+"_currency_points", WPServer.GameID+"_currency_credits", WPServer.GameID+"_currency_pebbles", };
		tapjoy_text.text = WULogin.fetched_info.Int(currencies[0]).ToString();
		credits_text.text = WULogin.fetched_info.Int(currencies[1]).ToString();
		pebbles_text.text = WULogin.fetched_info.Int(currencies[2]).ToString();
	}

	void OnPointsEarned(MBSEvent response) => tapjoy_text.text = response.details[0].Int("total").ToString();
	
	void UpdateTextField(MBSEvent response)
	{
		string value = response.details[0].Int().ToString();
		switch(response.details[0].String("currency"))
		{
		case "points"	: tapjoy_text.text = value; break;
		case "credits"	: credits_text.text = value; break;
		case "pebbles"	: pebbles_text.text = value; break;
		}
	}

	void UpdateTextFieldFailure(MBSEvent response)
	{
		StatusMessage.Message = response.details[0].String("message");
		UpdateTextField( response );
	}

	#if WUTJ
	public void ShowVideo()
	{
		WUTJPlacement.ShowPlacement(eTJPlacements.Video_Request);
	}

	public void ShowAd()
	{
		WUTJPlacement.ShowPlacement(eTJPlacements.Interstitial_Request);
	}

	public void ShowOfferWall()
	{
		WUTJPlacement.ShowPlacement(eTJPlacements.OfferWall_Request);
	}
	#endif

	/// <summary>
	/// Fetch the current balance of the specified currency
	/// </summary>
	/// <param name="currency">The exact, case sensitive name of the currency you want the balance of</param>
	public void FetchBalance(string currency) => WUMoney.GetCurrencyBalance(currency);	

	/// <summary>
	/// Spend some currency. In this demo the amount of currency is hardcoded to 15. Replace with an appropriate value
	/// </summary>
	/// <param name="currency">The exact, case sensitive name of the currency you wish to spend</param>
	public void SpendPoints(string currency) =>	WUMoney.SpendCurrency(15, currency, "Transactionid"+Time.realtimeSinceStartup);

	/// <summary>
	/// Add to the online currency balance. In this demo the amount of currency is hardcoded to 10. Replace with an appropriate value
	/// </summary>
	/// <param name="currency">The exact, case sensitive name of the currency you wish to add currency to</param>
	public void GivePoints(string currency) =>	WUMoney.AwardCurrency(10, currency, "Transactionid"+Time.realtimeSinceStartup);
}