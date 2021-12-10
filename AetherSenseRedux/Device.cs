using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AetherSenseRedux.Pattern;
using Buttplug;
using Dalamud.Logging;

namespace AetherSenseRedux
{
    internal class Device
    {
        public readonly ButtplugClientDevice ClientDevice;
        public List<IPattern> Patterns;
        public string Name { get => ClientDevice.Name; }
        private double _lastIntensity;
        private bool _active;

        public Device(ButtplugClientDevice clientDevice)
        {
            ClientDevice = clientDevice;
            Patterns = new List<IPattern>();
            _lastIntensity = 0;
            _active = true;
        }

        public void Start()
        {
            Task.Run(MainLoop).ConfigureAwait(false);
        }

        public async Task MainLoop()
        {
            while (_active)
            {
                OnTick();
                await Task.Yield();
            }
        }

        public void Stop()
        {
            _active = false;
            Patterns.Clear();

            var t = Task.Run(() => Write(0));
            t.Wait();
        }

        private void OnTick()
        {
            List<double> intensities = new List<double>();
            DateTime t = DateTime.UtcNow;
            var patternsToRemove = new List<IPattern>();

            lock (Patterns)
            {
                foreach (var pattern in Patterns)
                {
                    try
                    {
                        intensities.Add(pattern.GetIntensityAtTime(t));
                    }
                    catch (PatternExpiredException)
                    {
                        patternsToRemove.Add(pattern);
                    }
                }
            }
            foreach (var pattern in patternsToRemove)
            {
                lock (Patterns)
                {
                    Patterns.Remove(pattern);
                }
            }
            //TODO: Allow different merge modes besides average
            double intensity = (intensities.Any()) ? intensities.Average() : 0;

            Write(intensity);
        }

        private void Write(double intensity)
        {
            // clamp intensity before comparing to reduce unnecessary writes to device
            double clampedIntensity = Clamp(intensity, 0, 1);

            if (_lastIntensity == clampedIntensity)
            {
                return;
            }

            _lastIntensity = clampedIntensity;
            
            // If we don't wait on this, bad things happen on Linux and disappointing things happen on Windows, especially with slow BLE adapters.
            try
            {
                var t = ClientDevice.SendVibrateCmd(clampedIntensity);
                t.Wait();
            } catch (Exception)
            {
                // Connecting to an intiface server on Linux will spam the log with bluez errors
                // so we just ignore all exceptions from this statement. Good? Probably not. Necessary? Yes.
            }

        }

        private static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
