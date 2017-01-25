# MinesweepVariation
[C#] School Assignment 

1) Since an applicant is usually applying for a job working with the platform in which the game should be made, your instructor will tell you the platform



2)	The game grid will be 16x16 tiles in size. Applicants will often have some freedom when it comes to UI and placement of such elements. So ensure that your game window has room for the following UI elements:
a.	A mode toggle button. This functionality will be explained below.
b.	A resource counter. A text field that keeps a tally of your collected resource amount.
c.	A message bar. A text field that displays a short message to the user.
d.	Any optional elements you like. Companies will be looking for creativity from the applicant, but remember to keep your interface clean. Don’t assault the player with unnecessary information.



3)	The goal of the game is to collect as much of a special resource, i.e. Tiberium, Unobtanium, generic ore, gold... etc, from the grid. You will place a random amount (e.g. 2000-5000) of this element in 6-8 tiles each around the grid.
a.	To clarify, 6-8 tiles will contain the maximum amount of the resource. Then, each tile around this “central” maximum amount will contain half, then moving outward again, the amount will be ¼. Refer to the following diagram:
b.	You are free to use any color scheme you like but there should be four colors: max resource, half resource, quarter resource and minimal resource.
c.	A minimal resource tile will fill the remaining/blank spaces of the grid. It should be between 1/8th and 1/16th of the maximum



4)	The amount of resource, i.e. the colored tiles will be hidden from the user until they click on the toggle button to select Scan Mode that will allow them to click tiles on the grid. The maximum number of scans should be around 6.
a.	When the player clicks a tile, it will display the resource underneath as well as those tiles that immediately surround it, for a total of 9 tiles.
b.	From these scans, the player may find a maximum amount of resource or at lease close.



6)	When the player has extracted three times, ensure they cannot click anymore and display a final message with their total in the manner of your choice.



7)	You are only required to create the main gameplay screen and no title/help or end screen is required, however you may want to communicate the tile colors and their values to the player with a UI element or help pop-up. Your choice.


