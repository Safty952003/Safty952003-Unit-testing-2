using Bogus;
using System;
using System.Collections.Generic;
using System.Text;


namespace Simulation.Checkout.Tests.TestData;

public static class CustomerFaker
{
    public static (string Name, string Email) Create()
    {
        var faker = new Faker("en")
        {
            Random = new Randomizer(1234)
        };

        return (
            faker.Name.FullName(),
            faker.Internet.Email()
        );
    }
}