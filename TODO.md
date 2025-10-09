# TODO - "The Little Monster's Emotion Maze"

Development task list for Unity 2D interactive storybook game.

## Phase 1: Project Foundation

- [x] Set up Unity 2D project structure with scene hierarchy
- [x] Configure 16:9 aspect ratio support
- [x] Implement core game manager and state machine
- [x] Create Moro character controller with color/expression system
- [x] Build navigation system (Home ↔ Mazes ↔ Credits)

## Phase 2: UI Framework

- [x] Design and implement UI component library (rounded buttons, portals, indicators)
- [x] Create page layout system (5% top nav, 90% content, 5% bottom progress)
- [x] Implement Home button (cloud-shaped) with fixed positioning
- [x] Build 4-dot progress indicator system
- [x] Set up color palette system (#E6D7C1, #FFF9EC base + emotion-specific colors)

## Phase 3: Home Page (Frame ID: Home)

- [x] Create Emotion Town background scene
- [x] Add portal pulsing animations with dynamic highlighting
- [x] Implement portal tap detection and scene transitions
- [x] Moro is integrated into background - no separate character needed

## Phase 4: Anger Maze (Maze_Anger)

- [x] Display text: "Moro is very hot and angry. Help Moro cool down!"
- [x] Create "Take Deep Breaths" button with visual feedback
- [x] Add simple cooling particle effect (blue sparkles) on button tap
- [ ] Play calming breath audio (inhale/exhale sounds) - Placeholders added, needs audio files
- [x] Implement tap counter (3-5 deep breaths)
- [x] Show completion text: "Moro feels calmer now!"
- [x] Auto-navigate back to Home scene after 2 seconds

**Setup Instructions**: Open Maze_Anger scene in Unity, then run `Tools > Emotion Maze > Setup Anger Maze` to create all UI elements automatically.

## Phase 5: Sadness Maze (Maze_Sadness)

- [x] Display text: "Moro feels sad and grey. Let's find happy thoughts!"
- [x] Create 3-4 grey thought bubbles with simple tap interaction
- [x] Add color bloom effect on tap (grey → colorful: #66BB6A, #FFCA28, #80CBC4)
- [ ] Play gentle chime audio on each tap - Placeholders added, needs audio files
- [x] Implement completion counter (all bubbles colored)
- [x] Show completion text: "Remembering good things helps!" with rain stopping
- [x] Auto-navigate back to Home scene after 2 seconds

**Setup Instructions**: Open Maze_Sadness scene in Unity, then run `Tools > Emotion Maze > Setup Sadness Maze` to create all UI elements automatically.

## Phase 6: Fear Maze (Maze_Fear)

- [ ] Design deep indigo forest background with Moro in fearful state (#283593, #5E35B1) with dark atmosphere
- [ ] Display text: "It's dark and scary. Let's turn on the lights!"
- [ ] Create 3-4 dark shadow shapes on screen
- [ ] Add "Light" button that brightens one shadow at a time
- [ ] Play friendly sound effect when shadow reveals (gentle "pop" or friendly giggle)
- [ ] Transform dark shape → friendly shape (star, heart, smiley) with glow effect
- [ ] Show completion text: "Not so scary after all!" with scene brightening
- [ ] Auto-navigate back to Home scene after 2 seconds

## Phase 7: Joy Maze (Maze_Joy)

- [ ] Create rainbow backdrop with Moro in overstimulated state (#FFEB3B, #EC407A) and animated swirls
- [ ] Display text: "Too much excitement! Let's find calm."
- [ ] Create 5-6 bouncing colorful orbs with simple animations
- [ ] Add "Calm Down" button with tap interaction
- [ ] Play soft "ding" audio on each tap, orbs slow down gradually
- [ ] Implement tap counter to calm all orbs (5-6 taps)
- [ ] Show completion text: "Finding calm in the fun!" with colors settling
- [ ] Auto-navigate back to Home scene after 2 seconds

## Phase 8: Credits Page (Frame ID: Credits) [OPTIONAL]

- [ ] Design celebration background with calm Moro (#80CBC4)
- [ ] Add confetti particle system
- [ ] Display "Great job helping Moro!" message
- [ ] Auto-navigate back to Home scene after 3 seconds

**Note**: This phase is optional. Mazes now auto-return to Home, so Credits can be added later as a special completion screen if all 4 mazes are completed. Moro is part of the background, no separate animation needed.

## Phase 9: Audio & Assets

- [ ] Source/create simple background music for each emotion maze
- [ ] Implement basic SFX (breath sounds, chimes, friendly pops, dings)
- [ ] Create background images for each maze (with Moro integrated: Home, Anger, Sadness, Fear, Joy, Credits)
- [ ] Build simple particle effects (sparkles, rain, glow, color blooms)
- [ ] Create UI elements (thought bubbles, shadow shapes, friendly shapes, orbs)

## Phase 10: Polish & Testing

- [ ] Implement smooth scene transitions
- [ ] Add haptic feedback for mobile
- [ ] Optimize for children's interaction patterns (large touch targets)
- [ ] Test all 4 maze paths end-to-end
- [ ] Balance difficulty and engagement for 5-8 age group

---

## Key Technical Challenges

### Input Detection

- Simple button tap detection with visual feedback
- Tap counter systems for completion tracking
- UI state management for button interactions

### Visual Systems

- Simple particle effects (sparkles, rain, glow, color blooms)
- Progressive animation speed changes (bouncing orbs slowing down)
- Color transformation effects (grey → colorful, dark → bright)
- Scene transitions and background changes (Moro states are baked into backgrounds)

### Audio Design

- Simple background music tracks per emotion
- Basic SFX timing (breath sounds, chimes, pops, dings)
- Audio feedback on button interactions
- Volume/pitch variations for completion states

## Color Reference

### Base Colors

- Soft Clay: `#E6D7C1`
- Cream: `#FFF9EC`
- Dark Brown: `#5D4037`

### Emotion Colors

- **Calm/Default**: Soft Teal `#80CBC4`
- **Anger**: Bright Red `#FF5252` → Deep Red `#D32F2F`
- **Sadness**: Dusty Blue `#90A4AE` → Blue-Gray `#546E7A`
- **Fear**: Indigo `#3949AB` → Deep Purple `#5E35B1`
- **Joy**: Yellow `#FFEB3B` → Pink `#EC407A` (rainbow sheen)

### Interactive Elements

- Grey-white (colorable): `#F5F5F5`
- Bright Green: `#66BB6A`
- Bright Yellow: `#FFCA28`
- Light Gray (inactive): `#E0E0E0`
