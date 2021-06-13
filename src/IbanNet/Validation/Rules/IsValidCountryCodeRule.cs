﻿using System;
using IbanNet.Registry;
using IbanNet.Validation.Results;

namespace IbanNet.Validation.Rules
{
    /// <summary>
    /// Asserts that the IBAN has a valid and known country code.
    /// </summary>
    internal sealed class IsValidCountryCodeRule : IIbanValidationRule
    {
        private readonly IIbanRegistry _ibanRegistry;

        public IsValidCountryCodeRule(IIbanRegistry ibanRegistry)
        {
            _ibanRegistry = ibanRegistry ?? throw new ArgumentNullException(nameof(ibanRegistry));
        }

        /// <inheritdoc />
        public ValidationRuleResult Validate(ValidationRuleContext context)
        {
            IbanCountry? country = GetMatchingCountry(context.Value);
            if (country is null)
            {
                return new UnknownCountryCodeResult();
            }

            context.Country = country;
            return ValidationRuleResult.Success;
        }

        private IbanCountry? GetMatchingCountry(string iban)
        {
            string? countryCode = GetCountryCode(iban);
            if (countryCode is null)
            {
                return null;
            }

            return _ibanRegistry.TryGetValue(countryCode, out IbanCountry? country) ? country : null;
        }

        private static unsafe string? GetCountryCode(string value)
        {
            fixed (char* ch = value)
            {
                return value.Length < 2
                    ? null
                    : new string(ch, 0, 2);
            }
        }
    }
}
