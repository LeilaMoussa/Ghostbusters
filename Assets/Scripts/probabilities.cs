using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game {
	public static class Globals {
		public enum DisplayedColor { Red, Orange, Yellow, Green, None };
		public static Vector2 Ghost;

		// Will have to double check that these add up to 100, lol.
		// The professor said we could tweak these values and see how it affects the gameplay.
		public static Dictionary<DisplayedColor, double> RedDict = new Dictionary<DisplayedColor, double>() {
		                { DisplayedColor.Red, 0.90 },
		                { DisplayedColor.Orange, 0.06 },
		                { DisplayedColor.Yellow, 0.03 },
		                { DisplayedColor.Green, 0.01 },
		            };
		public static Dictionary<DisplayedColor, double> OrangeDict = new Dictionary<DisplayedColor, double>() {
		                { DisplayedColor.Red, 0.06 },
		                { DisplayedColor.Orange, 0.85 },
		                { DisplayedColor.Yellow, 0.05 },
		                { DisplayedColor.Green, 0.04 },
		            };
		public static Dictionary<DisplayedColor, double> YellowDict = new Dictionary<DisplayedColor, double>() {
		                { DisplayedColor.Red, 0.02 },
		                { DisplayedColor.Orange, 0.08 },
		                { DisplayedColor.Yellow, 0.80 },
		                { DisplayedColor.Green, 0.10 },
		            };
		public static Dictionary<DisplayedColor, double> GreenDict = new Dictionary<DisplayedColor, double>() {
						{ DisplayedColor.Red, 0.04 },
		                { DisplayedColor.Orange, 0.06 },
		                { DisplayedColor.Yellow, 0.15 },
		                { DisplayedColor.Green, 0.80 },
		            };
		            
		public static int WIDTH = 8;
		public static int LENGTH = 20;
		public static double [ , ] probabilities = new double [WIDTH, LENGTH]; // Probability of ghost being in i, j
		public static DisplayedColor [ , ] colors = new DisplayedColor[WIDTH, LENGTH];
	}
	

	public class GamePlay {
		
	    // This function is ran at the very beginning, in Start() I believe.
	    public static void PlaceGhost() {
	        //System.Random rand = new System.Random(); // var?
	        const double x = 2; // (double)rand.NextDouble();
	        const double y = 2; // (double)rand.NextDouble();
	        Globals.Ghost = new Vector2((float)x, (float)y);
	    }
	    
	    public static void Normalize() {
	        double sum = 0.0;
	        for (int i = 0; i < Globals.WIDTH; i++) {
	            for (int j = 0; j < Globals.LENGTH; j++) {
	                sum += Globals.probabilities[i, j];
	            }
	        }
	        for (int i = 0; i < Globals.WIDTH; i++) {
	            for (int j = 0; j < Globals.LENGTH; j++) {
	                Globals.probabilities[i, j] = Globals.probabilities[i, j] / sum;
	            }
	        }
	    }
	    
	    public static void InitializeProbabilities() {
	        double initial_proba = 1 / ( Globals.WIDTH * Globals.LENGTH );
	        for (int i = 0; i < Globals.WIDTH; i++) {
	            for (int j = 0; j < Globals.LENGTH; j++) {
	                Globals.probabilities[i, j] = initial_proba;
	            }
	        }
	        Normalize();
	    }
	    
	    public static void InitializeColors() {
	        // Not sure if this function is useful. We could remove it (along with the None color variation) if there's no point.
	        for (int i = 0; i < Globals.WIDTH; i++) {
	            for (int j = 0; j < Globals.LENGTH; j++) {
	                Globals.colors[i, j] = Globals.DisplayedColor.None;
	            }
	        }
	    }
	    
	    public static void Update(int i, int j) {
	        double prev_prob = Globals.probabilities[i, j];
	        // Later, we'll be consistent with our use of int or Vector2, and naming of x, y or i, j
	        int distance = GetDistanceFromGhost(i, j);
	        Globals.DisplayedColor curr_color = GetDisplayedColor(distance);
	        double color_prob = ConditionalProbabilityDistribution(curr_color, distance);
	        double new_prob = prev_prob * color_prob;
	        Globals.probabilities[i, j] = new_prob;
	    }
	    
	    public static void UpdatePosteriorProbabilities() {
	        // Run on each click.
	        // Now we need to think of timestamps (and intuitively understand that damn Bayesian Inference formula)
	        for (int i = 0; i < Globals.WIDTH; i++) {
	            for (int j = 0; j < Globals.LENGTH; j++) {
	                Update(i, j);
	            }
	        }
	    }
	    
	    public static int GetDistanceFromGhost(int x, int y) {
	        // Manhattan distance
	        return (int)(System.Math.Abs(Globals.Ghost.x - x) + System.Math.Abs(Globals.Ghost.y - y));
	    }
	    
	    public static Dictionary<Globals.DisplayedColor, double> GetRelevantDistribution(int distance) {
	        if (distance == 0) {
	            return Globals.RedDict;
	        } else if (distance == 1 || distance == 2) {
	            return Globals.OrangeDict;
	        } else if (distance == 3 || distance == 4) {
	            return Globals.YellowDict;
	        } else if (distance >= 5) {
	            return Globals.GreenDict;
	        }
			return Globals.RedDict; // Same problem as below with ensuring a value is always returned.
	    }
	    
	    public static Globals.DisplayedColor GetDisplayedColor(int distance) {
	        var rand = new System.Random(); // Unless I use var, I get an error. I found similar usage in the docs.
	        double rand_val = rand.NextDouble();
	        Dictionary<Globals.DisplayedColor, double> dist = GetRelevantDistribution(distance);
	        List<double> probas = System.Linq.Enumerable.ToList(dist.Values);
	        probas.Sort();
	        double to_match = 0; // This is insane. It's complaining about to_match being unassigned at compile time.
			// I'll find a better way, even if it means uglier code.

			if (rand_val <= probas[0]) {
				to_match = probas[0];
			} else if (rand_val > probas[0] && rand_val <= probas[0] + probas[1]) {
				to_match = probas[1];
			} else if (rand_val > probas[0] + probas[1] && rand_val <= probas[0] + probas[1] + probas[2]) {
				to_match = probas[2];
			} else if (rand_val > probas[0] + probas[1] + probas[2] && rand_val <= probas[0] + probas[1] + probas[2] + probas[3]) {
				to_match = probas[3];
			}

			foreach(KeyValuePair<Globals.DisplayedColor, double> entry in dist) {
				if (entry.Value == to_match) {
					return entry.Key;
				}
			}
			return Globals.DisplayedColor.None; // Trying to avoid error saying the function may not return anything sometimes.
			// Maybe we should make the default value an actual color -- why not? The sensor is supposed to be noisy anyway.
	    }
	    
	    public static double ConditionalProbabilityDistribution(Globals.DisplayedColor color, int distance) {
			double proba;
	        GetRelevantDistribution(distance).TryGetValue(color, out proba);
			return proba;
	    }
	    
	    public static void AssignColor(int i, int j, Globals.DisplayedColor color) {
	        Globals.colors[i, j] = color;
	    }
	    
	    public static void Bust(int x, int y) {
	        if (x == Globals.Ghost.x && y == Globals.Ghost.y) {
	            Debug.Log("Congrats!");
	        } else {
	            Debug.Log("Game over!");
	        }
	    }
	    
	    public static void Main(string[] args) {
	        Debug.Log("We probably don't need this.");
	    }
	}
}
