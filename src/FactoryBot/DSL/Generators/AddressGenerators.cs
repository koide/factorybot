﻿using FactoryBot.DSL.Attributes;
using FactoryBot.Generators;
using FactoryBot.Generators.Strings;

namespace FactoryBot.DSL.Generators
{
    public class AddressGenerators
    {
#pragma warning disable IDE0060 // Remove unused parameter
        [StringGeneratorFromResource(SourceNames.COUNTRIES)]
        public string Country() => default;

        [StringGeneratorFromResource(SourceNames.CITIES)]
        public string City() => default;

        [StringGeneratorFromResource(SourceNames.STATES)]
        public string State() => default;

        [Generator(typeof(PostalCodeGenerator))]
        public string PostalCode(PostalCodeFormat format) => default;

        [Generator(typeof(StreetAddressGenerator))]
        public string StreetAndBuilding() => default; // todo should support formats as well

        [Generator(typeof(PhoneNumberGenerator))]
        public string PhoneNumber() => default;

        [Generator(typeof(PhoneNumberGenerator))]
        public string PhoneNumber(string template) => default;

#pragma warning restore IDE0060 // Remove unused parameter
    }
}