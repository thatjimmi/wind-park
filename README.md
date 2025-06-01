## Running the Application

Start the application using Docker Compose:

```bash
docker compose up
```

This will start the API on:

http://localhost:5000

## API Endpoints

### Get Current Status

```bash
curl -X GET http://localhost:5000/api/turbines/status
```

Returns the current status of the wind park:

- Current production target and actual production
- Market price
- Status of each turbine
- Financial metrics (revenue, cost, profit)

### Set Market Price

```bash
curl -X POST http://localhost:5000/api/turbines/market-price \
  -H "Content-Type: application/json" \
  -d '{"price": 6.0}'
```

Updates the market price.

### Adjust Production Target

```bash
curl -X POST http://localhost:5000/api/turbines/production-target \
  -H "Content-Type: application/json" \
  -d '{"delta": 5}'
```

Adjusts the production target by the specified delta. Positive values increase the target, negative values decrease it.

---

## Running Tests

To run the tests:

```bash
dotnet test

```
