using FlipkartProj.Api;
using FlipkartProj.Entities;
using FlipkartProj.Support;
using NUnit.Framework;
using Reqnroll;

namespace FlipkartProj.StepDefinitions
{
    [Binding]
    public class BookingApiSteps
    {
        private readonly ApiDriver _api;
        private readonly BookingClient _bookingClient;
        private readonly BookingContext _ctx;

        public BookingApiSteps(ApiDriver api, BookingClient bookingClient, BookingContext ctx)
        {
            _api = api;
            _bookingClient = bookingClient;
            _ctx = ctx;
        }

        [Given("the Restful-Booker API is reachable")]
        public async Task GivenTheApiIsReachable()
        {
            var response = await _api.Request.GetAsync("ping");
            Assert.That(response.Status, Is.EqualTo(201),
                $"Ping failed with status {response.Status}");
        }

        [Given("a booking is created with random data")]
        public async Task GivenABookingIsCreatedWithRandomData()
        {
            _ctx.Submitted = BookingDataGenerator.Create();
            _ctx.LastResponse = await _bookingClient.CreateAsync(_ctx.Submitted);
        }

        [Then("the create response should be successful")]
        public async Task ThenTheCreateResponseShouldBeSuccessful()
        {
            var response = _ctx.LastResponse!;
            Assert.That(response.Status, Is.EqualTo(200),
                $"Create failed: {await response.TextAsync()}");

            var body = await response.JsonAsync<CreateBookingResponse>();
            Assert.That(body, Is.Not.Null, "Response body could not be parsed");
            Assert.That(body!.BookingId, Is.GreaterThan(0), "No booking ID was returned");

            // The API echoes back what we sent
            Assert.Multiple(() =>
            {
                Assert.That(body.Booking.FirstName, Is.EqualTo(_ctx.Submitted.FirstName));
                Assert.That(body.Booking.LastName, Is.EqualTo(_ctx.Submitted.LastName));
                Assert.That(body.Booking.TotalPrice, Is.EqualTo(_ctx.Submitted.TotalPrice));
                Assert.That(body.Booking.DepositPaid, Is.EqualTo(_ctx.Submitted.DepositPaid));
                Assert.That(body.Booking.BookingDates.CheckIn, Is.EqualTo(_ctx.Submitted.BookingDates.CheckIn));
                Assert.That(body.Booking.BookingDates.CheckOut, Is.EqualTo(_ctx.Submitted.BookingDates.CheckOut));
            });

            // Keep the ID for the read, update and delete steps
            _ctx.BookingId = body.BookingId;
            Console.WriteLine($"Created booking ID: {_ctx.BookingId}");
        }

        [When("the booking is retrieved by its ID")]
        public async Task WhenTheBookingIsRetrievedByItsId()
        {
            _ctx.LastResponse = await _bookingClient.GetAsync(_ctx.BookingId);
        }

        [Then("the booking details should match what was submitted")]
        public async Task ThenTheBookingDetailsShouldMatchWhatWasSubmitted()
        {
            var response = _ctx.LastResponse!;
            Assert.That(response.Status, Is.EqualTo(200),
                $"GET booking/{_ctx.BookingId} failed: {await response.TextAsync()}");

            var actual = await response.JsonAsync<Booking>();
            Assert.That(actual, Is.Not.Null, "Response body could not be parsed");

            Assert.Multiple(() =>
            {
                Assert.That(actual!.FirstName, Is.EqualTo(_ctx.Submitted.FirstName), "First name");
                Assert.That(actual.LastName, Is.EqualTo(_ctx.Submitted.LastName), "Last name");
                Assert.That(actual.TotalPrice, Is.EqualTo(_ctx.Submitted.TotalPrice), "Total price");
                Assert.That(actual.DepositPaid, Is.EqualTo(_ctx.Submitted.DepositPaid), "Deposit paid");
                Assert.That(actual.BookingDates.CheckIn, Is.EqualTo(_ctx.Submitted.BookingDates.CheckIn), "Check-in");
                Assert.That(actual.BookingDates.CheckOut, Is.EqualTo(_ctx.Submitted.BookingDates.CheckOut), "Check-out");
                Assert.That(actual.AdditionalNeeds, Is.EqualTo(_ctx.Submitted.AdditionalNeeds), "Additional needs");
            });

            // The GET body has no ID, so confirm the ID through the search endpoint
            var search = await _bookingClient.SearchByNameAsync(_ctx.Submitted.FirstName, _ctx.Submitted.LastName);
            Assert.That(search.Status, Is.EqualTo(200), "Search by name failed");

            var ids = await search.JsonAsync<List<BookingIdItem>>();
            Assert.That(ids!.Select(i => i.BookingId), Does.Contain(_ctx.BookingId),
                $"Booking ID {_ctx.BookingId} was not found when searching by name");
        }
    }
}