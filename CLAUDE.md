# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**"The Little Monster's Emotion Maze"** - An interactive storybook Unity 2D game designed to help children aged 5-8 learn emotional management through metaphorical maze exploration.

The game features a character named Moro who experiences four core emotions (Anger, Sadness, Fear, Joy), each represented as an explorable maze with unique mechanics based on evidence-based emotional regulation strategies from child psychology.

## Essential Documentation

- **LittleMonster.md**: Complete design document with functional specifications, UI mockups, storyboards, and interaction patterns
- **TODO.md**: Comprehensive development plan organized in 10 phases
- **LMfig/**: Visual design references and concept art

## Architecture & Design Principles

### Core Game Structure

The game follows a **simplified hub-and-spoke navigation pattern**:

```
Home (Emotion Town)
    ├─> Anger Maze → Auto-return to Home (2s delay)
    ├─> Sadness Maze → Auto-return to Home (2s delay)
    ├─> Fear Maze → Auto-return to Home (2s delay)
    └─> Joy Maze → Auto-return to Home (2s delay)

Credits (Optional) - Can be triggered after all 4 mazes completed
```

### Key Frame IDs (Scene References)

- `Home` - Central hub with Moro and 4 emotion portals
- `Maze_Anger` - Fiery background with "Take Deep Breaths" button interaction
- `Maze_Sadness` - Grey-blue garden with thought bubble color restoration
- `Maze_Fear` - Dark indigo forest with "Light" button shadow reveals
- `Maze_Joy` - Rainbow backdrop with "Calm Down" button to slow orbs
- `Credits` - Optional celebration page (triggered after all 4 mazes completed)

### Emotion-Driven Color System

Colors are the primary narrative tool. Moro is integrated into background images, displaying different emotional states:

| Emotion      | Color Codes           | Visual Treatment (in background) |
| ------------ | --------------------- | -------------------------------- |
| Calm/Default | `#80CBC4` (Soft Teal) | Neutral, friendly                |
| Anger        | `#FF5252` → `#D32F2F` | Red gradient, inflated           |
| Sadness      | `#90A4AE` → `#546E7A` | Blue-grey, shrunk                |
| Fear         | `#3949AB` → `#5E35B1` | Indigo-purple, shadowed          |
| Joy          | `#FFEB3B` → `#EC407A` | Yellow-pink rainbow              |

**Base/Neutral Colors**: `#E6D7C1` (Soft Clay), `#FFF9EC` (Cream), `#5D4037` (Dark Brown)

**Note**: Moro is not a separate animated character - each scene background includes Moro in the appropriate emotional state.

### Simplified Interaction Mechanics

Each maze teaches emotional regulation through simple button-tap interactions:

1. **Anger Maze**: "Take Deep Breaths" button (3-5 taps) with cooling sparkle effects + breath audio
2. **Sadness Maze**: Tap 3-4 grey thought bubbles to restore color (cognitive reframing) + gentle chimes
3. **Fear Maze**: "Light" button reveals friendly shapes from shadows + friendly sound effects
4. **Joy Maze**: "Calm Down" button (5-6 taps) slows bouncing orbs + soft ding audio

**Common Pattern**: All mazes use instructional text, simple tap counters, particle effects, audio feedback, completion messages, and auto-return to Home after 2 seconds.

## Development Workflow

1. **Always reference TODO.md** for the current phase and task breakdown
2. **Maintain the 16:9 aspect ratio** across all scenes
3. **DO NOT create documentation files** (\*.md) unless explicitly requested by the user

## Current Project State

Phase 1 and Phase 2 are complete. The core systems (GameManager, NavigationManager, ColorPalette) and UI framework (HomeButtonUI, ProgressIndicator, EmotionPortalUI, UILayoutManager) are implemented. Scenes are set up using the SceneSetupHelper editor tool.

**Important**: Moro character is integrated directly into background images for each scene. No separate character controller, sprites, or animations are needed - Moro's emotional states are part of the background art assets.
