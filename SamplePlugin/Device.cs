using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buttplug;

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
            this.ClientDevice = clientDevice;
            this.Patterns = new List<IPattern>();
            this._lastIntensity = 0;
            this._active = true;
        }

        public async Task Run()
        {
            while (this._active)
            {
                await OnTick();
                await Task.Delay(10);
            }
        }

        public void Stop()
        {
            this._active = false;
            this.Patterns.Clear();

            var t = Task.Run(() => WriteAsync(0));
            t.Wait();
        }

        private async Task OnTick()
        {
            List<double> intensities = new List<double>();
            foreach (var pattern in Patterns)
            {
                try
                {
                    intensities.Add(pattern.GetIntensityAtTime(DateTime.UtcNow));
                }
                catch (Exception)
                {
                    this.Patterns.Remove(pattern);
                }
            }

            //TODO: Allow different merge modes besides average
            double intensity = (intensities.Any()) ? intensities.Average() : 0;

            await WriteAsync(intensity);
        }

        private async Task WriteAsync(double intensity)
        {
            double clampedIntensity = Clamp(intensity, 0, 1);

            if (this._lastIntensity == clampedIntensity)
            {
                return;
            }

            this._lastIntensity = clampedIntensity;

            await this.ClientDevice.SendVibrateCmd(clampedIntensity);
        }
        private double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
