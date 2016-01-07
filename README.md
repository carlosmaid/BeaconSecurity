#Beacon Security
- anti-grief, indestructible private property
- indestructible admin property by list of grid names

##What is the point?
It's a beacon, which itself determines the presence in the vicinity of the grid, where it is, or her, the owner or a member of the faction.
If the owner close to the grid or manage it through the cabin via a remote control or even in some way, the beacon is switched off.
Beacon does not work, if the grid is not owned by anyone.
If the beacon is switched on - grid indestructible, a new blocks can not be installed on the grid (to avoid the possibility of conversion of the ship).
Adjusting for the beacon to work only for the stations.
Setting up a beacon for work only at the zero motion / rotation.
Setting delay beacon
Setting the zone of finding the owner.

##Chat Commands
- /bs help [1-10] - Show this page or explanation of variables
- /bs config  - Show current settings

###Admin chat commands
- /bs debug - Enable / disable this debug
- /bs on/off - Enable / disable this mod
- /bs set number/name value - Set variable, you can use numbers and on/off, true/false
- /bs list - List of indestructible grids
- /bs find radius - Look for the nearest grid specified radius. Then you can add them using the command /bs add or delete /bs rem
- /bs add GridName - Add a grid to the list indestructible
- /bs rem GridName - Remove the grid from the list indestructible
- /bs clear - Clearing the list

##Config variables
1. DelayBeforeTurningOn(0-3600) - The delay before turning on protection in seconds. This time the owner should not be in the zone of action Beacon Security. Default: 60
2. DistanceBeforeTurningOn(0-10000) - The distance at which no owner to be found. Or the player can simply leave the game.If value set to 0, search infinity space for owner, in other words checks owners online.Default: 400
3. OnlyForStations(on/off) - Turn on/off limitation of the Beacon Security only for stations. Default: off
4. OnlyWithZeroSpeed(on/off) -  If this option is enabled, a Beacon Security works only on grid with zero speed. Default: on
5. BuildingNotAllowed(on/off) - Turn on/off the ability to build on grid with the Beacon Security. Default: on
6. IndestructibleNoBuilds(on/off) - Turn on/off the ability to build on indestructible grids. Default: on
7. LimitGridSizes(0-1000) - Limitation on the sizes for grid in meters. If there are excess size, the Beacon Security would't work. Default: 150. 0 - disabled
8. LimitPerFaction(1-100) - Limitation on the number of Beacon Security per faction. Default: 30
9. LimitPerPlayer(1-100) - Limitation on the number of Beacon Security pers player. Default: 3
10. CleaningFrequency(0-3600) - How often is the cleaning in seconds. Default: 5. 0 - disabled

##Some chat commands examples
```
/bs set 1 10
/bs set 10 100
/bs set BuildingNotAllowed on
/bs set OnlyForStations false
/bs find 10 - found 3 grids: Platform 3545, Large Ship 2334, Small Ship 9943
/bs add
/bs rem Small Ship 9943
```


##Donation
- PayPal: jimless2015@gmail.com
- Webmoney: R711901402714, Z227667032008
- Money.Yandex.ru: 41001129000883

words: anti-grief, antigrief, indestructible, destructible, admin, private, property, zone, safezone, Beacon, Security

© 2015 JimLess