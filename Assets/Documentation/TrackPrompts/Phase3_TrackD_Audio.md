# Phase 3 - Track D: Audio Integration

## Assignment

You are implementing **Track D** of Phase 3: Content + Polish.

Your focus is creating the audio system and integrating sound effects with game events.

---

## Your Scope

### Files to CREATE

| File | Purpose |
|------|---------|
| `Assets/Scripts/Audio/AudioManager.cs` | Central audio controller |
| `Assets/Scripts/Audio/SFXPlayer.cs` | Simple SFX helper |
| `Assets/Scripts/Audio/MusicPlayer.cs` | Music loop with crossfade |

### Folders to CREATE

```
Assets/Audio/
├── SFX/
│   ├── Dice/       → dice_roll.wav, dice_lock.wav
│   ├── Combat/     → hit.wav, block.wav, kill.wav
│   └── UI/         → button_click.wav, turn_start.wav
├── Music/          → battle_loop.ogg, menu.ogg
└── Ambient/        → wind.ogg
```

### Assets to CREATE

| Asset | Purpose |
|-------|---------|
| `Assets/Audio/Mixers/MasterMixer.mixer` | Audio mixer with groups |

---

## DO NOT TOUCH

- `Assets/Scripts/Core/*` — Core systems
- `Assets/Scripts/Combat/*` — Combat logic
- `Assets/Scripts/UI/*` — UI components
- `Assets/Scripts/Visual/*` — Visual systems

---

## Implementation Details

### D1: Folder Structure

Create the folder hierarchy. Placeholder audio files can be empty `.wav` files or simple tones for testing.

**Recommended Free Audio Sources:**
- Freesound.org (CC0)
- Sonniss GDC Audio Bundle
- ZapSplat (free tier)
- Kenney.nl game assets

### D2: Audio Mixer

Create `MasterMixer.mixer` with groups:
- Master (output)
  - Music (volume slider)
  - SFX (volume slider)
  - Ambient (volume slider)

**Exposed Parameters:**
- `MusicVolume` (-80 to 0 dB)
- `SFXVolume` (-80 to 0 dB)
- `AmbientVolume` (-80 to 0 dB)

### D3: AudioManager

```csharp
using UnityEngine;
using UnityEngine.Audio;
using ShieldWall.Core;

namespace ShieldWall.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("Mixer")]
        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private AudioMixerGroup _sfxGroup;
        
        [Header("SFX Clips")]
        [SerializeField] private AudioClip _diceRoll;
        [SerializeField] private AudioClip _diceLock;
        [SerializeField] private AudioClip _hit;
        [SerializeField] private AudioClip _block;
        [SerializeField] private AudioClip _kill;
        [SerializeField] private AudioClip _buttonClick;
        [SerializeField] private AudioClip _turnStart;
        
        [Header("Music")]
        [SerializeField] private AudioClip _battleMusic;
        [SerializeField] private AudioClip _menuMusic;
        
        private AudioSource _sfxSource;
        private AudioSource _musicSource;
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            SetupAudioSources();
        }
        
        void OnEnable()
        {
            GameEvents.OnDiceRolled += _ => PlaySFX(_diceRoll);
            GameEvents.OnDieLockToggled += (_, __) => PlaySFX(_diceLock);
            GameEvents.OnEnemyKilled += _ => PlaySFX(_kill);
            GameEvents.OnAttackBlocked += _ => PlaySFX(_block);
            GameEvents.OnAttackLanded += _ => PlaySFX(_hit);
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
        }
        
        void OnDisable()
        {
            GameEvents.OnDiceRolled -= _ => PlaySFX(_diceRoll);
            GameEvents.OnDieLockToggled -= (_, __) => PlaySFX(_diceLock);
            GameEvents.OnEnemyKilled -= _ => PlaySFX(_kill);
            GameEvents.OnAttackBlocked -= _ => PlaySFX(_block);
            GameEvents.OnAttackLanded -= _ => PlaySFX(_hit);
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
        }
        
        private void SetupAudioSources()
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.outputAudioMixerGroup = _sfxGroup;
            _sfxSource.playOnAwake = false;
            
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.outputAudioMixerGroup = _musicGroup;
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
        }
        
        public void PlaySFX(AudioClip clip)
        {
            if (clip != null)
                _sfxSource.PlayOneShot(clip);
        }
        
        public void PlayMusic(AudioClip clip)
        {
            if (_musicSource.clip == clip && _musicSource.isPlaying)
                return;
                
            _musicSource.clip = clip;
            _musicSource.Play();
        }
        
        public void StopMusic()
        {
            _musicSource.Stop();
        }
        
        public void PlayBattleMusic() => PlayMusic(_battleMusic);
        public void PlayMenuMusic() => PlayMusic(_menuMusic);
        
        private void HandlePhaseChanged(TurnPhase phase)
        {
            if (phase == TurnPhase.DiceRoll)
                PlaySFX(_turnStart);
        }
        
        public void PlayButtonClick() => PlaySFX(_buttonClick);
    }
}
```

### D4: SFX Specifications

**Dice SFX:**
| Clip | Duration | Description |
|------|----------|-------------|
| dice_roll.wav | 0.5-1s | Multiple dice rattling |
| dice_lock.wav | 0.2s | Click/clunk sound |

**Combat SFX:**
| Clip | Duration | Description |
|------|----------|-------------|
| hit.wav | 0.3s | Impact, flesh/armor |
| block.wav | 0.3s | Metal clang, shield impact |
| kill.wav | 0.5s | Death grunt + impact |

**UI SFX:**
| Clip | Duration | Description |
|------|----------|-------------|
| button_click.wav | 0.1s | Soft click |
| turn_start.wav | 0.3s | Whoosh or horn |

**Music:**
| Clip | Duration | Description |
|------|----------|-------------|
| battle_loop.ogg | 60-120s | Tense, rhythmic, loopable |
| menu.ogg | 60s | Ambient, Nordic atmosphere |

### D5: Volume Defaults

```csharp
// Suggested default volumes (0-1 scale)
const float DEFAULT_MUSIC_VOLUME = 0.5f;
const float DEFAULT_SFX_VOLUME = 0.8f;
const float DEFAULT_AMBIENT_VOLUME = 0.3f;
```

---

## Integration Notes

The AudioManager should be added to a persistent game object (survives scene loads).

**Prefab:** Create `Assets/Prefabs/Systems/AudioManager.prefab`
- Empty GameObject named "AudioManager"
- Attach `AudioManager.cs`
- Assign audio clips and mixer groups

---

## Success Criteria

- [ ] Audio folder structure created
- [ ] AudioMixer with 3 groups exists
- [ ] AudioManager compiles without errors
- [ ] AudioManager is a singleton
- [ ] Dice roll plays sound on roll
- [ ] Kill plays sound on enemy death
- [ ] Block plays sound on block
- [ ] Hit plays sound on damage
- [ ] Battle music loops
- [ ] No audio errors in console

---

## Test Steps

1. Add AudioManager prefab to Battle scene
2. Assign placeholder audio clips (can be simple beeps)
3. Press Play
4. Roll dice → hear dice sound
5. Kill enemy → hear kill sound
6. Get hit → hear hit sound
7. Verify music is looping
8. Check no missing AudioClip warnings

---

## Reference Files

- `Assets/Scripts/Core/GameEvents.cs` — Event definitions
- `Assets/Scripts/Core/TurnPhase.cs` — Turn phases

---

## Audio Resources

Free audio sources (check licenses):
- https://freesound.org/ (search: dice, sword, shield)
- https://sonniss.com/gameaudiogdc (annual free bundle)
- https://www.zapsplat.com/
- https://kenney.nl/assets?q=audio

