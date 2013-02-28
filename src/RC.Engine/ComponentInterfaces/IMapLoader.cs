﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RC.Common.ComponentModel;
using RC.Common;
using RC.Engine.PublicInterfaces;

namespace RC.Engine.ComponentInterfaces
{
    /// <summary>
    /// Component interface for creating, loading or saving maps.
    /// </summary>
    [ComponentInterface]
    public interface IMapLoader
    {
        /// <summary>
        /// Creates a new map.
        /// </summary>
        /// <param name="tileset">The tileset of the new map.</param>
        /// <param name="defaultTerrain">Name of a terrain type defined by the tileset that will be the default terrain of the new map.</param>
        /// <param name="size">
        /// Size of the map in quadratic tiles. The first coordinate of the vector is the width, the second coordinate of
        /// the vector is the height of the new map. The constraints are the followings: the width of the map must be
        /// a multiple of MapConstants.QUAD_PER_ISO_VERT, the height of the map must be a multiple of MapContants.QUAD_PER_ISO_HORZ.
        /// </param>
        /// <returns>The interface of the created map.</returns>
        IMapAccess NewMap(ITileSet tileset, string defaultTerrain, RCIntVector size);

        /// <summary>
        /// Loads a map from the given data package.
        /// </summary>
        /// <param name="tileset">The tileset of the map.</param>
        /// <param name="data">The package that contains the serialized map.</param>
        /// <returns>The interface of the loaded map.</returns>
        IMapAccess LoadMap(ITileSet tileset, RCPackage data);

        /// <summary>
        /// Saves the given map to a data package.
        /// </summary>
        /// <param name="map">The map to be saved.</param>
        /// <returns>The package that contains the serialized map.</returns>
        RCPackage SaveMap(IMapAccess map);
    }
}
