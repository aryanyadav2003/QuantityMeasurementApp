using System;

namespace QuantityMeasurementApp.Business
{
    public class SupportsArithmeticImpl : ISupportsArithmetic
    {
        private Func<bool> _func;

        public SupportsArithmeticImpl(Func<bool> func)
        {
            _func = func;
        }

        public bool IsSupported()
        {
            return _func();
        }
    }
}