### WUMoney
Use an unlimited amount of virtual currencies in your gameand/ or use the Self Managed Currency option offered by Tapjoy. Also, sell in-game content directly on your website using WooCommerce and any payment option WooCommerce supports, including Crypto! <br>
Requires [WordPress For Unity Bridge](https://mybadstudios.com/product/wordpress-bridge/).

If You enjoy my work, please consider buying me a coffee...

[<img src="bmcbutton.png">](https://www.buymeacoffee.com/mybad)


#Features

- Sell in-game content directly on your website using WooCommerce
- Sell items without having to pay royalties to third parties
- Accept all forms of payment you can setup on your website including Crypto Currencies
- Includes native support to sell virtual currencies via WooCommerce
- Includes native support to sell games via WooCommerce and (optionally) automatically register it to the buyers' account meaning there is no need for the buyer to first enter a serial to register the game (Requires The Bridge : Serials extension)
- Tapjoy has a [free plugin for Unity](http://www.tapjoy.com) The Bridge : Money extension offers Tapjoy support in the form of a self hosted server

***

# Pros and cons: self hosting vs managed hosting


## TAPJOY HOSTED PROS

- They host the points on their servers for free
- They make sure that the points go to who they belong
- They take responsibility for player's points balances being accurate and up to date
- They provide functions to fetch balances from their servers as well as manually awarding or spending those points
- Receive live notifications of when points are awarded
- The easiest and fastest way to get started monetizing a game


## SELF HOSTED PROS
- Hosting the points on your own servers give you absolutely and complete control over the points!


## TAPJOY HOSTED CONS
- Points are allocated per device, not per user. I.e. if you have a shared device in the house (family iPad, etc) everybody uses the same points so if one person is saving up for the uber, pimped out, triple fortified mega tank but someone else comes along and wants a pink unicorn... Like I said, shared currency so the last person to play gets to decide what the currency is spent on
- If you own two mobile devices and want to play the game on both, each device will have it's own points balance. There is no option to continue play across devices and taking your points with you

## SELF HOSTED CONS
- Tapjoy no longer takes any responsibility for what happens to the points once they have sent it to you
- You have to create your own custom back end server system (per game) to handle interaction with Tapjoy
- This means either needing the skill to create a server to communicate with theirs or having to find someone to do it for you, most likely at a premium
- All responsibility for what happens after Tapjoy sent you the message is entirely on you
- You loose the functions to fetch balances, spend and award points and need to implement this functionality yourself within your custom built web server
- You no longer get any updates of when points are or were awarded. You need to create that functionality yourself

***


# THIS ASSET INCLUDES A COMPLETE SELF HOSTED SERVER 

Automatically works for all games on your domain. It is installed by as part of this asset and offers the following:

## PROS

- A fully functional Tapjoy self managed web server out of the box
- Automatically send Tapjoy the relevant success / error response they require when they request it
- Validate the security values Tapjoy sends with each message:

  * Authenticate the call came from them
  * Is it for your game?
  * Is it a currency you created?

- Prevents unauthorized calls to the server script
- Store points balances per user, not per device
- Points are stored in the cloud and follow players across devices
- Tapjoy supports only iOS or Android. This asset supports any device with an internet connection
- Tapjoy requires that you create a separate currency for Android and iOS for each game. This asset's currencies are interchangeable between devices since it is linked to an account, not to a device
- Includes the functions to fetch balances, spend and give points
- The function names are identical and work the same way as the Tapjoy functions, making it easier to follow along with Tapjoy's instructions on their website while making use of this asset to manage your points
- Each game can have unlimited currencies
- The MBS SDK currencies require 0 configuration: Just start using a currency and it takes care of the rest
- Regain the ability to send a notification when the game starts to tell a player whether any points were awarded to them while the game was closed
- Automatically works with all current and future games
- No configuration required apart from storing the currency keys received from the Tapjoy dashboard
- No need for a custom server per game!

## CONS

- Tapjoy monetization is still limited to iOS and Android devices in terms of earning points FROM Tapjoy
- When Tapjoy sends points to this server, games don't get live updates. Instead, whenever the game contacts the server for anything, any updated points will trigger an event in the game as well as sending the updated balance
- Tapjoy automatically triggers the OnAppLaunched placement as soon as a connection is made to their servers. Now that points are stored per user, not per device, we have to wait for a user to login first and thus their OnAppLaunched placement can no longer be used. This asset uses the Logged_In placement instead to do exactly the same thing

***

# MORE INFO

<img src="http://mybadstudios.com/wp-content/uploads/2017/03/PlacementNames.jpg">

Tapjoy offers a whole host of additional events (i.e. notifications of when a video has started / ended, when a user cancelled performing an action, analytics etc). For exact details on what the Tapjoy SDK can do, please visit their website. Apart from fetching point balances, spending points and awarding points, __developers still use the Tapjoy SDK directly__ for everything else so all those features are still available.

As an added bonus, though, the Bridge : Money extension comes with a simplified method of calling placements. Tapjoy requires a whole bunch of events to be setup and prepared and for the developer to create delegate responders per variable that you create. The MBS SDK takes a different approach and comes with a very long list of possible placements that a developer might want to use. Simply drag the WUTJPlacement component onto the WUTapjoy prefab loaded at the start of the game, pick a placement from the drop down list, make sure to add a match it in the Tapjoy dashboard and that is that. All loading, showing and reloading of placements are taken care of after that.


***

# DISCLAIMER

In order to make use of Tapjoy services developers need to install the Tapjoy asset for Unity, follow the setup instructions from the Tapjoy website for that asset and __must still adhere to all of Tapjoy's rules__!!! The single most important rule being that TapJoy does not allow their virtual currencies to be used in the purchase of real world items or in any form of gambling.

All matters relating to setting up TapJoy's plugin in Unity and getting it to work is all done and explained on their website


# [For more info on this asset, visit it's page on the myBad Studios website... ](https://mybadstudios.com/product/bridge-money/)
