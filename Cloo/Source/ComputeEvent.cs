﻿/*

Copyright (c) 2009 Fatjon Sakiqi

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

*/

using System;
using System.Collections.Generic;
using OpenTK.Compute.CL10;

namespace Cloo
{
    public class ComputeEvent: ComputeResource
    {
        #region Fields

        private readonly ComputeCommandQueue commandQueue;
        private readonly CommandType commandType;

        #endregion

        #region Properties

        /// <summary>
        /// Return the ComputeCommandQueue associated with event.
        /// </summary>
        public ComputeCommandQueue CommandQueue
        {
            get
            {
                return commandQueue;
            }
        }

        /// <summary>
        /// Return the command associated with event.
        /// </summary>
        public CommandType CommandType
        {
            get
            {
                return commandType; 
            }
        }

        /// <summary>
        /// Return the execution status of the command identified by event.
        /// </summary>
        public int ExecutionStatus
        {
            get
            {
                return GetInfo<EventInfo, int>(
                    EventInfo.EventCommandExecutionStatus, CL.GetEventInfo );
            }
        }

        /// <summary>
        /// A 64-bit value that describes the current device time counter in nanoseconds when the command identified by event has finished execution on the device.
        /// </summary>
        public long CommandFinishedTimestamp
        {
            get
            {
                return ( long )GetInfo<ProfilingInfo, ulong>(
                    ProfilingInfo.ProfilingCommandEnd, CL.GetEventProfilingInfo );
            }
        }

        /// <summary>
        /// A 64-bit value that describes the current device time counter in nanoseconds when the command identified by event is enqueued in a command-queue by the host.
        /// </summary>
        public long CommandQueuedTimestamp
        {
            get
            {
                return ( long )GetInfo<ProfilingInfo, ulong>(
                    ProfilingInfo.ProfilingCommandQueued, CL.GetEventProfilingInfo );
            }
        }

        /// <summary>
        /// A 64-bit value that describes the current device time counter in nanoseconds when the command identified by event starts execution on the device.
        /// </summary>
        public long CommandStartedTimestamp
        {
            get
            {
                return ( long )GetInfo<ProfilingInfo, ulong>(
                    ProfilingInfo.ProfilingCommandStart, CL.GetEventProfilingInfo );
            }
        }

        /// <summary>
        /// A 64-bit value that describes the current device time counter in nanoseconds when the command identified by event that has been enqueued is submitted by the host to the device associated with the command-queue.
        /// </summary>
        public long CommandSubmittedTimestamp
        {
            get
            {
                return ( long )GetInfo<ProfilingInfo, ulong>(
                    ProfilingInfo.ProfilingCommandSubmit, CL.GetEventProfilingInfo );
            }
        }

        #endregion

        #region Constructors

        internal ComputeEvent( IntPtr handle, ComputeCommandQueue queue )
        {
            Handle = handle;
            commandQueue = queue;
            commandType = ( CommandType )GetInfo<EventInfo, uint>( 
                EventInfo.EventCommandType, CL.GetEventInfo );
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the string representation of this event.
        /// </summary>
        public override string ToString()
        {
            return "ComputeEvent" + base.ToString();
        }

        /// <summary>
        /// Waits on the host thread for commands identified by event objects in the list to complete.
        /// </summary>
        /// <param name="events">The list of events to wait for.</param>
        public static void WaitFor( ICollection<ComputeEvent> events )
        {
            IntPtr[] eventHandles = ExtractHandles( events );
            
            int error = CL.WaitForEvents( eventHandles.Length, eventHandles );
            ComputeException.ThrowIfError( error );
        }

        #endregion

        #region Protected methods

        protected override void Dispose( bool manual )
        {
            if( Handle != IntPtr.Zero )
            {
                CL.ReleaseEvent( Handle );
                Handle = IntPtr.Zero;
            }
        }

        #endregion
    }
}