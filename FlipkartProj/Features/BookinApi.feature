@api
Feature: Restful-Booker Booking API

Scenario: API server is reachable
	Given the Restful-Booker API is reachable

Scenario: Booking CRUD lifecycle
	#POST
	Given a booking is created with random data
	Then the create response should be successful
	#Get 
	When the booking is retrieved by its ID                
	Then the booking details should match what was submitted
	#Update/Patch
	When the booking last name is updated
	Then the response should show the updated last name
	#Delete
	When the booking is deleted
	Then the delete response should be 201
	When the deleted booking is retrieved
	#Validate delete
	Then the response should be 404 Not Found
