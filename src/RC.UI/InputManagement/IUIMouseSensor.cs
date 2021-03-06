﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RC.Common;

namespace RC.UI
{
    /// <summary>
    /// Prototype of general input device event handlers.
    /// </summary>
    public delegate void UIInputEventHdl(UISensitiveObject sender);

    /// <summary>
    /// Prototype of mouse event handlers.
    /// </summary>
    public delegate void UIMouseEventHdl(UISensitiveObject sender, UIMouseEventArgs evtArgs);

    /// <summary>
    /// Defines the interface of mouse sensors.
    /// </summary>
    public interface IUIMouseSensor
    {
        /// <summary>
        /// Occurs when the mouse pointer enters to the area of the corresponding UISensitiveObject.
        /// </summary>
        event UIInputEventHdl Enter;

        /// <summary>
        /// Occurs when the mouse pointer leaves to the area of the corresponding UISensitiveObject.
        /// </summary>
        event UIInputEventHdl Leave;

        /// <summary>
        /// Occurs when the mouse pointer moves over the area of the corresponding UISensitiveObject.
        /// </summary>
        event UIMouseEventHdl Move;

        /// <summary>
        /// Occurs when a mouse button is pushed down while the pointer is over the area of the corresponding UISensitiveObject
        /// or while this sensor is active.
        /// </summary>
        event UIMouseEventHdl ButtonDown;

        /// <summary>
        /// Occurs when a mouse button is released while the pointer is over the area of the corresponding UISensitiveObject
        /// or while this sensor is active.
        /// </summary>
        event UIMouseEventHdl ButtonUp;

        /// <summary>
        /// Occurs when a mouse button is clicked while the pointer is over the area of the corresponding UISensitiveObject.
        /// </summary>
        event UIMouseEventHdl Click;

        /// <summary>
        /// Occurs when a mouse button is double-clicked while the pointer is over the area of the corresponding UISensitiveObject.
        /// </summary>
        event UIMouseEventHdl DoubleClick;

        /// <summary>
        /// Occurs when the mouse wheel changed it's position while the pointer is over the area of the corresponding UISensitiveObject
        /// or while this sensor is active.
        /// </summary>
        event UIMouseEventHdl Wheel;

        /// <summary>
        /// Occurs when the state of this mouse sensor has been reset.
        /// </summary>
        event EventHandler StateReset;

        /// <summary>
        /// Attaches this mouse sensor to the given target sensor so that the events arrived to the target sensor will be propagated to
        /// this mouse sensor.
        /// </summary>
        /// <param name="target">The target sensor to attach to.</param>
        void AttachTo(IUIMouseSensor target);

        /// <summary>
        /// Detaches this mouse sensor from the given target sensor so that the events arrived to the target sensor will no longer be
        /// propagated to this mouse sensor.
        /// </summary>
        /// <param name="target">The target sensor to detach from.</param>
        void DetachFrom(IUIMouseSensor target);
    }

    /// <summary>
    /// The implementation of the IUIMouseSensor interface. This class is instantiated by the UIMouseManager.
    /// </summary>
    /// TODO: implement the raising of Click and DoubleClick events
    class UIMouseSensor : IUIMouseSensor
    {
        /// <summary>
        /// Constructs a UIMouseSensor object.
        /// </summary>
        public UIMouseSensor(UISensitiveObject targetObj)
        {
            this.targetObject = targetObj;
            this.Reset();
        }

        /// <summary>
        /// Resets the state of this mouse sensor.
        /// </summary>
        public void Reset()
        {
            this.PropagateReset(this, new EventArgs());
        }

        #region Trigger methods

        /// <summary>
        /// Called by UIMouseManager.
        /// </summary>
        public UIPointer GetMousePointer(RCIntVector absPosition)
        {
            return this.targetObject.GetMousePointer(this.targetObject.TransformAbsToLocal(absPosition));
        }

        /// <summary>
        /// Called by UIMouseManager.
        /// </summary>
        public void OnButtonDown(RCIntVector absPosition, UIMouseButton whichButton)
        {
            bool raiseDoubleClickEvt = false;
            if (this.activatorButton == UIMouseButton.Undefined)
            {
                this.activatorButton = whichButton;
                this.activeOver = true;

                if (this.doubleClickButton == UIMouseButton.Undefined)
                {
                    /// Start double-click timer.
                    this.doubleClickButton = whichButton;
                    this.doubleClickStartPos = absPosition;
                    this.doubleClickTimer = 0;
                    UIRoot.Instance.GraphicsPlatform.RenderLoop.FrameUpdate += this.OnFrameUpdate;
                }
                else if (this.doubleClickButton == whichButton && this.doubleClickStartPos == absPosition)
                {
                    /// Double-click happened.
                    raiseDoubleClickEvt = true;
                    this.StopDoubleClickTimer();
                }
            }

            if (this.ButtonDown != null)
            {
                UIMouseEventArgs evtArgs = new UIMouseEventArgs(this.targetObject.TransformAbsToLocal(absPosition),
                                                                whichButton);
                this.ButtonDown(this.targetObject, evtArgs);
            }

            if (raiseDoubleClickEvt && this.DoubleClick != null)
            {
                UIMouseEventArgs evtArgs = new UIMouseEventArgs(this.targetObject.TransformAbsToLocal(absPosition),
                                                                whichButton);
                this.DoubleClick(this.targetObject, evtArgs);
            }
        }

        /// <summary>
        /// Called by UIMouseManager.
        /// </summary>
        public void OnButtonUp(RCIntVector absPosition, UIMouseButton whichButton)
        {
            bool wasActiveOver = this.activeOver;
            if (this.activatorButton == whichButton)
            {
                this.activatorButton = UIMouseButton.Undefined;
                this.activeOver = false;
            }

            if (this.ButtonUp != null)
            {
                UIMouseEventArgs evtArgs = new UIMouseEventArgs(this.targetObject.TransformAbsToLocal(absPosition),
                                                                whichButton);
                this.ButtonUp(this.targetObject, evtArgs);
            }

            if (this.Click != null && wasActiveOver)
            {
                UIMouseEventArgs evtArgs = new UIMouseEventArgs(this.targetObject.TransformAbsToLocal(absPosition),
                                                                whichButton);
                this.Click(this.targetObject, evtArgs);
            }
        }

        /// <summary>
        /// Called by UIMouseManager.
        /// </summary>
        public void OnEnter()
        {
            if (this.activatorButton != UIMouseButton.Undefined)
            {
                this.activeOver = true;
            }

            this.StopDoubleClickTimer();
            if (this.Enter != null) { this.Enter(this.targetObject); }
        }

        /// <summary>
        /// Called by UIMouseManager.
        /// </summary>
        public void OnLeave()
        {
            if (this.activatorButton != UIMouseButton.Undefined)
            {
                this.activeOver = false;
            }

            this.StopDoubleClickTimer();
            if (this.Leave != null) { this.Leave(this.targetObject); }
        }

        /// <summary>
        /// Called by UIMouseManager.
        /// </summary>
        public void OnMove(RCIntVector absPosition)
        {
            this.StopDoubleClickTimer();
            if (this.Move != null)
            {
                UIMouseEventArgs evtArgs = new UIMouseEventArgs(this.targetObject.TransformAbsToLocal(absPosition));
                this.Move(this.targetObject, evtArgs);
            }
        }

        /// <summary>
        /// Called by UIMouseManager.
        /// </summary>
        public void OnWheel(RCIntVector absPosition, int wheelDelta)
        {
            this.StopDoubleClickTimer();
            if (this.Wheel != null)
            {
                UIMouseEventArgs evtArgs = new UIMouseEventArgs(this.targetObject.TransformAbsToLocal(absPosition),
                                                                wheelDelta);
                this.Wheel(this.targetObject, evtArgs);
            }
        }

        #endregion Trigger methods

        #region Propagation methods

        private void PropagateEnter(UISensitiveObject sender) { if (this.Enter != null) { this.Enter(sender); } }
        private void PropagateLeave(UISensitiveObject sender) { if (this.Leave != null) { this.Leave(sender); } }
        private void PropagateMove(UISensitiveObject sender, UIMouseEventArgs evtArgs) { if (this.Move != null) { this.Move(sender, evtArgs); } }
        private void PropagateButtonDown(UISensitiveObject sender, UIMouseEventArgs evtArgs) { if (this.ButtonDown != null) { this.ButtonDown(sender, evtArgs); } }
        private void PropagateButtonUp(UISensitiveObject sender, UIMouseEventArgs evtArgs) { if (this.ButtonUp != null) { this.ButtonUp(sender, evtArgs); } }
        private void PropagateClick(UISensitiveObject sender, UIMouseEventArgs evtArgs) { if (this.Click != null) { this.Click(sender, evtArgs); } }
        private void PropagateDoubleClick(UISensitiveObject sender, UIMouseEventArgs evtArgs) { if (this.DoubleClick != null) { this.DoubleClick(sender, evtArgs); } }
        private void PropagateWheel(UISensitiveObject sender, UIMouseEventArgs evtArgs) { if (this.Wheel != null) { this.Wheel(sender, evtArgs); } }
        private void PropagateReset(object sender, EventArgs evtArgs)
        {
            this.activatorButton = UIMouseButton.Undefined;
            this.activeOver = false;
            this.StopDoubleClickTimer();
            this.targetObject.ResetState();
            if (this.StateReset != null) { this.StateReset(sender, evtArgs); }
        }

        #endregion Propagation methods

        #region Public properties

        /// <summary>
        /// Gets the target UISensitiveObject of this sensor.
        /// </summary>
        public UISensitiveObject TargetObject { get { return this.targetObject; } }

        /// <summary>
        /// Gets the mouse button that activated this sensor or UIMouseButton.Undefined if the sensor is inactive.
        /// </summary>
        public UIMouseButton ActivatorButton { get { return this.activatorButton; } }

        /// <summary>
        /// Gets whether the last position of the mouse pointer was inside the target object
        /// of this sensor or not.
        /// </summary>
        public bool ActiveOver { get { return this.activeOver; } }

        #endregion Public properties

        #region IUIMouseSensor members

        /// <see cref="IUIMouseSensor.Enter"/>
        public event UIInputEventHdl Enter;

        /// <see cref="IUIMouseSensor.Leave"/>
        public event UIInputEventHdl Leave;

        /// <see cref="IUIMouseSensor.Move"/>
        public event UIMouseEventHdl Move;

        /// <see cref="IUIMouseSensor.ButtonDown"/>
        public event UIMouseEventHdl ButtonDown;

        /// <see cref="IUIMouseSensor.ButtonUp"/>
        public event UIMouseEventHdl ButtonUp;

        /// <see cref="IUIMouseSensor.Click"/>
        public event UIMouseEventHdl Click;

        /// <see cref="IUIMouseSensor.DoubleClick"/>
        public event UIMouseEventHdl DoubleClick;

        /// <see cref="IUIMouseSensor.Wheel"/>
        public event UIMouseEventHdl Wheel;

        /// <see cref="IUIMouseSensor.StateReset"/>
        public event EventHandler StateReset;

        /// <see cref="IUIMouseSensor.AttachTo"/>
        public void AttachTo(IUIMouseSensor target)
        {
            if (target == null) { throw new ArgumentNullException("target"); }

            target.Enter += this.PropagateEnter;
            target.Leave += this.PropagateLeave;
            target.Move += this.PropagateMove;
            target.ButtonDown += this.PropagateButtonDown;
            target.ButtonUp += this.PropagateButtonUp;
            target.Click += this.PropagateClick;
            target.DoubleClick += this.PropagateDoubleClick;
            target.Wheel += this.PropagateWheel;
            target.StateReset += this.PropagateReset;
        }

        /// <see cref="IUIMouseSensor.DetachFrom"/>
        public void DetachFrom(IUIMouseSensor target)
        {
            if (target == null) { throw new ArgumentNullException("target"); }

            target.Enter -= this.PropagateEnter;
            target.Leave -= this.PropagateLeave;
            target.Move -= this.PropagateMove;
            target.ButtonDown -= this.PropagateButtonDown;
            target.ButtonUp -= this.PropagateButtonUp;
            target.Click -= this.PropagateClick;
            target.DoubleClick -= this.PropagateDoubleClick;
            target.Wheel -= this.PropagateWheel;
            target.StateReset -= this.PropagateReset;
        }

        #endregion IUIMouseSensor members

        #region Private fields

        /// <summary>
        /// Called on every frame update if the double-click timer is running.
        /// </summary>
        /// <param name="evtArgs">Contains timing informations.</param>
        private void OnFrameUpdate()
        {
            this.doubleClickTimer += UIRoot.Instance.GraphicsPlatform.RenderLoop.TimeSinceLastUpdate;
            if (this.doubleClickTimer > DOUBLE_CLICK_TIME)
            {
                /// Double-click time elapsed.
                this.StopDoubleClickTimer();
            }
        }

        /// <summary>
        /// Stops the double-click timer.
        /// </summary>
        private void StopDoubleClickTimer()
        {
            UIRoot.Instance.GraphicsPlatform.RenderLoop.FrameUpdate -= this.OnFrameUpdate;
            this.doubleClickTimer = 0;
            this.doubleClickButton = UIMouseButton.Undefined;
            this.doubleClickStartPos = RCIntVector.Undefined;
        }

        /// <summary>
        /// Reference to the target UISensitiveObject of this sensor.
        /// </summary>
        private UISensitiveObject targetObject;

        /// <summary>
        /// The mouse button that activated this sensor or UIMouseButton.Undefined if the sensor is inactive.
        /// </summary>
        private UIMouseButton activatorButton;

        /// <summary>
        /// This flag indicates whether the last position of the mouse pointer was inside the target object
        /// of this sensor or not.
        /// </summary>
        private bool activeOver;

        /// <summary>
        /// Timer for raising double-click events.
        /// </summary>
        private int doubleClickTimer;

        /// <summary>
        /// Stores the mouse button that will raise the double-click event if pressed another once.
        /// </summary>
        private UIMouseButton doubleClickButton;

        /// <summary>
        /// Stores the absolute position of the mouse when the double-click timer has been started, or
        /// RCIntVector.Undefined if no double-click timer is running.
        /// </summary>
        private RCIntVector doubleClickStartPos;

        /// <summary>
        /// The maximum time between clicks that raises a double-click event.
        /// </summary>
        private const int DOUBLE_CLICK_TIME = 400;

        #endregion Private fields
    }
}
