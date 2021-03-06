﻿using RC.Common.ComponentModel;
using RC.Engine.Maps.PublicInterfaces;
using RC.Engine.Simulator.Engine;
using RC.Engine.Simulator.Metadata;

namespace RC.Engine.Simulator.ComponentInterfaces
{
    /// <summary>
    /// Component interface for creating, loading or saving scenarios.
    /// </summary>
    [ComponentInterface]
    public interface IScenarioLoader
    {
        /// <summary>
        /// Creates a new scenario.
        /// </summary>
        /// <param name="map">The map of the scenario.</param>
        /// <returns>The interface of the created scenario.</returns>
        Scenario NewScenario(IMapAccess map);

        /// <summary>
        /// Loads a scenario from the given byte array.
        /// </summary>
        /// <param name="map">The map of the scenario.</param>
        /// <param name="data">The byte array the contains the serialized scenario.</param>
        /// <returns>The interface of the loaded scenario.</returns>
        Scenario LoadScenario(IMapAccess map, byte[] data);

        /// <summary>
        /// Saves the given scenario to a byte array.
        /// </summary>
        /// <param name="scenario">The scenario to be saved.</param>
        /// <returns>The byte array that contains the serialized scenario.</returns>
        byte[] SaveScenario(Scenario scenario);

        /// <summary>
        /// Gets the metadata of the RC game engine.
        /// </summary>
        IScenarioMetadata Metadata { get; }
    }
}
