@api
Feature: Restful-Booker Booking API

Scenario: API server is reachable
	Given the Restful-Booker API is reachable

Scenario: Booking CRUD lifecycle
	Given a booking is created with random data
	Then the create response should be successful
	When the booking is retrieved by its ID
	Then the booking details should match what was submitted

