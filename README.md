# Ghostbusters

Bust the ghost with your noisy sensors using Bayesian inferencing.  
Mini project for CSC 4301 with teammate Khaoula Ait Soussi.

## Overview
The player (you) starts the game with a 8x20 grid of tiles and is equipped with a ghost sensor that is often slightly inaccurate. The goal is to find the position of the ghost, which is static throughout the game, based on some beliefs that are updated whenever the player picks up on some signal. Initially, the ghost could be anywhere. The player can pick up on a signal in any tile by clicking it once. On click, a reading gives one of 4 colors:
- Red: the ghost is probably on this tile
- Orange: the ghost is probably 1 or 2 tiles away
- Yellow: the ghost is probably 3 or 4 tiles away
- Green: the ghost is probably further than 5 tiles away
These colors could be inaccurate, nevertheless they are displayed. Also displayed is a new numeric probability expressing how likely it is that the ghost is in that tile.  
The player wins if they bust (click a second time on a clicked tile) the ghost correctly, i.e. guess the true position. They lose if they bust incorrectly. Until then, the player is free to explore the grid and collect information based on the colors and the probabilities. 
In this project, we were required to design and implement the grid on Unity and implement the underlying inferencing logic using a Bayesion inference formula suggested in the requirements. 

## Demo
Here's a screen recording of a couple of games: https://www.youtube.com/watch?v=BwHmHBzISlk. 

## Code

### Files
Under `Assets/Scripts`, the 3 files we're concern with, in increasing importance, are:
- `GameGrid.cs`: generates the grid and calls the functions that initialize the Ghost's position and the prior probabilities
- `Tile.cs`: controls the display of each tile in the grid, including text, color, and user interactions
- `probabilities.cs`: the main piece of the project, involving the conditional probabilities we defined, belief updating, and normalization

### Logic

- Initialization:
    * The ghost's initial position on the grid is randomized
    * All colors are `None`
    * All probabilities are equal to 1 / (width * height)
- On click:
    * The tile's color is pseudo-randomly determined, based on a random sampling from the relevant color distribution given the real distance (`GetDisplayedColor()`)
    * The updated belief that the ghost is in that tile is computed (`UpdatePosteriorProbabilities()`)
    * The distribution over the grid is normalized to account for the fact that the formula we are using is not an exact equality but a proportionality. Each tile's probability is divided by the sum over the grid (`Normalize()`)
- On bust:
    * The player wins if the bust is correct, loses otherwise (`Bust()`)

## Inferencing

### Conditional distribution
Since the distance between any tile and the ghost is fixed throughout the game while the sensors are noisy, the color to be displayed based on the distance is probabilistic, calling for a definition of conditional probability distribution of color given distance. We defined such a distribution in `probabilities.cs`, such that the Manhattan distance in question defines what probability should be assigned to each color. The result is 4 dictionnaries representing the 4 distributions given the 4 ranges defined for the distance. For example, if the actual distance is 0, `RedDict` is the relevant distribution because it favors the true color, red. Though each distribution favors the true color, it assigns some smaller probabilities to the 3 other colors to create the noisy effect. 
After the relevant distribution is chosen, the color is assigned through sampling: a random number is generated and it falls in the interval corresponding to one of the 4 colors in the distribution. That color is displayed, though it may not necessarily be the favored color given the distance. 
Tweaking the values we chose for each of the 4 distributions would either increase or decrease the accuracy of the sensor, i.e. the correctness of the displayed color. We believe the displayed colors with our current values give a good experience in gameplay (for example, orange tiles surround red tiles, green tiles are often far away from the "hotter" tiles, etc.), but many other possibilities would have worked as well. 

### Prior and posterior probabilities
At first, the distribution of the probability of the ghost being in some tile is uniform over the grid = 1 / (8 * 20) = 0.625%. On each click, the player's beliefs are affected by the color provided by the sensor. Hence, the color is the evidence and the probability of the ghost being in the clicked tile is the query. Let's say we clicked tile g and it turned into color c. Following Bayes' Rule, this is the kind of formula we would expect: 
P(Ghost = g | Color = c) is proportional to P(g) * P(c | g)
That is, the displayed probability is proportional to the product of the prior probability that the ghost is in the clicked cell and the probability of the displayed color occurring given that the ghost is in the cell. That's the kind of inferencing we would have implemented: we would have taken the event "Distance = 0" as equivalent with "Ghost = g" and used a noisy P(c | g) using `RedDict`'s distribution only. 
However, because of the requirements, the following update was used:
P(g | c) = P(g) * P(c | d) 
where d is the actual distance of the clicked cell from the ghost, and P(c | d) is the probability of getting the displayed color given the actual distance. That probability is the closest value in the distribution to the random number we generated earlier in `GetDisplayedColor()`. My interpretation for the contribution of this factor in the inferencing formula is that it is a measure of how "spot-on" the color was. My guess is that it would guarantee some consistency between the color and value. See the next section for more thoughts on this.

## Challenges, Limitations
- Obscure Bayesian inferencing: the suggested formula for this project probably wouldn't have been my first pick as a way of updating posterior probabilities. A major issue we saw with the displayed values is that they are often not consistent with the colors, though we were very happy with the experience of the displayed colors. For example, a red tile might turn out to have a lower numeric value than a yellow one, etc.
- Accounting for neighbors:

## Making the project more interesting in the future
- Neighbors
- A dynamic ghost
