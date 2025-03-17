# CQRS Pattern Example in .NET 8
![.Net](https://img.shields.io/badge/-.NET%208.0-blueviolet?logo=dotnet) 
![Mysql](https://img.shields.io/badge/MySQL-4479A1?logo=mysql&logoColor=white)
![EF](https://img.shields.io/badge/-Entity_Framework-8C3D65?logo=dotnet&logoColor=white)
![Openapi](https://img.shields.io/badge/Docs-OpenAPI%208.0-success?style=flat-square)
![Swagger](https://img.shields.io/badge/-Swagger-%23Clojure?logo=swagger&logoColor=white)
![Rest](https://img.shields.io/badge/rest-40AEF0?logo=rest&logoColor=white)
![HTTP3](https://img.shields.io/badge/HTTP%203-v3.0-brightgreen)
![HTTP2](https://img.shields.io/badge/HTTP%202-v2.0-blue)
![HTTP1](https://img.shields.io/badge/HTTP%201-v1.1-orange)

Command Query Responsibility Segregation (CQRS) is a software architectural pattern that separates the operations of reading data (queries) from modifying data (commands). This distinction allows for independent optimization of each operation, enhancing performance, scalability, and security in applications.

This repository demonstrates the **CQRS (Command Query Responsibility Segregation)** pattern using **.NET 8**. The project incorporates the **MediatR** library to enable flexible and standardized querying of data. It also features **OpenAPI documentation** for seamless exploration and understanding of the API. The application uses **Entity Framework** for data access, with two separate databases: one for write operations and another for read operations.
**HTTP/3/2/1** fallback code supports **Brotli** compression and falls back to **Gzip** for **response compression**.

# Project structure / technology
`Onion Layer principle` Dependencies can only be made 1 way. From outside to the inside.
To communicate from controller to our business logic layer (CQRSPattern.Application)
* .NET 8: Technology
* Entity Framework Core: ORM mapper of Microsoft
* MediatR: Framework written by J. Bogard to decouple code more easily

## Hosting projects: CQRSPattern.Api
The executing code runs from these projects.

## Example Included

The Employee Controller endpoint uses separate database contexts for different operations. For create/add actions, it utilizes the write DB context and its command, while read operations rely on the read DB context and query. Both functionalities connect to separate databases (see connection string details).

Employee Endpoint
![employee_endpoint](./Screenshots/employee_endpoint.jpg)

Multiple DB connections
![connection_strings](./Screenshots/connection_strings.jpg)

## Libraries:
- **MediatR**: MediatR is a lightweight library designed for implementing the Mediator pattern in .NET applications.
- **OpenAPI Documentation**: Automatically generated API documentation using OpenAPI for better understanding and testing of the 
API.
- **Entity Framework**: Entity Framework (EF) is an open-source object-relational mapping (ORM) framework developed by Microsoft for .NET applications.
- **Scalar**: Replaces Swagger for calling and testing APIs.