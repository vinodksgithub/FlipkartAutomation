@UI
Feature: Flipkart scripts


Scenario: Launch flipkart homepage 
	Given User launches webpage 
	And User closes any popup open in homepage
	Then User should see home page 


Scenario: Search for iphone18 and validate result is displayed
	Given User launches webpage 
	And User closes any popup open in homepage
	Then User should see home page
	When User Inputs 'IPHONE 18' in search
	Then User should see search results
	And User should See first result as 'iphone 18'