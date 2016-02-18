﻿namespace FactoryBot.Generators.Numbers
{
    public class DecimalRandomGenerator : TypedGenerator<decimal>
    {
        private readonly DoubleRandomGenerator _doubleRandomGenerator;

        public DecimalRandomGenerator(decimal from = decimal.MinValue, decimal to = decimal.MaxValue)
        {
            Check.MinMax(from, to, nameof(from));

            _doubleRandomGenerator = new DoubleRandomGenerator((double)from, (double)to);
        }

        protected override decimal NextInternal()
        {
            return new decimal((double)_doubleRandomGenerator.Next());
        }
    }
}