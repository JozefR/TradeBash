namespace TradeBash.Core.Services
{
    public class Drawdown
    {
        public double Peak { get; set; }
        public double Trough { get; set; }
        public double MaxDrawDown { get; set; }
        public double TmpDrawDown { get; set; }

        public Drawdown()
        {
            Peak = 0;
            Trough = 0;
            MaxDrawDown = 0;
        }

        public void Calculate(double newValue)
        {
            if (newValue > Peak)
            {
                Peak = newValue;
                Trough = Peak;
            }
            else if (newValue < Trough)
            {
                Trough = newValue;
                TmpDrawDown = Peak - Trough;
                if (TmpDrawDown > MaxDrawDown)
                    MaxDrawDown = TmpDrawDown;
            }
        }
    }
}