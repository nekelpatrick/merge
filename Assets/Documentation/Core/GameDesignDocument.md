# SHIELD WALL - Game Design Document

**Version:** 0.2  
**Last Updated:** December 2024  
**Genre:** Turn-based Tactical Survival  
**Platform:** PC (Unity 3D)  
**Perspective:** First-Person  
**Developer:** Solo

---

## Table of Contents

1. [Game Overview](#1-game-overview)
2. [Solo Dev Reality Check](#2-solo-dev-reality-check)
3. [Core Gameplay Loop](#3-core-gameplay-loop)
4. [Rune Dice System](#4-rune-dice-system)
5. [Shield Wall System](#5-shield-wall-system)
6. [Combat System](#6-combat-system)
7. [Progression System](#7-progression-system)
8. [Visual Style](#8-visual-style)
9. [Audio Design](#9-audio-design)
10. [Technical Architecture](#10-technical-architecture)
11. [Development Roadmap](#11-development-roadmap)
12. [Content Scope](#12-content-scope)

---

## 1. Game Overview

### High Concept
You are a Viking warrior locked in a shield wall formation. Each turn, you roll runic dice to determine your actions - block attacks, strike enemies, or protect your shield brothers. As brothers fall, gaps open in your defense, and the wall crumbles. Survive the enemy waves. Hold the line.

### Core Fantasy
The claustrophobic, brutal intimacy of shield wall combat. You can't run. You can't flank. You can only push forward or die where you stand.

### Key Pillars
1. **Brotherhood** - Your shield brothers are your defense. Protect them or pay the price.
2. **Fate** - The dice represent the chaos of battle. Embrace uncertainty.
3. **Endurance** - Victory isn't about glory, it's about survival.
4. **Sacrifice** - Sometimes you must take a wound to save a brother.

### Target Experience
- Tense, sweaty-palms decision making
- Attachment to named shield brothers
- "Just one more turn" compulsion
- Satisfying dice combo discoveries
- Genuine dread as the wall crumbles

### Unique Selling Points
1. First-person shield wall perspective (novel viewpoint)
2. Dice-to-action combo system (satisfying discovery)
3. Living defense mechanic (brothers as resources AND characters)
4. Turn-based but visceral (strategic yet intense)

---

## 2. Solo Dev Reality Check

### What This Section Is For
Honest assessment of scope, time, and what actually matters. Reference this when feature creep tempts you.

### Time Budget (Realistic)

Assuming 15-20 hours/week of focused dev time:

| Milestone | Estimate | Cumulative |
|-----------|----------|------------|
| Prototype (core loop, no art) | 4-6 weeks | 6 weeks |
| Vertical Slice (1 battle, placeholder art) | 6-8 weeks | 14 weeks |
| Alpha (3 battles, real art, audio) | 8-12 weeks | 26 weeks |
| Beta (polish, balance, UI) | 4-6 weeks | 32 weeks |
| Release (bug fixes, store prep) | 2-4 weeks | 36 weeks |

**Total: 8-9 months** for a polished vertical slice / small release.

### Scope Tiers

**MUST HAVE (Core Loop):**
- Dice rolling + combo detection
- 4-5 basic actions (Block, Strike, Cover, Brace, Counter)
- Shield wall with 4 brothers
- 3 enemy types
- Stamina system
- Win/lose conditions
- Basic UI

**SHOULD HAVE (Vertical Slice):**
- 5 waves per battle
- All 6 rune types
- 10 actions
- 6 enemy types
- Brother auto-defense
- Wall integrity effects
- Sound effects

**NICE TO HAVE (Polish):**
- 3D dice physics
- Screen shake / juice
- Voice lines
- Music
- Progression between battles
- Multiple battle scenarios

**CUT FOR NOW (Full Game):**
- Camp scene
- Brother recruitment
- Dice upgrades
- Story/narrative
- Save system
- Multiple difficulty modes
- Achievements
- Localization

### Solo Dev Principles

1. **Playable > Pretty** - Greybox until mechanics are fun
2. **Steal Smart** - Use asset store, free sounds, AI art for prototypes
3. **Scope Knife** - When in doubt, cut it
4. **Weekly Builds** - Always have something runnable
5. **Test Early** - Get feedback before polishing
6. **Energy Management** - Work on hard problems when fresh, polish when tired
7. **Document Decisions** - Future you will forget why

### Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Dice system isn't fun | Medium | Critical | Prototype first, iterate fast |
| Art takes too long | High | Medium | Use asset store, stylized = forgiving |
| Combat feels flat | Medium | High | Focus on feedback/juice early |
| Burnout | High | Critical | Set sustainable pace, celebrate milestones |
| Feature creep | High | High | Reference this document constantly |

### Kill Triggers

Stop and reassess if:
- Prototype isn't fun after 2 weeks of iteration
- You haven't played the game yourself in a week
- A single feature takes 3x estimated time
- You're adding features not in MUST HAVE tier
- You dread working on the project

---

## 3. Core Gameplay Loop

```
┌─────────────────────────────────────────────────────────────────┐
│                         BATTLE LOOP                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│   ┌──────────────┐                                               │
│   │ ENEMY REVEAL │ ◄─────────────────────────────────┐           │
│   └──────┬───────┘                                   │           │
│          │ See incoming attacks                      │           │
│          ▼                                           │           │
│   ┌──────────────┐                                   │           │
│   │  DICE ROLL   │ ◄──── Re-roll (costs stamina)     │           │
│   └──────┬───────┘                                   │           │
│          │ Roll 4 rune dice                          │           │
│          ▼                                           │           │
│   ┌──────────────┐                                   │           │
│   │ DICE ASSIGN  │                                   │           │
│   └──────┬───────┘                                   │           │
│          │ Lock dice into combos                     │           │
│          ▼                                           │           │
│   ┌──────────────┐                                   │           │
│   │ACTION SELECT │                                   │           │
│   └──────┬───────┘                                   │           │
│          │ Choose actions from available combos      │           │
│          ▼                                           │           │
│   ┌──────────────┐                                   │           │
│   │   RESOLVE    │                                   │           │
│   └──────┬───────┘                                   │           │
│          │ Actions execute, brothers defend          │           │
│          ▼                                           │           │
│   ┌──────────────┐                                   │           │
│   │DAMAGE PHASE  │                                   │           │
│   └──────┬───────┘                                   │           │
│          │ Unblocked attacks wound                   │           │
│          ▼                                           │           │
│   ┌──────────────┐                                   │           │
│   │ STAMINA TICK │                                   │           │
│   └──────┬───────┘                                   │           │
│          │ -1 Stamina                                │           │
│          ▼                                           │           │
│   ┌──────────────┐     ┌─────────┐    ┌─────────┐   │           │
│   │  WAVE CHECK  │────►│ VICTORY │ OR │ DEFEAT  │   │           │
│   └──────┬───────┘     └─────────┘    └─────────┘   │           │
│          │ More enemies?                             │           │
│          └───────────────────────────────────────────┘           │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### Turn Breakdown

| Phase | Duration | Player Action | Dev Priority |
|-------|----------|---------------|--------------|
| Enemy Reveal | 2-3 sec | Observe threats | HIGH |
| Dice Roll | 1-2 sec | Watch dice | HIGH |
| Dice Assign | 10-30 sec | Lock dice | HIGH |
| Action Select | 5-15 sec | Confirm actions | HIGH |
| Resolve | 3-5 sec | Watch results | MEDIUM |
| Damage | 2-3 sec | See wounds | MEDIUM |
| Stamina | 1 sec | Track exhaustion | LOW (auto) |

**Average turn time:** 30-60 seconds

### Prototype Simplification

For the first prototype, combine phases:
1. **Show Enemies** (static display)
2. **Roll + Assign** (one interface)
3. **Resolve All** (instant resolution, no animation)
4. **Next Turn**

Add phases and polish incrementally.

---

## 4. Rune Dice System

### The Six Runes

| Rune | Symbol | Name | Domain | Color | Priority |
|------|--------|------|--------|-------|----------|
| ᚦ | Shield | THURS | Defense | Iron Gray | MUST |
| ᛏ | Axe | TYR | Attack | Blood Red | MUST |
| ᚷ | Spear | GEBO | Precision | Bronze | MUST |
| ᛒ | Tree | BERKANA | Support | Forest Green | MUST |
| ᛟ | Eye | OTHALA (Odin) | Wild | Gold | SHOULD |
| ᛚ | Wave | LAGUZ (Loki) | Chaos | Purple | NICE |

**Prototype:** Start with 4 runes (Shield, Axe, Spear, Brace). Add Odin/Loki later.

### Dice Pool

- **Starting dice:** 4 dice per turn
- **With full wall:** 5 dice (+1 bonus)
- **Alone:** 2 dice (minimum)

### Dice Mechanics

**Rolling:**
- All dice roll simultaneously
- For prototype: Instant random, no physics
- For polish: 3D dice with physics (or 2D animated)

**Re-rolling:**
- May re-roll any unlocked dice
- Each re-roll costs 1 Stamina
- Maximum 2 re-rolls per turn (simplified from 3)

**Locking:**
- Click dice to lock/unlock
- Locked dice form combos
- Unlocked dice are discarded at turn end

### Combo System

**Prototype Actions (5 total):**

| Combo | Dice | Action | Effect |
|-------|------|--------|--------|
| Block | ᚦᚦ | Shield Wall | Negate 1 attack on you |
| Strike | ᛏᚷ | Spear Thrust | Kill 1 enemy |
| Cover | ᚦᛒ | Protect Brother | Block attack on adjacent brother |
| Brace | ᛒᛒ | Steady | Reduce next wound to scratch |
| Counter | ᛏᚦ | Shield Bash | Block + kill attacker |

**Vertical Slice Actions (+5):**

| Combo | Dice | Action | Effect |
|-------|------|--------|--------|
| Testudo | ᚦᚦᚦ | Iron Defense | Block ALL attacks this turn |
| Berserker | ᛏᛏᛏ | Blood Rage | Kill 3 enemies, take 1 wound |
| Rally | ᛒᛒᛒ | War Cry | Heal 1 wound from each brother |
| Wall Push | Any 3 match | Advance | Stun all enemies 1 turn |
| Spear Wall | ᚷᚷᚷ | Reach | Kill 2 enemies |

**Cut for now:** 4-dice legendary combos, Odin wild card mechanics, Loki chaos mechanics.

### Implementation Notes

```
COMBO DETECTION (simplified):
1. Count occurrences of each rune in locked dice
2. Check against action requirements (unordered)
3. Greedy matching: largest combos first
4. Player can have multiple actions if dice allow
```

Example: Locked [ᚦ, ᚦ, ᛏ, ᚷ] = Block (ᚦᚦ) + Strike (ᛏᚷ) = 2 actions

---

## 5. Shield Wall System

### Formation Layout (Simplified)

```
         ENEMIES
            ▼
    ┌───┬───┬───┬───┬───┐
    │ B │ B │ P │ B │ B │
    │ 1 │ 2 │ L │ 3 │ 4 │
    │   │   │ A │   │   │
    │   │   │ Y │   │   │
    └───┴───┴───┴───┴───┘
     FAR LEFT    FAR RIGHT
     
    ADJACENT = positions next to player (B2, B3)
```

### Shield Brothers (4 Total for MVP)

| Name | Position | Health | Specialty | Prototype |
|------|----------|--------|-----------|-----------|
| Bjorn | Far Left | 3 | +block power | Just name + health |
| Erik | Left (adjacent) | 3 | Better auto-defend | Just name + health |
| Gunnar | Right (adjacent) | 3 | +strike power | Just name + health |
| Olaf | Far Right | 3 | Extra wound absorption | Just name + health |

**Prototype simplification:** All brothers identical. Specialties add later.

### Wall Integrity (Simplified)

| Brothers Alive | Dice | Notes |
|----------------|------|-------|
| 4 | 5 dice | Full wall bonus |
| 2-3 | 4 dice | Normal |
| 1 | 3 dice | Crumbling |
| 0 | 2 dice | Alone |

**Cut for prototype:** Morale system, gap mechanics, flanking damage.

### Auto-Defense (Simplified)

- Each brother has 50% chance to block attacks targeting them
- Player Cover action makes it 100%
- No morale modifiers for prototype

```csharp
// Prototype auto-defense
bool BrotherDefends() => Random.value < 0.5f;
```

---

## 6. Combat System

### Enemy Types (Tiered Implementation)

**Prototype (3 types):**

| Enemy | Health | Damage | Behavior |
|-------|--------|--------|----------|
| Thrall | 1 | 1 | Random target |
| Warrior | 1 | 1 | Targets lowest health |
| Spearman | 1 | 2 | Targets player |

**Vertical Slice (+3 types):**

| Enemy | Health | Damage | Behavior |
|-------|--------|--------|----------|
| Berserker | 2 | 2 | Always targets player |
| Archer | 1 | 1 | Ignores blocks (must kill) |
| Shield-Breaker | 1 | 1 | Destroys your block action |

**Cut for now:** Champion (boss), elite variants.

### Attack Resolution (Simplified)

```
RESOLUTION ORDER:
1. Player strikes kill enemies (prevent their attacks)
2. Player blocks negate remaining attacks
3. Brothers auto-defend (50% each)
4. Remaining attacks deal damage
```

**Cut for prototype:** Gap attacks, flanking, attack types.

### Damage & Health

**Player:**
- 5 Health
- 0 = Game Over
- No healing during battle

**Brothers:**
- 3 Health each
- 0 = Death (permanent this run)
- Rally action heals

**Stamina:**
- Starts at 12
- -1 per turn
- -1 per re-roll
- 0 = Game Over
- +2 when wave cleared

### Wave Structure (Prototype)

**3 waves for prototype:**

| Wave | Enemies | Composition |
|------|---------|-------------|
| 1 | 3 | 3 Thralls |
| 2 | 4 | 2 Thralls, 2 Warriors |
| 3 | 5 | 2 Warriors, 2 Spearmen, 1 Thrall |

**5 waves for vertical slice:**

| Wave | Enemies | Composition |
|------|---------|-------------|
| 1 | 3 | 3 Thralls (tutorial) |
| 2 | 4 | 2 Thralls, 2 Warriors |
| 3 | 5 | 2 Warriors, 2 Spearmen, 1 Berserker |
| 4 | 6 | 3 Warriors, 2 Archers, 1 Shield-Breaker |
| 5 | 5 | 2 Berserkers, 2 Spearmen, 1 Shield-Breaker |

---

## 7. Progression System

### MVP: No Progression

For vertical slice, each run is standalone:
- Fixed starting brothers
- Fixed dice
- Fixed stamina
- Win or lose, restart

### Future: Between Battles (Post-MVP)

**Glory earned:**
- +10 per wave survived
- +1 per enemy killed
- +5 if all brothers survive wave

**Glory spent:**
- Heal brother wound: 1
- Recruit new brother: 10
- Upgrade die: 5

**Cut entirely for MVP.** Add only if core loop is solid.

---

## 8. Visual Style

### Art Direction

**Aesthetic:** Stylized with painted textures

**Reference:** Banner Saga (characters) + Darkest Dungeon (mood)

**Why stylized:**
- More forgiving of solo dev art skills
- Faster to produce
- Ages better than realistic
- Strong visual identity

### Solo Dev Art Strategy

**Phase 1 - Prototype:**
- Unity primitives (cubes, capsules)
- Solid colors for rune types
- UI: Unity default + TextMeshPro
- No textures, no models

**Phase 2 - Vertical Slice:**
- Simple low-poly models (buy or make)
- Hand-painted textures (or AI-assisted)
- Custom UI sprites
- Placeholder VFX

**Phase 3 - Polish:**
- Refined character models
- Screen effects (vignette, blood overlay)
- Particle effects
- Shader polish

### Asset Sources (Recommended)

| Asset Type | Source | Budget |
|------------|--------|--------|
| Character models | Synty, Polygon | $20-50 |
| Environment | Unity Asset Store | $20-40 |
| UI Kit | GUI PRO, Modern UI | $15-30 |
| Sound Effects | Sonniss GDC bundles (free) | $0 |
| Music | Epidemic Sound or commission | $15/mo or $100 |
| Fonts | Google Fonts (free) | $0 |

**Total art budget estimate:** $100-200 for MVP

### First-Person View (Simplified)

```
┌────────────────────────────────────────────────────────────┐
│                        [SIMPLE SKY]                         │
│                                                             │
│        [ENEMY]   [ENEMY]   [ENEMY]   [ENEMY]               │
│                                                             │
│   [BROTHER]                              [BROTHER]          │
│                                                             │
│              ┌─────────────────────┐                        │
│              │   PLAYER SHIELD     │                        │
│              │   (simple quad)     │                        │
│              └─────────────────────┘                        │
└────────────────────────────────────────────────────────────┘

For prototype: 2D sprites in 3D space are fine.
Parallax layers can fake depth cheaply.
```

### UI Layout

```
┌────────────────────────────────────────────────────────────┐
│ WAVE 2/5                              STAMINA: ████████░░  │
│                                                             │
│                    [BATTLE VIEW]                            │
│                                                             │
├────────────────────────────────────────────────────────────┤
│ BJORN ♥♥♥   ERIK ♥♥░   YOU ♥♥♥♥♥   GUNNAR ♥♥♥   OLAF ♥♥░ │
├────────────────────────────────────────────────────────────┤
│                                                             │
│   YOUR DICE:  [ᚦ] [ᚦ] [ᛏ] [ᚷ]      [REROLL] [END TURN]   │
│                                                             │
│   ACTIONS:  [BLOCK ᚦᚦ]  [STRIKE ᛏᚷ]                       │
│                                                             │
└────────────────────────────────────────────────────────────┘
```

---

## 9. Audio Design

### Solo Dev Audio Strategy

**Phase 1 - Prototype:**
- No audio (focus on mechanics)
- Maybe 1-2 placeholder sounds for feedback

**Phase 2 - Vertical Slice:**
- Essential SFX only (dice, hit, block, death)
- No music yet
- Free sound libraries

**Phase 3 - Polish:**
- Full SFX pass
- Background music (loop)
- Maybe 1-2 voice lines

### Essential Sounds (Priority Order)

1. Dice roll / land
2. Enemy attack impact
3. Block success
4. Damage taken
5. Enemy killed
6. Brother death
7. Victory / defeat stings

### Sound Sources (Free/Cheap)

- **Sonniss GDC Bundle** - Free, high quality
- **Freesound.org** - Free, variable quality
- **Zapsplat** - Free with attribution
- **Epidemic Sound** - $15/mo, great quality

### Music (Minimal)

One looping battle track:
- War drums
- Low tension
- 2-3 minute loop
- Dynamic layers if time allows

**Option:** Commission a single track ($50-150) or use royalty-free Viking music.

---

## 10. Technical Architecture

### Project Structure

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs       # Game state, singleton
│   │   └── TurnManager.cs       # Turn phase state machine
│   │
│   ├── Dice/
│   │   ├── RuneType.cs          # Enum
│   │   ├── RuneDie.cs           # Single die
│   │   ├── DicePool.cs          # Collection + rolling
│   │   └── ComboResolver.cs     # Detect valid combos
│   │
│   ├── ShieldWall/
│   │   ├── ShieldWallManager.cs # Positions, integrity
│   │   ├── ShieldBrother.cs     # Brother state
│   │   └── PlayerWarrior.cs     # Player state
│   │
│   ├── Combat/
│   │   ├── EnemyWave.cs         # Wave spawning
│   │   ├── Enemy.cs             # Enemy state
│   │   └── CombatResolver.cs    # Resolve attacks
│   │
│   ├── UI/
│   │   ├── DiceUI.cs            # Dice display + interaction
│   │   ├── ActionUI.cs          # Action buttons
│   │   ├── WallStatusUI.cs      # Health displays
│   │   └── GameUI.cs            # Wave, stamina, etc.
│   │
│   └── Data/
│       ├── RuneSO.cs
│       ├── BrotherSO.cs
│       ├── EnemySO.cs
│       ├── ActionSO.cs
│       └── WaveConfigSO.cs
│
├── ScriptableObjects/
│   ├── Runes/
│   ├── Brothers/
│   ├── Enemies/
│   └── Actions/
│
├── Prefabs/
│   ├── UI/
│   └── Characters/
│
├── Art/
├── Audio/
│
├── Scenes/
│   └── Battle.unity
│
└── Documentation/
    └── GameDesignDocument.md
```

### Key Scripts (Simplified)

**TurnManager.cs** - Heart of the game:
```
States: 
  WaveStart → PlayerTurn → Resolution → WaveEnd → (loop or end)

Simplified from 10 states to 4 for prototype.
Add granularity as needed.
```

**ComboResolver.cs** - Core mechanic:
```
Input: List<RuneType> lockedDice
Output: List<ActionSO> availableActions

Simple matching algorithm. No complex graph theory needed.
```

**CombatResolver.cs** - Turn resolution:
```
Input: Player actions, enemy attacks, brother states
Output: Updated health values, dead enemies/brothers

Execute in order: strikes → blocks → auto-defense → damage
```

### Data-Driven Design

All game data in ScriptableObjects:
- Easy to tweak in editor
- No code changes for balance
- Designers can work without coding (future)

```csharp
[CreateAssetMenu]
public class ActionSO : ScriptableObject
{
    public string actionName;
    public RuneType[] requiredRunes;  // Unordered
    public ActionEffect effect;       // Enum: Block, Strike, Cover, etc.
    public int power;                 // 1 = kill 1, 2 = kill 2, etc.
    public Sprite icon;
}
```

---

## 11. Development Roadmap

### Phase 0: Setup (1 week)

- [ ] Create Unity project
- [ ] Set up folder structure
- [ ] Create placeholder ScriptableObjects
- [ ] Basic scene with camera

### Phase 1: Core Loop Prototype (4 weeks)

**Week 1: Dice System**
- [ ] RuneType enum
- [ ] RuneDie class (random roll)
- [ ] DicePool (4 dice)
- [ ] Basic dice UI (clickable buttons)

**Week 2: Combo System**
- [ ] ActionSO definitions (5 actions)
- [ ] ComboResolver (detect matches)
- [ ] ActionUI (show available actions)

**Week 3: Combat Foundation**
- [ ] EnemySO definitions (3 types)
- [ ] Enemy spawning (3 per wave)
- [ ] CombatResolver (basic resolution)
- [ ] Health tracking

**Week 4: Shield Wall + Polish**
- [ ] ShieldBrother class
- [ ] Wall status UI
- [ ] Turn flow (wave → dice → resolve)
- [ ] Win/lose conditions

**Milestone:** Playable prototype, all greybox

### Phase 2: Vertical Slice (6 weeks)

**Week 5-6: Complete Mechanics**
- [ ] All 10 actions implemented
- [ ] All 6 enemy types
- [ ] 5 wave battle
- [ ] Stamina system
- [ ] Re-roll mechanic

**Week 7-8: Visual Pass**
- [ ] Replace primitives with simple models
- [ ] Basic textures
- [ ] UI art pass
- [ ] First-person view setup

**Week 9-10: Audio + Polish**
- [ ] Core sound effects
- [ ] Screen feedback (shake, flash)
- [ ] Damage numbers
- [ ] Victory/defeat screens

**Milestone:** Shareable vertical slice

### Phase 3: Content + Polish (6 weeks)

**Week 11-12: Content**
- [ ] 3 different battle scenarios
- [ ] Enemy variety tuning
- [ ] Balance pass

**Week 13-14: Polish**
- [ ] VFX pass
- [ ] Music
- [ ] Tutorial hints
- [ ] Bug fixing

**Week 15-16: Release Prep**
- [ ] Final balance
- [ ] Steam page / itch.io
- [ ] Trailer capture
- [ ] Launch

---

## 12. Content Scope

### Minimum Viable Game

**Absolutely Required:**
- 1 battle (5 waves)
- 4 brothers (identical stats OK)
- 4 rune types
- 5 actions
- 3 enemy types
- Stamina system
- Win/lose

**This is shippable** as a free prototype or game jam entry.

### Vertical Slice (Target)

**For itch.io / demo:**
- 1 battle (5 waves)
- 4 brothers with personalities
- 6 rune types
- 10 actions
- 6 enemy types
- Audio + visual polish
- Title screen

### Full Game (Future)

**Only if vertical slice succeeds:**
- 5+ battles (campaign)
- Brother recruitment
- Dice upgrades
- Story elements
- Save system
- Multiple difficulties
- Endless mode

---

## Appendix A: Balancing Notes

### Starting Values (Tweak These)

| Parameter | Value | Notes |
|-----------|-------|-------|
| Player health | 5 | 4-6 range |
| Brother health | 3 | 2-4 range |
| Starting stamina | 12 | 10-15 range |
| Stamina per turn | -1 | Fixed |
| Stamina per reroll | -1 | Fixed |
| Stamina on wave clear | +2 | 1-3 range |
| Base dice count | 4 | Fixed |
| Full wall bonus | +1 die | Fixed |
| Alone penalty | -2 dice | Fixed |
| Auto-defend chance | 50% | 40-60% range |

### Balance Levers

When the game is too easy:
1. Add more enemies
2. Reduce starting stamina
3. Lower auto-defend chance
4. Add tougher enemy types

When the game is too hard:
1. Reduce enemies
2. Increase starting stamina
3. Add another starting die
4. Buff action effects

### Playtest Checklist

- [ ] Can a new player understand the core loop in 1 battle?
- [ ] Do players feel tension but not frustration?
- [ ] Do brothers feel valuable?
- [ ] Are dice combos discoverable and satisfying?
- [ ] Is stamina pressure real but fair?
- [ ] Does losing feel like player mistake, not RNG?

---

## Appendix B: Reference Games

Study these:
- **Dicey Dungeons** - Dice-as-actions, simple but deep
- **Slay the Spire** - Combo building, roguelike structure
- **Darkest Dungeon** - Party management, oppressive tone
- **For Honor** - Shield combat feel (for animations)
- **Banner Saga** - Art style, Viking theme, tough choices

---

## Appendix C: Quick Reference

### Runes
ᚦ Shield | ᛏ Axe | ᚷ Spear | ᛒ Brace | ᛟ Odin | ᛚ Loki

### Core Actions
- Block (ᚦᚦ): Negate attack
- Strike (ᛏᚷ): Kill enemy
- Cover (ᚦᛒ): Protect brother
- Brace (ᛒᛒ): Reduce damage
- Counter (ᛏᚦ): Block + kill

### Turn Order
1. Enemies reveal
2. Roll dice
3. Lock dice
4. Choose actions
5. Resolve
6. Stamina tick
7. Wave check

### Victory: Survive all waves
### Defeat: 0 health OR 0 stamina

---

*End of Document*

**Remember: Ship something small and finished, not something ambitious and abandoned.**
