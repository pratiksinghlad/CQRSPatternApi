# CQRS Pattern Example in .NET 9
Command Query Responsibility Segregation (CQRS) is a software architectural pattern that separates the operations of reading data (queries) from modifying data (commands). This distinction allows for independent optimization of each operation, enhancing performance, scalability, and security in applications.

This repository demonstrates the **CQRS (Command Query Responsibility Segregation)** pattern using **.NET 9**. The project incorporates the **MediatR** library to enable flexible and standardized querying of data. It also features **OpenAPI documentation** for seamless exploration and understanding of the API. The application uses **Entity Framework** for data access, with two separate databases: one for write operations and another for read operations.

## Libraries:
- **MediatR**: MediatR is a lightweight library designed for implementing the Mediator pattern in .NET applications.
- **OpenAPI Documentation**: Automatically generated API documentation using OpenAPI for better understanding and testing of the 
API.
- **Entity Framework**: Entity Framework (EF) is an open-source object-relational mapping (ORM) framework developed by Microsoft for .NET applications.
- **Scalar**: Replaces Swagger for calling and testing APIs.