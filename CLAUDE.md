# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**"The Little Monster's Emotion Maze"** - An interactive storybook Unity 2D game designed to help children aged 5-8 learn emotional management through metaphorical maze exploration.

The game features a character named Moro who experiences four core emotions (Anger, Sadness, Fear, Joy), each represented as an explorable maze with unique mechanics based on evidence-based emotional regulation strategies from child psychology.

## Essential Documentation

- **LittleMonster.md**: Complete design document with functional specifications, UI mockups, storyboards, and interaction patterns
- **TODO.md**: Comprehensive development plan organized in 10 phases
- **LMfig/**: Visual design references and concept art

**Read these documents thoroughly before making any design decisions.**

## Unity Version & Environment

- **Unity Version**: 6000.3.0b4 (Unity 6 Beta)
- **Platform**: Linux (developed on Arch Linux)
- **Project Type**: 2D
- **Target Aspect Ratio**: 16:9 (1920x1080px)

## Architecture & Design Principles

### Core Game Structure

The game follows a **hub-and-spoke navigation pattern**:

```
Home (Emotion Town)
    ├─> Anger Maze → Credits → [Home/Replay]
    ├─> Sadness Maze → Credits → [Home/Replay]
    ├─> Fear Maze → Credits → [Home/Replay]
    └─> Joy Maze → Credits → [Home/Replay]
```

### Key Frame IDs (Scene References)

- `Home` - Central hub with Moro and 4 emotion portals
- `Maze_Anger` - Fiery labyrinth with cooling mechanics
- `Maze_Sadness` - Grey garden with color restoration
- `Maze_Fear` - Shadow forest with lantern exploration
- `Maze_Joy` - Chaotic rainbow spire with rhythm harmonization
- `Credits` - Success celebration page

### Emotion-Driven Color System

Colors are the primary narrative tool. Moro's appearance changes based on emotional state:

| Emotion      | Color Codes           | Visual Treatment        |
| ------------ | --------------------- | ----------------------- |
| Calm/Default | `#80CBC4` (Soft Teal) | Neutral, friendly       |
| Anger        | `#FF5252` → `#D32F2F` | Red gradient, inflated  |
| Sadness      | `#90A4AE` → `#546E7A` | Blue-grey, shrunk       |
| Fear         | `#3949AB` → `#5E35B1` | Indigo-purple, shadowed |
| Joy          | `#FFEB3B` → `#EC407A` | Yellow-pink rainbow     |

**Base/Neutral Colors**: `#E6D7C1` (Soft Clay), `#FFF9EC` (Cream), `#5D4037` (Dark Brown)

### Layout System (All Scenes)

```
┌─────────────────────────────────┐
│ Top 5%: Home Button (fixed)     │ ← Cloud-shaped house icon
├─────────────────────────────────┤
│                                  │
│                                  │
│     Central 90%:                 │
│     Full-screen immersive        │
│     content area                 │
│                                  │
│                                  │
├─────────────────────────────────┤
│ Bottom 5%: Progress Indicator   │ ← 4 dots for 4 mazes
└─────────────────────────────────┘
```

### Critical Interaction Mechanics

Each maze teaches emotional regulation through specific interactions:

1. **Anger Maze**: Circular trace gestures guide cooling air + long-press (3s) on pulsating core
2. **Sadness Maze**: Tap grey objects to restore color (cognitive reframing)
3. **Fear Maze**: Drag lantern to reveal friendly shapes (facing fears mindfully)
4. **Joy Maze**: Rhythmic tapping to capture orbs (finding focus in overstimulation)

## C# Script Organization

When creating scripts, follow Unity best practices:

### Naming Conventions

- **MonoBehaviours**: PascalCase (e.g., `MoroController.cs`, `AngerMazeManager.cs`)
- **Managers**: Suffix with "Manager" or "Controller"
- **UI Components**: Suffix with "View" or "UI" (e.g., `EmotionPortalUI.cs`)

### Recommended Script Structure

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs          # Central state machine
│   │   ├── NavigationManager.cs    # Scene transitions
│   │   └── ColorPalette.cs         # Centralized color definitions
│   ├── Character/
│   │   └── MoroController.cs       # Moro's appearance/animation
│   ├── UI/
│   │   ├── ProgressIndicator.cs    # 4-dot navigation
│   │   ├── EmotionPortalUI.cs      # Portal buttons with pulse
│   │   └── HomeButtonUI.cs         # Cloud-shaped home button
│   ├── Mazes/
│   │   ├── AngerMaze/
│   │   │   ├── AngerMazeManager.cs
│   │   │   ├── AirCurrentController.cs
│   │   │   └── AngerCoreController.cs
│   │   ├── SadnessMaze/
│   │   │   ├── SadnessMazeManager.cs
│   │   │   └── ColorableObject.cs
│   │   ├── FearMaze/
│   │   │   ├── FearMazeManager.cs
│   │   │   ├── LanternController.cs
│   │   │   └── ShadowReveal.cs
│   │   └── JoyMaze/
│   │       ├── JoyMazeManager.cs
│   │       └── OrbController.cs
│   └── Input/
│       ├── CircleTraceDetector.cs  # Detects circular gestures
│       ├── LongPressDetector.cs    # 3-second press detection
│       └── RhythmicTapDetector.cs  # Timing-based taps
```

## Development Workflow

1. **Always reference TODO.md** for the current phase and task breakdown
2. **Maintain the 16:9 aspect ratio** across all scenes

## Current Project State

The project is in **initial setup phase**. A basic `PlayerController.cs` exists from template but should be replaced with emotion-specific game mechanics. The actual game implementation is yet to begin—follow TODO.md Phase 1 to start.
