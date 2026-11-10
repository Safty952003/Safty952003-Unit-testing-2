# Testing Summary

## 1. Test Results
- Total tests: 20
- Passing: 20
- Failing: 0

## 2. Line Coverage
- Overall line coverage: 90.5%

## 3. Testing Libraries
- **Moq:** Used to mock `IInventoryService`, `IOrderRepository`, and `INotificationService` and verify their interactions.
- **AutoFixture:** Used to automatically generate test orders and order items.
- **Bogus:** Used to generate realistic customer names and email addresses.

## 4. Discount Fairness
No, because a customer spending 1,000 receives a 100 discount, while a customer spending 999.99 receives no discount, creating an unfair pricing gap.

## 5. Bugs Found
None.