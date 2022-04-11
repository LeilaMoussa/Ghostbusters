using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game{

	static class Globals{
		public enum DisplayedColor { Red, Orange, Yellow, Green, None };
		// I don't even know if Vector2 can hold ints or just floats.
		public static Vector2 Ghost;

		// Will have to double check that these add up to 100, lol.
		// The professor said we could tweak these values and see how it affects the gameplay.
		public static Dictionary<DisplayedColor, float> RedDict = new Dictionary<DisplayedColor, float>(){
		                {DisplayedColor.Red, 0.90},
		                {DisplayedColor.Orange, 0.06},
		                {DisplayedColor.Yellow, 0.03},
		                {DisplayedColor.Green, 0.01},
		            };
		public static Dictionary<DisplayedColor, float> OrangeDict = new Dictionary<DisplayedColor, float>(){
		                { DisplayedColor.Red, 0.06 },
		                { DisplayedColor.Orange, 0.85 },
		                { DisplayedColor.Yellow, 0.05 },
		                { DisplayedColor.Green, 0.04 },
		            };
		public static Dictionary<DisplayedColor, float> YellowDict = new Dictionary<DisplayedColor, float>(){
		                { DisplayedColor.Red, 0.02 },
		                { DisplayedColor.Orange, 0.08 },
		                { DisplayedColor.Yellow, 0.80 },
		                { DisplayedColor.Green, 0.10 },
		            };
		public static Dictionary<DisplayedColor, float> GreenDict = new Dictionary<DisplayedColor, float>(){
		                { DisplayedColors.Red, 0.04 },
		                { DisplayedColors.Orange, 0.06 },
		                { DisplayedColors.Yellow, 0.15 },
		                { DisplayedColors.Green, 0.80 },
		            };
		            
		public static int WIDTH = 8;
		public static int LENGTH = 20;
		public static float [ , ] probabilities = new float [WIDTH, LENGTH]; // Probability of ghost being in i, j
		public static DisplayedColor [ , ] colors = new DisplayedColor[WIDTH, LENGTH];
	}
	

	public class HelloWorld
	{
		

	    // This function is ran at the very beginning, in Start() I believe.
	    public static void PlaceGhost() {
	        //Random rand = new Random();
	        const float x = 2; // (float)rand.NextDouble();
	        const float y = 2; // (float)rand.NextDouble();
	        Ghost = new Vector(x, y);
	    }
	    
	    public static void Normalize() {
	        int sum = 0;
	        for (int i = 0; i < WIDTH; i++) {
	            for (int j = 0; j < LENGTH; j++) {
	                sum += probabilities[i][j];
	            }
	        }
	        for (int i = 0; i < WIDTH; i++) {
	            for (int j = 0; j < LENGTH; j++) {
	                probabilities[i][j] = probabilities[i][j] / sum;
	            }
	        }
	    }
	    
	    public static void InitializeProbabilities() {
	        const float initial_proba = 1 / ( WIDTH * LENGTH );
	        for (int i = 0; i < WIDTH; i++) {
	            for (int j = 0; j < LENGTH; j++) {
	                probabilities[i][j] = initial_proba;
	            }
	        }
	        Normalize();
	    }
	    
	    public static void InitializeColors() {
	        // Not sure if this function is useful. We could remove it (along with the None color variation) if there's no point.
	        for (int i = 0; i < WIDTH; i++) {
	            for (int j = 0; j < LENGTH; j++) {
	                colors[i][j] = DisplayedColor.None;
	            }
	        }
	    }
	    
	    public static void Update(int i, int j) {
	        const float prev_prob = probabilities[i][j];
	        // Later, we'll be consistent with our use of int or Vector2, and naming of x, y or i, j
	        const int distance = GetDistanceFromGhost(i, j);
	        const DisplayedColor curr_color = GetDisplayedColor(distance);
	        const float color_prob = ConditionalProbabilityDistribution(curr_color, distance);
	        const float new_prob = prev_prob * color_prob; // right?
	        probabilities[i][j] = new_prob;
	    }
	    
	    public static void UpdatePosteriorProbabilities() {
	        // Run on each click.
	        // Now we need to think of timestamps (and intuitively understand that damn Bayesian Inference formula)
	        for (int i = 0; i < WIDTH; i++) {
	            for (int j = 0; j < LENGTH; j++) {
	                Update(i, j);
	            }
	        }
	    }
	    
	    public static int GetDistanceFromGhost(int x, int y) {
	        // Manhattan distance
	        return Math.Abs(Ghost.x - x) + Math.Abs(Ghost.y - y);
	    }
	    
	    public static Dictionary<DisplayedColor, float> GetRelevantDistribution(int distance) {
	        if (distance == 0) {
	            Dictionary dist = RedDict;
	        } else if (distance == 1 || distance == 2) {
	            Dictionary dist = OrangeDict;
	        } else if (distance == 3 || distance == 4) {
	            Dictionary dist = YellowDict;
	        } else if (distance >= 5) {
	            Dictionary dist = GreenDict;
	        }
	        return dist;
	    }
	    
	    public static DisplayedColor GetDisplayedColor(int distance) {
	        Random rand = new Random();
	        float rand_val = rand.NextDouble();
	        Dictionary dist = GetRelevantDistribution(distance);
	        List<float> probas = Enumerable.ToList(dist.Values);
	        probas.Sort();
	        if (rand_val <= probas[0]) {
	            // I need to get the color, which means I need a list of K and V, not just V
	        } else if (rand_val > probas[0] && rand_val <= probas[0] + probas[1]) {
	            //
	        } else if (rand_val > probas[0] + probas[1] && rand_val <= probas[0] + probas[1] + probas[2]) {
	            //
	        } else if (rand_val > probas[0] + probas[1] + probas[2] && rand_val <= probas[0] + probas[1] + probas[2] + probas[3]) {
	            //
	        } else if (rand_val > probas[0] + probas[1] + probas[2] + probas[3]) {
	            //
	        }
	    }
	    
	    public static float ConditionalProbabilityDistribution(DisplayedColor color, int distance) {
	        // That's a crappy name.
	        // Given a distance (evidence) and a color (query), this function looks up the probability
	        // of obtaining that color given that distance.
	        // The distance is computed using the function above.
	        // The probabilities are derived from the rules, representing noisy sensors.
	        return GetRelevantDistribution(distance).TryGetValue(color); // What's a nicer way to perform lookup?
	    }
	    
	    public static void AssignColor(int i, int j, DisplayedColor color) {
	        colors[i][j] = color;
	    }
	    
	    public static void Bust(int x, int y) {
	        if (x == Ghost.x && y == Ghost.y) {
	            Console.WriteLine("Congrats!");
	        } else {
	            Console.WriteLine("Game over!");
	        }
	    }
	    
	    public static void Main(string[] args)
	    {
	        Console.WriteLine ("Hello Mono World");
	    }
	}
}