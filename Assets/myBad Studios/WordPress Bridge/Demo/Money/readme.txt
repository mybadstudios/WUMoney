ABOUT THIS DEMO
===============

This demo demonstrates how to create 3 different currencies, add to them and spend them.
You are freeto createas many curriencies as you want to for each game. For instance, you could create a custom
currency for each race in your RPG... Skulls, Gold, Dablooms, FrogsLegs... it's all good.

To create a new currency, just award a player with some points in that currency and it will be created. That easy.

You can spend currency with a single call to SpendPoints(qty, currency). The server tracks your user's currency for
you to prevent cheating. For example, if a player has 5 copies of the game running on 5 devices and spends all his 
Gold on one device then tries to buy something else on another device that still shows he has all his Gold available,
the transaction will fail and his balance on that device will be updated to reflect his real balance as reported by the server.

Clicking onthe various buttons in thedemo will give you points, spend points and allow  you to just fetch the balances.
In the login prefab you can speecify the names of the currencies and it will then automatically fetch their balances
as part of the login process. This way your currencieswill start with their values all set. Alternatively, click the
"Fetch balance" button of each currency individually to fetch the value of that currency.

ABOUT TAPJOY
============

Integration with Tapjoy is optional but if you want to do so then you must first install their SDK into the project
and then add the WUTJ compiler directive to your Player Settings. Fromhere, plese follow the instructions on the Tapjoy
website to integrate Tapjoy into your project. 

WUB Money does, however, include the WUTJPlacement component to simplify setup of your Tapjoy placements inside your project.
Once you have Tapjoy setup, see the MoneyDemo_Bootstrap demo scene for an example of it's use. 

Note that until you have completed these steps Unity will show most of the scripts in that demo as "Not found".
Once you have completed Tapjoy setup and added the WUTJ directive to player settings the scripts will appear on the prefab
and you will be able to run the complete demo.

By default Tapjoy only supports watching videos and completing offers on mobile. It only supports awarding currency 
to a specific device which means that you can't play the game on two devices and take yor currency with you. It also means
that everyone using that device will share the currency. WUB Money solves both those problems by integrating a back end 
TapJoy server on your website. This allows Tapjoy to send the points to your website and from there WUB Money can assign
the points to the relevant player. This allows one player to spend his currency on every device he owns while also
allowing people to share a device without sharing their currency. Currency is now awarded to players,not to devices!

Tapjoy awards points for performing certain actions on mobile devices and awards the player these points anytime up to
two weeks after they performed that action. Each time you poll the currencies it will check to see if Tapjoy has sent any
points to the player and will send the player an in-game notification of how many points was recieved first chance it gets.
The currency will already have been updated so the info is merely for information purposes. 

On the server side WUP Money also keeps logs of when who recieves how much from Tapjoy to help you with any claims 
from players that they didn't recieve their points.

Note that while players can earn points from Tapjoy by performing certain tasks, their points are stored on your server
like any other currency. This means that you are able to give them extra currency in the same format that Tapjoy
awards them in exactly the same way as you award them any other currency.

Simply specify the currency name as "points" when you award them with currency and it will look like it came from Tapjoy.
It truly is as simple as that.
