﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RC.Common;

namespace RC.App.BizLogic.Core
{
    /// <summary>
    /// The command dispatcher is responsible for synchronizing the command sending and receiving
    /// between the UI- and the DSS-threads. Be aware that each methods are called from the appropriate thread.
    /// WARNING!!! The command dispatcher is not thread safe. You have to make sure that the following
    /// methods are not called concurrently from the UI- and the DSS-thread:
    ///     - DispatchOutgoingCommands & GetOutgoingCommands
    ///     - PushIncomingCommand & GetIncomingCommands
    /// </summary>
    class CommandDispatcher
    {
        /// <summary>
        /// Constructs a CommandDispatcher instance.
        /// </summary>
        public CommandDispatcher()
        {
            this.commandBuffer = new List<RCPackage>();
            this.outgoingCommands = new List<RCPackage>();
            this.incomingCommands = new List<Tuple<RCPackage, int>>();
        }

        #region Methods for the UI-thread

        /// <summary>
        /// Pushes a command into the command buffer.
        /// </summary>
        /// <param name="commandPackage">The command to push.</param>
        public void PushOutgoingCommand(RCPackage commandPackage)
        {
            if (commandPackage == null) { throw new ArgumentNullException("commandPackage"); }
            this.commandBuffer.Add(commandPackage);
        }

        /// <summary>
        /// Dispatches the commands from the command buffer. Calling this method automatically
        /// clears the command buffer.
        /// </summary>
        public void DispatchOutgoingCommands()
        {
            foreach (RCPackage commandPackage in this.commandBuffer)
            {
                this.outgoingCommands.Add(commandPackage);
            }
            this.commandBuffer.Clear();
        }

        /// <summary>
        /// Gets the list of the incoming commands and their senders (0 - host). Calling this method
        /// automatically clears the incoming command queue.
        /// </summary>
        /// <returns>A list of the incoming commands.</returns>
        public List<Tuple<RCPackage, int>> GetIncomingCommands()
        {
            List<Tuple<RCPackage, int>> retList = new List<Tuple<RCPackage, int>>(this.incomingCommands);
            this.incomingCommands.Clear();
            return retList;
        }

        #endregion Methods for the UI-thread

        #region Methods for the DSS-thread

        /// <summary>
        /// Pushes a command into the incoming command queue.
        /// </summary>
        /// <param name="commandPackage">The command to push.</param>
        /// <param name="senderIndex">The index of the sender (0 - host).</param>
        public void PushIncomingCommand(RCPackage commandPackage, int senderIndex)
        {
            if (senderIndex < 0) { throw new ArgumentOutOfRangeException("senderIndex"); }
            if (commandPackage == null) { throw new ArgumentNullException("commandPackage"); }
            this.incomingCommands.Add(new Tuple<RCPackage, int>(commandPackage, senderIndex));
        }

        /// <summary>
        /// Gets the list of the outgoing commands. Calling this method automatically clears
        /// the outgoing command queue.
        /// </summary>
        /// <returns>A list of the outgoing commands.</returns>
        public List<RCPackage> GetOutgoingCommands()
        {
            List<RCPackage> retList = new List<RCPackage>(this.outgoingCommands);
            this.outgoingCommands.Clear();
            return retList;
        }

        #endregion Methods for the DSS-thread

        /// <summary>
        /// The UI-thread is collecting the outgoing command packages from the local player into this buffer.
        /// </summary>
        private List<RCPackage> commandBuffer;

        /// <summary>
        /// The list of the commands that can be shared by the DSS-thread on the network.
        /// </summary>
        private List<RCPackage> outgoingCommands;

        /// <summary>
        /// The list of the incoming commands and their senders (0 - host) that can be executed by the UI-thread.
        /// </summary>
        private List<Tuple<RCPackage, int>> incomingCommands;
    }
}
