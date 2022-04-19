using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game {
	public static class Globals {
		public enum DisplayedColor { Red, Orange, Yellow, Green, None };
		public static Vector2 Ghost;
		public static Dictionary<Vector2, Tile> _tiles;
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
		            
		public static int WIDTH;
		public static int LENGTH;
		public static double [ , ] probabilities; // Probability of ghost being in i, j
	}
	

	public class GamePlay {
		
		public static void InitGamePlay(Dictionary<Vector2, Tile> _tiles, int _width, int _height) {
			Globals._tiles = _tiles;
			Globals.WIDTH = _width;
			Globals.LENGTH = _height;
			PlaceGhost();
		 	Globals.probabilities = new double [_width, _height]; // Probability of ghost being in i, j
		 	InitializeProbabilities();
	    }
		
	    // This function is ran at the very beginning, in Start() I believe.
	    public static void PlaceGhost() {
	    	var ghost = new Vector2(Random.Range(0, Globals.WIDTH - 1), Random.Range(0, Globals.LENGTH - 1));
	        Globals.Ghost = ghost;
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
	                Vector2 vec = new Vector2((float)i, (float)j);
	                Tile t = GetTileAtPosition(vec);
	                t.SetProba(Globals.probabilities[i, j]);
	            }
	        }
	    }
	    
	    public static void InitializeProbabilities() {
	        double initial_proba = (double) 1 / ( Globals.WIDTH * Globals.LENGTH );
	        for(int i = 0; i < Globals._tiles.Count; i++) {
	        	Tile t = Globals._tiles.ElementAt(i).Value;
	            t.SetProba(initial_proba);
	        }
	        for (int i = 0; i < Globals.WIDTH; i++) {
	            for (int j = 0; j < Globals.LENGTH; j++) {
	                Globals.probabilities[i, j] = initial_proba;
	            }
	        }
	    }

	    // this should be called by the onMouseDown() and should take the position of the tile
	    // Change the proba of the clicked tile and normalize with other cells
	    public static void UpdatePosteriorProbabilities(Vector3 clickedTilePos, Tile t) {
	        // Run on each click.
	        int x = (int)clickedTilePos.x;
	        int y = (int)clickedTilePos.y;
	        Debug.Log("The clicked tile: "+ x+ " "+y);
	        double prev_prob = Globals.probabilities[x, y];
	        int distance = GetDistanceFromGhost(x, y);
	        Globals.DisplayedColor curr_color = GetDisplayedColor(distance);
	        double color_prob = ConditionalProbabilityDistribution(curr_color, distance);
	        double new_prob = prev_prob * color_prob;
	        Debug.Log("Ghost pos: " + Globals.Ghost);
	        Debug.Log(prev_prob + " " + distance + " " + curr_color + " " + color_prob);
	        Globals.probabilities[x, y] = new_prob;
	        t.SetProba(new_prob);
	        t.SetColor(curr_color);

	        Normalize();
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
			return Globals.GreenDict;
	    }
	    
	    public static Globals.DisplayedColor GetDisplayedColor(int distance) {
	        var rand = new System.Random(); // Unless I use var, I get an error. I found similar usage in the docs.
	        double rand_val = rand.NextDouble();
	        Dictionary<Globals.DisplayedColor, double> dist = GetRelevantDistribution(distance);
	        List<double> probas = System.Linq.Enumerable.ToList(dist.Values);
	        probas.Sort();
	        double to_match = probas[rand.Next(0, 4)];

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
			return Globals.DisplayedColor.Orange;
	    }
	    
	    public static double ConditionalProbabilityDistribution(Globals.DisplayedColor color, int distance) {
			double proba;
	        GetRelevantDistribution(distance).TryGetValue(color, out proba);
			return proba;
	    }
	    
	    public static bool Bust(Vector2 busted) {
	    	return busted.x == Globals.Ghost.x && busted.y == Globals.Ghost.y;
	    }
	    // Not sure if we'll need this
	    public static Tile GetTileAtPosition(Vector2 pos) {
	        if (Globals._tiles.TryGetValue(pos, out var tile)) return tile;
	        return null;
	    }
	    
	    public static void Main(string[] args) {
	        Debug.Log("We probably don't need this.");
	    }
	}
}
