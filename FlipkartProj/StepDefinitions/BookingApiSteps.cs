using FlipkartProj.Api;
using FlipkartProj.Entities;
using FlipkartProj.Support;
using FlipkartProj.Support.Data;
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

        private readonly AuthClient _authClient;

        public BookingApiSteps(ApiDriver api, BookingClient bookingClient, AuthClient authClient, BookingContext ctx)
        {
            _api = api;
            _bookingClient = bookingClient;
            _authClient = authClient;
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

        [When("the booking last name is updated")]
        public async Task WhenTheBookingLastNameIsUpdated()
        {
            _ctx.Token ??= await _authClient.GetTokenAsync();

            _ctx.UpdatedLastName = $"Updated{new Random().Next(1000, 9999)}";
            _ctx.LastResponse = await _bookingClient.UpdateLastNameAsync(
                _ctx.BookingId, _ctx.UpdatedLastName, _ctx.Token);
        }

        [Then("the response should show the updated last name")]
        public async Task ThenTheResponseShouldShowTheUpdatedLastName()
        {
            var response = _ctx.LastResponse!;
            Assert.That(response.Status, Is.EqualTo(200),
                $"Update failed: {await response.TextAsync()}");

            var updated = await response.JsonAsync<Booking>();
            Assert.Multiple(() =>
            {
                Assert.That(updated!.LastName, Is.EqualTo(_ctx.UpdatedLastName), "Last name in PATCH response");
                // Fields we didn't touch must be unchanged
                Assert.That(updated.FirstName, Is.EqualTo(_ctx.Submitted.FirstName), "First name should not change");
                Assert.That(updated.TotalPrice, Is.EqualTo(_ctx.Submitted.TotalPrice), "Price should not change");
            });

            // Confirm the change was actually saved
            var check = await _bookingClient.GetAsync(_ctx.BookingId);
            var saved = await check.JsonAsync<Booking>();
            Assert.That(saved!.LastName, Is.EqualTo(_ctx.UpdatedLastName), "Last name was not persisted");
        }

        [When("the booking is deleted")]
        public async Task WhenTheBookingIsDeleted()
        {
            _ctx.Token ??= await _authClient.GetTokenAsync();
            _ctx.LastResponse = await _bookingClient.DeleteAsync(_ctx.BookingId, _ctx.Token);
        }

        [Then("the delete response should be {int}")]
        public async Task ThenTheDeleteResponseShouldBe(int expectedStatus)
        {
            var response = _ctx.LastResponse!;
            Assert.That(response.Status, Is.EqualTo(expectedStatus),
                $"Delete returned {response.Status}: {await response.TextAsync()}");
        }

        [When("the deleted booking is retrieved")]
        public async Task WhenTheDeletedBookingIsRetrieved()
        {
            _ctx.LastResponse = await _bookingClient.GetAsync(_ctx.BookingId);
        }

        [Then("the response should be {int} Not Found")]
        public async Task ThenTheResponseShouldBeNotFound(int expectedStatus)
        {
            var response = _ctx.LastResponse!;
            Assert.That(response.Status, Is.EqualTo(expectedStatus),
                $"Expected {expectedStatus} for deleted booking {_ctx.BookingId} but got " +
                $"{response.Status}: {await response.TextAsync()}");
        }
    }
}