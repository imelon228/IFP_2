Aidyn Yeskendirov IT-2504

Assignment 2

## Overview

This project implements a functional-style call pricing and parallel processing system in C# 12 / .NET 8.

The project contains:

- `CallRecord` — an immutable record struct representing a call.
- `CallPricing` — pure call-pricing logic implemented using a single switch expression.
- `CallProcessor` — sequential and two-thread parallel processing.
- `Assignment2.Tests` — automated xUnit tests.

---

## Build and Test

Build the solution:

```bash
dotnet build
