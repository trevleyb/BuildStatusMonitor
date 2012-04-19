using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildStatusMonitor.Utilities.DelcomLights
{
    public class DelcomLight {

        public DelcomLight(Colors color, Modes mode) {
            Color = color;
            Mode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelcomLight"/> class.
        /// Given String representations of the states, will set this class up with the Enum versions
        /// of the color and mode states. 
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="mode">The mode.</param>
        public DelcomLight(string color, string mode) {

            Colors enumColor = Colors.Green;
            Modes enumMode   = Modes.On;

            if (!Enum.TryParse(color, true, out enumColor)) {
                FileLogger.Logger.LogError("Invalid Color '{0}' Specified. ", color);    
            }

            if (!Enum.TryParse(mode, true, out enumMode)) {
                FileLogger.Logger.LogError("Invalid Mode '{0}' Specified. ", mode);    
            }

            Color = enumColor;
            Mode  = enumMode;
        }

        public Colors Color { get; private set; }
        public Modes Mode { get; private set; }

    }
}
