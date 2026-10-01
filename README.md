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
```

Conclusion

The assignment demonstrates functional programming concepts in C#, including immutable data, pure functions, pattern matching, validation, controlled mutation, and deterministic parallel processing.
The final implementation builds successfully on .NET 8, and all 22 automated tests pass.
