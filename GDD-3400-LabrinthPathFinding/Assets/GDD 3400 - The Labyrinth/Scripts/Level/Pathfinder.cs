using System.Collections.Generic;
using UnityEngine;

namespace GDD3400.Labyrinth
{
    public class Pathfinder
    {
        public static List<PathNode> FindPath(PathNode startNode, PathNode endNode)
        {
            //TODO: Implement A* pathfinding algorithm
            
            //List of the nodes we might want to take
            List<PathNode> openSet = new List<PathNode>();

            //Nodes we've already looked at
            List<PathNode> closeSet = new List<PathNode>();

            Dictionary<PathNode, PathNode> cameFromNode = new Dictionary<PathNode, PathNode>();

            //Saves path information back to Start
            Dictionary<PathNode, float> costSoFar = new Dictionary<PathNode, float>();
            Dictionary<PathNode, float> costToEnd = new Dictionary<PathNode, float>();

            //Initialize the starting info
            openSet.Add(startNode);
            costSoFar[startNode] = 0f;
            costToEnd[startNode] = Heuristic(startNode, endNode);


            while (openSet.Count > 0)
            {
                PathNode current = GetLowestCost(openSet, costToEnd);

                if (current == endNode)
                {
                    return ReconstructPath(cameFromNode, current);

                }

                //move the current node from open to closed
                openSet.Remove(current);
                closeSet.Add(current);

                //Look at each of the current node's neighbors
                foreach (var connection in current.Connections)
                {
                    PathNode neighbor = connection.Key;

                    //If we've already looked at the neighbor, skip
                    if (closeSet.Contains(neighbor)) continue;

                    float tentativeCostFromStart = costSoFar[current] + connection.Value;

                    //if we haven't yet looked at this node, add it to the open set
                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                    //otherwise if the cost from start is greater (longer path), skip this neighbor
                    else if (tentativeCostFromStart >= costSoFar[neighbor]) continue;

                    //Record best path, and update costs
                    cameFromNode[neighbor] = current;
                    costSoFar[neighbor] = tentativeCostFromStart;
                    costToEnd[neighbor] = costSoFar[neighbor] + Heuristic(neighbor, endNode);

                }


            }

            

            return new List<PathNode>(); // Return an empty path if no path is found
        }

        // Calculate the heuristic cost from the start node to the end node, manhattan distance
        private static float Heuristic(PathNode startNode, PathNode endNode)
        {
            return Vector3.Distance(startNode.transform.position, endNode.transform.position);
        }

        // Get the node in the provided open set with the lowest cost (eg closest to the end node)
        private static PathNode GetLowestCost(List<PathNode> openSet, Dictionary<PathNode, float> costs)
        {
            PathNode lowest = openSet[0];
            float lowestCost = costs[lowest];

            foreach (var node in openSet)
            {
                float cost = costs[node];
                if (cost < lowestCost)
                {
                    lowestCost = cost;
                    lowest = node;
                }
            }

            return lowest;
        }

        // Reconstruct the path from the cameFrom map
        private static List<PathNode> ReconstructPath(Dictionary<PathNode, PathNode> cameFrom, PathNode current)
        {
            List<PathNode> totalPath = new List<PathNode> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }
            return totalPath;
        }
    }
}
