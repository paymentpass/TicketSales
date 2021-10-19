# TicketSales

Concert tickets sale solution

Roles:
  - Admin
  - User

Domain rules:
  - User can buy tickets for various concerts presented in the list
  - User can view his own purchase history of tickets
  - Admin can create concerts
  - Admin can view how many tickets are sold for which concert
  - Admin needs to specify how many tickets are for sale per concert
  - User cannot buy more tickets than available

Services:
  - Core service should accept commands to
    (1) create concert and
    (2) buy concert tickets. 
    When concert is created it should publish event. 
    When new ticket is bought it should publish event.
  - Admin service (web-ui) should have a view
    (1) to display how many tickets are sold for which concert and a view
    (2) to create new concert
  - User service (web-ui) should
    (1) display tickets bought by user for specific concert and
    (2) offer user to buy new tickets for concerts.

Implement solution using .net core, event based architecture, MassTransit. For extra points use clean architecture and event sourcing.

**For RabbitMQ virtual host use your name**

**For DBs use in memory DB or embedded DB for easier setup**