﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RC.Common;

namespace RC.App.PresLogic.Panels
{
    /// <summary>
    /// The tooltip bar on the gameplay page
    /// </summary>
    public class RCTooltipBar : RCAppPanel
    {
        /// <summary>
        /// Constructs a tooltip bar.
        /// </summary>
        /// <param name="backgroundRect">The area of the background of the panel in workspace coordinates.</param>
        /// <param name="contentRect">The area of the content of the panel relative to the background rectangle.</param>
        /// <param name="backgroundSprite">Name of the sprite resource that will be the background of this panel or null if there is no background.</param>
        public RCTooltipBar(RCIntRectangle backgroundRect, RCIntRectangle contentRect, string backgroundSprite)
            : base(backgroundRect, contentRect, ShowMode.Appear, HideMode.Disappear, 0, 0, backgroundSprite)
        {
        }
    }
}
