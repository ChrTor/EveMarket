using RichardSzalay.MockHttp;
using EveMarket.HttpClients;
using System.Net;
using Microsoft.Extensions.Options;
using FakeItEasy;
using EveMarket.Features.Market;
using Application.Tests.Common;
using FluentAssertions;
using ErrorOr;

namespace Application.Tests.Features;

[TestFixture]
public class FetchPricingTests
{
    private FetchPricing.Handler _sut;
    private EveClient _client;
    private IOptionsMonitor<EveOptions> _eveOptions;
    private IOptionsMonitor<Profile> _profile;
    private MockHttpMessageHandler _eveClientHttpMock;

    [SetUp]
    public void Setup()
    {
        _eveClientHttpMock = new MockHttpMessageHandler();
        _eveOptions = A.Fake<IOptionsMonitor<EveOptions>>();
        _profile = A.Fake<IOptionsMonitor<Profile>>();

        _client = new EveClient(
            _eveClientHttpMock.ToHttpClient(), _eveOptions, _profile);

        _sut = new FetchPricing.Handler(_client);
    }

    [Test]
    public async Task GivenNoOrdersExist_WhenLookingForPricing_ThenIsNotFound()
    {
        ExpectPricing("all", 10000030, 245, Json.EmptyPricingInformation);

        var response = await _sut.Handle(
            new FetchPricing.ForCommodity(
                245,
            EveMarket.EveData.EveRegions.RegionEnum.Heimatar,
            EveMarket.HttpClients.EveEntities.Market.OrderType.all),
            CancellationToken.None);

        response.IsError.Should().BeTrue();
        response.FirstError.Code.Should().Be(Error.NotFound().Code);
    }

    [Test]
    public async Task GivenSingleOrdersExist_WhenLookingForPricing_ThenRecieveListOfPrices()
    {
        ExpectPricing("all", 10000030, 245, Json.SinglePricingInformation);

        var response = await _sut.Handle(
            new FetchPricing.ForCommodity(
                245,
            EveMarket.EveData.EveRegions.RegionEnum.Heimatar,
            EveMarket.HttpClients.EveEntities.Market.OrderType.all),
            CancellationToken.None);

        response.IsError.Should().BeFalse();
        response.Value.Orders.Count().Should().Be(1);
    }

    [Test]
    public async Task GivenMultipleOrdersExist_WhenLookingForPricing_ThenRecieveListOfPrices()
    {
        ExpectPricing("all", 10000030, 245, Json.PricingInformation);

        var response = await _sut.Handle(
            new FetchPricing.ForCommodity(
                245,
            EveMarket.EveData.EveRegions.RegionEnum.Heimatar,
            EveMarket.HttpClients.EveEntities.Market.OrderType.all),
            CancellationToken.None);

        response.IsError.Should().BeFalse();
        response.Value.Orders.Count().Should().BeGreaterThan(1);
    }

    private void ExpectPricing(string orderType, int regionId, int typeId, string json)
    {
        _eveClientHttpMock
            .When(HttpMethod.Get, $"https://esi.evetech.net/latest/markets/{regionId}/orders/?datasource=tranquility&order_type={orderType}&page=1&type_id={typeId}")
            .Respond(HttpStatusCode.OK, "application/json", json);
    }
}