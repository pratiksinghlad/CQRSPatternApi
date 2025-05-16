# Architecture Rules

This document outlines the architectural rules enforced in the CQRS Pattern application. These rules are validated at startup in development and staging environments using NetArchTest.

## Layer Dependencies

The application follows the Onion Architecture pattern with dependencies flowing from outside to inside:
