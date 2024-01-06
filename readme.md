# SWEN MTCG

## Project Structure

### MTCG

Contains the main part of the application.
Including businesslogic, repositories and models.

### MTCG.Tests

Contains the unit test for MTCG.
Additionally contains sql scripts to prepare the test database accordingly.

### MTCG.Integration

Contains the integration script for MTCG and a few helper classes to display the results.

### HttpClient / HttpServer

Helper classes used to send / recieve http messages.

### DbInitializer

Helper class which initializes a database according to a provided configuration.

## Design

The application spawns a REST server, which creates a new Task for every request. For persisiting data it uses a postgres database which runs in a docker container.

Incoming request are routed to their corresponding endpoint handler (MTCG/Businesslogic/EndpointHandlers), which makes use of repositories (MTCG/DataAccess/Repositories) to query against the database.

To prevent race conditions, queries against the database have been made as atomic as possible using the EXISTS clause to ensure that certain conditions are met and the RETURNING clause to get data enterd by a non returning query.

It uses the "unit of work" pattern to ensure that changes are correctly rolled back in case of error.

At the beginning of each endpoint handler a unit of work is created (with a using statement).
All following queries against the database are performed through this unit. 
In the end the unit has to be commited. 
If in case of an error the unit can't be commited or commiting fails, the unit disposes of itself and rolls back all non commited changes.

```
// create unit
using UnitOfWork unit = new();

// perform operations here

// commit unit
unit.Commit();
```

## Lessons learned

My first attempt at preventing race conditions was to set constraints in the database directly and catch and handle the exceptions thrown if a query violated one of them.
This polluted the DataAccess with Businesslogic and made the code unnecessarily complex.

Making queries atomic made things (except the queries) much simpler and easier to follow.
Learning about the unit of work pattern made error handling and rollbacks much easier.

## Unit testing decisions

Because most endpoints are in the end just queries against the database, I created a db exclusively for testing.
It mirrors the real database in terms of tables and relations.

Before every group of tests the test database is dropped, created and filled with data relevant to these tests.

The testing framework used is NUnit and all tests follow the AAA pattern.

## Unique feature

The matchhistory of all users is stored with a short description, game result and date + time when the game was played.
The matchhistory can be aquired through the resource GET /history.

Example output:

```
[
  {
    "Description": "Bob vs Alice",
    "Result": "win"
  },
  {
    "Description": "Bob vs Charlie",
    "Result": "lose"
  }
]
```

## Git repo

https://github.com/FMahdi09/SWEN-MTCG