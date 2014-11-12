﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RC.App.BizLogic.Services.Core;
using RC.App.BizLogic.Views;
using RC.App.BizLogic.Views.Core;
using RC.Common.ComponentModel;
using RC.Common.Diagnostics;
using RC.Engine.Maps.ComponentInterfaces;
using RC.Engine.Maps.PublicInterfaces;
using RC.Engine.Simulator.ComponentInterfaces;
using RC.Engine.Simulator.MotionControl;
using RC.Engine.Simulator.Scenarios;

namespace RC.App.BizLogic.BusinessComponents.Core
{
    /// <summary>
    /// The implementation of the ScenarioManager business component.
    /// </summary>
    [Component("RC.App.BizLogic.ScenarioManagerBC")]
    class ScenarioManagerBC : IScenarioManagerBC, IComponent
    {
        #region IComponent methods

        /// <see cref="IComponent.Start"/>
        public void Start()
        {
            this.viewFactoryRegistry = ComponentManager.GetInterface<IViewFactoryRegistry>();
            this.tilesetManager = ComponentManager.GetInterface<ITilesetManagerBC>();
            this.mapLoader = ComponentManager.GetInterface<IMapLoader>();
            this.scenarioLoader = ComponentManager.GetInterface<IScenarioLoader>();
            this.navmeshLoader = ComponentManager.GetInterface<INavMeshLoader>();
            this.pathFinder = ComponentManager.GetInterface<IPathFinder>();
        }

        /// <see cref="IComponent.Stop"/>
        public void Stop()
        {
        }

        #endregion IComponent methods

        #region IScenarioManagerBC methods

        /// <see cref="IScenarioManagerBC.NewScenario"/>
        public void NewScenario(string mapName, string tilesetName, string defaultTerrain, Common.RCIntVector mapSize)
        {
            if (this.activeScenario != null) { throw new InvalidOperationException("Another scenario is currently active!"); }

            IMapAccess map = this.mapLoader.NewMap(mapName, this.tilesetManager.GetTileSet(tilesetName), defaultTerrain, mapSize);
            this.activeScenario = this.scenarioLoader.NewScenario(map);

            this.RegisterFactoryMethods();
        }

        /// <see cref="IScenarioManagerBC.OpenScenario"/>
        public void OpenScenario(string filename)
        {
            if (this.activeScenario != null) { throw new InvalidOperationException("Another scenario is currently active!"); }
            if (filename == null) { throw new ArgumentNullException("fileName"); }

            byte[] mapBytes = File.ReadAllBytes(filename);
            MapHeader mapHeader = this.mapLoader.LoadMapHeader(mapBytes);
            IMapAccess map = this.mapLoader.LoadMap(this.tilesetManager.GetTileSet(mapHeader.TilesetName), mapBytes);
            this.pathFinder.Initialize(this.navmeshLoader.LoadNavMesh(mapBytes), MAX_PATHFINDING_ITERATIONS_PER_FRAMES);
            this.activeScenario = this.scenarioLoader.LoadScenario(map, mapBytes);

            this.RegisterFactoryMethods();
        }

        /// <see cref="IScenarioManagerBC.SaveScenario"/>
        public void SaveScenario(string filename)
        {
            if (this.activeScenario == null) { throw new InvalidOperationException("There is no active scenario!"); }
            if (filename == null) { throw new ArgumentNullException("fileName"); }

            /// Try to create a navmesh from the map but do not crash.
            INavMesh navmesh = null;
            try { navmesh = this.navmeshLoader.NewNavMesh(new MapWalkabilityReader(this.activeScenario.Map)); }
            catch (Exception ex) { TraceManager.WriteExceptionAllTrace(ex, false); }

            /// Serialize the map, the scenario and the navmesh if it has been successfully created.
            byte[] mapBytes = this.mapLoader.SaveMap(this.activeScenario.Map);
            byte[] scenarioBytes = this.scenarioLoader.SaveScenario(this.activeScenario);
            byte[] navmeshBytes = navmesh != null ? this.navmeshLoader.SaveNavMesh(navmesh) : new byte[0];

            /// Write the serialized data into the output file.
            int outIdx = 0;
            byte[] outputBytes = new byte[mapBytes.Length + scenarioBytes.Length + navmeshBytes.Length];
            for (int i = 0; i < mapBytes.Length; i++, outIdx++) { outputBytes[outIdx] = mapBytes[i]; }
            for (int i = 0; i < scenarioBytes.Length; i++, outIdx++) { outputBytes[outIdx] = scenarioBytes[i]; }
            for (int i = 0; i < navmeshBytes.Length; i++, outIdx++) { outputBytes[outIdx] = navmeshBytes[i]; }
            File.WriteAllBytes(filename, outputBytes);
        }

        /// <see cref="IScenarioManagerBC.CloseScenario"/>
        public void CloseScenario()
        {
            if (this.activeScenario != null)
            {
                this.UnregisterFactoryMethods();

                this.activeScenario.Map.Close();
                this.activeScenario = null;
            }
        }

        /// <see cref="IScenarioManagerBC.ActiveScenario"/>
        public Scenario ActiveScenario { get { return this.activeScenario; } }

        /// <see cref="IScenarioManagerBC.Metadata"/>
        public IScenarioMetadata Metadata { get { return this.scenarioLoader.Metadata; } }

        #endregion IScenarioManagerBC methods

        #region View factory methods

        /// <summary>
        /// Creates a view of type IMapTerrainView.
        /// </summary>
        /// <returns>The created view.</returns>
        private IMapTerrainView CreateMapTerrainView()
        {
            if (this.activeScenario == null) { throw new InvalidOperationException("There is no active scenario!"); }
            return new MapTerrainView();
        }

        /// <summary>
        /// Creates a view of type ITileSetView.
        /// </summary>
        /// <returns>The created view.</returns>
        private ITileSetView CreateTileSetView()
        {
            if (this.activeScenario == null) { throw new InvalidOperationException("There is no active scenario!"); }
            return new TileSetView();
        }

        /// <summary>
        /// Creates a view of type IMetadataView.
        /// </summary>
        /// <returns>The created view.</returns>
        private IMetadataView CreateMetadataView()
        {
            return new MetadataView();
        }

        /// <summary>
        /// Creates a view of type IMapObjectView.
        /// </summary>
        /// <returns>The created view.</returns>
        private IMapObjectView CreateMapObjectView()
        {
            if (this.activeScenario == null) { throw new InvalidOperationException("There is no active scenario!"); }
            return new MapObjectView();
        }

        /// <summary>
        /// Creates a view of type IMapObjectDataView.
        /// </summary>
        /// <param name="objectID">The ID of the map object to read.</param>
        /// <returns>The created view.</returns>
        private IMapObjectDataView CreateMapObjectDataView(int objectID)
        {
            if (this.activeScenario == null) { throw new InvalidOperationException("There is no opened map!"); }
            if (objectID < 0) { throw new ArgumentOutOfRangeException("objectID"); }

            return new MapObjectDataView(objectID);
        }

        /// <summary>
        /// Creates a view of type ITerrainObjectPlacementView.
        /// </summary>
        /// <returns>The created view.</returns>
        private ITerrainObjectPlacementView CreateTerrainObjectPlacementView(string terrainObjectName)
        {
            if (this.activeScenario == null) { throw new InvalidOperationException("There is no opened map!"); }
            if (terrainObjectName == null) { throw new ArgumentNullException("terrainObjectName"); }

            return new TerrainObjectPlacementView(terrainObjectName);
        }

        /// <summary>
        /// Creates a view of type IMapObjectPlacementView.
        /// </summary>
        /// <returns>The created view.</returns>
        private IMapObjectPlacementView CreateMapObjectPlacementView(string mapObjectTypeName)
        {
            if (this.activeScenario == null) { throw new InvalidOperationException("There is no opened map!"); }
            if (mapObjectTypeName == null) { throw new ArgumentNullException("mapObjectTypeName"); }

            return new MapObjectPlacementView(mapObjectTypeName);
        }

        /// <summary>
        /// Creates a view of type ISelectionIndicatorView.
        /// </summary>
        /// <returns>The created view.</returns>
        private ISelectionIndicatorView CreateSelIndicatorView()
        {
            return new SelectionIndicatorView();
        }

        /// <summary>
        /// Creates a view of type ICommandView.
        /// </summary>
        /// <returns>The created view.</returns>
        private ICommandView CreateCommandView()
        {
            return new CommandView();
        }

        /// <summary>
        /// Registers the implemented factory methods to the view factory.
        /// </summary>
        private void RegisterFactoryMethods()
        {
            this.viewFactoryRegistry.RegisterViewFactory(this.CreateMapTerrainView);
            this.viewFactoryRegistry.RegisterViewFactory(this.CreateTileSetView);
            this.viewFactoryRegistry.RegisterViewFactory(this.CreateMetadataView);
            this.viewFactoryRegistry.RegisterViewFactory(this.CreateMapObjectView);
            this.viewFactoryRegistry.RegisterViewFactory(this.CreateSelIndicatorView);
            this.viewFactoryRegistry.RegisterViewFactory(this.CreateCommandView);
            this.viewFactoryRegistry.RegisterViewFactory<IMapObjectDataView, int>(this.CreateMapObjectDataView);
            this.viewFactoryRegistry.RegisterViewFactory<ITerrainObjectPlacementView, string>(this.CreateTerrainObjectPlacementView);
            this.viewFactoryRegistry.RegisterViewFactory<IMapObjectPlacementView, string>(this.CreateMapObjectPlacementView);
        }

        /// <summary>
        /// Unregisters the implemented factory methods from the view factory.
        /// </summary>
        private void UnregisterFactoryMethods()
        {
            this.viewFactoryRegistry.UnregisterViewFactory<IMapTerrainView>();
            this.viewFactoryRegistry.UnregisterViewFactory<ITileSetView>();
            this.viewFactoryRegistry.UnregisterViewFactory<IMetadataView>();
            this.viewFactoryRegistry.UnregisterViewFactory<IMapObjectView>();
            this.viewFactoryRegistry.UnregisterViewFactory<ISelectionIndicatorView>();
            this.viewFactoryRegistry.UnregisterViewFactory<ICommandView>();
            this.viewFactoryRegistry.UnregisterViewFactory<IMapObjectDataView>();
            this.viewFactoryRegistry.UnregisterViewFactory<ITerrainObjectPlacementView>();
            this.viewFactoryRegistry.UnregisterViewFactory<IMapObjectPlacementView>();
        }

        #endregion View factory methods

        /// <summary>
        /// Reference to the currently active scenario.
        /// </summary>
        private Scenario activeScenario;

        /// <summary>
        /// Reference to the RC.Engine.Maps.MapLoader component.
        /// </summary>
        private IMapLoader mapLoader;

        /// <summary>
        /// Reference to the RC.Engine.Scenarios.ScenarioLoader component.
        /// </summary>
        private IScenarioLoader scenarioLoader;

        /// <summary>
        /// Reference to the RC.App.BizLogic.TilesetManagerBC business component.
        /// </summary>
        private ITilesetManagerBC tilesetManager;

        /// <summary>
        /// Reference to the RC.Engine.Maps.NavMeshLoader component.
        /// </summary>
        private INavMeshLoader navmeshLoader;

        /// <summary>
        /// Reference to the RC.Engine.Simulator.PathFinder component.
        /// </summary>
        private IPathFinder pathFinder;

        /// <summary>
        /// Reference to the registry interface of the RC.App.BizLogic.ViewFactory component.
        /// </summary>
        private IViewFactoryRegistry viewFactoryRegistry;

        /// <summary>
        /// The maximum number of pathfinding iterations per frames.
        /// </summary>
        private const int MAX_PATHFINDING_ITERATIONS_PER_FRAMES = 2500;
    }
}