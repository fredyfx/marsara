﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RC.Common;
using RC.Engine.Simulator.ComponentInterfaces;
using RC.Engine.Simulator.Engine;

namespace RC.Engine.Simulator.Commands
{
    /// <summary>
    /// Factory for lift-off command executions.
    /// </summary>
    public class LiftOffExecutionFactory : CommandExecutionFactoryBase<Entity>
    {
        /// <summary>
        /// Constructs a LiftOffExecutionFactory instance.
        /// </summary>
        /// <param name="entityType">The type of the recipient entities.</param>
        public LiftOffExecutionFactory(string entityType)
            : base(COMMAND_TYPE, entityType)
        {
        }

        /// <see cref="CommandExecutionFactoryBase.GetCommandAvailability"/>
        protected override AvailabilityEnum GetCommandAvailability(RCSet<Entity> entitiesToHandle, RCSet<Entity> fullEntitySet, string parameter)
        {
            if (entitiesToHandle.Count != 1) { return AvailabilityEnum.Unavailable; }

            // TODO: this is only a temporary implementation for testing!
            Entity recipientEntity = entitiesToHandle.First();
            return !recipientEntity.MotionControl.IsFlying && recipientEntity.ActiveProductionLine == null ? AvailabilityEnum.Enabled : AvailabilityEnum.Unavailable;
        }

        /// <see cref="CommandExecutionFactoryBase.CreateCommandExecutions"/>
        protected override IEnumerable<CmdExecutionBase> CreateCommandExecutions(RCSet<Entity> entitiesToHandle, RCSet<Entity> fullEntitySet, RCNumVector targetPosition, int targetEntityID, string parameter)
        {
            /// Create the command executions.
            foreach (Entity entity in entitiesToHandle)
            {
                yield return new LiftOffExecution(entity);
            }
        }

        /// <summary>
        /// The type of the command handled by this factory.
        /// </summary>
        private const string COMMAND_TYPE = "LiftOff";
    }
}