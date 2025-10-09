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

- [ ] Create Emotion Town background scene
- [ ] Implement Moro idle breathing animation
- [ ] Build 4 emotion portals (Red/Anger, Blue/Sadness, Indigo/Fear, Rainbow/Joy)
- [ ] Add portal pulsing animations with dynamic highlighting
- [ ] Implement portal tap detection and scene transitions

## Phase 4: Anger Maze (Maze_Anger)

- [ ] Design fiery labyrinth environment (#D32F2F, #B71C1C palette)
- [ ] Implement cool air current particle system (blue ribbons)
- [ ] Create circular trace detection for guiding airflow
- [ ] Build Anger Core with aggressive pulsation animation
- [ ] Implement long-press (3 sec) mechanic with visual/audio feedback
- [ ] Add spark extinguishing mechanics and rainbow bridge exit

## Phase 5: Sadness Maze (Maze_Sadness)

- [ ] Create grey-blue garden scene (#546E7A) with rain animation
- [ ] Implement colorable object system (trees, flowers, clouds)
- [ ] Build tap-to-color mechanic with color restoration (#66BB6A, #FFCA28)
- [ ] Add progressive rain reduction logic
- [ ] Create sky clearing and sunlight breakthrough effects

## Phase 6: Fear Maze (Maze_Fear)

- [ ] Design deep indigo forest (#283593) with shadow system
- [ ] Implement draggable lantern mechanic with light radius
- [ ] Create shadow reveal system (transform scary → friendly)
- [ ] Build dynamic lighting that follows player input
- [ ] Add reassuring sound triggers on shadow revelation

## Phase 7: Joy Maze (Maze_Joy)

- [ ] Create chaotic rainbow backdrop with swirling colors
- [ ] Implement bouncing overstimulation orbs system
- [ ] Build rhythmic tap detection mechanic
- [ ] Create orb capture and harmonization system
- [ ] Add visual/audio transition from chaos → harmony

## Phase 8: Credits Page (Frame ID: Credits)

- [ ] Design celebration scene with calm Moro (#80CBC4)
- [ ] Implement Moro happy dance animation
- [ ] Add confetti particle system
- [ ] Create Replay button (circular arrow)
- [ ] Build navigation back to Home

## Phase 9: Audio & Assets

- [ ] Source/create ambient music for each emotion
- [ ] Implement SFX (crackling fire, drizzle, wind, chimes)
- [ ] Create hand-drawn texture assets for all scenes
- [ ] Design Moro character sprites (multiple emotional states)
- [ ] Build particle effects (sparks, rain, confetti, glow)

## Phase 10: Polish & Testing

- [ ] Implement smooth scene transitions
- [ ] Add haptic feedback for mobile
- [ ] Optimize for children's interaction patterns (large touch targets)
- [ ] Test all 4 maze paths end-to-end
- [ ] Balance difficulty and engagement for 5-8 age group

---

## Key Technical Challenges

### Input Detection

- Circular trace recognition for Anger Maze
- Long-press with timer UI for Anger Core
- Drag tracking for Fear Maze lantern
- Rhythmic tap timing for Joy Maze orbs

### Visual Systems

- Dynamic color transitions for Moro
- Particle systems (air currents, rain, sparks, confetti)
- Light radius calculation for lantern
- Progressive color restoration in Sadness Maze

### Audio Design

- Layered ambient tracks per emotion
- Dynamic music transitions (chaos → harmony in Joy Maze)
- Context-sensitive SFX triggers
- Calming breath sounds for long-press feedback

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
