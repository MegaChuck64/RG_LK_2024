using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GameCode;

/// <summary>
/// Credit: https://github.com/dmkim/SimpleAStarExample
/// </summary>
public class PathFinder
{
    private int width;
    private int height;
    private PathFinderNode[,] nodes;
    private PathFinderNode startNode;
    private PathFinderNode endNode;
    private PathFinderSearchParams searchParameters;

    /// <summary>
    /// Create a new instance of PathFinder
    /// </summary>
    /// <param name="searchParameters"></param>
    public PathFinder(PathFinderSearchParams searchParameters)
    {
        this.searchParameters = searchParameters;
        InitializeNodes(searchParameters.Map);
        this.startNode = this.nodes[searchParameters.StartLocation.X, searchParameters.StartLocation.Y];
        this.startNode.State = PathFinderNodeState.Open;
        this.endNode = this.nodes[searchParameters.EndLocation.X, searchParameters.EndLocation.Y];
    }

    /// <summary>
    /// Attempts to find a path from the start location to the end location based on the supplied SearchParameters
    /// </summary>
    /// <returns>A List of Points representing the path. If no path was found, the returned list is empty.</returns>
    public List<Point> FindPath()
    {
        // The start node is the first entry in the 'open' list
        List<Point> path = new List<Point>();
        bool success = Search(startNode);
        if (success)
        {
            // If a path was found, follow the parents from the end node to build a list of locations
            PathFinderNode node = this.endNode;
            while (node.ParentNode != null)
            {
                path.Add(node.Location);
                node = node.ParentNode;
            }

            // Reverse the list so it's in the correct order when returned
            path.Reverse();
        }

        return path;
    }

    /// <summary>
    /// Builds the node grid from a simple grid of booleans indicating areas which are and aren't walkable
    /// </summary>
    /// <param name="map">A boolean representation of a grid in which true = walkable and false = not walkable</param>
    private void InitializeNodes(bool[,] map)
    {
        this.width = map.GetLength(0);
        this.height = map.GetLength(1);
        this.nodes = new PathFinderNode[this.width, this.height];
        for (int y = 0; y < this.height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                this.nodes[x, y] = new PathFinderNode(x, y, map[x, y], this.searchParameters.EndLocation);
            }
        }
    }

    /// <summary>
    /// Attempts to find a path to the destination node using <paramref name="currentNode"/> as the starting location
    /// </summary>
    /// <param name="currentNode">The node from which to find a path</param>
    /// <returns>True if a path to the destination has been found, otherwise false</returns>
    private bool Search(PathFinderNode currentNode)
    {
        // Set the current node to Closed since it cannot be traversed more than once
        currentNode.State = PathFinderNodeState.Closed;
        List<PathFinderNode> nextNodes = GetAdjacentWalkableNodes(currentNode);

        // Sort by F-value so that the shortest possible routes are considered first
        nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
        foreach (var nextNode in nextNodes)
        {
            // Check whether the end node has been reached
            if (nextNode.Location == this.endNode.Location)
            {
                return true;
            }
            else
            {
                // If not, check the next set of nodes
                if (Search(nextNode)) // Note: Recurses back into Search(Node)
                    return true;
            }
        }

        // The method returns false if this path leads to be a dead end
        return false;
    }

    /// <summary>
    /// Returns any nodes that are adjacent to <paramref name="fromNode"/> and may be considered to form the next step in the path
    /// </summary>
    /// <param name="fromNode">The node from which to return the next possible nodes in the path</param>
    /// <returns>A list of next possible nodes in the path</returns>
    private List<PathFinderNode> GetAdjacentWalkableNodes(PathFinderNode fromNode)
    {
        List<PathFinderNode> walkableNodes = new List<PathFinderNode>();
        IEnumerable<Point> nextLocations = GetAdjacentLocations(fromNode.Location);

        foreach (var location in nextLocations)
        {
            int x = location.X;
            int y = location.Y;

            // Stay within the grid's boundaries
            if (x < 0 || x >= this.width || y < 0 || y >= this.height)
                continue;

            PathFinderNode node = this.nodes[x, y];
            // Ignore non-walkable nodes
            if (!node.IsWalkable)
                continue;

            // Ignore already-closed nodes
            if (node.State == PathFinderNodeState.Closed)
                continue;

            // Already-open nodes are only added to the list if their G-value is lower going via this route.
            if (node.State == PathFinderNodeState.Open)
            {
                float traversalCost = PathFinderNode.GetTraversalCost(node.Location, node.ParentNode.Location);
                float gTemp = fromNode.G + traversalCost;
                if (gTemp < node.G)
                {
                    node.ParentNode = fromNode;
                    walkableNodes.Add(node);
                }
            }
            else
            {
                // If it's untested, set the parent and flag it as 'Open' for consideration
                node.ParentNode = fromNode;
                node.State = PathFinderNodeState.Open;
                walkableNodes.Add(node);
            }
        }

        return walkableNodes;
    }

    /// <summary>
    /// Returns the eight locations immediately adjacent (orthogonally and diagonally) to <paramref name="fromLocation"/>
    /// </summary>
    /// <param name="fromLocation">The location from which to return all adjacent points</param>
    /// <returns>The locations as an IEnumerable of Points</returns>
    private static IEnumerable<Point> GetAdjacentLocations(Point fromLocation)
    {
        return new Point[]
        {
                new Point(fromLocation.X-1, fromLocation.Y-1),
                new Point(fromLocation.X-1, fromLocation.Y  ),
                new Point(fromLocation.X-1, fromLocation.Y+1),
                new Point(fromLocation.X,   fromLocation.Y+1),
                new Point(fromLocation.X+1, fromLocation.Y+1),
                new Point(fromLocation.X+1, fromLocation.Y  ),
                new Point(fromLocation.X+1, fromLocation.Y-1),
                new Point(fromLocation.X,   fromLocation.Y-1)
        };
    }
}

public class PathFinderSearchParams
{
    public Point StartLocation { get; set; }

    public Point EndLocation { get; set; }

    public bool[,] Map { get; set; }

    public PathFinderSearchParams(Point startLocation, Point endLocation, bool[,] map)
    {
        this.StartLocation = startLocation;
        this.EndLocation = endLocation;
        this.Map = map;
    }
}

public class PathFinderNode
{
    private PathFinderNode parentNode;

    /// <summary>
    /// The node's location in the grid
    /// </summary>
    public Point Location { get; private set; }

    /// <summary>
    /// True when the node may be traversed, otherwise false
    /// </summary>
    public bool IsWalkable { get; set; }

    /// <summary>
    /// Cost from start to here
    /// </summary>
    public float G { get; private set; }

    /// <summary>
    /// Estimated cost from here to end
    /// </summary>
    public float H { get; private set; }

    /// <summary>
    /// Flags whether the node is open, closed or untested by the PathFinder
    /// </summary>
    public PathFinderNodeState State { get; set; }

    /// <summary>
    /// Estimated total cost (F = G + H)
    /// </summary>
    public float F
    {
        get { return this.G + this.H; }
    }

    /// <summary>
    /// Gets or sets the parent node. The start node's parent is always null.
    /// </summary>
    public PathFinderNode ParentNode
    {
        get { return this.parentNode; }
        set
        {
            // When setting the parent, also calculate the traversal cost from the start node to here (the 'G' value)
            this.parentNode = value;
            this.G = this.parentNode.G + GetTraversalCost(this.Location, this.parentNode.Location);
        }
    }

    /// <summary>
    /// Creates a new instance of Node.
    /// </summary>
    /// <param name="x">The node's location along the X axis</param>
    /// <param name="y">The node's location along the Y axis</param>
    /// <param name="isWalkable">True if the node can be traversed, false if the node is a wall</param>
    /// <param name="endLocation">The location of the destination node</param>
    public PathFinderNode(int x, int y, bool isWalkable, Point endLocation)
    {
        this.Location = new Point(x, y);
        this.State = PathFinderNodeState.Untested;
        this.IsWalkable = isWalkable;
        this.H = GetTraversalCost(this.Location, endLocation);
        this.G = 0;
    }

    public override string ToString()
    {
        return string.Format("{0}, {1}: {2}", this.Location.X, this.Location.Y, this.State);
    }

    /// <summary>
    /// Gets the distance between two points
    /// </summary>
    internal static float GetTraversalCost(Point location, Point otherLocation)
    {
        float deltaX = otherLocation.X - location.X;
        float deltaY = otherLocation.Y - location.Y;
        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }


}

public enum PathFinderNodeState
{
    /// <summary>
    /// The node has not yet been considered in any possible paths
    /// </summary>
    Untested,
    /// <summary>
    /// The node has been identified as a possible step in a path
    /// </summary>
    Open,
    /// <summary>
    /// The node has already been included in a path and will not be considered again
    /// </summary>
    Closed
}