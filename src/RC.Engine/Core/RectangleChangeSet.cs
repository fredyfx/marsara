﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RC.Common;
using RC.Engine.PublicInterfaces;

namespace RC.Engine.Core
{
    /// <summary>
    /// This changeset overwrites a specific field in a given rectangle of the target.
    /// </summary>
    class RectangleChangeSet : CellDataChangeSetBase
    {
        /// <summary>
        /// Constructs a changeset for overwriting an integer field.
        /// </summary>
        /// <param name="targetRect">The rectangle of the target to perform the changeset.</param>
        /// <param name="targetField">The name of the target field.</param>
        /// <param name="value">The new value of the target field.</param>
        /// <param name="tileset">The tileset of this changeset.</param>
        public RectangleChangeSet(RCIntRectangle targetRect, string targetField, int value, TileSet tileset)
            : base(targetField, value, tileset)
        {
            this.CheckAndAssignCtorParams(targetRect);
        }

        /// <summary>
        /// Constructs a changeset for overwriting a bool field.
        /// </summary>
        /// <param name="targetRect">The rectangle of the target to perform the changeset.</param>
        /// <param name="targetField">The name of the target field.</param>
        /// <param name="value">The new value of the target field.</param>
        /// <param name="tileset">The tileset of this changeset.</param>
        public RectangleChangeSet(RCIntRectangle targetRect, string targetField, bool value, TileSet tileset)
            : base(targetField, value, tileset)
        {
            this.CheckAndAssignCtorParams(targetRect);
        }

        /// <summary>
        /// Gets the target rectangle of this changeset.
        /// </summary>
        public RCIntRectangle TargetRect { get { return this.targetRect; } }

        /// <see cref="CellDataChangeSetBase.CollectTargetSet"/>
        protected override HashSet<RCIntVector> CollectTargetSet(ICellDataChangeSetTarget target)
        {
            HashSet<RCIntVector> targetset = new HashSet<RCIntVector>();
            for (int x = this.targetRect.X; x < this.targetRect.Right; x++)
            {
                for (int y = this.targetRect.Y; y < this.targetRect.Bottom; y++)
                {
                    RCIntVector index = new RCIntVector(x, y);
                    if (target.GetCell(index) != null)
                    {
                        targetset.Add(index);
                    }
                }
            }
            return targetset;
        }

        /// <summary>
        /// Checks and assigns the parameters coming from the constructor.
        /// </summary>
        /// <param name="targetRect">The target rectangle of this changeset.</param>
        private void CheckAndAssignCtorParams(RCIntRectangle targetRect)
        {
            if (targetRect == RCIntRectangle.Undefined) { throw new ArgumentNullException("targetRect"); }

            /// TODO: check the parameter.
            this.targetRect = targetRect;
        }

        /// <summary>
        /// The target rectangle of this changeset.
        /// </summary>
        private RCIntRectangle targetRect;
    }
}
