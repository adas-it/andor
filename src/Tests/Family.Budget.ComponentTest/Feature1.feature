Feature: Feature1

A short summary of the feature

@tag1
Scenario: Create a new Configuration
	Given a valid configuration
	When the configuration are sended to request
	Then the id should not be null
