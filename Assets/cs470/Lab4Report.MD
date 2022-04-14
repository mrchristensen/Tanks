# CS 470 - Lab 4 Report

## Intro
For this lab, I wanted to add an AI opponent to the tank game I have been making over that last couple years.
As the game currently stands, there are two players placed into a map.
Each tank has 4 shots and can take 2 hits before dying.
I wanted to create the ability for someone to play against a computer controlled AI tank, which mimics the behaviour of a human player and offers some challenge.

The files for this lab can be found in: ```Assets/Scripts/Tank/AI```

## Concept
Initially, I sat down to roughly sketch the approach I would take the model the AI opponent.
I first played a round of the game while trying to be cognizant of my own approach to the game.
After this testing, I realized that I thought of the game in stages of attacking and defending, which is when I knew I wanted to think about the model in terms of states (think: finite state automata).
I briefly sketched out rough ideas of states, and the transitions between them on paper before jumping into the code.

![](./images/plan.jpg)

## Navigation
The first thing I implemented was navigation for the AI opponent.
I knew that the ability to move to a given location was going to be important functionality in all states of the AI's program, and thus wanted to make sure I could tackle it first.
I am using the Unity Game Engine for this project, and I such I generated a nav mesh for the AI tank.
Generating a proper nav mesh required me to define which objects were obstacles and which where traversable terrain.

![](./images/nav_mesh.PNG)

With the nav mesh generated, I was able to create a nav mesh agent that would be able to navigate on the mesh's plain.
The nav mesh agent component utilizes the [A* search algorithm](https://en.wikipedia.org/wiki/A*_search_algorithm) to be able to traverse the plain.
After some configuration I made a new prefab for the AI opponent that replaced human movement script with a temporary script that set the nav mesh agent's destination to the player's current position:

![](./images/Basic%20Movement.gif)

This worked quite well, as the opponent now constantly chases the player.
I then started to think about how I wanted to handle the way that the AI choose it's destination.
My mind first went to finding ways of detecting the player: by sight lines and by noise.
I could add sensors to my AI tank to detect movement and noise and then pursue the sensed object.
But then I realized that the goal was to mimic a human player, and as such, the human player would be able to see the whole screen.
Thus, it follows that the AI opponent would also be able to observe what is visible on the screen, such as the location of the other player.
So I continued to allow the AI to move directly towards the player while in the ```seeking state```.

Next, I worked on a state transition to an ```attack phase```.
I initially set up the transition to happened when the player was in site.
At this point, the AI opponent would stop updating it's destination as it entered it's ```attack phase```: 

![](./images/Basic%20States%20(seek%20and%20attack).gif)

Next, a simple "look at" function was applied to the tank so that the enemy would aim at the player.
Again, using the logical flow that the AI opponent should be privy to the same information as a human player, I allowed the AI to track at the other player without need of sensing the opponent: 

![](./images/Aiming.gif)

I simply then called the tank's function to shoot whenever in the ```attack stage```, the same function as the human player.
The state only runs as long as the opponent is in sight.
If the player breaks the sight line the AI will seek to find the player by navigating towards them:

![](./images/Stoping%20after%20finding.gif)

Finally, I added one last state: when the player was in the ```attack phase``` (in sight), but the AI was out of bullets, the AI would move to a "```flee```" (or ```reload state```).
In this state, the AI would attempt to break the sight line with the player so that would not have an easy shot while the AI was harmless (reloading).
To do this, I placed potential hiding place markers on the map for the AI (again, things that a human player could visually see and recall).
The AI would go through each potential hiding place and rate it with a heuristic.
The heuristic valued sites that were close that also break the line of sight.
As such, after the AI runs out of ammunition they run to seek cover until reloading is completed, in which case the AI transitions to the ```search state``` (to hunt down the player)":

![](./images/flee.gif)

## Future work
There are many ways this AI could be improved.

Firstly, I want to be able to give the AI opponent different difficulty levels.
For example, the AI could predict where the player is going in order to lead its shot (and count for the bullet's travel time).
Additionally, the AI could also factor in the ability to bounce the bullet off of walls.
This would allowed the AI to shoot at the player before having a line of sight, or to flush the player out of hiding.
The AI could also be programmed to avoid bullets that would hit the AI (or even shoot them out of the air).
The potential is endless.

Additionally, the AI only handles one opponent.
For the final game, it would be important handle multiple adversaries.
It would also be interesting to try and create an AI that can work with other teammates (both AI and human).
Perhaps lay down cover fire when a teammate is reloading, pull pincer maneuvers, or coordinate pushes.

The ```reload state``` is also very temporary.
It doesn't take into account the ability the player has to bounce, nor does it find cover optimally.
It would be interesting to allow the AI to find cover closest through A* pathfinding, or another organic way.

## Conclusion
In closing, I think the best way to go forward with this AI would be to attempt to create an unbeatable computer opponent.
By implementing all the possible enhancement functionality, the AI could then be scaled back to create different difficulties.
Additionally, by trying to create an optimal AI, it would be easy to verify which functionality helps the AI to perform better against opponents.

Additionally, I have come to realize how the closed and finite domain of a game is perfect for meaning AI creation.
The ability to have a clear goal, a way to measure incremental success towards that overarching goal, and a clear set of actions makes for the perfect playground for AI.
I enjoyed having a concrete domain in which to apply the principles of this class in order to create something meaningful and useful.

Nonetheless, it still took considerable time to create the bare-bones essentials for an AI opponent in my game.
It is still difficult (both mentally and technically) to simulate the though process and strategies of a human player to an acceptable degree.
However, what stands already is a fun and challenging AI opponent, which was equally fun to design and implement:

~[](./images/final%20product.gif)

### Time Spent
![](./images/time.PNG)