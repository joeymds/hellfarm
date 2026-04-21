# Spawn Progression Analysis

This document analyzes the current state of spawn progression over the 5-minute (300s) arena run and details clear tuning recommendations for improving the difficulty curve.

## Part 1: Current Baseline

### Arena Timing & Difficulty
- **Run Duration**: 5 minutes (300 seconds).
- **Difficulty Scaling**: Increases by 1 every 5 seconds.
- **Maximum Difficulty**: 59 (at 295s).

### Spawn Rate Formula
- **Current Formula**: `baseTime - min((0.1/12) * difficulty, 0.7)`
- **Analysis**:
  - The spawn interval starts at `baseTime` (presumably 1.0s or so).
  - Every level decreases the spawn delay by `0.1 / 12 ≈ 0.0083s`.
  - It caps out at `-0.7s` reduction. 
  - To reach the cap of `0.7`, difficulty needs to be `(0.7 / 0.1) * 12 = 84`. However, since max difficulty is `59`, the cap is actually *never reached*. The max reduction achieved is `(0.1/12) * 59 ≈ 0.49s`.
  - **Result**: Spawn rates decrease linearly throughout the *entire* 5-minute run at a very slow pace, leading to a flat feeling of increased density rather than sharp tactical changes.

### Enemy Unlocks & Composition
- **Start (Diff 0, 0s)**: Sheep (weight 10), Chicken (weight 15)
- **Diff 6 (30s)**: Pig introduced (weight 20)
- **Diff 8 (40s)**: Chicken weight increased by +10 (total 25)
- **Enemy Stats**:
  - Sheep (Basic): 10 HP, 40 speed
  - Chicken (Swarm): 5 HP, 65 speed
  - Pig (Tank): 15 HP, 50 speed
- **Analysis**: By 40 seconds, *all* current enemy introductions and weight changes are finished. The remaining 4 minutes 20 seconds have zero composition changes.

### Burst Waves
- Base: 17s interval
- Diff 4 (20s): 14s interval
- Diff 8 (40s): 11s interval
- Diff 12 (60s): 8s interval
- **Analysis**: Burst interval hits its absolute minimum at just 60 seconds into a 300-second run, front-loading the tension heavily.

### Elite Spawns
- **Base**: 5%
- **Scaling**: +1% per difficulty level starting at Diff 6 (30s).
- **Cap**: 25% at Diff 30 (150s, the exact midpoint of the run).
- **Analysis**: Elites cap out at the midpoint, leaving the second half of the run with no additional elite pressure, relying solely on linear spawn rate drops.

---

## Part 2: Tuned Progression Recommendations

### Design Goals
- **Early Game (0 - 60s)**: Learning phase, low pressure. Basic mechanics.
- **Mid-Game (60 - 150s)**: Clear pressure increase, shifting enemy variety.
- **Late Game (150 - 300s)**: Intense but playable, sustained challenge with high stakes.

### 1. Enemy Unlock Redistribution
To maintain interest across the run, changes in composition should be spread out.
- **Start (0s, Diff 0)**: Sheep (Weight 10), Chicken (Weight 10). (Slightly less starting swarms).
- **Mid-Game Push (60s, Diff 12)**: Pig introduced (Weight 20). Delays the beefier enemy so the first minute is strictly learning swarms.
- **Swarm Escalation (120s, Diff 24)**: Chicken weight +10 (Total 20). Increases the fast-moving threat as base spawn rates drop.
- **Late-Game Shift (180s, Diff 36)**: Sheep weight +15 (Total 25), Pig weight +10 (Total 30). Shifts the balance toward harder-to-kill enemies the longer you survive.

### 2. Burst Wave Tiers
Instead of capping at 60s, burst waves should escalate all the way into the late game.
- **Base (0s, Diff 0)**: 20s interval.
- **Diff 10 (50s)**: 16s interval.
- **Diff 20 (100s)**: 12s interval.
- **Diff 30 (150s - Midpoint)**: 9s interval.
- **Diff 45 (225s)**: 6s interval. (Creates intense, punishing waves for the final minute).

### 3. Maximum Active Enemy Count
Currently, enemies likely stack infinitely if not killed, leading to engine lag or guaranteed, un-dodgable death.
- **Implement a Hard Cap**: Start at a max of 30 enemies on screen.
- **Scaling Cap**: Increase the cap by `1` every `2` difficulties (every 10s).
- **Maximum Cap**: Cap it at `60` total enemies. This ensures the player is heavily pressured but not physically walled off by hitboxes. If the cap is reached, new spawns are skipped until enemies are cleared.

### 4. Spawn Rate Formula Adjustments
- **Revised Formula**: Increase the scaling severity but implement a hard floor.
  `baseTime - (0.015 * difficulty)`
- **Floor Limit**: Ensure spawn interval never goes below `0.3s`.
  `Math.Max(baseTime - (0.015 * difficulty), 0.3)`
- **Reasoning**: This gives a more noticeable speedup per level (`0.015s` vs `0.008s`), making difficulty jumps more impactful, while strictly preventing negative or instantaneous spawn rates.

### 5. Elite Spawn Tuning
- **Delay Elites**: Don't spawn any elites until Diff 10 (50s). The player needs time to scale damage.
- **Base**: Starts at 0%, jumps to 5% at Diff 10.
- **Slower Scaling, Higher Cap**: +0.5% per difficulty level. This means by Diff 50 (250s), Elite chance will be `5% + (40 * 0.5) = 25%`. 
- **Late Game Surge**: At Diff 55 (275s - the final 25 seconds of the run), jump the Elite chance to `35%` as a final test of the player's build.

---

## Expected Pacing Arc

1. **0:00 - 1:00 (The Field)**: Slower spawns with Sheep and Chickens. Burst waves are far apart. Elites start appearing right at the end.
2. **1:00 - 2:30 (The Swarm)**: Pigs are introduced, requiring more focused damage. Burst waves become frequent enough that players can't just run away; they have to clear them.
3. **2:30 - 4:00 (The Pressure)**: Enemy density reaches its maximum scaling. Faster spawn rates combine with higher weights for Pigs and Sheep, meaning enemies take longer to kill while spawning faster.
4. **4:00 - 5:00 (The Harvest)**: Burst waves hit every 6 seconds. Elite swarms rapidly drain health if the player's damage output isn't optimized. It becomes a true test of their upgrade choices.
