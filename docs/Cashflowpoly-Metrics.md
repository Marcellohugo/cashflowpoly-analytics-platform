# Cashflowpoly Gameplay Variables and Analytics Metrics
## Comprehensive Analysis of Measurable Data for Player Performance Evaluation

---

## Executive Overview

This analysis identifies **gameplay variables** (raw observable data from Cashflowpoly) and distinguishes them from **analytics metrics** (derived player performance indicators). The document follows a four-step analysis:

1. **Physical gameplay variables** observable from game components
2. **Derived metrics** from physical variables (quantitative performance)
3. **Narrative gameplay variables** from TTRPG dialogue and player behaviors
4. **Narrative-based metrics** for qualitative performance assessment

---

## Part 1: Physical Gameplay Variables

These variables are directly observable from Cashflowpoly's game components: boards, tokens, cards, coins, and scoresheets.

### 1.1 Coin-Based Financial Variables

**Raw Gameplay Variables** (Observable):
- `starting_coins`: Initial capital at game start (20 coins in Beginner, 10 in Advanced mode)
- `coins_held_current`: Coins in player's possession at any moment
- `coins_spent_per_turn`: Coins expended during a single action
- `coins_earned_per_turn`: Coins received during a single action (freelance work, meal orders)
- `coins_donated`: Total coins given to bank on Fridays
- `coins_saved`: Coins deposited toward Financial Goals (Advanced only)
- `coins_net_end_game`: Final coin balance at game completion

**Calculation**:
```
Coins_net_end_game = starting_coins + total_income - total_expenses
Where:
  total_income = freelance_work_coins + meal_order_income + loan_received + gold_sales
  total_expenses = ingredients_purchases + needs_purchases + donations + insurance_payments + loan_repayment + life_risk_payments
```

---

### 1.2 Ingredient Card Variables

**Raw Gameplay Variables** (Observable):
- `ingredients_collected`: Total ingredient cards acquired during game
- `ingredients_held_current`: Cards currently in hand (max 6 total, max 3 of same type)
- `ingredient_types_held`: Distribution across ingredient types (Rice, Egg, Meat, etc.)
- `ingredients_used_per_meal`: Cards expended to claim meal orders
- `ingredients_wasted`: Cards discarded (never used in meal orders)
- `ingredient_investment_coins_total`: Total coins spent purchasing ingredients

**Patterns**:
- Efficient players: High `ingredients_used_per_meal` ratio (most ingredients convert to income)
- Hoarders: High `ingredients_held_current` but low `ingredients_used_per_meal` ratio
- Wasteful: High `ingredients_wasted` relative to `ingredients_collected`

---

### 1.3 Meal Order Variables

**Raw Gameplay Variables** (Observable):
- `meal_orders_claimed`: Total meal orders successfully completed
- `meal_orders_available_passed`: Meal orders player chose NOT to claim
- `meal_order_income_per_order`: Coins received for each specific meal order
- `meal_order_income_total`: Sum of all meal order payouts
- `meal_orders_per_turn_average`: Average meal orders claimed per turn

**Business Efficiency Pattern**:
```
Business_Efficiency_Ratio = meal_order_income_total / ingredient_investment_coins_total

High ratio (> 2.0): Profitable business
Low ratio (< 1.5): Inefficient business (spent more on ingredients than earned)
```

---

### 1.4 Need Card Variables

**Raw Gameplay Variables** (Observable):
- `need_cards_purchased`: Total need cards acquired
- `primary_needs_owned`: Number of Primary Need cards
- `secondary_needs_owned`: Number of Secondary Need cards
- `tertiary_needs_owned`: Number of Tertiary Need cards
- `specific_tertiary_need`: Whether correct tertiary for collection mission acquired
- `collection_mission_complete`: Boolean (yes/no) mission requirements met
- `need_cards_coins_spent`: Total coins invested in needs

**Categorization**:
- **Basic Profile**: All three need categories represented (minimum happiness structure)
- **Collector Profile**: High diversity in need cards (4+ different types)
- **Specialist Profile**: Heavy concentration in one category (unbalanced)

---

### 1.5 Donation Variables (Friday Actions)

**Raw Gameplay Variables** (Observable):
- `donation_amount_per_friday`: Coins donated each Friday (5 Fridays typical)
- `donation_rank_per_friday`: Placement (1st, 2nd, 3rd, 4th) each Friday
- `donation_total_coins`: Sum of all Friday donations
- `donation_champion_cards_earned`: Cards won (7pts, 5pts, 2pts, 0pts)
- `donation_happiness_points`: Total happiness from donations

**Donation Strategy Pattern**:
```
Donation_Aggressiveness = donation_total_coins / coins_net_end_game

High (> 30%): Aggressive donor, prioritizes social contribution over financial accumulation
Low (< 10%): Conservative donor, prioritizes personal wealth

Stability = std_deviation(donation_amount_per_friday)
High variance: Reactive to financial situation
Low variance: Consistent commitment
```

---

### 1.6 Gold Investment Variables

**Raw Gameplay Variables** (Observable):
- `gold_cards_purchased`: Total gold cards acquired
- `gold_cards_sold`: Total gold cards liquidated
- `gold_cards_held_end`: Gold cards remaining at game end
- `gold_prices_per_purchase`: Purchase price for each gold transaction
- `gold_price_per_sale`: Sale price for each gold liquidation
- `gold_investment_coins_spent`: Total coins invested in gold
- `gold_investment_coins_earned`: Total coins received from gold sales
- `gold_investment_net`: coins_earned - coins_spent

**Investment Efficiency**:
```
Gold_ROI_Percentage = (gold_investment_coins_earned - gold_investment_coins_spent) / gold_investment_coins_spent × 100%

Positive ROI: Profitable investment (bought low, sold high)
Negative ROI: Loss (bought high, sold low)
Zero: No net gain/loss
```

---

### 1.7 Pension Fund Variables (End-Game Scoring)

**Raw Gameplay Variables** (Observable):
- `leftover_coins_end_game`: Coins not spent by game finish
- `ingredient_cards_value_end`: Leftover ingredients (1 coin each)
- `coins_in_savings_goal`: Unspent coins on Financial Goal board (Advanced)
- `pension_fund_total`: Sum of above three items
- `pension_fund_rank_per_game`: Placement (1st/3pts, 2nd/2pts, 3rd/1pt)
- `pension_fund_happiness_points`: Happiness from ranking

---

### 1.8 Life Risk Variables (Advanced Mode Only)

**Raw Gameplay Variables** (Observable):
- `life_risk_cards_drawn`: Total life risk events triggered
- `life_risk_costs_per_card`: Coin penalties from each risk
- `life_risk_costs_total`: Sum of all penalties
- `life_risk_mitigated_with_insurance`: Number of risks avoided via insurance
- `insurance_payments_made`: Coins spent reactivating insurance
- `emergency_options_used`: Instances of selling needs/gold to cover risks

**Risk Resilience Pattern**:
```
Risk_Exposure_Percentage = life_risk_costs_total / total_income × 100%

High exposure (> 30%): High risk game (or unlucky draws)
Low exposure (< 10%): Protected or low-risk game

Risk_Mitigation_Effectiveness = life_risk_mitigated_with_insurance / life_risk_cards_drawn × 100%
High effectiveness: Good insurance strategy
Low effectiveness: Unprepared for risks
```

---

### 1.9 Financial Goal Variables (Advanced Mode Only)

**Raw Gameplay Variables** (Observable):
- `financial_goals_attempted`: Number of different financial goal cards pursued
- `financial_goals_completed`: Number fully funded and acquired
- `financial_goals_coins_per_goal`: Coin cost of each goal
- `financial_goals_coins_total_invested`: Total coins saved toward goals
- `financial_goals_incomplete_coins_wasted`: Coins saved but goal not completed (wasted progress)
- `sharia_loan_cards_taken`: Number of loans acquired
- `sharia_loans_repaid`: Loans paid back
- `sharia_loans_unpaid_end`: Loans still outstanding at game end
- `loan_penalty_if_unpaid`: Happiness point penalties from unpaid loans

**Debt Management Pattern**:
```
Debt_Ratio = sharia_loans_unpaid_end / sharia_loans_taken

0 (all repaid): Responsible borrower
0.5-1.0 (some unpaid): Leveraged play style
Unpaid loans at end: Failed debt management (negative happiness consequences)
```

---

### 1.10 Action Token Usage Variables

**Raw Gameplay Variables** (Observable):
- `actions_per_turn`: Actions taken each turn (always 2, but distribution varies)
- `action_repetitions_per_turn`: Same action taken twice vs. different actions
- `action_sequence`: Order of actions (income-seeking vs. need-seeking vs. diversified)
- `actions_skipped`: Days when player unable to act (passed full turn, rare)

**Action Pattern Analysis**:
```
Action_Diversity_Score = distinct_action_types / 2 (per turn)

1.0: Player diversifies actions (meal order + needs, or freelance + donation, etc.)
0.5: Player repetitive (meal order + meal order, freelance + freelance)

Early-game pattern: More ingredient gathering, business building
Late-game pattern: More needs collection, financial goals, donations
```

---

### 1.11 Turn-by-Turn Progression Variables

**Raw Gameplay Variables** (Observable):
- `coins_per_turn_progression`: Coin balance over time (curve)
- `net_income_per_turn`: Income minus expenses per turn
- `turn_number_when_debt_introduced`: When first loan taken
- `turn_number_when_first_risk_hit`: When first life risk occurred
- `turn_number_game_completion`: Total turns to reach Finish Line

**Economic Trajectory Pattern**:
```
Growth_Pattern = coins_end / coins_start

Exponential growth (> 3x): Strong business scaling
Linear growth (1.5-3x): Steady accumulation
Stagnation (1-1.5x): Slow progress
Decline (< 1x): Lost money or spent aggressively on needs
```

---

## Part 2: Derived Analytics Metrics from Physical Variables

These are calculated player performance metrics derived from the raw physical variables.

### 2.1 Financial Performance Metrics

**Metric 1: Net Worth Index**
```
Net_Worth_Index = coins_net_end_game / starting_coins × 100%

Interpretation:
  > 300%: Exceptional wealth accumulation
  200-300%: Strong financial growth
  100-200%: Moderate growth
  < 100%: Net loss (spent more than earned)
```

**Metric 2: Income Diversification Ratio**
```
Income_Diversification = (freelance_income + meal_income + gold_income + donations_received) / total_income × 100%

Measures: Did player rely on one income source or diversify?
High diversification: Multiple income streams (resilient)
Low diversification: One dominant source (risky if disrupted)
```

**Metric 3: Expense Management Efficiency**
```
Expense_Efficiency = essential_expenses / total_expenses × 100%

Where essential = ingredients necessary for meal orders
non-essential = donations, insurance, financial goals

High ratio: Efficient (most spending productive)
Low ratio: Wasteful (much discretionary spending)
```

**Metric 4: Profit Margin on Business Operations**
```
Business_Profit_Margin = (meal_income - ingredient_costs) / meal_income × 100%

Typical range: 20-60%
High margin: Efficient business (selective in which orders to fulfill)
Low margin: Taking unprofitable orders (trying to do everything)
```

---

### 2.2 Strategic Decision Metrics

**Metric 5: Risk Appetite Score** (Advanced Mode)
```
Risk_Appetite = (life_risks_accepted / life_risks_available) × (average_risk_cost) × (insurance_activation_rate)

Scaled 0-100:
  0-25: Extremely risk-averse (uses insurance frequently)
  25-50: Cautious (manages risks carefully)
  50-75: Balanced risk-taker
  75-100: High-risk player (ignores or accepts risks)
```

**Metric 6: Debt Leverage Ratio**
```
Debt_Leverage = total_loans_outstanding / net_worth × 100%

Low (0-25%): Conservative debt usage
Medium (25-75%): Strategic leverage
High (> 75%): Over-leveraged, risky

Paired metric: Loan_Repayment_Discipline = loans_repaid / loans_taken × 100%
```

**Metric 7: Goal-Setting Ambition**
```
Goal_Ambition = financial_goals_attempted + financial_goals_coins_invested / net_worth × 100%

High ambition: Multiple simultaneous goals, aggressive savings targets
Low ambition: Few goals, passive saving
```

---

### 2.3 Player Behavior Metrics

**Metric 8: Action Efficiency per Turn**
```
Action_Efficiency = (income_actions_per_turn) / (total_actions_per_turn)

Ranges:
  > 60%: Income-focused (building wealth)
  40-60%: Balanced approach
  < 40%: Need-focused or exploration-focused
```

**Metric 9: Meal Order Success Rate**
```
Meal_Order_Success_Rate = (meal_orders_completed) / (meal_orders_attempted) × 100%

Attempted = completed + passed up

High rate (> 80%): Good ingredient planning
Low rate (< 60%): Overcommitted or poor planning
```

**Metric 10: Long-term Planning Indicator**
```
Planning_Horizon = (savings_invested + goals_pursued + insurance_purchases) / total_actions

High (> 40%): Future-oriented
Low (< 20%): Present-focused
```

---

### 2.4 Flourishing-Related Metrics (Physical Components)

**Metric 11: Need Fulfillment Diversity Index**
```
Fulfillment_Diversity = sqrt((primary_needs^2 + secondary_needs^2 + tertiary_needs^2)) / total_needs_owned

Values 0-1:
  > 0.7: Balanced need fulfillment across categories
  0.4-0.7: Partial balance
  < 0.4: Heavily skewed to one need type

Also measure: Did player collect items matching personal collection mission?
Mission_Achievement = specific_tertiary_acquired × complete_primary × complete_secondary
```

**Metric 12: Donation Consistency & Commitment**
```
Donation_Commitment_Score = (donation_stability × donation_ratio) × friday_participation_rate

Where:
  donation_stability = 100 - std_deviation(donation_amounts)
  donation_ratio = total_donations / net_worth
  friday_participation_rate = fridays_donated / total_fridays (%)

Interpretation:
  High score: Consistent, committed philanthropist
  Low score: Inconsistent, opportunistic giver
```

**Metric 13: Happiness Points Composition Analysis**
```
Happiness_Portfolio = [need_cards_pts, donations_pts, gold_pts, pension_pts, financial_goals_pts, mission_bonus_pts]

Determines: Which systems did player prioritize?
Example:
  [20, 15, 8, 3, 0, -2] = Needs-focused, social-minded, failed mission
  [5, 0, 12, 5, 10, 4] = Wealth-focused, less socially engaged
```

---

## Part 3: Narrative Gameplay Variables

These variables come from observing players' TTRPG-style dialogue, decision-making during narrative moments, and behavioral patterns during gameplay (not from physical game components).

### 3.1 Dialogue-Based Variables

**Raw Narrative Variables** (Observable from TTRPG dialogue):

1. **Player-Initiated Dialogue Frequency**
   - `dialogue_initiations_per_session`: How often player asks questions or engages GM in dialogue
   - `dialogue_topics_raised`: Categories of dialogue (asking about story, explaining decisions, asking for help, making jokes, etc.)
   - `dialogue_depth`: Brief answers vs. extended conversation (word count or turn count)
   - Example: "Why did my business fail?" vs. just accepting outcome silently

2. **Character Identity Articulation**
   - `character_identity_statement_count`: How many times player explicitly states who their character is
   - `character_identity_consistency`: Are statements aligned or contradictory across session?
   - `character_personality_descriptors_given`: Adjectives player uses ("I'm a cautious investor," "I'm a risk-taker," "I'm generous," etc.)
   - Example: Does player maintain consistent personality in dialogue, or change based on circumstances?

3. **Decision Rationale Verbalization**
   - `decision_rationale_provided`: Does player explain WHY they're making decisions, or just execute silently?
   - `rationale_complexity`: Simple ("I need money") vs. complex ("I'm saving for my goal but the risk card forced me to spend, so I'll take a loan")
   - `rationale_financial_literacy_evidence`: Does rationale show financial understanding? 
     - Naive: "I'll buy gold because it went up"
     - Informed: "I'll buy gold now at price 2 because the average is 3, then sell when price returns"
   - Example: Compare player explaining strategic decisions vs. making random choices

4. **Emotional Expression During Play**
   - `emotional_response_to_loss`: Does player express frustration, acceptance, laugh it off, or blame others?
   - `emotional_response_to_success`: Celebrates, understates, attributes to luck vs. skill?
   - `emotional_tone_consistency`: Stays in character emotionally vs. breaks immersion
   - `stress_signals`: Nervous laughter, sighing, rushed speech when facing crisis (life risk, debt)
   - Example: "Oh no, I failed! I'll do better next time" (growth mindset) vs. "This game is unfair!" (external locus of control)

5. **Relationship Dialogue with NPCs**
   - `npc_dialogue_initiations`: How many times player engages with NPCs in dialogue
   - `npc_character_distinctness`: Can player remember NPC names, personalities, backstories?
   - `npc_relationship_development`: Does dialogue show deepening relationship? ("Remember when you said X last time?")
   - `npc_agency_perception`: Does player treat NPCs as autonomous or obviously scripted?
   - Example: "Chef Lee told me that rice prices would drop, and they did!" (NPC felt real) vs. "The game just said rice dropped" (mechanical understanding only)

6. **Narrative Callback & Continuity**
   - `narrative_references_to_past_events`: Player refers back to earlier story moments
   - `narrative_coherence_statements`: Player articulates how game events connect to a story arc
   - `life_story_integration`: Does player see their financial journey as a coherent narrative?
   - Example: "This debt reminds me of the time I failed before, but now I know better..."

---

### 3.2 Decision-Making Context Variables

**Raw Decision Variables** (Beyond which action, HOW decisions are made):

1. **Deliberation Time**
   - `seconds_spent_deciding_per_turn`: How long player takes before committing to action
   - `deliberation_pattern_change`: Does player deliberate more/less as game progresses?
   - `high_stakes_deliberation_change`: Does player take longer for high-consequence decisions?
   - Interpretation: Rushed = superficial; Extended = reflective

2. **Information Seeking Behavior**
   - `questions_about_mechanics_asked`: How many times player asks "How does this work?"
   - `questions_about_strategy_asked`: "What would you recommend?" "Is this a good move?"
   - `questions_about_narrative_asked`: "Why is the price changing?" "What happens if I do this?"
   - `self_correction_moments`: Does player catch own mistake and adjust?
   - Interpretation: High mechanics questions = learning; High strategy questions = seeking guidance vs. autonomy

3. **Risk-Taking Behavior in Dialogue**
   - `risk_decisions_verbalized`: Player articulates risky moves
   - `risk_justification_quality`: Simple ("I need money") vs. strategic ("I'm taking calculated risk")
   - `risk_reframing_in_dialogue`: If risk fails, does player reframe as lesson or unfair?
   - Example: "I took that loan to fund my goal, knowing it was risky. I learned my lesson." (growth) vs. "The game screwed me with that risk card" (victim stance)

4. **Social Decision Context**
   - `donation_dialogue_context`: Player talks about why donating (values, social pressure, spare change?)
   - `peer_awareness`: Does player compare to others? ("I'm beating them" vs. "We're all struggling")
   - `cooperation_vs_competition_language`: Dialogue reveals cooperative or competitive mindset
   - Example: "I'm donating to help people" (values-driven) vs. "I'm donating to beat Adhi's score" (competition-driven)

---

### 3.3 Narrative Experience Variables

**Raw Experience Variables** (What the player experiences and expresses):

1. **Frustration & Challenge Signals**
   - `verbal_frustration_expressions`: "This is impossible," "I hate this," "Why me?"
   - `difficulty_complaint_frequency`: How often player says "too hard"
   - `challenge_satisfaction_signals`: "That was tough but I made it," "I got lucky," "Good strategy"
   - `quit_signals`: "I give up" (serious vs. joking)
   - Interpretation: Balance of frustration vs. satisfaction indicates optimal challenge zone

2. **Engagement & Immersion**
   - `narrative_immersion_signals`: Player forgets it's a game, refers to self as character
   - `mechanical_intrusion_complaints`: "The rules don't make sense," "That's not realistic"
   - `story_continuation_interest`: Does player want more narrative depth or prefer mechanics?
   - `replayability_interest_statements`: "Can we play again?" "What if I tried different strategy?"

3. **Learning Articulation During Play**
   - `aha_moment_expressions`: "Oh! So compound interest means...," "Now I see how..."
   - `misconception_correction_moments`: GM corrects false belief, player responds with understanding or resistance
   - `transfer_thinking_statements`: "This is like real life when..." (shows transfer thinking)
   - `metacognitive_statements`: "I'm realizing I was wrong about," "I used to think X but now..."

4. **Autonomy & Agency Perception**
   - `choice_availability_statements`: "I had to," "I could have," "I got lucky"
   - `control_perception_language`: "I caused that" (internal locus) vs. "It happened to me" (external locus)
   - `authentic_choice_signals`: Player makes decision nobody expected (GM improvises response)
   - `guidance_acceptance_language`: Player asks for help vs. figures it out themselves

5. **Identity Development Signals**
   - `character_voice_consistency`: Does player use consistent manner of speaking for character?
   - `identity_struggle_expressions`: Player articulates growth ("I'm getting better at this")
   - `values_alignment_statements`: Player connects decisions to personal/character values
   - `failure_identity_impact`: How does failure affect player's sense of self?
     - Negative impact: "I'm bad at this, I'm stupid" (fixed mindset)
     - Neutral: "That didn't work, I'll try differently" (growth mindset)

---

## Part 4: Narrative-Based Analytics Metrics

These are analytics metrics derived from narrative gameplay variables and TTRPG dialogue.

### 4.1 Dialogue Engagement Metrics

**Metric 1: Narrative Engagement Index**
```
Narrative_Engagement = (dialogue_initiations × depth_score) + (character_consistency × 2) + (npc_relationship_depth)
                       / maximum_possible

Scaled 0-100:
  0-25: Minimal engagement (silent play, mechanics-only)
  25-50: Moderate engagement (asks questions, minimal dialogue)
  50-75: High engagement (regular dialogue, character voice)
  75-100: Deep engagement (rich character, collaborative storytelling)
```

**Metric 2: Character Identity Coherence Score**
```
Character_Coherence = (identity_consistency_score + personality_distinctiveness + values_alignment) / 3

Where:
  identity_consistency_score: Do statements align? (0-100)
  personality_distinctiveness: Is character distinctive from other players'? (0-100)
  values_alignment: Do decisions match stated values? (0-100)

Interpretation:
  High (70+): Clear, coherent character identity
  Medium (40-70): Evolving or conflicted character
  Low (< 40): No clear character, mechanistic play
```

**Metric 3: Decision Rationale Sophistication**
```
Rationale_Sophistication = (strategic_thinking_level × 0.4) + (financial_literacy_evidence × 0.3) + (narrative_integration × 0.3)

Scale:
  Level 1: "I need money" (simplistic)
  Level 2: "I'll do meal orders to build cash" (tactical)
  Level 3: "I'm saving for financial goals while managing risk with insurance" (strategic)
  Level 4: "My character is building sustainable income to fund long-term goals while maintaining social values through donations" (integrated)
```

**Metric 4: NPC Relationship Authenticity Perception**
```
NPC_Authenticity = (npc_recall_accuracy × 0.3) + (npc_dialogue_reciprocity × 0.4) + (relationship_continuity × 0.3)

Measures: Did NPCs feel like real characters with own motivations?
High score: NPCs felt autonomous and real
Low score: NPCs felt scripted or generic
```

---

### 4.2 Psychological & Learning Metrics from Narrative

**Metric 5: Growth Mindset Score** (from dialogue analysis)
```
Growth_Mindset = (failure_reframing + learning_articulation + challenge_seeking) / 3

Failure_Reframing (0-100): 
  When things go wrong, does player say "I'll learn" (high) or "I'm bad at this" (low)?

Learning_Articulation (0-100):
  Does player articulate what they learned? Evidence in dialogue?

Challenge_Seeking (0-100):
  Does player take harder risks? Choose challenging goals? Accept difficult quests?

Overall interpretation:
  70+: Strong growth mindset
  40-70: Mixed (sometimes growth, sometimes fixed)
  < 40: Fixed mindset (blames external factors)
```

**Metric 6: Autonomy Experience Index** (from dialogue)
```
Autonomy_Experience = (choice_frequency × 0.4) + (authentic_decision_ownership × 0.3) + (deviation_from_guidance × 0.3)

Measures: Did player feel they were making autonomous choices or following rails?

Choice_Frequency: How often did player make unexpected choices?
Decision_Ownership: Does player say "I chose to" vs. "I had to"?
Deviation_From_Guidance: Did player ignore or challenge GM suggestions?

High score (70+): Genuine autonomy experience
Low score (< 40): Felt controlled or scripted
```

**Metric 7: Transfer Thinking Evidence**
```
Transfer_Evidence = (real_world_connections + concept_application + principle_articulation) / dialogue_count

Measures: In dialogue, how often does player connect game to real finance?

Real_World_Connections: "This is like when I..." (count explicit references)
Concept_Application: "So compound interest means..." (demonstrates understanding)
Principle_Articulation: Can explain underlying financial principle? (yes/no)

Interpretation:
  High frequency: Strong transfer thinking emerging
  Low frequency: Game remains isolated experience
```

**Metric 8: Emotional Resilience During Challenge**
```
Emotional_Resilience = (stress_expression_severity × -1 + recovery_time_inverse + reengagement_speed) / 3

Measures: When facing setbacks, how does player respond emotionally?

Stress_Expression_Severity (0-100):
  0 = Calm throughout
  50 = Noticeable stress but managed
  100 = Extreme frustration/shutdown

Recovery_Time: How quickly does player bounce back?
Reengagement_Speed: Does player re-engage with game or disengage?

High resilience (70+): Faces setbacks, processes emotion, continues
Low resilience (< 40): Shutdown or extended frustration
```

---

### 4.3 Narrative Quality Metrics

**Metric 9: Narrative Immersion Level**
```
Immersion_Level = (character_voice_use_frequency × 0.4) + (story_continuation_seeking × 0.3) + (mechanical_distance × -0.3)

Character_Voice_Frequency: Does player speak as character or as "I the player"?
Story_Continuation: Does player want more narrative depth?
Mechanical_Distance: How often does player comment on rules/mechanics (breaking immersion)?

Scale:
  80-100: Deep immersion (forgets it's a game)
  60-79: Good immersion
  40-59: Moderate immersion
  < 40: Mechanical/detached play
```

**Metric 10: Dialogue Depth & Quality**
```
Dialogue_Depth = (avg_words_per_exchange × 0.3) + (topic_diversity × 0.3) + (narrative_complexity × 0.4)

Avg_Words_Per_Exchange: Brief answers vs. extended explanation
Topic_Diversity: How many types of dialogue topics? (strategy, story, emotion, values, etc.)
Narrative_Complexity: Does player discuss character arc, interconnections, meaning?

Interpretation:
  Shallow dialogue (0-30): Single-word answers, no narrative engagement
  Medium dialogue (30-70): Some elaboration, basic narrative
  Deep dialogue (70-100): Rich conversation, narrative co-creation
```

**Metric 11: Narrative Coherence Integration**
```
Narrative_Coherence = (past_event_references × 0.3) + (consequence_understanding × 0.3) + (story_arc_articulation × 0.4)

Past_Event_References: Does player connect current events to earlier moments in game?
Consequence_Understanding: Does player see how past decisions led to current situations?
Story_Arc_Articulation: Can player describe their character's journey as coherent narrative?

High coherence (70+): "I started as cautious but learned to take risks, and it paid off"
Low coherence (< 40): Events seem disconnected, no narrative arc perceived
```

---

### 4.4 Social & Values-Based Metrics from Narrative

**Metric 12: Values Clarity & Alignment Score**
```
Values_Clarity = (values_articulation × 0.4) + (decision_alignment_with_values × 0.6)

Values_Articulation: Does player express what they value? (generosity, security, growth, independence, etc.)
Decision_Alignment: Do game decisions align with articulated values?

Example:
  Player articulates: "I value helping others"
  Donations made: High and consistent
  Score: High (75+) - values-aligned

  Player articulates: "I value helping others"
  Donations made: Zero (all wealth accumulation)
  Score: Low (< 40) - values misaligned
```

**Metric 13: Prosocial Mindset Index** (from dialogue context)
```
Prosocial_Mindset = (donation_rationale_prosocial + peer_cooperation_signals + mutual_success_language) / 3

Donation_Rationale: Does player donate for values or just competition?
Peer_Cooperation: Does player root for others or only themselves?
Mutual_Success_Language: "We're all learning" vs. "I'm beating them"

High score (70+): Prosocial orientation
Low score (< 40): Competition-only orientation
```

---

## Part 5: Synthesis - Complete Analytics Dashboard

### Dashboard Variables by Category

```
PHYSICAL GAMEPLAY ANALYTICS:
├── Financial Performance
│   ├── Net Worth Index (%)
│   ├── Income Diversification Ratio (%)
│   ├── Expense Management Efficiency (%)
│   └── Business Profit Margin (%)
├── Strategic Decisions
│   ├── Risk Appetite Score (0-100)
│   ├── Debt Leverage Ratio (%)
│   └── Goal-Setting Ambition (%)
├── Behavioral Patterns
│   ├── Action Efficiency per Turn (%)
│   ├── Meal Order Success Rate (%)
│   └── Long-term Planning Indicator (%)
└── Flourishing Elements
    ├── Need Fulfillment Diversity Index (0-1)
    ├── Donation Consistency & Commitment (0-100)
    └── Happiness Points Composition (array)

NARRATIVE GAMEPLAY ANALYTICS:
├── Dialogue Engagement
│   ├── Narrative Engagement Index (0-100)
│   ├── Character Identity Coherence (0-100)
│   ├── Decision Rationale Sophistication (Level 1-4)
│   └── NPC Authenticity Perception (0-100)
├── Psychological & Learning
│   ├── Growth Mindset Score (0-100)
│   ├── Autonomy Experience Index (0-100)
│   ├── Transfer Thinking Evidence (frequency)
│   └── Emotional Resilience During Challenge (0-100)
├── Narrative Quality
│   ├── Narrative Immersion Level (0-100)
│   ├── Dialogue Depth & Quality (0-100)
│   └── Narrative Coherence Integration (0-100)
└── Social & Values
    ├── Values Clarity & Alignment Score (0-100)
    ├── Prosocial Mindset Index (0-100)
    └── Donation Philosophy (prosocial vs. competitive)
```

---

## Part 6: Implementation Guidance

### Data Collection Recommendations

**Physical Variables**: Auto-logged from game board state
- Use scoresheet digitization or photo capture
- Track coin movements in real-time (if using digital interface)
- Automated calculation of derived metrics

**Narrative Variables**: Observation + Recording
- Video record TTRPG dialogue sessions
- Transcribe key dialogue moments
- Code using qualitative analysis framework
- Cross-check between multiple observers

**Optimal Setup**: 
1. Physical game play with digital tracking (camera, scoresheet scanning)
2. Audio/video recording of dialogue
3. Post-game interview with structured questions
4. Thematic coding of dialogue + metrics calculation

---

## Conclusion

Cashflowpoly offers rich data for player performance analytics across:

✓ **13 physical gameplay variables** yielding **13 derived quantitative metrics**
✓ **5 narrative variable categories** yielding **13 derived qualitative metrics**
✓ **26 total metrics** enabling comprehensive evaluation of financial literacy + psychological flourishing

This framework enables evaluation systems to measure both:
- **What players achieve** (financial metrics from physical play)
- **How they think and feel** (narrative metrics from dialogue and reflection)
- **Why they choose what they choose** (integration of decision rationale with values)

The distinction between raw variables and derived metrics clarifies what data is observable and what must be calculated or inferred, enabling reproducible, systematic evaluation of Cashflowpoly's educational and developmental impacts.

---

## Appendix A: Scope Adaptation for This Repository

Bagian ini menyesuaikan dokumen metrik di atas agar selaras dengan scope implementasi di repository ini (web analytics + ruleset management + API + database), dan tidak mencampur area aplikasi IDN yang dikerjakan tim lain.

### A.1 Scope yang dipakai di project ini

1. Platform web untuk analitik dan manajemen ruleset.
2. API backend untuk ingest event, baca data, hitung agregasi metrik.
3. Database untuk event log, proyeksi arus kas, snapshot metrik, ruleset, sesi.
4. UI analitik sesi dan per pemain.
5. UI manajemen ruleset (create, list, detail, archive, delete, activate ke sesi).

### A.2 Scope yang tidak dipakai di project ini

1. Sistem narasi interaktif, dialog TTRPG, dan metrik naratif kualitatif.
2. Presentasi RPG/novel visual untuk gameplay IDN app.
3. Monitoring non-diegetik di aplikasi klien seluler.

Catatan: komponen ini tetap valid sebagai kebutuhan sistem besar, tetapi bukan target implementasi utama repository ini.

### A.3 Metrik yang menjadi acuan implementasi saat ini

#### A.3.1 Output API analitik sesi

Endpoint:
- `GET /api/v1/analytics/sessions/{sessionId}`

Field ringkasan sesi:
- `event_count`
- `cash_in_total`
- `cash_out_total`
- `cashflow_net_total`
- `rules_violations_count`

Field agregasi per pemain:
- `player_id`
- `cash_in_total`
- `cash_out_total`
- `donation_total`
- `gold_qty`
- `happiness_points_total`
- `need_points_total`
- `need_set_bonus_points`
- `donation_points_total`
- `gold_points_total`
- `pension_points_total`
- `saving_goal_points_total`
- `mission_penalty_total`
- `loan_penalty_total`
- `has_unpaid_loan`

#### A.3.2 Snapshot metric key yang disimpan backend

Session + player level:
- `cashflow.in.total`
- `cashflow.out.total`
- `cashflow.net.total`
- `donation.total`
- `happiness.points.total`
- `rules.violations.count`

Player level:
- `gold.qty.current`
- `orders.completed.count`
- `inventory.ingredient.total`
- `actions.used.total`
- `compliance.primary_need.rate`
- `happiness.need.points`
- `happiness.need.bonus`
- `happiness.donation.points`
- `happiness.gold.points`
- `happiness.pension.points`
- `happiness.saving_goal.points`
- `happiness.mission.penalty`
- `happiness.loan.penalty`
- `loan.unpaid.flag`
- `gameplay.raw.variables` (JSON)
- `gameplay.derived.metrics` (JSON)

Endpoint gameplay snapshot:
- `GET /api/v1/analytics/sessions/{sessionId}/players/{playerId}/gameplay`

Endpoint histori transaksi:
- `GET /api/v1/analytics/sessions/{sessionId}/transactions?playerId=...`

### A.4 Penyesuaian terhadap kebutuhan Anda (bagian 2)

#### A.4.1 UI analitik

Sudah sejalan:
1. Performa pembelajaran pemain individu: ada di detail pemain.
2. Performa pembelajaran agregat: ada di analitik sesi.
3. Performa misi pemain individu: tercermin di `mission_penalty_total`, `saving_goal_points_total`, `loan_penalty_total`.
4. Performa misi agregat: tersedia via agregasi session/by-player.

Perlu perluasan jika ingin eksplisit:
1. Pengelompokan data berdasarkan ruleset set secara eksplisit di UI (saat ini metrik sudah dihitung dalam konteks ruleset aktif sesi, tetapi tampilan grouped-by-ruleset khusus belum jadi halaman tersendiri).

#### A.4.2 UI manajemen aturan

Sudah sejalan:
1. Membuat ruleset baru.
2. Daftar ruleset.
3. Menghapus ruleset (dengan guard jika sudah dipakai sesi).
4. Mengatur ruleset aktif pada sesi.

Catatan domain:
1. Yang diubah adalah variabel konfigurasi ruleset, bukan aturan inti engine.

### A.5 Peta file implementasi (source of truth)

Backend:
- `src/Cashflowpoly.Api/Controllers/AnalyticsController.cs`
- `src/Cashflowpoly.Api/Controllers/RulesetsController.cs`
- `src/Cashflowpoly.Api/Controllers/SessionsController.cs`

Frontend UI:
- `src/Cashflowpoly.Ui/Views/Analytics/Index.cshtml`
- `src/Cashflowpoly.Ui/Views/Sessions/Details.cshtml`
- `src/Cashflowpoly.Ui/Views/Players/Details.cshtml`
- `src/Cashflowpoly.Ui/Views/Rulesets/Index.cshtml`
- `src/Cashflowpoly.Ui/Views/Rulesets/Details.cshtml`
- `src/Cashflowpoly.Ui/Views/Rulesets/Create.cshtml`

Dokumen metrik formal repository:
- `docs/02-Perancangan/02-02-definisi-metrik-dan-agregasi.md`

### A.6 Keputusan operasional untuk pengerjaan repository ini

Mulai sekarang, implementasi di repository ini memprioritaskan:
1. Metrik kuantitatif berbasis event gameplay Cashflowpoly.
2. Dashboard web analitik sesi dan pemain.
3. Manajemen ruleset berbasis API.

Sementara itu, metrik naratif/qualitative dari dokumen utama diperlakukan sebagai:
1. referensi lintas-tim, atau
2. backlog fase integrasi IDN app.
