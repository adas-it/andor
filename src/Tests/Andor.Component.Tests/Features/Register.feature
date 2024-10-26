Feature: Register

A short summary of the feature

@register
Scenario: Create a new Account
	Given a valid registration
	When we send a request to create account
	Then we should receive a e-mail with the code
	When we send a request to complete creation
