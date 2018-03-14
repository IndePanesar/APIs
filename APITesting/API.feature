@SoapUIAPI
Feature: SoapUITest To test a SOAPUI endpoint at www.webservicex.net UKLocations
Scenario Outline:  Get a location by county as a parameter and verify the response is same with second parameter
	Given I have an api end point at '<url>'
		And I want to get uk location by county '<county>'
	When I submit an http request of type '<method>' to '<endpoint>'
	Then I get response status of OK
		And the response body of locations by county '<county>'
	When I submit request '<method>' to get uk location by county '<county>' and town '<town>'
	Then I get response status of OK
		And the response body of locations by county '<county>'
		And the response body is the same as previous
Examples:
| url                              | endpoint              | method | county    | town          |
| webservicex.net/UKLocation.asmx/ | GetUKLocationByCounty | GET    | Berkshire | IndesHomeTown |
| webservicex.net/UKLocation.asmx/ | GetUKLocationByCounty | GET    | Berkshire | Slough        |

Scenario Outline:  Fibonacci
	Given I have a local api end point at port '<port>'
		And I want the Fibonacci number at index '<index>'
	When I submit request for Fibonacci number 
	Then the value should be '<Fibonacci>'
Examples:
| url  | index | Fibonacci |
| 7003 | 5     | 5         |
| 7003 | 15    | 610       |
| 7003 | 29    | 514229    |

Scenario Outline:  Fibonacci ranges
	Given I have a local api end point at port '<port>'
		And I want the Fibonacci numbers from '<startIndex>' to '<finishIndex>'
	When I submit request for Fibonacci range 
	Then the value should be '<Fibonacci>'
Examples:
| url  | startIndex | finishIndex | Fibonacci                                                                            |
| 7003 | 5          | 10          | ["5","8","13","21","34"]                                                             |
| 7003 | 15         | 20          | ["610","987","1597","2584","4181"]                                                   |
| 7003 | 20         | 30          | ["6765","10946","17711","28657","46368","75025","121393","196418","317811","514229"] |

