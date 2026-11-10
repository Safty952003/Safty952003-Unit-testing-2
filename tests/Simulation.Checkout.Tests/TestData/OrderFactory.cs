using AutoFixture;
using Simulation.Checkout.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation.Checkout.Tests.TestData;

public static class OrderFactory
{
    public static Order Create()
    {
        var fixture = new Fixture();

        var items = fixture.Build<OrderItem>()
            .With(x => x.Quantity, 2)
            .With(x => x.UnitPrice, 100m)
            .CreateMany(2)
            .ToList();

        return fixture.Build<Order>()
            .With(x => x.Items, items)
            .Create();
    }
}

