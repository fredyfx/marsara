﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RC.Common;
using RC.Engine.Maps.PublicInterfaces;

namespace RC.Engine.Maps.Core
{
    /// <summary>
    /// This changeset overwrites a specific field in a given column of the target.
    /// </summary>
    class ColumnChangeSet : CellDataChangeSetBase
    {
        /// <summary>
        /// Constructs a changeset for overwriting an integer field.
        /// </summary>
        /// <param name="targetCol">The column of the target to perform the changeset.</param>
        /// <param name="modifier">Reference to the modifier.</param>
        /// <param name="tileset">The tileset of this changeset.</param>
        public ColumnChangeSet(int targetCol, ICellDataModifier modifier, TileSet tileset)
            : base(modifier, tileset)
        {
            this.CheckAndAssignCtorParams(targetCol);
        }

        /// <see cref="CellDataChangeSetBase.CollectTargetSet"/>
        protected override RCSet<RCIntVector> CollectTargetSet(ICellDataChangeSetTarget target)
        {
            RCSet<RCIntVector> targetset = new RCSet<RCIntVector>();
            for (int y = 0; y < target.CellSize.Y; y++)
            {
                RCIntVector index = new RCIntVector(this.targetCol, y);
                if (target.GetCell(index) != null)
                {
                    targetset.Add(index);
                }
            }
            return targetset;
        }

        /// <summary>
        /// Checks and assigns the parameters coming from the constructor.
        /// </summary>
        /// <param name="targetCol">The target column of this changeset.</param>
        private void CheckAndAssignCtorParams(int targetCol)
        {
            /// TODO: check the parameter.
            this.targetCol = targetCol;
        }

        /// <summary>
        /// The target column of this changeset.
        /// </summary>
        private int targetCol;
    }
}
