﻿using RC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RC.Engine.Simulator.Commands;
using RC.Engine.Simulator.ComponentInterfaces;
using RC.Engine.Simulator.Engine;
using RC.Engine.Simulator.Metadata;
using RC.Engine.Simulator.Terran.Units;

namespace RC.Engine.Simulator.Terran.Commands
{
    /// <summary>
    /// Enumerates the special Terran SCV commands.
    /// </summary>
    public enum SCVCommandEnum
    {
        Undefined = -1,
        [EnumMapping("Repair")]
        Repair = 0,
        [EnumMapping("Gather")]
        Gather = 1,
        [EnumMapping("Return")]
        Return = 2,
        [EnumMapping("Build")]
        Build = 3,
        [EnumMapping("StopBuild")]
        StopBuild = 4,
        [EnumMapping("Move")]
        Move = 5,
        [EnumMapping("Stop")]
        Stop = 6,
        [EnumMapping("Attack")]
        Attack = 7,
        [EnumMapping("Patrol")]
        Patrol = 8,
        [EnumMapping("Hold")]
        Hold = 9
    }

    /// <summary>
    /// Factory for special Terran SCV command executions.
    /// </summary>
    class SCVCmdExecutionFactory : CommandExecutionFactoryBase<SCV>
    {
        /// <summary>
        /// Constructs a SCVCmdExecutionFactory instance.
        /// </summary>
        /// <param name="command">The type of the special Terran SCV command.</param>
        public SCVCmdExecutionFactory(SCVCommandEnum command)
            : base(command != SCVCommandEnum.Undefined ? EnumMap<SCVCommandEnum, string>.Map(command) : null, SCV.SCV_TYPE_NAME)
        {
            this.commandType = command;
        }

        /// <see cref="CommandExecutionFactoryBase.GetCommandAvailability"/>
        protected override AvailabilityEnum GetCommandAvailability(RCSet<SCV> scvsToHandle, RCSet<Entity> fullEntitySet, string parameter)
        {
            switch (this.commandType)
            {
                case SCVCommandEnum.Repair:
                    return this.CheckRepairAvailability(scvsToHandle, fullEntitySet);
                case SCVCommandEnum.Gather:
                    return this.CheckGatherAvailability(scvsToHandle, fullEntitySet);
                case SCVCommandEnum.Return:
                    return this.CheckReturnAvailability(scvsToHandle, fullEntitySet);
                case SCVCommandEnum.Build:
                    return this.CheckBuildAvailability(scvsToHandle, fullEntitySet, parameter);
                case SCVCommandEnum.StopBuild:
                    return this.CheckStopBuildAvailability(scvsToHandle, fullEntitySet);
                case SCVCommandEnum.Move:
                case SCVCommandEnum.Stop:
                case SCVCommandEnum.Attack:
                case SCVCommandEnum.Patrol:
                case SCVCommandEnum.Hold:
                    return this.CheckBasicCommandAvailability(scvsToHandle);
                default:
                    return AvailabilityEnum.Unavailable;
            }
        }

        /// <see cref="CommandExecutionFactoryBase.CreateCommandExecutions"/>
        protected override IEnumerable<CmdExecutionBase> CreateCommandExecutions(RCSet<SCV> scvsToHandle, RCSet<Entity> fullEntitySet, RCNumVector targetPosition, int targetEntityID, string parameter)
        {
            switch (this.commandType)
            {
                case SCVCommandEnum.Repair:
                    return this.CreateRepairExecutions(scvsToHandle, targetPosition, targetEntityID);
                case SCVCommandEnum.Gather:
                    return this.CreateGatherExecutions(scvsToHandle, targetPosition, targetEntityID);
                case SCVCommandEnum.Return:
                    return this.CreateReturnExecutions(scvsToHandle);
                case SCVCommandEnum.Build:
                    return this.CreateBuildExecutions(scvsToHandle, (RCIntVector)targetPosition, parameter);
                case SCVCommandEnum.StopBuild:
                    return this.CreateStopBuildExecutions(scvsToHandle);
                case SCVCommandEnum.Move:
                    return this.CreateMoveExecutions(scvsToHandle, fullEntitySet, targetPosition, targetEntityID);
                case SCVCommandEnum.Stop:
                    return this.CreateStopExecutions(scvsToHandle);
                case SCVCommandEnum.Attack:
                    return this.CreateAttackExecutions(scvsToHandle, fullEntitySet, targetPosition, targetEntityID);
                case SCVCommandEnum.Patrol:
                    return this.CreatePatrolExecutions(scvsToHandle, fullEntitySet, targetPosition);
                case SCVCommandEnum.Hold:
                    return this.CreateHoldExecutions(scvsToHandle);
                default:
                    /// TODO: implement more intelligent handling of undefined commands!
                    return this.CreateMoveExecutions(scvsToHandle, fullEntitySet, targetPosition, targetEntityID);
            }
        }

        #region Availability checker methods

        /// <summary>
        /// Checks the availability of the Repair command for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to check.</param>
        /// <param name="fullEntitySet">The set of selected entities.</param>
        /// <returns>The availability of the Repair command for the given SCVs.</returns>
        private AvailabilityEnum CheckRepairAvailability(RCSet<SCV> scvsToHandle, RCSet<Entity> fullEntitySet)
        {
            if (scvsToHandle.Count != fullEntitySet.Count) { return AvailabilityEnum.Unavailable; }
            return scvsToHandle.Any(scv => !scv.IsConstructing) ? AvailabilityEnum.Enabled : AvailabilityEnum.Unavailable;
        }

        /// <summary>
        /// Checks the availability of the Gather command for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to check.</param>
        /// <param name="fullEntitySet">The set of selected entities.</param>
        /// <returns>The availability of the Gather command for the given SCVs.</returns>
        private AvailabilityEnum CheckGatherAvailability(RCSet<SCV> scvsToHandle, RCSet<Entity> fullEntitySet)
        {
            if (scvsToHandle.Count != fullEntitySet.Count) { return AvailabilityEnum.Unavailable; }

            // TODO: implement this method!
            return AvailabilityEnum.Enabled;
        }

        /// <summary>
        /// Checks the availability of the Return command for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to check.</param>
        /// <param name="fullEntitySet">The set of selected entities.</param>
        /// <returns>The availability of the Return command for the given SCVs.</returns>
        private AvailabilityEnum CheckReturnAvailability(RCSet<SCV> scvsToHandle, RCSet<Entity> fullEntitySet)
        {
            if (scvsToHandle.Count != fullEntitySet.Count) { return AvailabilityEnum.Unavailable; }

            // TODO: implement this method!
            return AvailabilityEnum.Enabled;
        }

        /// <summary>
        /// Checks the availability of the Build command for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to check.</param>
        /// <param name="fullEntitySet">The set of selected entities.</param>
        /// <param name="buildingTypeName">The name of the type of the building or null if no building is specified.</param>
        /// <returns>The availability of the Build command for the given SCVs.</returns>
        private AvailabilityEnum CheckBuildAvailability(RCSet<SCV> scvsToHandle, RCSet<Entity> fullEntitySet, string buildingTypeName)
        {
            if (scvsToHandle.Count != fullEntitySet.Count || scvsToHandle.Count != 1) { return AvailabilityEnum.Unavailable; }

            /// If the selected SCV is currently constructing -> unavailable.
            SCV scv = scvsToHandle.First();
            if (scv.IsConstructing) { return AvailabilityEnum.Unavailable; }

            /// If building type is not specified -> it is the parent build button availability check -> enable.
            if (buildingTypeName == null) { return AvailabilityEnum.Enabled; }

            /// Check the requirements of the building.
            IBuildingType buildingType = scv.Owner.Metadata.GetBuildingType(buildingTypeName);
            foreach (IRequirement requirement in buildingType.Requirements)
            {
                if (!scv.Owner.HasBuilding(requirement.RequiredBuildingType.Name)) { return AvailabilityEnum.Disabled; }
                if (requirement.RequiredAddonType != null && !scv.Owner.HasAddon(requirement.RequiredAddonType.Name)) { return AvailabilityEnum.Disabled; }
            }
            return AvailabilityEnum.Enabled;
        }

        /// <summary>
        /// Checks the availability of the StopBuild command for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to check.</param>
        /// <param name="fullEntitySet">The set of selected entities.</param>
        /// <returns>The availability of the StopBuild command for the given SCVs.</returns>
        private AvailabilityEnum CheckStopBuildAvailability(RCSet<SCV> scvsToHandle, RCSet<Entity> fullEntitySet)
        {
            if (scvsToHandle.Count != fullEntitySet.Count) { return AvailabilityEnum.Unavailable; }
            return scvsToHandle.All(scv => scv.IsConstructing) ? AvailabilityEnum.Enabled : AvailabilityEnum.Unavailable;
        }

        /// <summary>
        /// Checks the availability of the basic commands for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to check.</param>
        /// <returns>The availability of the basic commands for the given SCVs.</returns>
        private AvailabilityEnum CheckBasicCommandAvailability(RCSet<SCV> scvsToHandle)
        {
            return scvsToHandle.Any(scv => !scv.IsConstructing) ? AvailabilityEnum.Enabled : AvailabilityEnum.Unavailable;
        }

        #endregion Availability checker methods

        #region Execution creator methods

        /// <summary>
        /// Creates return executions for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to return.</param>
        private IEnumerable<CmdExecutionBase> CreateReturnExecutions(RCSet<SCV> scvsToHandle)
        {
            // TODO: implement!
            yield break;
        }

        /// <summary>
        /// Creates repair executions for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The reapiring SCVs.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="targetEntityID">The ID of the target entity or -1 if undefined.</param>
        private IEnumerable<CmdExecutionBase> CreateRepairExecutions(RCSet<SCV> scvsToHandle, RCNumVector targetPosition, int targetEntityID)
        {
            // TODO: implement!
            yield break;
        }

        /// <summary>
        /// Creates gather executions for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to gather.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="targetEntityID">The ID of the target entity or -1 if undefined.</param>
        private IEnumerable<CmdExecutionBase> CreateGatherExecutions(RCSet<SCV> scvsToHandle, RCNumVector targetPosition, int targetEntityID)
        {
            // TODO: implement!
            yield break;
        }

        /// <summary>
        /// Creates build executions for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to build.</param>
        /// <param name="topLeftQuadTile">The coordinates of the top-left quadratic tile of the building.</param>
        /// <param name="buildingType">The type of the building.</param>
        private IEnumerable<CmdExecutionBase> CreateBuildExecutions(RCSet<SCV> scvsToHandle, RCIntVector topLeftQuadTile, string buildingType)
        {
            foreach (SCV scv in scvsToHandle)
            {
                if (!scv.IsConstructing)
                {
                    yield return new SCVStartBuildCmdExecution(scv, buildingType, topLeftQuadTile);
                }
            }
        }

        /// <summary>
        /// Creates stop-build executions for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to stop building.</param>
        private IEnumerable<CmdExecutionBase> CreateStopBuildExecutions(RCSet<SCV> scvsToHandle)
        {
            // TODO: implement!
            yield break;
        }

        /// <summary>
        /// Creates move executions for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to move.</param>
        /// <param name="fullEntitySet">The set of selected entities.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="targetEntityID">The ID of the target entity or -1 if undefined.</param>
        private IEnumerable<CmdExecutionBase> CreateMoveExecutions(RCSet<SCV> scvsToHandle, RCSet<Entity> fullEntitySet, RCNumVector targetPosition, int targetEntityID)
        {
            MagicBox magicBox = new MagicBox(fullEntitySet, targetPosition);
            foreach (SCV scv in scvsToHandle.Where(scv => !scv.IsConstructing))
            {
                yield return new MoveExecution(scv, magicBox.GetTargetPosition(scv), targetEntityID);
            }
        }

        /// <summary>
        /// Creates stop executions for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to stop.</param>
        private IEnumerable<CmdExecutionBase> CreateStopExecutions(RCSet<SCV> scvsToHandle)
        {
            foreach (SCV scv in scvsToHandle)
            {
                if (scv.IsConstructing)
                {
                    // TODO: yield return new StopBuildExecution(scv);
                }
                else
                {
                    yield return new StopExecution(scv);
                }
            }
        }

        /// <summary>
        /// Creates hold executions for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to hold.</param>
        private IEnumerable<CmdExecutionBase> CreateHoldExecutions(RCSet<SCV> scvsToHandle)
        {
            foreach (SCV scv in scvsToHandle.Where(scv => !scv.IsConstructing))
            {
                yield return new HoldExecution(scv);
            }
        }

        /// <summary>
        /// Creates attack executions for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The attacking SCVs.</param>
        /// <param name="fullEntitySet">The set of selected entities.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="targetEntityID">The ID of the target entity or -1 if undefined.</param>
        private IEnumerable<CmdExecutionBase> CreateAttackExecutions(RCSet<SCV> scvsToHandle, RCSet<Entity> fullEntitySet, RCNumVector targetPosition, int targetEntityID)
        {
            MagicBox magicBox = new MagicBox(fullEntitySet, targetPosition);
            foreach (SCV scv in scvsToHandle.Where(scv => !scv.IsConstructing))
            {
                yield return new AttackExecution(scv, magicBox.GetTargetPosition(scv), targetEntityID);
            }
        }

        /// <summary>
        /// Creates patrol executions for the given SCVs.
        /// </summary>
        /// <param name="scvsToHandle">The SCVs to patrol.</param>
        /// <param name="fullEntitySet">The set of selected entities.</param>
        /// <param name="targetPosition">The target position.</param>
        private IEnumerable<CmdExecutionBase> CreatePatrolExecutions(RCSet<SCV> scvsToHandle, RCSet<Entity> fullEntitySet, RCNumVector targetPosition)
        {
            MagicBox magicBox = new MagicBox(fullEntitySet, targetPosition);
            foreach (SCV scv in scvsToHandle.Where(scv => !scv.IsConstructing))
            {
                yield return new PatrolExecution(scv, magicBox.GetTargetPosition(scv));
            }
        }

        #endregion Execution creator methods

        /// <summary>
        /// The type of the command execution that this factory creates.
        /// </summary>
        private readonly SCVCommandEnum commandType;
    }
}