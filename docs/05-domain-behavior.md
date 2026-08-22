# Domain Behavior

## 1. Purpose

This document defines the initial business rules, domain behaviors, and state transitions for the Enterprise Order & Inventory Platform.

The goal is to make important business invariants explicit before they are implemented in the application.

The rules documented here are subject to refinement as additional business requirements and technical constraints are discovered.

---

## 2. Domain Behavior Principles

The domain model should protect important business invariants rather than relying exclusively on API or user-interface validation.

Key principles:

* Business invariants should be enforced at the appropriate domain boundary.
* Aggregates should control changes to their own state.
* External callers should not directly modify aggregate internals.
* Invalid state transitions should be rejected.
* Cross-aggregate operations should respect aggregate boundaries.
* User-interface validation should improve user experience but should not be the sole enforcement mechanism for critical business rules.

---

## 3. Order Aggregate

The Order aggregate is responsible for managing the order lifecycle and maintaining consistency within the order.

### Core responsibilities

The Order aggregate is responsible for:

* Managing order items
* Maintaining order state
* Validating order-level invariants
* Calculating the transactional order total
* Controlling valid order state transitions
* Preserving transaction-specific order information

---

## 4. Order Invariants

### 4.1 An Order Must Contain Items

An order cannot be placed without at least one OrderItem.

```text
Order
 ├── OrderItem
 ├── OrderItem
 └── ...
```

An empty order has no valid commercial purpose and therefore cannot transition into a placed order.

The user interface may prevent an empty cart from being submitted, but the business rule must also be enforced on the server side.

---

### 4.2 Confirmed Orders Cannot Be Modified

Once an order has reached the confirmed state, customers cannot add additional products to the order under the initial business rules.

```text
Draft
  │
  │ AddItem()
  ▼
Draft
  │
  │ Place()
  ▼
PendingPayment
  │
  │ Payment succeeds
  ▼
Confirmed
```

Once confirmed:

```text
Confirmed
    │
    └── AddItem() → Rejected
```

The platform may support different business policies in the future, but post-confirmation order modification is outside the initial scope.

---

### 4.3 Orders Cannot Be Cancelled After Shipment

The initial business rule does not allow customer cancellation once the order has been shipped.

Conceptually:

```text
Confirmed
    ↓
Fulfillment
    ↓
Shipped
```

Cancellation behavior before shipment will depend on the final order lifecycle and fulfillment requirements.

---

### 4.4 Payment Failure Should Be Retryable

A failed payment should not require the customer to recreate the entire order.

Initial behavior:

```text
PendingPayment
      │
      │ Payment fails
      ▼
PendingPayment
      │
      │ Retry payment
      ▼
Payment processing
```

The Order remains associated with the same customer, items, and order information while payment can be retried.

Payment state and Order state are related but remain separate domain concepts.

---

### 4.5 Inventory Must Be Validated During Order Placement

The user interface should prevent customers from selecting quantities that exceed currently displayed inventory.

However, user-interface validation is not sufficient.

At order placement, the Inventory domain must independently validate and reserve the required inventory.

Conceptually:

```text
Customer
   ↓
Place Order
   ↓
Order
   ↓
Inventory
   ↓
Atomic reservation
```

This protects the system from concurrent requests where multiple customers attempt to purchase the same limited inventory.

---

### 4.6 Order Total Is Derived from Order Items

The Order aggregate is responsible for determining its transactional total from its OrderItems.

Conceptually:

```text
Order
├── Item A
│   ├── Quantity: 2
│   └── UnitPrice: $50
│
├── Item B
│   ├── Quantity: 1
│   └── UnitPrice: $30
│
└── CalculateTotal()
        ↓
      $130
```

The application layer should not arbitrarily assign the calculated total to the Order.

Future pricing concerns such as discounts, promotions, taxes, and shipping charges will be modeled separately.

---

## 5. Initial Order State Model

The initial order lifecycle is:

```text
Draft
  │
  │ Place
  ▼
PendingPayment
  │
  ├── Payment Failed ──────┐
  │                        │
  │                        ▼
  │                  PendingPayment
  │
  │ Payment Succeeded
  ▼
Confirmed
  │
  ▼
Fulfillment
  │
  ▼
Shipped
  │
  ▼
Delivered
```

Potential cancellation paths:

```text
Draft ───────────────→ Cancelled

PendingPayment ──────→ Cancelled

Confirmed ───────────→ Cancelled
       │
       │ subject to final business rules
       ▼
Fulfillment

Shipped ──────────────→ Cancellation rejected

Delivered ────────────→ Cancellation rejected
```

The exact cancellation rules for states between confirmation and shipment remain subject to further business analysis.

---

## 6. Order Behaviors

The Order aggregate will eventually expose domain-oriented operations rather than allowing unrestricted state mutation.

Potential behaviors include:

```text
AddItem()
RemoveItem()
Place()
Cancel()
CalculateTotal()
```

The final method signatures and implementation will be determined during the C# domain-layer design.

---

## 7. Inventory Responsibilities

Inventory owns inventory-related business rules.

These include:

* Tracking available inventory
* Tracking reserved inventory
* Validating inventory availability
* Creating inventory reservations
* Releasing reservations
* Preventing invalid inventory quantities
* Protecting inventory consistency during concurrent order attempts

The Inventory aggregate boundary and exact reservation model remain provisional.

---

## 8. Inventory Concurrency Requirement

Inventory availability cannot be protected solely by displaying the current quantity to the customer.

For example:

```text
Available inventory = 3

Customer A → attempts to reserve 3
Customer B → attempts to reserve 3
```

Both customers may have observed the same available quantity.

Therefore, the backend must perform inventory validation and reservation as a concurrency-safe operation.

The implementation strategy may involve database transactions, locking, optimistic concurrency, or another appropriate mechanism.

The specific strategy will be selected during system and persistence design.

---

## 9. Payment Responsibilities

Payment is treated as a separate domain concept with its own lifecycle.

Potential states include:

```text
Pending
   ↓
Authorized
   ↓
Captured
```

or:

```text
Pending
   ↓
Failed
```

Payment retry behavior must allow a customer to retry payment for an existing order rather than requiring a new order.

Payment processing may involve external payment providers and therefore requires additional design for:

* Idempotency
* Retries
* Provider failures
* Webhooks
* Duplicate payment requests
* Payment reconciliation

These concerns will be designed in later phases.

---

## 10. Shipment Responsibilities

Shipment represents the fulfillment and delivery lifecycle after an order reaches the appropriate fulfillment stage.

Potential states include:

```text
Created
   ↓
LabelGenerated
   ↓
Shipped
   ↓
InTransit
   ↓
Delivered
```

Shipment creation is expected to occur after the relevant order workflow has reached the appropriate state.

The exact interaction between Orders, Fulfillment, and Shipments will be finalized during system design.

---

## 11. Aggregate Responsibility Boundaries

Initial responsibility boundaries are:

```text
Order
 ├── Order lifecycle
 ├── Order items
 ├── Transactional total
 └── Order-level invariants

Inventory
 ├── Stock availability
 ├── Reservations
 └── Inventory consistency

Payment
 ├── Payment lifecycle
 ├── Payment state
 └── Payment transaction identity

Shipment
 ├── Shipment lifecycle
 ├── Tracking state
 └── Delivery state

Product
 └── Product information and lifecycle

Customer
 └── Customer account and customer-specific state
```

One aggregate should not directly manipulate another aggregate's internal state.

Cross-aggregate behavior should occur through defined application or domain contracts.

---

## 12. Domain vs Application Responsibilities

The domain model owns business rules and state transitions.

The application layer is responsible for coordinating use cases and interactions between domain components.

For example:

```text
API
 ↓
Application Use Case
 ↓
Order
 ↓
Inventory
 ↓
Payment
```

The application layer coordinates the workflow while each domain boundary remains responsible for its own rules.

---

## 13. Provisional Decisions

The following areas require additional analysis:

* Complete Order state machine
* Cancellation rules during fulfillment
* Inventory aggregate boundary
* Inventory reservation lifecycle
* Payment state machine
* Payment retry and idempotency strategy
* Shipment creation workflow
* Transaction boundaries across aggregates
* Event-driven versus synchronous interactions
* Pricing, tax, discount, and promotion rules

These decisions should be refined before the corresponding production implementation.

---

## 14. Evolution

Domain behavior will evolve as requirements and system design become more precise.

Changes to important business rules should be reflected in this document and, where appropriate, in the corresponding requirements, domain model, architecture documentation, and Architecture Decision Records.
