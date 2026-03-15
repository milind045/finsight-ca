# SKILL: RRSP vs TFSA Prioritization

## When to use this skill
Use this skill when the user asks:
- Which account should I contribute to?
- Should I put money in my RRSP or TFSA?
- How should I allocate a windfall, bonus, or savings surplus?
- Is my RRSP or TFSA the better choice this year?

---

## Step-by-step reasoning process

Follow these steps in order. Do not skip steps.

### Step 1 — Check emergency fund FIRST
Use CashflowAgent to get emergency fund status.
- If coverage < 3 months: flag this as the first priority before any investment recommendation.
- State clearly: "Before investing, your emergency fund needs attention."
- Calculate the shortfall dollar amount.

### Step 2 — Get account balances and contribution room
Use AccountsAgent to retrieve:
- TFSA available room (2025 limit: $7,000 total, room = $7,000 minus YTD contributions)
- RRSP available room
- FHSA available room + whether user is a first-time buyer

### Step 3 — Determine marginal tax rate
Use the province and income to determine combined federal + provincial marginal rate:

| Province | ~$130k income marginal rate |
|----------|----------------------------|
| ON       | 43.41%                      |
| BC       | 40.70%                      |
| AB       | 33.00%                      |
| QC       | 49.97%                      |
| SK       | 38.00%                      |
| MB       | 43.40%                      |

For incomes not in the table, use the bracket that best fits.

### Step 4 — Apply the core RRSP vs TFSA decision rule

**If marginal rate > 40%:** RRSP is generally preferred first.
- The immediate tax deduction is high value.
- RRSP deduction at 43.41% on $10,000 = $4,341 back at tax time.

**If marginal rate < 30%:** TFSA is generally preferred first.
- Future tax-free growth outweighs the deduction value at low rates.

**If marginal rate is 30–40%:** Consider both. Ask:
- Does the user expect a higher income in retirement? If yes → TFSA.
- Does the user need liquidity? TFSA withdrawals restore room; RRSP does not.

### Step 5 — Check FHSA eligibility
If the user is a first-time buyer AND has FHSA room remaining:
- FHSA should be prioritized BEFORE RRSP.
- FHSA gives BOTH a tax deduction (like RRSP) AND tax-free growth on withdrawal (like TFSA).
- This is the best of both worlds — it is almost always the right first dollar for eligible users.
- 2025 FHSA annual limit: $8,000. Lifetime limit: $40,000.

### Step 6 — Build the ranked recommendation
Present a clear ranked allocation. Example format:

> **Recommended allocation of $X:**
> 1. Emergency fund top-up: $Y (brings you to 3 months coverage)
> 2. FHSA: $Z (max remaining room — best tax efficiency for first-time buyers)
> 3. TFSA: $A (max remaining 2025 room)
> 4. RRSP: $B (remaining — deduction worth $C at your marginal rate)

### Step 7 — Show the tax math
Always calculate and state:
- Dollar value of RRSP deduction: contribution × marginal rate
- Dollar value of FHSA deduction: contribution × marginal rate
- Total estimated tax savings from this allocation

---

## Key rules to always mention if relevant
- TFSA room is **restored January 1** following a withdrawal — if user withdrew this year, they cannot re-contribute until January.
- RRSP contribution deadline for the prior tax year is **60 days after December 31** (i.e., ~March 1).
- Over-contributing to TFSA by more than $2,000 triggers a **1% per month penalty** on the excess.
- RRSP must be converted to RRIF by end of the year you turn **71**.