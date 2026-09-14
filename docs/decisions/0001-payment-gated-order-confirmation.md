# ADR-0001: Confirm orders only after a successful payment

**Status:** Accepted  
**Date:** 2026-09-13

## Context

The platform must distinguish an editable shopping intent from a commercial order that is eligible for warehouse fulfillment. A payment may fail, require customer authentication, be submitted more than once, or succeed at the provider while the platform is temporarily unavailable.

## Decision

The `Cart` is the customer's editable draft. Checkout creates an immutable-item `Order` with status `PendingPayment`. Only a verified successful payment result may call `Order.Confirm()` and move the order to `Confirmed`.

The early lifecycle is:

```text
Cart (editable draft) -> Order: PendingPayment -> payment succeeds -> Confirmed
                                          \-> payment fails -> PendingPayment (retry)
```

`Confirmed` is the contract that allows inventory fulfillment to begin. A failed or merely initiated payment must never produce a confirmed order.

## Consequences

* The current synchronous checkout service creates the pending-payment order only; it does not claim to charge a customer.
* A later payment module will own payment attempts, provider transaction identifiers, idempotency keys, webhook verification, and reconciliation.
* A successful provider result must be persisted before (or atomically with) order confirmation. We will implement this with an outbox/inbox pattern once persistence and provider integration are introduced.
* Inventory reservation policy will be decided before the payment module: reserve during the payment window and release on expiry/failure, or reserve only after payment. This is a product trade-off, not an incidental implementation detail.

## Alternatives considered

* **Create a confirmed order at checkout:** rejected because checkout is not proof of a successful payment.
* **Model both Cart and Order as Draft:** rejected for now because it duplicates ownership of the editable purchase intent.
