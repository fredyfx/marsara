﻿using RC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC.Engine.Simulator.MotionControl
{
    /// <summary>
    /// Represents a connected part of a navmesh. A Sector might contain walls. A wall is a connected sub-part of the interior area of the sector that doesn't
    /// belong to the sector.
    /// </summary>
    class Sector
    {
        /// <summary>
        /// Constructs a new Sector instance.
        /// </summary>
        /// <param name="grid">The grid that contains the walkability informations.</param>
        /// <param name="sectorArea">The area of the sector on the grid.</param>
        /// <param name="maxError">The maximum error between the edges of the created sector and the walkability informations.</param>
        public Sector(IWalkabilityGrid grid, WalkabilityGridArea sectorArea, RCNumber maxError)
        {
            if (grid == null) { throw new ArgumentNullException("grid"); }
            if (sectorArea == null) { throw new ArgumentNullException("sectorArea"); }
            if (!sectorArea.IsWalkable) { throw new ArgumentException("The area of the sector must be walkable!"); }
            if (maxError < 0) { throw new ArgumentOutOfRangeException("maxError", "The maximum error shall not be negative!"); }

            /// Create the polygons of the border and the walls.
            this.border = new Polygon(grid, sectorArea.TopLeftCell, maxError);
            if (this.border.VertexCount <= 2) { return; }
            this.walls = new List<Polygon>();
            foreach (WalkabilityGridArea wallArea in sectorArea.Children)
            {
                if (wallArea.IsWalkable) { throw new ArgumentException("The area of the walls inside the sector must be non-walkable!"); }
                Polygon wallPolygon = new Polygon(grid, wallArea.TopLeftCell, maxError);
                if (wallPolygon.VertexCount > 2) { this.walls.Add(wallPolygon); }
            }

            /// Construct a tessellation of the vertices of the polygons.
            Tessellation tessellation = new Tessellation();
            for (int i = 0; i < this.border.VertexCount; i++) { tessellation.AddVertex(this.border[i]); }
            for (int wallIdx = 0; wallIdx < this.walls.Count; wallIdx++)
            {
                for (int i = 0; i < this.walls[wallIdx].VertexCount; i++) { tessellation.AddVertex(this.walls[wallIdx][i]); }
            }

            /// Add the constraints to the tessellation.
            tessellation.AddBorder(this.border);
            foreach (Polygon wall in this.walls) { tessellation.AddBorder(wall); }

            /// TODO: finish this constructor!
        }

        /// <summary>
        /// Gets the border of this Sector.
        /// </summary>
        public Polygon Border { get { return this.border; } }

        /// <summary>
        /// Gets the list of the walls of this Sector.
        /// </summary>
        public IEnumerable<Polygon> Walls { get { return this.walls; } }

        /// <summary>
        /// The Polygon that defines the border of this Sector. A point on the 2D plane is said to be inside this Sector if and only if
        /// it is inside this border but outside of any of the walls of this Sector.
        /// </summary>
        private Polygon border;

        /// <summary>
        /// List of the Polygons defining the walls of this Sector.
        /// </summary>
        private List<Polygon> walls;

        /// <summary>
        /// The list of the nodes of this Sector of the navmesh.
        /// </summary>
        private List<NavMeshNode> nodes;
    }
}