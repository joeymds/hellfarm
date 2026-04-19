---
name: dotnet-logging
description: "Use when adding, reviewing, fixing, or standardizing .NET logging with ILogger, ASP.NET Core request logging, background service logging, structured logs, log levels, correlation IDs, tracing context, exception logging, LoggerMessage patterns, or observability improvements."
---

# .NET Logging

## Purpose

Use this skill when working on logging in .NET applications and libraries.

Focus on logs that are:
- Structured
- Low-noise
- Safe to ship
- Useful during incident diagnosis
- Consistent across services

This skill applies to:
- ASP.NET Core APIs
- Background services and workers
- Message handlers
- Scheduled jobs
- Shared libraries that accept `ILogger` or `ILogger<T>`

## Outcomes

When using this skill, aim to:
- Log events that help explain system behavior and failures
- Prefer structured logging over string interpolation
- Choose log levels intentionally
- Preserve correlation and request context
- Avoid logging secrets, tokens, raw credentials, or unnecessary PII
- Keep high-volume code paths efficient

## Core Rules

### 1. Prefer `ILogger<T>` and structured templates

Use message templates with named properties.

Good:

```csharp
logger.LogInformation("Processed order {OrderId} for customer {CustomerId}", order.Id, order.CustomerId);
```

Avoid:

```csharp
logger.LogInformation($"Processed order {order.Id} for customer {order.CustomerId}");
```

Reason:
- Structured properties are queryable in log backends
- Interpolated strings lose property structure and usually allocate more

### 2. Log at meaningful boundaries

Prefer logs at boundaries and state transitions:
- Request start or completion when needed
- External service calls
- Queue publish or consume operations
- Important domain actions
- Retries, fallbacks, and degraded behavior
- Failures that require diagnosis

Avoid filling core business logic with repetitive line-by-line logs.

### 3. Choose log levels deliberately

Use this default guidance:

- `Trace`: very detailed flow data for deep debugging only
- `Debug`: developer-oriented diagnostic detail, usually disabled in production
- `Information`: important normal business or system events
- `Warning`: unexpected but recoverable conditions, retries, missing optional data, degraded paths
- `Error`: operation failure or unhandled exception for the current unit of work
- `Critical`: service-wide or safety-critical failure needing urgent attention

Do not log expected control flow as `Error`.

### 4. Log exceptions with the exception object

Pass the exception as the first argument.

Good:

```csharp
catch (HttpRequestException ex)
{
    logger.LogError(ex, "Call to payment provider failed for order {OrderId}", orderId);
    throw;
}
```

Avoid:

```csharp
logger.LogError("Payment provider failed: {Message}", ex.Message);
```

Reason:
- The full exception, stack trace, and inner exceptions are preserved

### 5. Avoid duplicate logging

If an exception is logged and rethrown, be careful not to log it again at every layer.

Preferred pattern:
- Log once at the boundary that owns the failure handling
- Add context at intermediate layers only when that context materially helps diagnosis

### 6. Preserve correlation and scope

Use scopes for request-level or operation-level context.

```csharp
using var scope = logger.BeginScope(new Dictionary<string, object>
{
    ["OrderId"] = orderId,
    ["CorrelationId"] = correlationId
});

logger.LogInformation("Starting order processing");
```

Prefer existing framework correlation where available:
- `HttpContext.TraceIdentifier`
- OpenTelemetry `Activity.Current`
- Message IDs and correlation IDs from the transport

### 7. Use `LoggerMessage` for hot paths

In high-throughput paths, prefer source-generated logging or `LoggerMessage` patterns.

Example:

```csharp
internal static partial class Log
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "Dequeued job {JobId} from queue {QueueName}")]
    public static partial void JobDequeued(ILogger logger, string jobId, string queueName);
}
```

Use this when:
- The log statement is in a tight loop
- The code path is very hot
- Allocation sensitivity matters

### 8. Do not log sensitive data

Never log:
- Passwords
- Access tokens
- Session keys
- Connection strings
- Full payment details
- Full personal data unless explicitly justified and approved

When useful, log stable identifiers instead:
- Entity IDs
- Correlation IDs
- External reference IDs
- Safe status values

### 9. Keep property names stable

Prefer consistent property names across the codebase.

Examples:
- `OrderId`
- `CustomerId`
- `UserId`
- `CorrelationId`
- `RequestPath`
- `StatusCode`
- `ElapsedMs`

Do not alternate between inconsistent variants like `OrderID`, `orderId`, and `order_id` in the same system unless an existing convention requires it.

### 10. Separate telemetry concerns clearly

Logging is not a substitute for:
- Metrics
- Traces
- Auditing

Use logs for explanation and diagnosis. Use metrics for aggregation and alerts. Use traces for distributed execution flow.

## Workflow

When asked to add or improve logging, follow this sequence:

1. Identify the execution boundary.
2. Determine what operators or developers need to know when the flow succeeds, degrades, or fails.
3. Add structured logs only at meaningful points.
4. Select the lowest correct log level.
5. Include stable identifiers and correlation context.
6. Ensure no secrets or noisy payload dumps are logged.
7. If the path is hot, consider `LoggerMessage`.
8. Verify the final result does not create duplicate or misleading logs.

## Service-Specific Guidance

### ASP.NET Core endpoints

Prefer:
- Framework request logging where already configured
- Additional logs only for important domain actions or unusual branches
- Logging validation or auth issues at `Warning` only when operationally useful

Avoid:
- Logging every controller entry and exit by default
- Logging full request bodies unless explicitly required and redacted

### Background services and workers

Prefer logs for:
- Start and stop events
- Job or batch identifiers
- Retry attempts
- Poison message handling
- External dependency failures
- Throughput checkpoints when useful

### Libraries

Prefer:
- Accepting `ILogger` or `ILogger<T>` through DI or caller-provided abstractions
- Logging only when the library owns meaningful operational context

Avoid:
- Hidden static loggers
- Hard dependencies on a specific sink vendor in shared code

## Review Checklist

When reviewing logging changes, check for:
- Structured templates instead of string interpolation
- Correct log levels
- Exception objects passed correctly
- Minimal duplication across layers
- Useful context fields
- No secrets or raw sensitive payloads
- No noisy logs in tight loops unless justified
- Consistent property naming

## Anti-Patterns

Avoid these patterns:

```csharp
logger.LogInformation("Entering method X");
logger.LogInformation("Leaving method X");
```

```csharp
logger.LogError("Unhandled exception: {Message}", ex.Message);
```

```csharp
logger.LogInformation($"User {user.Id} updated profile {profile.Id}");
```

```csharp
logger.LogInformation("Request payload {@Payload}", request);
```

The last example may expose sensitive or excessively large data unless strongly justified and carefully redacted.

## Output Expectations

When producing code or review guidance with this skill:
- Be explicit about why each new log exists
- Prefer minimal, high-value additions over blanket logging
- Explain log level choices when they are not obvious
- Call out privacy, duplication, and performance risks
- Keep the solution aligned with existing logging infrastructure in the repo